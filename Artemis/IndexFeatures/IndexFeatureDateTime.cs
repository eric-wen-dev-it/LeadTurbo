using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureDateTime : IndexFeature<PropertyIndexFeatureDateTime>
    {
        protected override PropertyIndexFeatureDateTime ComputeFeature(object obj)
        {
            return (PropertyIndexFeatureDateTime)obj;
        }
    }
}