using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    /// <summary>
    /// 字符串类索引特征基类
    /// </summary>
    public abstract class IndexFeatureString : IndexFeature<string>
    {
        protected override string ComputeFeature(object obj)
        {
            return (string)obj;
            
        }
    }
}
