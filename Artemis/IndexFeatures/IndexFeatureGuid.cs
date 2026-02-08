using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureGuid : IndexFeature<Guid>
    {
        protected override Guid ComputeFeature(object obj)
        {
            return (Guid)obj;
        }
    }
}