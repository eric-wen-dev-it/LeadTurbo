using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureInt8 : IndexFeature<sbyte>
    {
        protected override sbyte ComputeFeature(object obj)
        {
            return (sbyte)obj;
        }
    }
}