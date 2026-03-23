using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace LeadTurbo.Artemis.IndexFeatures
{
    public abstract class IndexFeatureBase
    {
        // 缓存：每个 “原始数据字符串” 对应一个 Task<ulong>，确保只计算一次
        private static readonly ConcurrentDictionary<string, Task<ulong>> _typeKeyCache = new ConcurrentDictionary<string, Task<ulong>>();

        // 存储本实例的 key 值（类型级别的 key）
        private ulong typeKey = 0UL;

        /// <summary>
        /// 同步属性：如果本实例还未计算 type-key，则等待缓存的任务完成，再返回结果。
        /// 注意：在 UI 线程或有同步上下文环境中调用可能导致死锁。
        /// </summary>
        public ulong Key
        {
            get
            {
                if (typeKey != 0UL)
                {
                    return typeKey;
                }

                // 构建用于唯一标识类型的原始数据字符串
                string rawData = BuildIndexFeatureKeyRawData();

                // 从缓存获取或启动计算任务
                Task<ulong> keyTask = _typeKeyCache.GetOrAdd(
                    rawData,
                    _ => ComputeKeyInternalAsync(rawData)   // 注意传 rawData 而不是 “this” 来避免闭包问题
                );

                try
                {
                    // 等待异步任务完成
                    typeKey = keyTask.GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[Error] ComputeKey for type {GetType().FullName} failed: {ex}");
                    throw;
                }

                return typeKey;
            }
        }

        /// <summary>
        /// 构建用于 “类型级别键” 的原始数据字符串。默认实现：命名空间＋类型名。
        /// 子类可重写以增加额外维度。
        /// </summary>
        protected virtual string BuildIndexFeatureKeyRawData()
        {
            Type type = GetType();
            return $"{type.Namespace}|{type.Name}";
        }

        /// <summary>
        /// 内部异步逻辑：做实际的 Key 计算。
        /// </summary>
        /// <param name="rawData">用于计算的唯一标识字符串</param>
        /// <returns>计算结果</returns>
        protected static async Task<ulong> ComputeKeyInternalAsync(string rawData)
        {
            Debug.WriteLine($"ComputeKeyInternalAsync rawData = {rawData}");
            try
            {
                // 这里调用你自己的异步方法，比如 ComputeBlake2b64FromStringAsync
                ulong result = await Function.ComputeBlake2b64FromStringAsync(rawData).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Error] in ComputeKeyInternalAsync for rawData {rawData}: {ex}");
                throw;
            }
        }

        public abstract void Initialize(Entity entity);

        public abstract void Initialize(object obj);



        public static bool TryCreate(Type entityType, PropertyInfo propertyInfo, object feature, out IndexFeatureBase entityIndexBase)
        {
            bool result = true;
            entityIndexBase = null;

            switch (propertyInfo.PropertyType)
            {
                case Type t when t == typeof(sbyte):
                {
                    if (feature is IndexRangeSortedFeature<sbyte>.FeatureValue<sbyte>)
                        entityIndexBase = new PropertyIndexRangeSortedFeatureInt8(entityType, propertyInfo, feature);
                    else
                        entityIndexBase = new PropertyIndexFeatureInt8(entityType, propertyInfo, feature);
                    break;
                }
                case Type t when t == typeof(byte):
                {
                    if (feature is IndexRangeSortedFeature<byte>.FeatureValue<byte>)
                        entityIndexBase = new PropertyIndexRangeSortedFeatureUint8(entityType, propertyInfo, feature);
                    else
                        entityIndexBase = new PropertyIndexFeatureUint8(entityType, propertyInfo, feature);
                    break;
                }
                case Type t when t == typeof(short):
                {
                    if (feature is IndexRangeSortedFeature<short>.FeatureValue<short>)
                        entityIndexBase = new PropertyIndexRangeSortedFeatureInt16(entityType, propertyInfo, feature);
                    else
                        entityIndexBase = new PropertyIndexFeatureInt16(entityType, propertyInfo, feature);
                    break;
                }
                case Type t when t == typeof(ushort):
                {
                    if (feature is IndexRangeSortedFeature<ushort>.FeatureValue<ushort>)
                        entityIndexBase = new PropertyIndexRangeSortedFeatureUint16(entityType, propertyInfo, feature);
                    else
                        entityIndexBase = new PropertyIndexFeatureUint16(entityType, propertyInfo, feature);
                    break;
                }
                case Type t when t == typeof(int):
                {
                    if (feature is IndexRangeSortedFeature<int>.FeatureValue<int>)
                        entityIndexBase = new PropertyIndexRangeSortedFeatureInt32(entityType, propertyInfo, feature);
                    else
                        entityIndexBase = new PropertyIndexFeatureInt32(entityType, propertyInfo, feature);
                    break;
                }
                case Type t when t == typeof(uint):
                {
                    if (feature is IndexRangeSortedFeature<uint>.FeatureValue<uint>)
                        entityIndexBase = new PropertyIndexRangeSortedFeatureUint32(entityType, propertyInfo, feature);
                    else
                        entityIndexBase = new PropertyIndexFeatureUint32(entityType, propertyInfo, feature);
                    break;
                }
                case Type t when t == typeof(long):
                {
                    if (feature is IndexRangeSortedFeature<long>.FeatureValue<long>)
                        entityIndexBase = new PropertyIndexRangeSortedFeatureInt64(entityType, propertyInfo, feature);
                    else
                        entityIndexBase = new PropertyIndexFeatureInt64(entityType, propertyInfo, feature);
                    break;
                }
                case Type t when t == typeof(ulong):
                {
                    if (feature is IndexRangeSortedFeature<ulong>.FeatureValue<ulong>)
                        entityIndexBase = new PropertyIndexRangeSortedFeatureUint64(entityType, propertyInfo, feature);
                    else
                        entityIndexBase = new PropertyIndexFeatureUint64(entityType, propertyInfo, feature);
                    break;
                }
                case Type t when t == typeof(char):
                {
                    if (feature is IndexRangeSortedFeature<char>.FeatureValue<char>)
                        entityIndexBase = new PropertyIndexRangeSortedFeatureChar(entityType, propertyInfo, feature);
                    else
                        entityIndexBase = new PropertyIndexFeatureChar(entityType, propertyInfo, feature);
                    break;
                }
                case Type t when t == typeof(bool):
                {

                    entityIndexBase = new PropertyIndexFeatureBoolean(entityType, propertyInfo, feature);
                    break;
                }
                case Type t when t == typeof(float):
                {
                    if (feature is IndexRangeSortedFeature<float>.FeatureValue<float>)
                        entityIndexBase = new PropertyIndexRangeSortedFeatureSingle(entityType, propertyInfo, feature);
                    else
                        entityIndexBase = new PropertyIndexFeatureSingle(entityType, propertyInfo, feature);
                    break;
                }
                case Type t when t == typeof(double):
                {
                    if (feature is IndexRangeSortedFeature<double>.FeatureValue<double>)
                        entityIndexBase = new PropertyIndexRangeSortedFeatureDouble(entityType, propertyInfo, feature);
                    else
                        entityIndexBase = new PropertyIndexFeatureDouble(entityType, propertyInfo, feature);
                    break;
                }
                case Type t when t == typeof(decimal):
                {
                    if (feature is IndexRangeSortedFeature<decimal>.FeatureValue<decimal>)
                        entityIndexBase = new PropertyIndexRangeSortedFeatureDecimal(entityType, propertyInfo, feature);
                    else
                        entityIndexBase = new PropertyIndexFeatureDecimal(entityType, propertyInfo, feature);
                    break;
                }
                case Type t when t == typeof(string):
                {
                    if (feature is IndexRangeSortedFeature<string>.FeatureValue<string>)
                    {
                        StringLengthAttribute? attr = propertyInfo.GetCustomAttribute<StringLengthAttribute>();
                        if (attr == null || attr.MaximumLength > 255)
                        {
                            throw new LeadTurbo.Exceptions.AssertException("不支持");
                        }
                        else
                        {
                            entityIndexBase = new PropertyIndexRangeSortedFeatureShortString(entityType, propertyInfo, feature);
                        }
                    }
                    else
                    {
                        StringLengthAttribute? attr = propertyInfo.GetCustomAttribute<StringLengthAttribute>();
                        if (attr == null || attr.MaximumLength > 255)
                        {
                            entityIndexBase = new PropertyIndexFeatureString(entityType, propertyInfo, feature);
                        }
                        else
                        {
                            entityIndexBase = new PropertyIndexFeatureShortString(entityType, propertyInfo, feature);
                        }
                    }
                    break;
                }
                case Type t when t == typeof(System.DateTime):
                {
                    if (feature is IndexRangeSortedFeature<DateTime>.FeatureValue<System.DateTime>)
                        entityIndexBase = new PropertyIndexRangeSortedFeatureDateTime(entityType, propertyInfo, feature);
                    else
                        entityIndexBase = new PropertyIndexFeatureDateTime(entityType, propertyInfo, feature);
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
