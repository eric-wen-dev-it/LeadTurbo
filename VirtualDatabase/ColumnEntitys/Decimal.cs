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
    /// 十进制数列
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class Decimal: PrecisionEntity
    {
        public Decimal()
        {
            
        }

        protected override uint DefaultPrecision()
        {
            return 4;
        }

      

        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.Decimal);
        }
    }
}
