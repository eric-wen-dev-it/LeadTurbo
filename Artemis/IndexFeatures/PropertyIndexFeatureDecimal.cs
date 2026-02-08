using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public class PropertyIndexFeatureDecimal : PropertyIndexFeature<Decimal>
    {
        public PropertyIndexFeatureDecimal(Type entityType, PropertyInfo propertyInfo)
       : base(entityType, propertyInfo)
        {

        }

        public PropertyIndexFeatureDecimal(Type entityType, PropertyInfo propertyInfo, object feature)
        : base(entityType, propertyInfo, feature)
        {

        }

        public PropertyIndexFeatureDecimal(Type entityType, PropertyInfo propertyInfo, Entity entity)
            : base(entityType, propertyInfo, entity)
        {
            // …
        }



    }
}
