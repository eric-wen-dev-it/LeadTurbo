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
    public class Enumerate64 : Integer64
    {
        public Enumerate64()
        {
            this.Name = "Banner";
            this.Tag = "Flag";
        }

        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.Enumerate64);
        }
    }
}