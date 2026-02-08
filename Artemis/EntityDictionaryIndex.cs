using LeadTurbo.Artemis.IndexFeatures;
using LeadTurbo.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis
{
    /// <summary>
    /// 使用哈希表的索引，只能完成精确比较
    /// </summary>
    public abstract class EntityDictionaryIndex<T> : EntityIndexBase
    {
        /// <summary>
        /// 正向索引，特征Key对应的实体对象的主key
        /// </summary>
        IDictionary<T, HashSet<long>> featureToPrimaryKey;

        /// <summary>
        /// 反向索引，实体对象的主key对应的特征Key
        /// </summary>
        Dictionary<long, T> primaryKeyToFeature = new Dictionary<long, T>();

        public EntityDictionaryIndex()
        {

            
        }

        public override void Initialize()
        {
            base.Initialize();
            featureToPrimaryKey = CreateDictionary();
        }



        protected virtual IDictionary<T, HashSet<long>> CreateDictionary()
        {

            return new Dictionary<T, HashSet<long>>();
        }

        protected IDictionary<T, HashSet<long>> FeatureToPrimaryKey
        {
            get => featureToPrimaryKey;
        }

        /// <summary>
        /// 设置索引
        /// </summary>
        public override void Set(Entity entity)
        {
            Remove(entity);

            IndexFeatureBase indexFeatureBase = CreateIndexFeature();
            indexFeatureBase.Initialize(entity);
            if (indexFeatureBase is IndexFeature<T> indexFeature)
            {
                T newFeature = indexFeature.Feature;
                HashSet<long> primaryKeys;
                if (!featureToPrimaryKey.TryGetValue(newFeature, out primaryKeys))
                {
                    primaryKeys = new HashSet<long>();
                    featureToPrimaryKey.Add(newFeature, primaryKeys);
                }
                primaryKeys.Add(entity.PrimaryKey);
                if (!primaryKeyToFeature.ContainsKey(entity.PrimaryKey))
                {
                    primaryKeyToFeature.Add(entity.PrimaryKey, newFeature);
                }
                else
                {
                    primaryKeyToFeature[entity.PrimaryKey] = newFeature;
                }

            }
        }

        /// <summary>
        /// 从索引中移除对象索引。
        /// </summary>
        /// <param name="entity"></param>
        public override void Remove(Entity entity)
        {
            T oldFeature;
            if (primaryKeyToFeature.TryGetValue(entity.PrimaryKey, out oldFeature))
            {
                HashSet<long> primaryKeys;
                if (featureToPrimaryKey.TryGetValue(oldFeature, out primaryKeys))
                {
                    primaryKeys.Remove(entity.PrimaryKey);

                    if (primaryKeys.Count == 0)
                    {
                        featureToPrimaryKey.Remove(oldFeature);
                    }
                }


                primaryKeyToFeature.Remove(entity.PrimaryKey);
            }
        }


        /// <summary>
        /// 按索引特征获得主键
        /// </summary>
        /// <param name="indexFeatureBase"></param>
        /// <returns></returns>
        public override long[] Get(IndexFeatureBase indexFeatureBase)
        {
            if (IndexFeaturekey == indexFeatureBase.Key)
            {
                if (indexFeatureBase is IndexFeature<T> indexFeature)
                {
                    HashSet<long> primaryKeys;
                    if (!featureToPrimaryKey.TryGetValue(indexFeature.Feature, out primaryKeys))
                    {
                        primaryKeys = new HashSet<long>();
                    }
                    return primaryKeys.ToArray();
                }
                else
                {
                    throw new AssertException("查询使用的索引类型与本索引不兼容。");
                }
            }
            else
            {
                throw new AssertException("查询使用的索引类型特征与本索引不兼容。");

            }
        }







    }
}
