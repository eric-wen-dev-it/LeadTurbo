using System;
using System.Collections.Generic;

namespace LeadTurbo.Artemis
{
    /// <summary>
    /// 分段 LRU 缓存。segmentCount 必须是 2 的幂，默认 16 段。
    /// 不同主键落在不同段时完全不竞争，并发度 ≈ segmentCount。
    /// </summary>
    public class EntityCache : IDisposable
    {
        private readonly CacheSegment[] segments;
        private readonly int segmentMask;

        public EntityCache(int segmentCount = 16, int maxCacheCount = 5000)
        {
            if (segmentCount <= 0 || (segmentCount & (segmentCount - 1)) != 0)
                throw new ArgumentException("segmentCount 必须是 2 的幂", nameof(segmentCount));

            segmentMask = segmentCount - 1;
            int perSegment = Math.Max(1, maxCacheCount / segmentCount);
            int remainder = maxCacheCount % segmentCount;
            segments = new CacheSegment[segmentCount];
            for (int i = 0; i < segmentCount; i++)
                segments[i] = new CacheSegment(perSegment + (i < remainder ? 1 : 0));
        }

        // 用主键低位取模，Snowflake ID 低位分布均匀
        private CacheSegment GetSegment(long primaryKey)
            => segments[primaryKey & segmentMask];

        /// <summary>
        /// 本 Cache 最大缓存数量（均分到各段）
        /// </summary>
        public int MaxCacheCount
        {
            get
            {
                int total = 0;
                foreach (CacheSegment seg in segments)
                    total += seg.MaxCount;
                return total;
            }
            set
            {
                int perSegment = Math.Max(1, value / segments.Length);
                int remainder = value % segments.Length;
                for (int i = 0; i < segments.Length; i++)
                    segments[i].MaxCount = perSegment + (i < remainder ? 1 : 0);
            }
        }

        /// <summary>
        /// 向 Cache 添加实体。已存在返回 false。
        /// </summary>
        public bool Add(Entity entity)
            => GetSegment(entity.PrimaryKey).Add(entity);

        /// <summary>
        /// 更新 Cache 中的实体，置于 LRU 头部。
        /// </summary>
        public void Updated(Entity entity)
            => GetSegment(entity.PrimaryKey).Updated(entity);

        /// <summary>
        /// 移除指定实体。
        /// </summary>
        public void Remove(Entity entity)
            => GetSegment(entity.PrimaryKey).Remove(entity);

        /// <summary>
        /// 判断 Cache 中是否包含指定主键的实体。
        /// </summary>
        public bool IsContains(long primaryKey)
            => GetSegment(primaryKey).IsContains(primaryKey);

        /// <summary>
        /// 按主键获得 Cache 中的实体，并将其置于 LRU 头部。
        /// </summary>
        public Entity Get(long primaryKey)
            => GetSegment(primaryKey).Get(primaryKey);

        /// <summary>
        /// 按主键尝试获得 Cache 中的实体，并将其置于 LRU 头部。
        /// </summary>
        public bool TryGet(long primaryKey, out Entity entity)
            => GetSegment(primaryKey).TryGet(primaryKey, out entity);

        public void Dispose()
        {
            foreach (CacheSegment seg in segments)
                seg.Dispose();
        }

        // ── 单段 LRU ──────────────────────────────────────────────────────────
        private sealed class CacheSegment : IDisposable
        {
            private readonly object lockObj = new object();
            private readonly Dictionary<long, LinkedListNode<Entity>> index = new();
            private readonly LinkedList<Entity> list = new();
            private int maxCount;

            public CacheSegment(int maxCount) => this.maxCount = maxCount;

            public int MaxCount
            {
                get
                {
                    lock (lockObj) return maxCount;
                }
                set
                {
                    lock (lockObj)
                    {
                        maxCount = value;
                        Trim();
                    }
                }
            }

            public bool Add(Entity entity)
            {
                var newNode = new LinkedListNode<Entity>(entity);
                lock (lockObj)
                {
                    if (!index.TryAdd(entity.PrimaryKey, newNode))
                        return false;
                    list.AddFirst(newNode);
                    Trim();
                    return true;
                }
            }

            public void Updated(Entity entity)
            {
                var newNode = new LinkedListNode<Entity>(entity);
                lock (lockObj)
                {
                    if (index.Remove(entity.PrimaryKey, out var existing))
                        list.Remove(existing);
                    index[entity.PrimaryKey] = newNode;
                    list.AddFirst(newNode);
                    Trim();
                }
            }

            public void Remove(Entity entity)
            {
                lock (lockObj)
                {
                    if (index.Remove(entity.PrimaryKey, out var node))
                        list.Remove(node);
                }
            }

            public bool IsContains(long primaryKey)
            {
                lock (lockObj) return index.ContainsKey(primaryKey);
            }

            public Entity Get(long primaryKey)
            {
                lock (lockObj)
                {
                    if (!index.TryGetValue(primaryKey, out var node))
                        throw new KeyNotFoundException("Cache中不存在该对象。");
                    MoveToFront(node);
                    return node.Value;
                }
            }

            public bool TryGet(long primaryKey, out Entity entity)
            {
                lock (lockObj)
                {
                    if (!index.TryGetValue(primaryKey, out var node))
                    {
                        entity = null;
                        return false;
                    }
                    MoveToFront(node);
                    entity = node.Value;
                    return true;
                }
            }

            // 调用方须持有 lockObj
            private void MoveToFront(LinkedListNode<Entity> node)
            {
                list.Remove(node);
                list.AddFirst(node);
            }

            // 调用方须持有 lockObj
            private void Trim()
            {
                if (index.Count <= maxCount) return;
                // 超出上限时批量驱逐至 9/10，均摊驱逐开销
                int lowWaterMark = maxCount - maxCount / 10;
                while (index.Count > lowWaterMark)
                {
                    LinkedListNode<Entity>? last = list.Last;
                    if (last == null) break;
                    index.Remove(last.Value.PrimaryKey);
                    list.RemoveLast();
                }
            }

            public void Dispose()
            {
                lock (lockObj)
                {
                    index.Clear();
                    list.Clear();
                }
            }
        }
    }
}
