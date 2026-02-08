using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureUint8 : IndexFeature<byte>
    {
        protected override byte ComputeFeature(object obj)
        {
            return (byte)obj;
        }
    }
}