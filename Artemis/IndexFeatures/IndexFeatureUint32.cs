using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureUint32 : IndexFeature<uint>
    {
        protected override uint ComputeFeature(object obj)
        {
            return (uint)obj;
        }
    }
}