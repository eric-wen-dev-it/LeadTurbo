using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.VirtualDatabase.ColumnEntitys
{
    /// <summary>
    /// 32位整数列
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class Integer32 : ColumnEntity
    {
        public Integer32()
        {
            
        }


        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.Integer32);
        }
    }
}
