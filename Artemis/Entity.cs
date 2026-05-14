using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using static LeadTurbo.Edit.Operate;
using static LeadTurbo.Function;

namespace LeadTurbo.Artemis
{
    [ProtoContract]
    public abstract class Entity : IComparable, INotifyPropertyChanged, INotifyPropertyChanging
    {
        static SnowflakeIdGenerator snowflakeIdGenerator = new SnowflakeIdGenerator(
            long.TryParse(Environment.GetEnvironmentVariable("SNOWFLAKE_WORKER_ID"), out long wid) ? wid : 0,
            long.TryParse(Environment.GetEnvironmentVariable("SNOWFLAKE_DATACENTER_ID"), out long did) ? did : 0);

        long primaryKey = snowflakeIdGenerator.NextId();
        int editVer = 0;



        /// <summary>
        /// 主键
        /// </summary>
        [JsonInclude]
        [ProtoMember(1)]
        public long PrimaryKey
        {
            get
            {
                return primaryKey;
            }
            protected set
            {
                if (primaryKey != value)
                {
                    primaryKey = value;
                }
            }
        }
        
        [JsonInclude]
        [ProtoMember(2)]
        public int EditVer
        {
            get
            {
                return editVer;
            }
            protected set
            {
                editVer = value;

            }

        }

        internal void Upgrade()
        {
            editVer++;
        }

        public void ResetEditVer()
        {
            editVer = 0;

        }

        protected virtual string GetDbViewName()
        {
            Type type = this.GetType();

            return type.Name;
        }


        /// <summary>
        /// 创造一个和自身类型匹配的对象
        /// </summary>
        /// <returns></returns>
        protected abstract object Create();


        /// <summary>
        /// 深拷贝（与 Jiamparts.Core 行为对齐）。通过 protobuf-net 二进制往返获得完整副本，
        /// 保留 PrimaryKey 与 EditVer——语义为"同一实体的镜像"。
        /// </summary>
        public virtual Entity Clone()
        {
            return ProtoBuf.Serializer.DeepClone<Entity>(this);
        }

        /// <summary>
        /// 深拷贝并分配新身份。protobuf-net 往返之后，PrimaryKey 替换为新雪花 ID 并将
        /// EditVer 重置为 0——语义为"基于现有数据的全新实体"。
        /// </summary>
        public virtual Entity Copy()
        {
            Entity copy = ProtoBuf.Serializer.DeepClone<Entity>(this);
            copy.primaryKey = snowflakeIdGenerator.NextId();
            copy.editVer = 0;
            return copy;
        }


        public override bool Equals(object? obj)
        {
            bool _return = false;
            if (obj is Entity entity)
            {
                if (this.GetType().Equals(entity.GetType()))
                {
                    if (this.primaryKey == entity.primaryKey)
                    {
                        if (this.editVer == entity.editVer)
                        {
                            _return = true;
                        }
                    }
                }

            }
            return _return;
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangingEventHandler? PropertyChanging;
        protected virtual void OnNotifyPropertyChanging(string propertyName)
        {


            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        protected static int Comparable(object a, object b)
        {
            Entity A = a as Entity;
            Entity B = b as Entity;
            if (A == null && B == null)
            {
                return 0;
            }
            else if (A == null)
            {
                return -1;
            }
            else if (B == null)
            {
                return 1;
            }
            else
            {
                Type aT = A.GetType();
                Type bT = B.GetType();
                string aTname = aT.FullName;
                string bTname = bT.FullName;
                int com = aTname.CompareTo(bTname);
                //Debug.WriteLine(string.Format("aTname:{0} bTname{1} com:{2}", aTname,bTname,com));

                return com;




            }

        }

        public virtual int CompareTo(object? obj)
        {
            return Comparable(this, obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(GetType(), PrimaryKey, EditVer);
        }


        protected virtual void ValidateProperty<T>(T value, string propertyName)
        {
            Validator.ValidateProperty(value,
                new ValidationContext(this, new ServiceProvider(), null) { MemberName = propertyName });
        }

        [Flags]
        public enum SetPropertyEnum : byte
        {
            Nothink=0,
            enableValidateProperty=1<<0,
            enableOnNotifyPropertyChanging=1<<1,
            enableOnNotifyPropertyChanged=1<<2,
        }

        private SetPropertyEnum setPropertyFlags;

        /// <summary>
        /// 获取或设置所有标志位。
        /// </summary>
        public SetPropertyEnum Flags
        {
            get => setPropertyFlags;
            set => setPropertyFlags = value;
        }

        /// <summary>
        /// 是否启用 ValidateProperty 功能。
        /// </summary>
        public bool EnableValidateProperty
        {
            get => (setPropertyFlags & SetPropertyEnum.enableValidateProperty) == SetPropertyEnum.enableValidateProperty;
            set
            {
                if (value)
                {
                    setPropertyFlags |= SetPropertyEnum.enableValidateProperty;
                }
                else
                {
                    setPropertyFlags &= ~SetPropertyEnum.enableValidateProperty;
                }
            }
        }

        /// <summary>
        /// 是否启用 OnNotifyPropertyChanging 功能。
        /// </summary>
        public bool EnableOnNotifyPropertyChanging
        {
            get => (setPropertyFlags & SetPropertyEnum.enableOnNotifyPropertyChanging) == SetPropertyEnum.enableOnNotifyPropertyChanging;
            set
            {
                if (value)
                {

                    setPropertyFlags |= SetPropertyEnum.enableOnNotifyPropertyChanging;
                }
                else
                {
                    setPropertyFlags &= ~SetPropertyEnum.enableOnNotifyPropertyChanging;
                }
            }
        }

        /// <summary>
        /// 是否启用 OnNotifyPropertyChanged 功能。
        /// </summary>
        public bool EnableOnNotifyPropertyChanged
        {
            get => (setPropertyFlags & SetPropertyEnum.enableOnNotifyPropertyChanged) == SetPropertyEnum.enableOnNotifyPropertyChanged;
            set
            {
                if (value)
                {
                    setPropertyFlags |= SetPropertyEnum.enableOnNotifyPropertyChanged;
                }
                else
                {
                    setPropertyFlags &= ~SetPropertyEnum.enableOnNotifyPropertyChanged;
                }
            }
        }



        protected virtual bool SetValidatedProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                if (EnableValidateProperty)
                {
                    ValidateProperty(value, propertyName);
                }
                if (EnableOnNotifyPropertyChanging)
                {
                    OnNotifyPropertyChanging(propertyName);
                }
                field = value;
                if (EnableOnNotifyPropertyChanged)
                {
                    OnNotifyPropertyChanged(propertyName);
                }
                return true;
            }
            return false;
        }


        public void ManualPropertyChanged(string propertyName)
        {
            OnNotifyPropertyChanged(propertyName);
        }

        public class ServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                throw new NotImplementedException();
            }
        }




        static Type[]? knownTypes;
        static readonly Lazy<JsonSerializerOptions> entityJsonSerializerOptions = new(CreateEntityJsonSerializerOptions);
        static readonly Lazy<Dictionary<Type, JsonDerivedType[]>> entityJsonDerivedTypes = new(CreateEntityJsonDerivedTypes);

        public string ToJson(bool writeIndented = true)
        {
            JsonSerializerOptions options = writeIndented
                ? new JsonSerializerOptions(EntityJsonSerializerOptions) { WriteIndented = true }
                : EntityJsonSerializerOptions;
            return JsonSerializer.Serialize<Entity>(this, options);
        }

        public static Entity FromJson(string json)
        {
            ArgumentNullException.ThrowIfNull(json);
            return JsonSerializer.Deserialize<Entity>(json, EntityJsonSerializerOptions)
                ?? throw new JsonException("JSON deserialization returned null.");
        }

        public static T FromJson<T>(string json) where T : Entity
        {
            ArgumentNullException.ThrowIfNull(json);
            return JsonSerializer.Deserialize<T>(json, EntityJsonSerializerOptions)
                ?? throw new JsonException($"JSON deserialization returned null for type {typeof(T).FullName}.");
        }

        public static JsonSerializerOptions EntityJsonSerializerOptions => entityJsonSerializerOptions.Value;

        static JsonSerializerOptions CreateEntityJsonSerializerOptions()
        {
            DefaultJsonTypeInfoResolver resolver = new DefaultJsonTypeInfoResolver();
            resolver.Modifiers.Add(ConfigureEntityJsonTypeInfo);

            return new JsonSerializerOptions(JsonSerializerDefaults.General)
            {
                PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate,
                TypeInfoResolver = resolver,
                WriteIndented = false
            };
        }

        static Dictionary<Type, JsonDerivedType[]> CreateEntityJsonDerivedTypes()
        {
            Type[] types = GetKnownTypes();
            HashSet<Type> hierarchyTypes = new HashSet<Type>(types) { typeof(Entity) };
            Dictionary<Type, JsonDerivedType[]> result = new Dictionary<Type, JsonDerivedType[]>();

            foreach (Type baseType in hierarchyTypes)
            {
                if (!typeof(Entity).IsAssignableFrom(baseType))
                {
                    continue;
                }

                JsonDerivedType[] derivedTypes = types
                    .Where(type => type != baseType
                        && !type.IsAbstract
                        && !type.IsInterface
                        && !type.ContainsGenericParameters
                        && baseType.IsAssignableFrom(type))
                    .OrderBy(type => type.FullName, StringComparer.Ordinal)
                    .Select(type => new JsonDerivedType(type, GetJsonTypeDiscriminator(type)))
                    .ToArray();

                if (derivedTypes.Length > 0)
                {
                    result[baseType] = derivedTypes;
                }
            }

            return result;
        }

        static void ConfigureEntityJsonTypeInfo(JsonTypeInfo jsonTypeInfo)
        {
            if (!typeof(Entity).IsAssignableFrom(jsonTypeInfo.Type))
            {
                return;
            }

            if (!entityJsonDerivedTypes.Value.TryGetValue(jsonTypeInfo.Type, out JsonDerivedType[]? derivedTypes))
            {
                return;
            }

            JsonPolymorphismOptions polymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$type",
                IgnoreUnrecognizedTypeDiscriminators = false,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization
            };

            foreach (JsonDerivedType derivedType in derivedTypes)
            {
                polymorphismOptions.DerivedTypes.Add(derivedType);
            }

            jsonTypeInfo.PolymorphismOptions = polymorphismOptions;
        }

        static string GetJsonTypeDiscriminator(Type type)
        {
            return type.FullName ?? type.Name;
        }

        public static Type[] GetKnownTypes()
        {
            if (knownTypes == null)
            {

                List<Type> Return = new List<Type>();
                Type thisType = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
                HashSet<Type> temp = new HashSet<Type>();

                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (a != null)
                    {
                        try
                        {


                            Type[] aTypes = a.GetTypes();

                            foreach (Type t in aTypes)
                            {
                                if(t.IsSubclassOf(typeof(Entity)))
                                {
                                    if (t.Name != "EntityCollection`1")
                                    {
                                        temp.Add(t);
                                        
                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[Entity.GetKnownTypes] 加载程序集 {a.FullName} 失败：{ex.Message}");
                        }
                    }
                }

                //temp.Add(typeof(KeyValuePair<Guid, int>));
                //temp.Add(typeof(KeyValuePair<Guid, int>[]));
                //temp.Add(typeof(Guid[]));

                //temp.Add(typeof(Entity));
                //temp.Add(typeof(Entity[]));

                //temp.Add(typeof(string[]));
                knownTypes = temp.ToArray();



            }
            return knownTypes;
        }


        static System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create();
        const int MAX = 536_870_911;
        static int ComputeHash(Type type)
        {
            var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(type.FullName));
            int raw = BitConverter.ToInt32(bytes, 0) & int.MaxValue;
            int tag = (raw % MAX) + 1;
            if (tag >= 19000 && tag <= 19999)
                tag = (tag + 10000) % MAX + 1;

            return tag;
        }



      
        static void AddSubType(Type type, HashSet<Type> types)
        {
            var model = RuntimeTypeModel.Default;
            MetaType meta = model.Add(type, applyDefaultBehaviour: true);

            Debug.WriteLine($"Type:{type.Name}");
            
            foreach (Type subType in GetKnownTypes())
            {
                if (subType.BaseType == type)
                {
                    Debug.WriteLine($"    SubType:{subType.Name}");
                    meta.AddSubType(ComputeHash(subType), subType);
                    AddSubType(subType,types);
                }
            }

        }




        static Entity()
        {
            try
            {

                Type[] knownTypes = LeadTurbo.Artemis.Entity.GetKnownTypes();
                HashSet<Type> types = new HashSet<Type>(knownTypes);
                AddSubType(typeof(Entity), types);

                string bs1= ProtoHelpers.GenerateProto(new Type[] { typeof(Entity)}, "LeadTurbo");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Entity..cctor] Protobuf 类型注册失败：{ex}");
            }

        }


        /// <summary>
        /// 按分页枚举返回初始化用存储过程
        /// </summary>
        /// <param name="maxRangeKeyCount"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GetDatabasInitializationResultProcedureName(MaxRangeKeyCount maxRangeKeyCount, Entity entity)
        {
            string bs1 = string.Format("{0}_INITIALIZATION_SELECT_KEY", entity.GetDbViewName());
            switch (maxRangeKeyCount)
            {

                case MaxRangeKeyCount.Count50:
                {
                    bs1 = string.Format("{0}_50", bs1);
                    break;
                }
                case MaxRangeKeyCount.Count20:
                {
                    bs1 = string.Format("{0}_20", bs1);
                    break;
                }
                case MaxRangeKeyCount.Count10:
                {
                    bs1 = string.Format("{0}_10", bs1);
                    break;
                }
                case MaxRangeKeyCount.Count5:
                {
                    bs1 = string.Format("{0}_5", bs1);
                    break;
                }
                case MaxRangeKeyCount.Count2:
                {
                    bs1 = string.Format("{0}_2", bs1);
                    break;
                }
                case MaxRangeKeyCount.Count1:
                {
                    break;
                }
                default:
                {
                    throw new NotImplementedException(Enum.GetName(typeof(MaxRangeKeyCount), maxRangeKeyCount));
                }
            }
            return bs1;
        }

       


        /// <summary>
        /// 按分页枚举返回存储过程
        /// </summary>
        /// <param name="maxRangeKeyCount"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string GetDatabasResultProcedureName(MaxRangeKeyCount maxRangeKeyCount, Entity entity)
        {
            string bs1 = string.Format("{0}_SELECT_KEY", entity.GetDbViewName());
            switch (maxRangeKeyCount)
            {

                case MaxRangeKeyCount.Count50:
                {
                    bs1 = string.Format("{0}_50", bs1);
                    break;
                }
                case MaxRangeKeyCount.Count20:
                {
                    bs1 = string.Format("{0}_20", bs1);
                    break;
                }
                case MaxRangeKeyCount.Count10:
                {
                    bs1 = string.Format("{0}_10", bs1);
                    break;
                }
                case MaxRangeKeyCount.Count5:
                {
                    bs1 = string.Format("{0}_5", bs1);
                    break;
                }
                case MaxRangeKeyCount.Count2:
                {
                    bs1 = string.Format("{0}_2", bs1);
                    break;
                }
                case MaxRangeKeyCount.Count1:
                {
                    break;
                }
                default:
                {
                    throw new NotImplementedException(Enum.GetName(typeof(MaxRangeKeyCount), maxRangeKeyCount));
                }
            }
            return bs1;
        }


     



        /// <summary>
        /// 返回所有主键的存储过程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string Get_AllMainKeysProcedureName(Entity entity)
        {
            return string.Format("{0}_SELECT_ALL", entity.GetDbViewName());
        }




        /// <summary>
        /// 返回所有主键和版本号的存储过程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string Get_AllKeyAndVerProcedureName(Entity entity)
        {
            return string.Format("{0}_SELECT_ALLKEY_AND_VER", entity.GetDbViewName());
        }

        

        /// <summary>
        /// 返回按主键删除数据的存储过程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string Get_DELStoredProcedureName(Entity entity)
        {
            return string.Format("{0}_DEL", entity.GetDbViewName());
        }

        

        /// <summary>
        /// 返回插入数据的存储过程
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string Get_INSERTStoredProcedureName(Entity entity)
        {
            return string.Format("{0}_INSERT", entity.GetDbViewName());
        }

      


        public static string Get_UPDATEStoredProcedureName(Entity entity)
        {
            return string.Format("{0}_UPDATE", entity.GetDbViewName());
        }





        public static string Get_MainTableForeignKeySelectStoredProcedureName(Entity entity)
        {
            return string.Format("{0}_SELECT_ForeignKey", entity.GetDbViewName());
        }
    }
}
