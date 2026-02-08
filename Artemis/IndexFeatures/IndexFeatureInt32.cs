using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureInt32 : IndexFeature<int>
    {
        protected override int ComputeFeature(object obj)
        {
            return (int)obj;
        }
    }
}