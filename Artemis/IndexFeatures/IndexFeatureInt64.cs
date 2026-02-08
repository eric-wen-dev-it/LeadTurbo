using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureInt64 : IndexFeature<long>
    {
        protected override long ComputeFeature(object obj)
        {
            return (long)obj;
        }
    }
}