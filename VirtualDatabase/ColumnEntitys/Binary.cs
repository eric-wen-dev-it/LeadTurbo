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
    /// 长度为 n 字节的固定长度二进制数据
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class Binary : LengthEntity
    {

        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.Binary);
        }

        public override uint MaxLength()
        {
            return 8000;
        }

        protected override uint DefaultLength()
        {
            return 16;
        }



    }
}
