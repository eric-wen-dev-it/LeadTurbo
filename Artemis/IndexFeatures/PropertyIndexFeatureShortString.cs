using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public class PropertyIndexFeatureShortString : PropertyIndexFeature<string>
    {
        public PropertyIndexFeatureShortString(Type entityType, PropertyInfo propertyInfo)
       : base(entityType, propertyInfo)
        {

        }

        public PropertyIndexFeatureShortString(Type entityType, PropertyInfo propertyInfo, object feature)
        : base(entityType, propertyInfo, feature)
        {

        }

        public PropertyIndexFeatureShortString(Type entityType, PropertyInfo propertyInfo, Entity entity)
            : base(entityType, propertyInfo, entity)
        {
            // …
        }

        protected override string ComputeFeature(object obj)
        {
            if (obj is null)
            {
                obj = "";
            }

            return base.ComputeFeature(obj);
        }
      
    }
}