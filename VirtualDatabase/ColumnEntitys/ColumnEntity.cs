using LeadTurbo.Artemis;
using ProtoBuf;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace LeadTurbo.VirtualDatabase.ColumnEntitys
{
    public enum TypeOfData
    {
        Nothing,
        ArtemisEntity,
        Binary,
        Boolean,
        Char,
        DataRowName,
        DataRowNameNumSection,
        DataRowNameTextSection,
        Date,
        DateTime,
        Decimal,
        Double,
        Enumerate,
        Enumerate64,
        ForeignKey,
        Integer32,
        Integer64,
        Json,
        PrimaryKey,
        Sequence,
        Uuid,
        VarBinary,
        VarBinaryMax,
        VarChar,
        Ver,
        Xml

    }



    /// <summary>
    /// 列实体类
    /// </summary>
    [DataContract]
    [ProtoContract]
    public abstract class ColumnEntity : Entity
    {
        public static ColumnEntity Create(TypeOfData typeOfData)
        {
            switch (typeOfData)
            {
                case TypeOfData.Binary:
                {
                    return new Binary();
                }
                case TypeOfData.ForeignKey:
                {
                    return new ForeignKey();
                }
                case TypeOfData.Boolean:
                {
                    return new Boolean();
                }
                case TypeOfData.Char:
                {
                    return new Char();
                }
                case TypeOfData.Date:
                {
                    return new Date();
                }
                case TypeOfData.DateTime:
                {
                    return new DateTime();
                }
                case TypeOfData.Decimal:
                {
                    return new Decimal();
                }
                case TypeOfData.Double:
                {
                    return new Double();
                }

                case TypeOfData.Integer32:
                {
                    return new Integer32();
                }
                case TypeOfData.Integer64:
                {
                    return new Integer64();
                }
                case TypeOfData.Json:
                {
                    return new Json();
                }
                case TypeOfData.PrimaryKey:
                {
                    return new PrimaryKey();
                }
                case TypeOfData.Sequence:
                {
                    return new Sequence();
                }
                case TypeOfData.ArtemisEntity:
                {
                    return new ArtemisEntity();
                }
                case TypeOfData.Enumerate:
                {
                    return new Enumerate();
                }
                case TypeOfData.Enumerate64:
                {
                    return new Enumerate64();
                }
                case TypeOfData.DataRowName:
                {
                    return new DataRowName();
                }
                case TypeOfData.DataRowNameNumSection:
                {
                    return new DataRowNameNumSection();
                }
                case TypeOfData.DataRowNameTextSection:
                {
                    return new DataRowNameTextSection();
                }

                case TypeOfData.Uuid:
                {
                    return new Uuid();
                }
                case TypeOfData.VarBinary:
                {
                    return new VarBinary();
                }
                case TypeOfData.VarBinaryMax:
                {
                    return new VarBinaryMax();
                }
                case TypeOfData.VarChar:
                {
                    return new VarChar();
                }
                case TypeOfData.Xml:
                {
                    return new Xml();
                }
                case TypeOfData.Ver:
                {
                    return new Ver();
                }

                default:
                {
                    throw new NotImplementedException(string.Format("{0}", typeOfData.ToString()));
                }
            }
        }
        protected new static int Comparable(object a, object b)
        {
            ColumnEntity A = a as ColumnEntity;
            ColumnEntity B = b as ColumnEntity;
            if (A == null && B == null)
            {
                return 0;
            }
            else if (A == null)
            {
                return -1;
            }
            else if (B == null)
            {
                return 1;
            }
            else
            {
                int Return = A.order.CompareTo(B.order);
                if (Return == 0)
                {
                    Return = Entity.Comparable(a, b);
                }
                return Return;
            }
        }




        string name;
        [ProtoMember(101)]
        [DataMember]
        [Key]
        [Required]
        public virtual string Name
        {
            get => name;
            set => SetValidatedProperty(ref name, value);
        }



        public string FieldName
        {
            get
            {

                string fieldName = PropertyName;

                if (fieldName[0] > 255)
                {
                    fieldName = "_" + fieldName;
                }
                else
                {
                    char[] temp = fieldName.ToCharArray();
                    temp[0] = char.ToLower(temp[0]);
                    fieldName = new string(temp);
                }

                return fieldName;
            }
        }

        public string PropertyName
        {
            get
            {
                string propertyName = Name.Trim();
                Regex regex = new Regex(@"[\W\s]");
                propertyName = regex.Replace(propertyName, "_");

                if (propertyName[0] <= 255)
                {
                    char[] temp = propertyName.ToCharArray();
                    temp[0] = char.ToUpper(temp[0]);
                    propertyName = new string(temp);
                }

                return propertyName;
            }
        }

        bool permitNull = false;
        int order = 2;
        public ColumnEntity()
        {
            Name = "Column";
        }


        [DataMember]
        public string TypeName
        {
            get
            {

                Type type = this.GetType();
                return type.Name;
            }

        }


        [ProtoMember(102)]
        [DataMember]
        public virtual int Order
        {
            get => order;
            set => SetValidatedProperty(ref order, value);

        }
        /// <summary>
        /// 是否可以是Null
        /// </summary>
        [ProtoMember(103)]
        [DataMember]
        public virtual bool PermitNull
        {
            get => permitNull;
            set => SetValidatedProperty(ref permitNull, value);
        }


        bool isUnique = false;
        /// <summary>
        /// 是否可以是唯一值
        /// </summary>
        [ProtoMember(104)]
        [DataMember]
        public virtual bool IsUnique
        {
            get => isUnique;
            set => SetValidatedProperty(ref isUnique, value);
        }
        /// <summary>
        /// 附加
        /// </summary>

        string tag = "";
        [ProtoMember(105)]
        [DataMember]
        public string Tag
        {
            get => tag;
            set => SetValidatedProperty(ref tag, value);

        }







        public override int CompareTo(object obj)
        {
            return Comparable(this, obj);
        }


    }
}
