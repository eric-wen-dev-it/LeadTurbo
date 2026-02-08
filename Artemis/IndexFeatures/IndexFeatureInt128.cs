using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureInt128 : IndexFeature<Int128>
    {
        protected override Int128 ComputeFeature(object obj)
        {
            return (Int128)obj;
        }
    }
}
