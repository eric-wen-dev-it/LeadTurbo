using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.VirtualDatabase.ColumnEntitys
{
    /// <summary>
    /// WCF数据契约实体。
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class ArtemisEntity: Xml
    {
        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.ArtemisEntity);
        }
    }
}
