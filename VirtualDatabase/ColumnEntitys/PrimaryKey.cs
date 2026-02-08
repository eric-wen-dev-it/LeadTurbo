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
    /// 数据表唯一主key列
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class PrimaryKey : ColumnEntity
    {
        public PrimaryKey()
        {
            this.Name = "PrimaryKey";
            this.Order = 0;
            PermitNull = false;
            IsUnique = true;
        }



        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.PrimaryKey);
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
                base.Order = 0;
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
