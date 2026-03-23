using LeadTurbo.Artemis.IndexFeatures;
using LeadTurbo.Exceptions;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis
{
    public abstract class EntityIndexSet
    {
        /// <summary>
        /// 创造一组和本实体相关的索引
        /// </summary>
        /// <returns></returns>
        protected abstract EntityIndexBase[] CreateIndexs();

        FrozenDictionary<ulong, EntityIndexBase> indexs;

        public EntityIndexSet()
        {

        }

        public void Initialize()
        {
            EntityIndexBase[] entityIndices = CreateIndexs();

            foreach (EntityIndexBase entityIndex in entityIndices)
            {
                entityIndex.Initialize();
            }




            Dictionary<ulong, EntityIndexBase> tempIndexs = new Dictionary<ulong, EntityIndexBase>();


            foreach (EntityIndexBase entityIndex in entityIndices)
            {

                if (!tempIndexs.ContainsKey(entityIndex.IndexFeaturekey))
                {
                    tempIndexs.Add(entityIndex.IndexFeaturekey, entityIndex);
                }
                else
                {
                    throw new AssertException("{0}:已经添加到索引中了。", entityIndex.IndexFeaturekey);
                }

            }

            indexs = tempIndexs.ToFrozenDictionary();

        }


        public override string ToString()
        {
            StringBuilder sb1 = new StringBuilder();
            foreach (EntityIndexBase item in indexs.Values)
            {
                sb1.AppendLine(item.ToString());

            }

            return sb1.ToString();


        }






        /// <summary>
        /// 设置索引
        /// </summary>
        public virtual void Set(Entity entity)
        {

            foreach (EntityIndexBase entityIndex in indexs.Values)
            {
                entityIndex.Set(entity);
            }

        }
        /// <summary>
        /// 从索引中移除对象索引。
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Remove(Entity entity)
        {
            foreach (EntityIndexBase entityIndex in indexs.Values)
            {
                entityIndex.Remove(entity);

            }
        }


        /// <summary>
        /// 按索引特征获得主键
        /// </summary>
        /// <param name="indexFeature"></param>
        /// <returns></returns>
        public virtual long[] Get(IndexFeatureBase indexFeature)
        {
            EntityIndexBase entityIndex;
            if (indexs.TryGetValue(indexFeature.Key, out entityIndex))
            {
                return entityIndex.Get(indexFeature);

            }
            else
            {
                throw new AssertException("{0}:无此类型的特征索引。", indexFeature.Key);
            }
        }








    }
}

