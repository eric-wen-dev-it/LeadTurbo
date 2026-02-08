using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    

    public abstract class IndexFeature<T>: IndexFeatureBase
    {
        

        // 存储本实例 “特征值” （实体级别或对象级别）
        private T feature;
        

        public IndexFeature()
        {
            
            feature = default;
        }

       

        /// <summary>
        /// 使用实体建立索引特征。调用子类实现。
        /// </summary>
        /// <param name="entity">实体对象</param>
        public override void Initialize(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            object obj = BuildingRawData(entity);
            feature = ComputeFeature(obj);
        }

        /// <summary>
        /// 使用任意对象建立索引特征。默认可调用子类实现。
        /// </summary>
        /// <param name="obj">对象</param>
        public override void Initialize(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            feature = ComputeFeature(obj);
        }

        /// <summary>
        /// 子类必须实现：用对象计算特征值。
        /// </summary>
        /// <param name="obj">已经准备好的对象（可能是实体或原始数据）</param>
        /// <returns>返回特征值</returns>
        protected abstract T ComputeFeature(object obj);

        /// <summary>
        /// 子类必须实现：如何从实体提取用于计算特征的原始数据。
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>用于 ComputeFeature 的对象</returns>
        protected abstract object BuildingRawData(Entity entity);

        /// <summary>
        /// 获取特征值。如果尚未调用 Initialize，会抛异常。
        /// </summary>
        public T Feature
        {
            get
            {
                return feature;
                //if (!EqualityComparer<T>.Default.Equals(feature, default))
                //{
                //    return feature;
                //}
                //throw new InvalidOperationException("尚未调用 Initialize() 即访问 Feature");
            }
        }
    }

}
