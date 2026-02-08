using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureUint128 : IndexFeature<UInt128>
    {
        protected override UInt128 ComputeFeature(object obj)
        {
            return (UInt128)obj;
        }
    }
}
