using LeadTurbo.Artemis.IndexFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis
{
    public abstract class EntityIndexBase
    {
        ulong indexFeaturekey;
        
        public EntityIndexBase()
        {
           
        }

        public virtual void Initialize()
        {
            IndexFeatureBase indexFeatureBase = CreateIndexFeature();
            indexFeaturekey = indexFeatureBase.Key;
        }

        public ulong IndexFeaturekey
        {
            get
            {
                return indexFeaturekey;
            }
        }


        /// <summary>
        /// 设置索引
        /// </summary>
        public abstract void Set(Entity entity);
        /// <summary>
        /// 从索引中移除对象索引。
        /// </summary>
        /// <param name="entity"></param>
        public abstract void Remove(Entity entity);

        public abstract long[] Get(IndexFeatureBase indexFeature);

        /// <summary>
        /// 创造一个和本索引对象相容的索引特征
        /// </summary>
        /// <returns></returns>
        public abstract IndexFeatureBase CreateIndexFeature();


        public static bool TryCreate(Type entityType, PropertyInfo propertyInfo,out EntityIndexBase? entityIndexBase)
        {
            bool result = true;
            entityIndexBase = null;
            switch (propertyInfo.PropertyType)
            {
                case Type t when t == typeof(sbyte):
                {
                    entityIndexBase= new EntityPropertyDictionaryIndexInt8(entityType, propertyInfo);
                    break;
                }
                case Type t when t == typeof(byte):
                {
                    entityIndexBase= new EntityPropertyDictionaryIndexUint8(entityType, propertyInfo);
                    break;
                }
                case Type t when t == typeof(short):
                {
                    entityIndexBase= new EntityPropertyDictionaryIndexInt16(entityType, propertyInfo);
                    break;
                }
                case Type t when t == typeof(ushort):
                {
                    entityIndexBase= new EntityPropertyDictionaryIndexUint16(entityType, propertyInfo);
                    break;
                }
                case Type t when t == typeof(int):
                {
                    entityIndexBase= new EntityPropertyDictionaryIndexInt32(entityType, propertyInfo);
                    break;
                }
                case Type t when t == typeof(uint):
                {
                    entityIndexBase= new EntityPropertyDictionaryIndexUint32(entityType, propertyInfo);
                    break;
                }
                case Type t when t == typeof(long):
                {
                    entityIndexBase= new EntityPropertyDictionaryIndexInt64(entityType, propertyInfo);
                    break;
                }
                case Type t when t == typeof(ulong):
                {
                    entityIndexBase= new EntityPropertyDictionaryIndexUint64(entityType, propertyInfo);
                    break;
                }
                case Type t when t == typeof(Int128):
                {
                    entityIndexBase = new EntityPropertyDictionaryIndexInt128(entityType, propertyInfo);
                    break;
                }
                case Type t when t == typeof(UInt128):
                {
                    entityIndexBase = new EntityPropertyDictionaryIndexUint128(entityType, propertyInfo);
                    break;
                }
                case Type t when t == typeof(char):
                {
                    entityIndexBase= new EntityPropertyDictionaryIndexChar(entityType, propertyInfo);
                    break;
                }
                case Type t when t == typeof(bool):
                {
                    entityIndexBase= new EntityPropertyDictionaryIndexBoolean(entityType, propertyInfo);
                    break;
                }
                case Type t when t == typeof(float):
                {
                    entityIndexBase= new EntityPropertyDictionaryIndexSingle(entityType, propertyInfo);
                    break;
                }
                case Type t when t == typeof(double):
                {
                    entityIndexBase= new EntityPropertyDictionaryIndexDouble(entityType, propertyInfo);
                    break;
                }
                case Type t when t == typeof(decimal):
                {
                    entityIndexBase= new EntityPropertyDictionaryIndexDecimal(entityType, propertyInfo);
                    break;
                }
                case Type t when t == typeof(string):
                {
                    StringLengthAttribute? attr = propertyInfo.GetCustomAttribute<StringLengthAttribute>();
                    if (attr == null || attr.MaximumLength > 255)
                    {
                        entityIndexBase = new EntityPropertyDictionaryIndexString(entityType, propertyInfo);
                    }
                    else
                    {
                        entityIndexBase = new EntityPropertyDictionaryIndexShortString(entityType, propertyInfo);
                    }
                    break;
                }
                case Type t when t == typeof(System.DateTime):
                {
                    entityIndexBase= new EntityPropertyDictionaryIndexDateTime(entityType, propertyInfo);
                    break;
                }
                default:
                {
                    result = false;
                    break;
                    //throw new NotSupportedException($"Property type '{propertyInfo.PropertyType}' is not supported.");
                }
            }

            


            return result;
        }



    }
}
