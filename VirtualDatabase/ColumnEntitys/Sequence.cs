using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LeadTurbo.VirtualDatabase.ColumnEntitys
{
    /// <summary>
    /// 序列列
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class Sequence: ColumnEntity
    {
        public Sequence()
        {
            this.Name = "Sequence";
            this.Order = 2;
            IsUnique = true;
        }


        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.Sequence);
        }

 
        [ProtoMember(101)]
        [DataMember]
        public override bool PermitNull
        {
            get
            {
                return base.PermitNull;
            }

            set
            {
                if (value == true)
                {
                    base.PermitNull = false;
                }
                else
                {

                    base.PermitNull = value;
                }
            }
        }

     
        [ProtoMember(102)]
        [DataMember]
        public override int Order
        {
            get
            {
                return base.Order;
            }

            set
            {
                base.Order = 2;
            }
        }

 
        [ProtoMember(103)]
        [DataMember]
        public override bool IsUnique
        {
            get
            {
                return base.IsUnique;
            }

            set
            {
                base.IsUnique = true;
            }
        }



    }
}
