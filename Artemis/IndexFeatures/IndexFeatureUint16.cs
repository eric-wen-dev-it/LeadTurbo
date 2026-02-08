using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureUInt16 : IndexFeature<ushort>
    {
        protected override ushort ComputeFeature(object obj)
        {
            return (ushort)obj;
        }
    }
}