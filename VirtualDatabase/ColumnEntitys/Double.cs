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
    /// 双精度浮点数列
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class Double : ColumnEntity
    {
        public Double()
        {
            
        }

        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.Double);
        }
    }
}
