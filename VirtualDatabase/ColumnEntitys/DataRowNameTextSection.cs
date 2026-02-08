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
    public class DataRowNameTextSection:VarChar
    {
        public DataRowNameTextSection()
        {
            this.Name = "NameTextSection";
            this.Order = -8;
            Length = 255;
        }



        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.DataRowNameTextSection);
        }



        [ProtoMember(101)]
        public override int Order
        {

            get => base.Order;
            set
            {
                base.Order = -8;
            }
        }

    }
}
