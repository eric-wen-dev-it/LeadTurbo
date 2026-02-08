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
    /// 最大的二进制存储
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class VarBinaryMax : LengthMaxEntity
    {
        public VarBinaryMax()
        {

        }

        public override uint MaxLength()
        {
            return 4000;
        }

        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.VarBinaryMax);
        }

        protected override uint DefaultLength()
        {
            return 16;
        }
    }
}
