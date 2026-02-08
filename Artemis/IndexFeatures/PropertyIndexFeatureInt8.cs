using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public class PropertyIndexFeatureInt8 : PropertyIndexFeature<byte>
    {
        public PropertyIndexFeatureInt8(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexFeatureInt8(Type entityType, PropertyInfo propertyInfo, object feature)
            : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexFeatureInt8(Type entityType, PropertyInfo propertyInfo, Entity entity)
            : base(entityType, propertyInfo, entity)
        {
        }
    }
}
