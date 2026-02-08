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
    /// 可变长度文本列(小于等于2k)
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class VarChar: LengthMaxEntity
    {
        public VarChar()
        {
            
        }


        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.VarChar);
        }

        public override uint MaxLength()
        {
            return 4000;
        }
        protected override uint DefaultLength()
        {
            return 255;
        }


    }
}
