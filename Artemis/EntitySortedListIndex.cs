using LeadTurbo.Artemis.IndexFeatures;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis
{
    public abstract class EntitySortedListIndex<T> : EntityDictionaryIndex<T>
    {
        public EntitySortedListIndex()
        {


        }
        
        protected override IDictionary<T, HashSet<long>> CreateDictionary()
        {
            return new SortedList<T, HashSet<long>>();
        }


        /// <summary>
        /// 按索引特征获得主键
        /// </summary>
        /// <param name="indexFeature"></param>
        /// <returns></returns>
        public override long[] Get(IndexFeatureBase indexFeatureBase)
        {
            return base.Get(indexFeatureBase);
        }
    }
}

 