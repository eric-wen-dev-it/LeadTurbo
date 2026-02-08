using LeadTurbo.Artemis.Attributes;
using LeadTurbo.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    /// <summary>
    /// 按对象属性获得索引的对象。
    /// </summary>
    public class PropertyIndexFeatureString : PropertyIndexFeature<ulong>
    {
        public PropertyIndexFeatureString(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {

        }

        public PropertyIndexFeatureString(Type entityType, PropertyInfo propertyInfo, object feature)
            : base(entityType, propertyInfo, feature)
        {

        }

        public PropertyIndexFeatureString(Type entityType, PropertyInfo propertyInfo, Entity entity)
            : base(entityType, propertyInfo, entity)
        {
           
        }

        protected override ulong ComputeFeature(object obj)
        {
            string rawData = (string)obj;
            if (rawData is null)
            {
                rawData = "rawData is Null.";
            }


            Task<ulong> keyTask = ComputeKeyInternalAsync(rawData);
            ulong feature = 0;
            try
            {
                feature = keyTask.GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error] ComputeKey for type {GetType().FullName} failed: {ex}");
                throw;
            }
            return feature;
        }
       


    }
}