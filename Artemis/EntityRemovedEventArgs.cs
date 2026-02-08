using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis
{
    public class EntityRemovedEventArgs : EventArgs
    {
        public Entity RemovedEntity
        {
            get;
        }

        public EntityRemovedEventArgs(Entity removedEntity)
        {
            RemovedEntity = removedEntity;
        }
    }
}
