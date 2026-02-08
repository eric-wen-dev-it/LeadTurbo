using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureUint64 : IndexFeature<ulong>
    {
        protected override ulong ComputeFeature(object obj)
        {
            return (ulong)obj;
        }
    }
}