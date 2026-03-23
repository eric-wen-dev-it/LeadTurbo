using LeadTurbo.Collections;
using LeadTurbo.Serialization;
using LeadTurbo.Artemis.IndexFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LeadTurbo.Artemis
{
    public abstract class EntityBPlusTreeIndex<T> : EntityDictionaryIndex<T>
    {
        public EntityBPlusTreeIndex()
        {


        }

        protected override IDictionary<T, HashSet<long>> CreateDictionary()
        {
           
            BPlusTree<T, HashSet<long>> bPlusTree = new BPlusTree<T, HashSet<long>>();
            return bPlusTree;
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

