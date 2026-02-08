using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.IndexFeatures
{
    public class PropertyIndexRangeSortedFeatureDateTime : PropertyIndexRangeSortedFeature<DateTime>
    {
        public PropertyIndexRangeSortedFeatureDateTime(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureDateTime(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureDateTime(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }

        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureDateTime);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }
    }

    public class PropertyIndexRangeSortedFeatureDecimal : PropertyIndexRangeSortedFeature<Decimal>
    {
        public PropertyIndexRangeSortedFeatureDecimal(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureDecimal(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureDecimal(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }

        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureDecimal);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }

    }

    public class PropertyIndexRangeSortedFeatureInt8 : PropertyIndexRangeSortedFeature<sbyte>
    {
        public PropertyIndexRangeSortedFeatureInt8(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureInt8(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureInt8(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }

        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureInt8);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }
    }
    public class PropertyIndexRangeSortedFeatureInt16 : PropertyIndexRangeSortedFeature<short>
    {
        public PropertyIndexRangeSortedFeatureInt16(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureInt16(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureInt16(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }

        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureInt16);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }
    }
    public class PropertyIndexRangeSortedFeatureInt32 : PropertyIndexRangeSortedFeature<int>
    {
        public PropertyIndexRangeSortedFeatureInt32(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureInt32(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureInt32(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }

        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureInt32);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }

    }

    public class PropertyIndexRangeSortedFeatureInt64 : PropertyIndexRangeSortedFeature<Int64>
    {
        public PropertyIndexRangeSortedFeatureInt64(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureInt64(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureInt64(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }

        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureInt64);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }
    }

    public class PropertyIndexRangeSortedFeatureInt128 : PropertyIndexRangeSortedFeature<Int128>
    {
        public PropertyIndexRangeSortedFeatureInt128(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureInt128(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureInt128(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }
        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureInt128);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }
    }



    public class PropertyIndexRangeSortedFeatureUint8 : PropertyIndexRangeSortedFeature<byte>
    {
        public PropertyIndexRangeSortedFeatureUint8(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureUint8(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureUint8(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }

        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureUint8);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }

    }

    public class PropertyIndexRangeSortedFeatureUint16 : PropertyIndexRangeSortedFeature<ushort>
    {
        public PropertyIndexRangeSortedFeatureUint16(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureUint16(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureUint16(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }
        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureUint16);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }
    }

    public class PropertyIndexRangeSortedFeatureUint32 : PropertyIndexRangeSortedFeature<UInt32>
    {
        public PropertyIndexRangeSortedFeatureUint32(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureUint32(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureUint32(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }
        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureUint32);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }
    }

    public class PropertyIndexRangeSortedFeatureUint64 : PropertyIndexRangeSortedFeature<UInt64>
    {
        public PropertyIndexRangeSortedFeatureUint64(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureUint64(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureUint64(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }
        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureUint64);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }
    }

    public class PropertyIndexRangeSortedFeatureUint128 : PropertyIndexRangeSortedFeature<UInt128>
    {
        public PropertyIndexRangeSortedFeatureUint128(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureUint128(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureUint128(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }
        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureUint128);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }
    }

    public class PropertyIndexRangeSortedFeatureShortString : PropertyIndexRangeSortedFeature<string>
    {
        public PropertyIndexRangeSortedFeatureShortString(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureShortString(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureShortString(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }
        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureShortString);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }
    }

    public class PropertyIndexRangeSortedFeatureDouble : PropertyIndexRangeSortedFeature<double>
    {
        public PropertyIndexRangeSortedFeatureDouble(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureDouble(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureDouble(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }
        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureDouble);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }
    }

    public class PropertyIndexRangeSortedFeatureSingle : PropertyIndexRangeSortedFeature<float>
    {
        public PropertyIndexRangeSortedFeatureSingle(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureSingle(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureSingle(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }
        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureSingle);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }
    }

    public class PropertyIndexRangeSortedFeatureChar : PropertyIndexRangeSortedFeature<char>
    {
        public PropertyIndexRangeSortedFeatureChar(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public PropertyIndexRangeSortedFeatureChar(Type entityType, PropertyInfo propertyInfo, object feature) : base(entityType, propertyInfo, feature)
        {
        }

        public PropertyIndexRangeSortedFeatureChar(Type entityType, PropertyInfo propertyInfo, Entity entity) : base(entityType, propertyInfo, entity)
        {
        }
        protected override string BuildIndexFeatureKeyRawData()
        {
            Type type = typeof(PropertyIndexFeatureChar);
            return string.Format("{0}|{1}|{2}|{3}", entityTypeNamespace, entityTypeName, entityPropertyName, type.FullName);
        }
    }
}
