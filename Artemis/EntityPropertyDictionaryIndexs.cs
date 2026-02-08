using LeadTurbo.Artemis.IndexFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis
{
    public class EntityPropertyDictionaryIndexInt8 : EntityPropertyDictionaryIndex<sbyte>
    {
        public EntityPropertyDictionaryIndexInt8(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureInt8(EntityType, PropertyInfo);
        }
    }


    public class EntityPropertyDictionaryIndexInt32: EntityPropertyDictionaryIndex<int>
    {
        public EntityPropertyDictionaryIndexInt32(Type entityType, PropertyInfo propertyInfo) : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureInt32(EntityType, PropertyInfo);
        }
    }

    public class EntityPropertyDictionaryIndexInt16 : EntityPropertyDictionaryIndex<short>
    {
        public EntityPropertyDictionaryIndexInt16(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureInt16(EntityType, PropertyInfo);
        }
    }

    public class EntityPropertyDictionaryIndexInt64 : EntityPropertyDictionaryIndex<long>
    {
        public EntityPropertyDictionaryIndexInt64(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureInt64(EntityType, PropertyInfo);
        }
    }

    public class EntityPropertyDictionaryIndexInt128 : EntityPropertyDictionaryIndex<Int128>
    {
        public EntityPropertyDictionaryIndexInt128(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureInt128(EntityType, PropertyInfo);
        }
    }


    public class EntityPropertyDictionaryIndexSingle : EntityPropertyDictionaryIndex<Single>
    {
        public EntityPropertyDictionaryIndexSingle(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureSingle(EntityType, PropertyInfo);
        }
    }

    public class EntityPropertyDictionaryIndexDouble : EntityPropertyDictionaryIndex<double>
    {
        public EntityPropertyDictionaryIndexDouble(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureDouble(EntityType, PropertyInfo);
        }
    }

    public class EntityPropertyDictionaryIndexDecimal : EntityPropertyDictionaryIndex<decimal>
    {
        public EntityPropertyDictionaryIndexDecimal(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureDecimal(EntityType, PropertyInfo);
        }
    }

    public class EntityPropertyDictionaryIndexUint8 : EntityPropertyDictionaryIndex<byte>
    {
        public EntityPropertyDictionaryIndexUint8(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureUint8(EntityType, PropertyInfo);
        }
    }

    public class EntityPropertyDictionaryIndexUint16 : EntityPropertyDictionaryIndex<ushort>
    {
        public EntityPropertyDictionaryIndexUint16(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureUint16(EntityType, PropertyInfo);
        }
    }

    public class EntityPropertyDictionaryIndexChar : EntityPropertyDictionaryIndex<Char>
    {
        public EntityPropertyDictionaryIndexChar(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureChar(EntityType, PropertyInfo);
        }
    }


    


    public class EntityPropertyDictionaryIndexUint32 : EntityPropertyDictionaryIndex<uint>
    {
        public EntityPropertyDictionaryIndexUint32(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureUint32(EntityType, PropertyInfo);
        }
    }

    public class EntityPropertyDictionaryIndexUint64 : EntityPropertyDictionaryIndex<ulong>
    {
        public EntityPropertyDictionaryIndexUint64(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureUint64(EntityType, PropertyInfo);
        }
    }

    public class EntityPropertyDictionaryIndexUint128 : EntityPropertyDictionaryIndex<UInt128>
    {
        public EntityPropertyDictionaryIndexUint128(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureUint128(EntityType, PropertyInfo);
        }
    }

    public class EntityPropertyDictionaryIndexString : EntityPropertyDictionaryIndex<ulong>
    {
        public EntityPropertyDictionaryIndexString(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureString(EntityType, PropertyInfo);
        }
    }

    public class EntityPropertyDictionaryIndexShortString : EntityPropertyDictionaryIndex<string>
    {
        public EntityPropertyDictionaryIndexShortString(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureShortString(EntityType, PropertyInfo);
        }
    }


    public class EntityPropertyDictionaryIndexGuid : EntityPropertyDictionaryIndex<Guid>
    {
        public EntityPropertyDictionaryIndexGuid(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureGuid(EntityType, PropertyInfo);
        }
    }

    public class EntityPropertyDictionaryIndexBoolean : EntityPropertyDictionaryIndex<bool>
    {
        public EntityPropertyDictionaryIndexBoolean(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureBoolean(EntityType, PropertyInfo);
        }
    }

    

    public class EntityPropertyDictionaryIndexDateTime : EntityPropertyDictionaryIndex<DateTime>
    {
        public EntityPropertyDictionaryIndexDateTime(Type entityType, PropertyInfo propertyInfo)
            : base(entityType, propertyInfo)
        {
        }

        public override IndexFeatureBase CreateIndexFeature()
        {
            return new PropertyIndexFeatureDateTime(EntityType, PropertyInfo);
        }
    }
}
