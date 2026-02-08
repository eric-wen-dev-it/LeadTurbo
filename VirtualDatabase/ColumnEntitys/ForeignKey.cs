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
    /// 数据从表唯一外键列
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class ForeignKey : ColumnEntity
    {
        public ForeignKey()
        {
            this.Name = "ForeignKey";
            this.Order = -5;
            PermitNull = false;
        }



        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.ForeignKey);
        }
        
        [ProtoMember(101)]
        public override bool PermitNull
        {
            get
            {
                return base.PermitNull;
            }

            set
            {
                base.PermitNull = false;
            }
        }

  
        [ProtoMember(102)]
        public override int Order
        {
            get
            {
                return base.Order;
            }

            set
            {
                base.Order = -5;
            }
        }


    }
}
