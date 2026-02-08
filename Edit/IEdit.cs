using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Edit
{
    public interface IEdit
    {
        void Undo();
        void Redo();


    }
}
