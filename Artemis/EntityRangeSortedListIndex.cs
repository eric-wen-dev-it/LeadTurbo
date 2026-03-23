using LeadTurbo.Artemis.IndexFeatures;
using LeadTurbo.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis
{
    public abstract class EntityRangeSortedListIndex<T> : EntityDictionaryIndex<T>
    {
        RangeSortedList<T, HashSet<long>> rangeSortedList;

        public EntityRangeSortedListIndex()
        {


        }

        protected override IDictionary<T, HashSet<long>> CreateDictionary()
        {
            rangeSortedList = new RangeSortedList<T, HashSet<long>>();


            return rangeSortedList;
        }


        /// <summary>
        /// 按索引特征获得主键
        /// </summary>
        /// <param name="indexFeature"></param>
        /// <returns></returns>
        public override long[] Get(IndexFeatureBase indexFeatureBase)
        {
            List<long> result = new List<long>();
            if (indexFeatureBase is IndexRangeSortedFeature<T> indexRangeSortedFeature)
            {
                IList<KeyValuePair<T, HashSet<long>>> keyValuePairs = new List<KeyValuePair<T, HashSet<long>>>();
                if ((indexRangeSortedFeature.Feature.Banner & IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.LessThan) == IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.LessThan)
                {
                    keyValuePairs = rangeSortedList.LessThan(indexRangeSortedFeature.Feature.ToValue);
                }
                else if ((indexRangeSortedFeature.Feature.Banner & IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.LessThanOrEqual) == IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.LessThanOrEqual)
                {
                    keyValuePairs = rangeSortedList.LessThanOrEqual(indexRangeSortedFeature.Feature.ToValue);
                }
                else if ((indexRangeSortedFeature.Feature.Banner & IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.GreaterThan) == IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.GreaterThan)
                {
                    keyValuePairs = rangeSortedList.GreaterThan(indexRangeSortedFeature.Feature.FromValue);
                }
                else if ((indexRangeSortedFeature.Feature.Banner & IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.GreaterThanOrEqual) == IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.GreaterThanOrEqual)
                {
                    keyValuePairs = rangeSortedList.GreaterThanOrEqual(indexRangeSortedFeature.Feature.FromValue);
                }
                else if ((indexRangeSortedFeature.Feature.Banner & IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.Max) == IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.Max)
                {
                    KeyValuePair<T, HashSet<long>> keyValuePair = rangeSortedList.Max();
                    result.AddRange(keyValuePair.Value);
                }
                else if ((indexRangeSortedFeature.Feature.Banner & IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.Min) == IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.Min)
                {
                    KeyValuePair<T, HashSet<long>> keyValuePair = rangeSortedList.Min();
                    result.AddRange(keyValuePair.Value);
                }
                else
                {
                    bool rangeIncludeFrom = false;
                    bool rangeIncludeTo = false;

                    if ((indexRangeSortedFeature.Feature.Banner & IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.RangeIncludeFrom) == IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.RangeIncludeFrom)
                    {
                        rangeIncludeFrom = true;
                    }

                    if ((indexRangeSortedFeature.Feature.Banner & IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.RangeIncludeTo) == IndexRangeSortedFeature<T>.FeatureValue<T>.Symbol.RangeIncludeTo)
                    {
                        rangeIncludeTo = true;
                    }

                    keyValuePairs = rangeSortedList.Range(indexRangeSortedFeature.Feature.FromValue, rangeIncludeFrom, indexRangeSortedFeature.Feature.ToValue, rangeIncludeTo);
                }


                foreach (KeyValuePair<T, HashSet<long>> item in keyValuePairs)
                {
                    result.AddRange(item.Value);
                }
            }
            else
            {
                result.AddRange(base.Get(indexFeatureBase));
            }


            return result.ToArray();



        }




    }
}