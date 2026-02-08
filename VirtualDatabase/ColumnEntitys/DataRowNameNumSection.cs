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
    public class DataRowNameNumSection:Integer32
    {
        public DataRowNameNumSection()
        {
            this.Name = "NameNumSection";
            this.Order = -7;
        }
        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.DataRowNameNumSection);
        }

        [ProtoMember(101)]
        public override int Order
        {
            get => base.Order;
            set
            {
                base.Order = -7;
            }
        }
    }
}
