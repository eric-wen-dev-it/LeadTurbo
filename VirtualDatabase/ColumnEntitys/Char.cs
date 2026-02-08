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
    /// 固定长度文本列
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class Char: LengthEntity
    {
        public Char()
        {
            
        }


        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.Char);
        }

        protected override uint DefaultLength()
        {
            return 255;
        }

        public override uint MaxLength()
        {
            return 4000;
        }
    }
}
