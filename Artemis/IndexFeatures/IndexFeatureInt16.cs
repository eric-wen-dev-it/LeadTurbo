using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureInt16 : IndexFeature<short>
    {
        protected override short ComputeFeature(object obj)
        {
            return (short)obj;
        }
    }
}