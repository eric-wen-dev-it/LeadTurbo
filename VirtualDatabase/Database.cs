using LeadTurbo.Artemis;
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
    public class Database : Entity
    {
        protected override object Create()
        {
            return new Database();
        }

        string name=string.Empty;
        [DataMember]
        [StringLength(50)]
        [ProtoMember(101)]
        public string Name
        {
            get => name;
            set=>SetValidatedProperty(ref name, value);
        }

        string connectString = string.Empty;
        [DataMember]
        [ProtoMember(102)]
        public string ConnectString
        {
            get => connectString;
            set => SetValidatedProperty(ref connectString, value);
        }


        string rootNamespace = string.Empty;
        [DataMember]
        [ProtoMember(103)]
        public string RootNamespace
        {
            get => rootNamespace;
            set => SetValidatedProperty(ref rootNamespace, value);
        }



        readonly Tables tables = new Tables();
        [DataMember]
        [ProtoMember(104)]
        public Tables Tables
        {
            get=>tables;
        }



        bool changed=false;
        [DataMember]
        public bool Changed
        {
            get => changed;
            set => SetValidatedProperty(ref changed, value);
        }

    }
}
