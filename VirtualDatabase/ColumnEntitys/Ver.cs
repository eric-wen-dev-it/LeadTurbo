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
    [ProtoContract]
    [DataContract]
    public class Ver:ColumnEntity
    {
        public Ver()
        {
            this.Name = "EditVer";
            this.Order = 1;
        }


        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.Ver);
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
                base.Order =1;
            }
        }

      
        [ProtoMember(103)]
        [DataMember]
        public override string Name
        {
            get => base.Name;
            set => base.Name = "EditVer";
        }



    }
}
