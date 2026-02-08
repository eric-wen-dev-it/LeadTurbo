using LeadTurbo.Artemis.Attributes;
using System.Reflection;


namespace LeadTurbo.Artemis
{ 
    /// <summary>
    /// 按对象属性建立的索引
    /// </summary>
    public abstract class EntityPropertyDictionaryIndex<T> : EntityDictionaryIndex<T>
    {
        readonly Type entityType;
        readonly PropertyInfo propertyInfo;

        public Type EntityType
        {
            get
            {
                return entityType;
            }
        }

        public PropertyInfo PropertyInfo
        {
            get
            {
                return propertyInfo;
            }
        }

        public EntityPropertyDictionaryIndex(Type entityType, PropertyInfo propertyInfo)
        {
            this.entityType = entityType;
            this.propertyInfo = propertyInfo;
        }

        protected override IDictionary<T, HashSet<long>> CreateDictionary()
        {
            if (propertyInfo.IsDefined(typeof(DictionaryIndexPropertyAttribute),false))
            {
                return new Dictionary<T, HashSet<long>>();
            }
            else if (propertyInfo.IsDefined(typeof(SortedDictionaryIndexPropertyAttribute), false))
            {
                return new SortedDictionary<T, HashSet<long>>();
            }
            else
            {
                return new RangeSortedList<T, HashSet<long>>();
            }
        }



        public override string ToString()
        {
            return string.Format("PropertyIndex:{0},{1},{2}", entityType.Namespace, entityType.Name, propertyInfo.Name);
        }


        
    }
}
