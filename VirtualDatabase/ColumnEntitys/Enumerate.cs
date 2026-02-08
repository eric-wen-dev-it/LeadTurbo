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
    public class Enumerate : Integer32
    {
        public Enumerate()
        {
            this.Name = "Banner";
            this.Tag = "Flag";
        }

        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.Enumerate);
        }
    }
}