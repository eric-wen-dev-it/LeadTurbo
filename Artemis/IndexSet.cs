using LeadTurbo.Artemis.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis
{
    public class IndexSet : EntityIndexSet
    {
        Type entityType;


        /// <summary>
        /// 索引的实体类型
        /// </summary>
        /// <param name="entityIndex"></param>
        public IndexSet(Type entityType) : base()
        {
            this.entityType = entityType;
        }

        protected Type EntityType
        {
            get
            {
                return entityType;
            }

        }

        protected override EntityIndexBase[] CreateIndexs()
        {
            List<EntityIndexBase> entityIndices = new List<EntityIndexBase>();
            PropertyInfo[] propertyInfos = entityType.GetProperties();
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (!propertyInfo.IsDefined(typeof(NotIndexPropertyAttribute), false))
                {
                    EntityIndexBase? entityIndexBase;
                    if (EntityIndexBase.TryCreate(entityType, propertyInfo, out entityIndexBase))
                    {
                        entityIndexBase.Initialize();
                        entityIndices.Add(entityIndexBase!);
                    }
                }
            }

            return entityIndices.ToArray();


        }
    }
}
