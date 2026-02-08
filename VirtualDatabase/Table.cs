using LeadTurbo.Artemis;
using LeadTurbo.VirtualDatabase.ColumnEntitys;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.VirtualDatabase
{
    [DataContract]
    [ProtoContract]
    public class Table : Entity
    {
        readonly Columns columns = new Columns();

        protected override object Create()
        {
            return new Table();
        }

        string name = string.Empty;
        [DataMember]
        [ProtoMember(101)]
        [StringLength(50)]
        [Key]
        [Required]
        public string Name
        {
            get => name;
            set => SetValidatedProperty(ref name, value);
        }
        [DataMember]
        [ProtoMember(102)]
        public Columns Columns
        {
            get
            {
                return columns;
            }
        }


        public bool HasSequence()
        {
            bool result = false;
            foreach (var column in columns)
            {
                if (column is Sequence)
                {
                    result = true;
                }
            }
            return result;
        }
            

    }
}
