using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public class PropertyIndexRangeSortedFeature<T> : IndexRangeSortedFeature<T>
    {
        protected readonly string entityTypeNamespace;
        protected readonly string entityTypeName;
        protected readonly string entityPropertyName;
        protected readonly PropertyInfo propertyInfo;


        public PropertyIndexRangeSortedFeature(Type entityType, PropertyInfo propertyInfo) : base()
        {
            this.entityTypeNamespace = entityType.Namespace;
            this.entityTypeName = entityType.Name;
            this.entityPropertyName = propertyInfo.Name;
            this.propertyInfo = propertyInfo;
        }

        public PropertyIndexRangeSortedFeature(Type entityType, PropertyInfo propertyInfo, object feature) : this(entityType, propertyInfo)
        {
            this.Initialize(feature);
        }

        public PropertyIndexRangeSortedFeature(Type entityType, PropertyInfo propertyInfo, Entity entity) : this(entityType, propertyInfo)
        {
            this.Initialize(entity);
        }


        

    }
}
