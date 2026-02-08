using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis
{
    public class SaveEntityData
    {
        public enum Operation
        {
            Nothing,
            Insert, 
            Update, 
            Delete
        }

        private readonly Entity targetEntity;
        private readonly Operation banner;

        public SaveEntityData(Operation banner, Entity targetEntity)
        {
            this.banner = banner;
            this.targetEntity = targetEntity;
        }


        public Operation Banner
        {
            get
            {
                return banner;
            }

            
        }

        public Entity TargetEntity
        {
            get
            {
                return targetEntity;
            }
        }


        public string TypeNameOfTargetEntity
        {
            get
            {
                return targetEntity.GetType().Name;
            }
        }


    }
}
