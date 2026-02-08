using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureBoolean : IndexFeature<bool>
    {
        protected override bool ComputeFeature(object obj)
        {
            return (bool)obj;
        }
    }
}