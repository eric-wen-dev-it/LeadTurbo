using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureDouble : IndexFeature<double>
    {
        protected override double ComputeFeature(object obj)
        {
            return (double)obj;
        }
    }
}
