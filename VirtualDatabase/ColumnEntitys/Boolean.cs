using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.VirtualDatabase.ColumnEntitys
{
    [ProtoContract]
    [DataContract]
    public class Boolean : ColumnEntity
    {
        public Boolean()
        {

        }


        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.Boolean);
        }
    }
}
