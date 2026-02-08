using ProtoBuf.Meta;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LeadTurbo.Artemis
{
    public struct MemberData : IComparable<MemberData>
    {
        public string MemberName
        {
            get;
        }
        public int Tag
        {
            get;
        }
        public object Value
        {
            get; set;
        }

        public MemberData(string memberName, int tag, object value)
        {
            MemberName = memberName;
            Tag = tag;
            Value = value;
        }

        public MemberData(string memberName, int tag)
        {
            MemberName = memberName;
            Tag = tag;
            Value = null;
        }

        public int CompareTo(MemberData other)
        {
            return this.Tag.CompareTo(other.Tag);
        }
    }

    public static class ProtobufNetHelper
    {
        // 缓存：每个目标类型的 Tag->赋值委托 映射
        private static readonly ConcurrentDictionary<Type, Dictionary<int, Action<object, object>>> _setterCache
            = new ConcurrentDictionary<Type, Dictionary<int, Action<object, object>>>();

        /// <summary>
        /// 优化后的版本：按位置 dataList[] 中的顺序赋值给 instance 中按 Tag 升序排列的成员。
        /// </summary>
        public static void ApplyMemberDataList(Entity instance, object[] dataList)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (dataList == null)
                throw new ArgumentNullException(nameof(dataList));

            Type targetType = instance.GetType();
            var model = RuntimeTypeModel.Default;
            var meta = model[targetType];
            if (meta == null)
                throw new InvalidOperationException($"Type {targetType.FullName} is not configured in RuntimeTypeModel");

            // 获取按 Tag 升序排列的字段（ValueMember[]）  
            var fields = meta.GetFields()
                             .OrderBy(vm => vm.FieldNumber)
                             .ToArray();

            int fieldCount = fields.Length;
            int dataCount = dataList.Length;

            if (dataCount < fieldCount)
                throw new ArgumentException($"dataList length ({dataCount}) is less than the number of serializable fields ({fieldCount}) for type {targetType.FullName}");

            // 获取／生成 setter 映射
            var tagToSetter = _setterCache.GetOrAdd(targetType, t => BuildSetters(t));

            // 执行赋值
            for (int i = 0; i < fieldCount; i++)
            {
                var vm = fields[i];
                int tag = vm.FieldNumber;
                object value = dataList[i];

                if (tagToSetter.TryGetValue(tag, out var setter))
                {
                    try
                    {
                        setter(instance, value);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Failed setting value for tag={tag} on type {targetType.FullName}: {ex.Message}", ex);
                    }
                }
                else
                {
                    // 可选择记录日志或忽略
                    // e.g. Debug.WriteLine($"No setter for tag {tag} in type {targetType.FullName}");
                }
            }
        }

        public static void ApplyMemberDataList(object instance, DbDataReader reader)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }
            
            Type targetType = instance.GetType();
            var model = RuntimeTypeModel.Default;
            var meta = model[targetType];

            if (meta == null)
            {
                throw new InvalidOperationException($"Type {targetType.FullName} is not configured in RuntimeTypeModel");
            }
            // 获取当前类型及其基类所有 protobuf 字段
            var fields = GetAllProtoFields(meta)
                         .OrderBy(vm => vm.FieldNumber)
                         .ToArray();

            int fieldCount = fields.Length;
            int colCount = reader.FieldCount;

            if (colCount < fieldCount)
                throw new ArgumentException(
                    $"Reader columns ({colCount}) are fewer than serializable fields ({fieldCount}) for type {targetType.FullName}");

            // 获取 setter 和 类型映射
            var tagToSetter = _setterCache.GetOrAdd(targetType, t => BuildSettersIncludingInherited(t));
            var tagToType = GetMemberTypesIncludingInherited(targetType);

            for (int i = 0; i < fieldCount; i++)
            {
                var vm = fields[i];
                int tag = vm.FieldNumber;

                if (!tagToSetter.TryGetValue(tag, out var setter))
                    continue;

                object dbValue = reader.IsDBNull(i) ? null : reader.GetValue(i);

                try
                {
                    Type destType = tagToType[tag];
                    object safeVal = ConvertDbValue(dbValue, destType);
                    setter(instance, safeVal);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Failed to set field (tag={tag}, member={vm.Member.Name}) to type {targetType.FullName}: {ex.Message}", ex);
                }
            }
        }

        // 获取当前类型及所有继承类型上 protobuf 字段
        private static IEnumerable<ValueMember> GetAllProtoFields(MetaType meta)
        {
            var seenTags = new HashSet<int>();
            while (meta != null)
            {
                foreach (var vm in meta.GetFields())
                {
                    // 如果父类字段已经被子类覆盖，避免重复
                    if (seenTags.Add(vm.FieldNumber))
                        yield return vm;
                }

                var baseType = meta.Type.BaseType;
                if (baseType == null)
                    break;
                meta = RuntimeTypeModel.Default[baseType];
            }
        }

        // 构建包含继承 setter 映射
        private static Dictionary<int, Action<object, object>> BuildSettersIncludingInherited(Type type)
        {
            var setters = new Dictionary<int, Action<object, object>>();
            Type current = type;

            while (current != null)
            {
                var meta = RuntimeTypeModel.Default[current];
                if (meta != null)
                {
                    foreach (var vm in meta.GetFields())
                    {
                        int tag = vm.FieldNumber;
                        if (!setters.ContainsKey(tag))
                            setters[tag] = BuildSetter(vm);
                    }
                }
                current = current.BaseType;
            }

            return setters;
        }

        // 构建 Tag -> 类型 映射（包含继承）
        private static Dictionary<int, Type> GetMemberTypesIncludingInherited(Type type)
        {
            var dict = new Dictionary<int, Type>();
            Type current = type;

            while (current != null)
            {
                var meta = RuntimeTypeModel.Default[current];
                if (meta != null)
                {
                    foreach (var vm in meta.GetFields())
                    {
                        if (!dict.ContainsKey(vm.FieldNumber))
                            dict[vm.FieldNumber] = vm.MemberType;
                    }
                }
                current = current.BaseType;
            }

            return dict;
        }

        // 生成 setter delegate
        private static Action<object, object> BuildSetter(ValueMember vm)
        {
            var paramTarget = Expression.Parameter(typeof(object), "target");
            var paramValue = Expression.Parameter(typeof(object), "value");

            var castTarget = Expression.Convert(paramTarget, vm.Member.DeclaringType);
            var castValue = Expression.Convert(paramValue, vm.MemberType);

            var assignExpr = Expression.Assign(
                Expression.PropertyOrField(castTarget, vm.Member.Name),
                castValue);

            return Expression.Lambda<Action<object, object>>(assignExpr, paramTarget, paramValue).Compile();
        }


        // 缓存：Type -> (Tag -> Member Type)
        private static readonly ConcurrentDictionary<Type, Dictionary<int, Type>> _tagToTypeCache
            = new ConcurrentDictionary<Type, Dictionary<int, Type>>();

      

        private static object ConvertDbValue(object dbValue, Type destType)
        {
            if (dbValue == null)
                return null;

            // 处理 Nullable<T>
            Type target = Nullable.GetUnderlyingType(destType) ?? destType;

            // 如果值本来就是目标类型
            if (target.IsInstanceOfType(dbValue))
                return dbValue;

            // 处理枚举
            if (target.IsEnum)
            {
                return Enum.ToObject(target, dbValue);
            }

            // 尝试按基础类型转换
            try
            {
                if (target == typeof(Guid))
                {
                    return Guid.Parse(dbValue.ToString());
                }

                if (target == typeof(DateTime))
                {
                    // SQLite TEXT 日期可能是 ISO 字符串
                    return DateTime.Parse(dbValue.ToString());
                }

                // Convert.ChangeType 处理大部分基础类型
                return Convert.ChangeType(dbValue, target);
            }
            catch
            {
                // 若 ChangeType 失败，尝试 ToString → 目标结构
                return dbValue;
            }
        }






        private static Dictionary<int, Action<object, object>> BuildSetters(Type targetType)
        {
            var model = RuntimeTypeModel.Default;
            var meta = model[targetType];
            if (meta == null)
                throw new InvalidOperationException($"Type {targetType.FullName} is not configured in RuntimeTypeModel");

            var dict = new Dictionary<int, Action<object, object>>();

            foreach (var vm in meta.GetFields())
            {
                int tag = vm.FieldNumber;
                MemberInfo member = vm.Member;

                var setter = CreateSetter(targetType, member);
                if (setter != null)
                {
                    dict[tag] = setter;
                }
            }

            return dict;
        }

        private static Action<object, object> CreateSetter(Type targetType, MemberInfo member)
        {
            if (member is PropertyInfo pi && pi.CanWrite)
            {
                var instanceParam = Expression.Parameter(typeof(object), "instance");
                var valueParam = Expression.Parameter(typeof(object), "value");

                var typedInstance = Expression.Convert(instanceParam, targetType);
                var typedValue = Expression.Convert(valueParam, pi.PropertyType);

                var propertyAccess = Expression.Property(typedInstance, pi);
                var assign = Expression.Assign(propertyAccess, typedValue);
                var lambda = Expression.Lambda<Action<object, object>>(assign, instanceParam, valueParam);

                return lambda.Compile();
            }
            else if (member is FieldInfo fi && !fi.IsInitOnly)
            {
                var instanceParam = Expression.Parameter(typeof(object), "instance");
                var valueParam = Expression.Parameter(typeof(object), "value");

                var typedInstance = Expression.Convert(instanceParam, targetType);
                var typedValue = Expression.Convert(valueParam, fi.FieldType);

                var fieldAccess = Expression.Field(typedInstance, fi);
                var assign = Expression.Assign(fieldAccess, typedValue);
                var lambda = Expression.Lambda<Action<object, object>>(assign, instanceParam, valueParam);

                return lambda.Compile();
            }

            // 不支持只读属性或其它成员类型
            return null;
        }

        /// <summary>
        /// 获取对象 instance 中被标记为 ProtoMember 的成员的 名称／Tag／当前值 数组。
        /// </summary>
        public static MemberData[] GetMembersWithValues(Entity instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Type targetType = instance.GetType();
            var model = RuntimeTypeModel.Default;
            var result = new List<MemberData>();

            // 递归遍历类型及其所有基类
            for (Type current = targetType; current != null; current = current.BaseType)
            {
                var meta = model[current];
                if (meta != null)
                {
                    foreach (var vm in meta.GetFields())
                    {
                        string name = vm.Member.Name;
                        int tag = vm.FieldNumber;
                        object value = null;

                        // 获取字段／属性值
                        var mi = vm.Member;
                        if (mi is PropertyInfo pi)
                        {
                            value = pi.GetValue(instance);
                        }
                        else if (mi is FieldInfo fi)
                        {
                            value = fi.GetValue(instance);
                        }

                        result.Add(new MemberData(name, tag, value));
                    }
                }
            }

            // 按 tag 或 name 排序返回
            result.Sort();
            return result.ToArray();
        }

    }


}
