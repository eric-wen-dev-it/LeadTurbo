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
    public class Xml : ColumnEntity
    {

        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.Xml);
        }

    }
}
