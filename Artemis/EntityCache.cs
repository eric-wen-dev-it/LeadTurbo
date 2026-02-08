using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis
{
    public class EntityCache : IDisposable
    {
        // 用于保护链表操作的锁对象
        private readonly object listLock = new object();

        // 使用线程安全的ConcurrentDictionary来存储主键到链表节点的映射
        private ConcurrentDictionary<long, LinkedListNode<Entity>> keyIndex = new ConcurrentDictionary<long, LinkedListNode<Entity>>();
        // 链表用于维护LRU顺序：头部为最近使用，尾部为最久未使用
        private LinkedList<Entity> cacheIndex = new LinkedList<Entity>();

        private int maxCacheCount = 5000;
        /// <summary>
        /// 本Cache最大缓存数量
        /// </summary>
        public int MaxCacheCount
        {
            get => maxCacheCount;
            set => maxCacheCount = value;
        }

        /// <summary>
        /// 向Cache添加实体。
        /// </summary>
        /// <param name="entity"></param>
        public bool Add(Entity entity)
        {
            // 创建新的节点
            var newNode = new LinkedListNode<Entity>(entity);
            // 尝试添加到字典（无需加锁）
            if (keyIndex.TryAdd(entity.PrimaryKey, newNode))
            {
                lock (listLock)
                {
                    // 加入链表头部
                    cacheIndex.AddFirst(newNode);
                    // 超出缓存容量时移除尾部节点
                    while (keyIndex.Count > maxCacheCount)
                    {
                        RemoveLastInternal();
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 更新Cache中的实体，如果已存在则先移除后添加（相当于将其置于链表头部）。
        /// </summary>
        /// <param name="entity"></param>
        public void Updated(Entity entity)
        {
            // 首先在字典中尝试移除旧节点（无需锁保护字典）
            if (keyIndex.TryRemove(entity.PrimaryKey, out var existingNode))
            {
                lock (listLock)
                {
                    cacheIndex.Remove(existingNode);
                }
            }

            // 添加新的实体
            var newNode = new LinkedListNode<Entity>(entity);
            if (keyIndex.TryAdd(entity.PrimaryKey, newNode))
            {
                lock (listLock)
                {
                    cacheIndex.AddFirst(newNode);
                    while (keyIndex.Count > maxCacheCount)
                    {
                        RemoveLastInternal();
                    }
                }
            }
        }

        /// <summary>
        /// 移除指定实体。
        /// </summary>
        /// <param name="entity"></param>
        public void Remove(Entity entity)
        {
            if (keyIndex.TryRemove(entity.PrimaryKey, out var node))
            {
                lock (listLock)
                {
                    cacheIndex.Remove(node);
                }
            }
        }

        /// <summary>
        /// 移除链表尾部的节点（最不常用的缓存项）。
        /// 此方法假设调用者已经获取了listLock。
        /// </summary>
        private void RemoveLastInternal()
        {
            // 注意：这里在锁内操作链表
            var last = cacheIndex.Last;
            if (last != null)
            {
                // 从字典中移除时忽略返回值
                keyIndex.TryRemove(last.Value.PrimaryKey, out _);
                cacheIndex.RemoveLast();
            }
            else
            {
                throw new InvalidOperationException("链表为空，无法移除最后一个节点。");
            }
        }

        /// <summary>
        /// 判断Cache中是否包含指定主键的实体。
        /// </summary>
        public bool IsContains(long primaryKey)
        {
            return keyIndex.ContainsKey(primaryKey);
        }

        /// <summary>
        /// 按主键获得Cache中的实体，并将其置于链表头部（LRU更新）。
        /// </summary>
        public Entity Get(long primaryKey)
        {
            if (keyIndex.TryGetValue(primaryKey, out var node))
            {
                // 调整链表顺序：将节点移到头部
                lock (listLock)
                {
                    cacheIndex.Remove(node);
                    cacheIndex.AddFirst(node);
                }
                return node.Value;
            }
            else
            {
                throw new KeyNotFoundException("Cache中不存在该对象。");
            }
        }

        /// <summary>
        /// 按主键尝试获得Cache中的实体，并更新其使用状态。
        /// </summary>
        public bool TryGet(long primaryKey, out Entity entity)
        {
            if (keyIndex.TryGetValue(primaryKey, out var node))
            {
                lock (listLock)
                {
                    cacheIndex.Remove(node);
                    cacheIndex.AddFirst(node);
                }
                entity = node.Value;
                return true;
            }
            else
            {
                entity = null;
                return false;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 用于检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放每个实体，如果它们实现了IDisposable
                    foreach (var node in cacheIndex)
                    {
                        if (node is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                    keyIndex.Clear();
                    cacheIndex.Clear();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion



    }
}
