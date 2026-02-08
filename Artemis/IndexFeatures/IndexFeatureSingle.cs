using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureSingle : IndexFeature<float>
    {
        protected override float ComputeFeature(object obj)
        {
            return (float)obj;
        }
    }
}