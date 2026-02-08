using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis.Attributes
{
    /// <summary>
    /// 标识出该属性为序列的一部分
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class SequencePropertyAttribute : Attribute
    {
    }
}
