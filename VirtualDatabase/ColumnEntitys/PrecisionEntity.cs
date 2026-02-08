
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
    /// 需要控制精度的列实体类
    /// </summary>
    [ProtoContract]
    [DataContract]
    public abstract class PrecisionEntity: ColumnEntity
    {
        public PrecisionEntity()
        {
            precision = DefaultPrecision();
        }

        protected abstract uint DefaultPrecision();

        uint precision;
        [ProtoMember(101)]
        [DataMember]
        public uint Precision
        {
            get
            {
                return precision;
            }

            set
            {
                precision = value;
            }
        }




    }
}
