using LeadTurbo.VirtualDatabase.ColumnEntitys;
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
    /// 可以有MAX选项的类型
    /// </summary>
    [ProtoContract]
    [DataContract]
    public abstract class LengthMaxEntity : LengthEntity
    {
      
    }
}
