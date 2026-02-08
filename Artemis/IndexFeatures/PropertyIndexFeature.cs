using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class PropertyIndexFeature<T>: IndexFeature<T>
    {
        protected readonly string entityTypeNamespace;
        protected readonly string entityTypeName;
        protected readonly string entityPropertyName;
        protected readonly PropertyInfo propertyInfo;


        public PropertyIndexFeature(Type entityType, PropertyInfo propertyInfo):base()
        {
            this.entityTypeNamespace = entityType.Namespace;
            this.entityTypeName = entityType.Name;
            this.entityPropertyName = propertyInfo.Name;
            this.propertyInfo = propertyInfo;
        }

        public PropertyIndexFeature(Type entityType, PropertyInfo propertyInfo, object feature) : this(entityType, propertyInfo)
        {
            this.Initialize(feature);
        }

        public PropertyIndexFeature(Type entityType, PropertyInfo propertyInfo, Entity entity) : this(entityType, propertyInfo)
        {
            this.Initialize(entity);
        }


        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = GetType();
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }


        protected override object BuildingRawData(Entity entity)
        {
            return propertyInfo.GetValue(entity);
        }

        protected override T ComputeFeature(object obj)
        {
            if (propertyInfo.PropertyType != typeof(T))
            {
                throw new LeadTurbo.Exceptions.AssertException("类型不匹配");
            }


            return (T)obj;
        }






    }
}
