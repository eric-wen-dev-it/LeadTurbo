using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LeadTurbo.Artemis.IndexFeatures
{
    public class IndexRangeSortedFeature<T> : IndexFeatureBase
    {
        public class FeatureValue<T>
        {
            [Flags]
            public enum Symbol
            {
                Nothing = 0,
                GreaterThan = 1 << 0,
                GreaterThanOrEqual = 1 << 1,
                LessThanOrEqual = 1 << 2,
                LessThan = 1 << 3,
                RangeIncludeFrom = 1 << 4,
                RangeIncludeTo = 1 << 5,
                Equal = 1 << 6,
                Min = 1 << 7,
                Max = 1 << 8
            }


            public Symbol Banner
            {
                get; set;
            }






            public T FromValue
            {
                get; set;
            }

            public T ToValue
            {
                get; set;
            }

        }

        // 存储本实例 “特征值” （实体级别或对象级别）
        private FeatureValue<T> feature = default;

        public override void Initialize(Entity entity)
        {
            throw new NotImplementedException();
        }

        public override void Initialize(object obj)
        {
            if (obj is FeatureValue<T> featureValue)
            {
                feature = featureValue;
            }
            else
            {
                throw new LeadTurbo.Exceptions.AssertException("Initialize类型不是FeatureValue<T>");
            }
        }


        public FeatureValue<T> Feature
        {
            get
            {
                if (!feature.Equals(default))
                {
                    return feature;
                }
                else
                {
                    throw new InvalidOperationException("尚未调用 Initialize() 即访问 Feature");
                }
            }
        }





    }
}
