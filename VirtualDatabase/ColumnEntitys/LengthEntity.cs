using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LeadTurbo.VirtualDatabase.ColumnEntitys
{
    /// <summary>
    /// 有最大长度，或者限制最大长度的实体
    /// </summary>
    [ProtoContract]
    [DataContract]
    public abstract class LengthEntity : ColumnEntity
    {
        uint length;
        public LengthEntity()
        {
            length = DefaultLength();
        }

        protected abstract uint DefaultLength();

        public abstract uint MaxLength();

        [ProtoMember(101)]
        [DataMember]
        public uint Length
        {
            get => length;
           
            set => SetValidatedProperty(ref length, value);

        }

     


    }
}
