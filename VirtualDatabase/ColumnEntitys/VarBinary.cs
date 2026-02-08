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
    /// 可变长度二进制数据
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class VarBinary : LengthMaxEntity
    {

        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.VarBinary);
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
