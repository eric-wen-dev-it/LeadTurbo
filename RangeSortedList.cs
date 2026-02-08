using System;
using System.Collections.Generic;

public class RangeSortedList<TKey, TValue> : SortedList<TKey, TValue>
{
    private readonly IComparer<TKey> _comparer;

    // 构造器：使用默认比较器（Comparer<TKey>.Default）
    public RangeSortedList() : base()
    {
        _comparer = this.Comparer;
        if (_comparer == null)
        {
            throw new InvalidOperationException(
                $"No comparer supplied and Default comparer for {typeof(TKey)} is null.");
        }
    }

    // 构造器：允许用户提供自定义 comparer
    public RangeSortedList(IComparer<TKey> comparer) : base(comparer)
    {
        _comparer = this.Comparer;
        if (_comparer == null)
        {
            throw new ArgumentNullException(nameof(comparer),
                "Comparer cannot be null when provided explicitly.");
        }
    }

    /// <summary>
    /// 返回所有 key 严格大于 fromKey 的项。
    /// </summary>
    public IList<KeyValuePair<TKey, TValue>> GreaterThan(TKey fromKey)
    {
        int startIndex = FindIndexGreaterThan(fromKey);
        return GetRangeFrom(startIndex, this.Count - startIndex);
    }

    /// <summary>
    /// 返回所有 key ≥ fromKey 的项。
    /// </summary>
    public IList<KeyValuePair<TKey, TValue>> GreaterThanOrEqual(TKey fromKey)
    {
        int startIndex = FindIndexGreaterThanOrEqual(fromKey);
        return GetRangeFrom(startIndex, this.Count - startIndex);
    }

    /// <summary>
    /// 返回所有 key 严格小于 toKey 的项。
    /// </summary>
    public IList<KeyValuePair<TKey, TValue>> LessThan(TKey toKey)
    {
        int endIndex = FindIndexLessThan(toKey);
        return GetRangeFrom(0, endIndex + 1);
    }

    /// <summary>
    /// 返回所有 key ≤ toKey 的项。
    /// </summary>
    public IList<KeyValuePair<TKey, TValue>> LessThanOrEqual(TKey toKey)
    {
        int endIndex = FindIndexLessThanOrEqual(toKey);
        return GetRangeFrom(0, endIndex + 1);
    }

    /// <summary>
    /// 返回 key 在 [fromKey, toKey] 范围内（边界可选 includeFrom/includeTo）。
    /// </summary>
    public IList<KeyValuePair<TKey, TValue>> Range(TKey fromKey, bool includeFrom, TKey toKey, bool includeTo)
    {
        int startIndex = includeFrom ? FindIndexGreaterThanOrEqual(fromKey) : FindIndexGreaterThan(fromKey);
        int endIndex = includeTo ? FindIndexLessThanOrEqual(toKey) : FindIndexLessThan(toKey);

        if (startIndex < 0)
            startIndex = 0;
        if (endIndex < 0)
            return new List<KeyValuePair<TKey, TValue>>(0);
        if (startIndex > endIndex || startIndex >= this.Count)
            return new List<KeyValuePair<TKey, TValue>>(0);

        int count = endIndex - startIndex + 1;
        return GetRangeFrom(startIndex, count);
    }

    #region — 私有辅助方法（二分查找） —

    private int FindIndexGreaterThan(TKey key)
    {
        int lo = 0, hi = this.Keys.Count - 1;
        int result = this.Keys.Count;
        while (lo <= hi)
        {
            int mid = lo + ((hi - lo) >> 1);
            TKey midKey = this.Keys[mid];
            if (_comparer.Compare(midKey, key) > 0)
            {
                result = mid;
                hi = mid - 1;
            }
            else
            {
                lo = mid + 1;
            }
        }
        return result;
    }

    private int FindIndexGreaterThanOrEqual(TKey key)
    {
        int lo = 0, hi = this.Keys.Count - 1;
        int result = this.Keys.Count;
        while (lo <= hi)
        {
            int mid = lo + ((hi - lo) >> 1);
            TKey midKey = this.Keys[mid];
            if (_comparer.Compare(midKey, key) >= 0)
            {
                result = mid;
                hi = mid - 1;
            }
            else
            {
                lo = mid + 1;
            }
        }
        return result;
    }

    private int FindIndexLessThan(TKey key)
    {
        int lo = 0, hi = this.Keys.Count - 1;
        int result = -1;
        while (lo <= hi)
        {
            int mid = lo + ((hi - lo) >> 1);
            TKey midKey = this.Keys[mid];
            if (_comparer.Compare(midKey, key) < 0)
            {
                result = mid;
                lo = mid + 1;
            }
            else
            {
                hi = mid - 1;
            }
        }
        return result;
    }

    private int FindIndexLessThanOrEqual(TKey key)
    {
        int lo = 0, hi = this.Keys.Count - 1;
        int result = -1;
        while (lo <= hi)
        {
            int mid = lo + ((hi - lo) >> 1);
            TKey midKey = this.Keys[mid];
            if (_comparer.Compare(midKey, key) <= 0)
            {
                result = mid;
                lo = mid + 1;
            }
            else
            {
                hi = mid - 1;
            }
        }
        return result;
    }

    private IList<KeyValuePair<TKey, TValue>> GetRangeFrom(int index, int count)
    {
        var list = new List<KeyValuePair<TKey, TValue>>(count);
        for (int i = index; i < index + count && i < this.Keys.Count; i++)
        {
            list.Add(new KeyValuePair<TKey, TValue>(this.Keys[i], this.Values[i]));
        }
        return list;
    }

    #endregion
}
