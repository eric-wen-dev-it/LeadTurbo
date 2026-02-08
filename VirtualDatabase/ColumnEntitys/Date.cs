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
    /// 日期实体类
    /// </summary>
    [ProtoContract]
    [DataContract]
    public class Date : ColumnEntity
    {
        public Date()
        {
           
        }


        protected override object Create()
        {
            return ColumnEntity.Create(TypeOfData.Date);
        }
    }
}

