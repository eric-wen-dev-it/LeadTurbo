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
    /// 全局唯一标识符列
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class Uuid : ColumnEntity
    {
        public Uuid()
        {
           
        }



        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.Uuid);
        }
    }
}
