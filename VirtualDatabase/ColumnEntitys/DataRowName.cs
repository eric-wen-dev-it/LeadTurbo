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
    public class DataRowName : VarChar
    {
        public DataRowName()
        {
            this.Name = "Name";
            this.Order = -9;
            PermitNull = false;
            IsUnique = true;
            
        }

        [StringLength(50)]
        [ProtoMember(101)]
        public override string Name
        {
            get => base.Name;
            set => base.Name = "Name";
        }

        protected override uint DefaultLength()
        {
            return 255;
        }

       


        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.DataRowName);
        }

       
        [ProtoMember(102)]
        public override bool PermitNull
        {

            get => base.PermitNull;
            set
            {
                base.PermitNull = false;
            }
        }

        /// <summary>
        /// 是否可以是唯一值
        /// </summary>
       
        [ProtoMember(103)]
        public override bool IsUnique
        {
            get => base.IsUnique;
            set
            {
                base.IsUnique = true;
            }
        }

        [ProtoMember(104)]
        public override int Order
        {

            get => base.Order;
            set
            {
                base.Order = -9;
            }
        }


    }
}
