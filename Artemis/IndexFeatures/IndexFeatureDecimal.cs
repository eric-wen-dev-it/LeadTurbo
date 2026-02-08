using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureDecimal : IndexFeature<decimal>
    {
        protected override decimal ComputeFeature(object obj)
        {
            return (decimal)obj;
        }
    }
}