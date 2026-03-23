using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Edit
{
    public class Operate
    {

        public static Operate[] OperateGet(object sender, object[] oldlist, object[] newlist)
        {
            List<Operate> Return = new List<Operate>();
            if (oldlist != null)
            {
                foreach (object item in oldlist)
                {
                    Return.Add(new Operate(Flag.CollectionRemove, sender, "Add", item));
                }
            }

            if (newlist != null)
            {
                foreach (object item in newlist)
                {
                    Return.Add(new Operate(Flag.CollectionAdd, sender, "Remove", item));
                }
            }
            return Return.ToArray();
        }

        public static Operate OperateGet(object sender, string propertyName)
        {
            Flag banner = Flag.ItemPropertyChanged;
            object target = sender;
            string propertyTarget = propertyName;
            Type type = target.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyTarget)
                ?? throw new InvalidOperationException($"Property '{propertyTarget}' not found on type '{type.FullName}'.");
            object value = propertyInfo.GetValue(target);
            return new Operate(banner, target, propertyTarget, value);
        }

        public static void OperateSet(Operate operate)
        {
            switch (operate.Banner)
            {
                case Flag.ItemPropertyChanged:
                {

                    object target = operate.Target;
                    string propertyTarget = operate.propertyTarget;
                    Type type = target.GetType();
                    PropertyInfo propertyInfo = type.GetProperty(propertyTarget)
                        ?? throw new InvalidOperationException($"Property '{propertyTarget}' not found on type '{type.FullName}'.");
                    object value = operate.Value;
                    propertyInfo.SetValue(target, value);
                    break;
                }
                case Flag.CollectionRemove:
                case Flag.CollectionAdd:
                {
                    object target = operate.Target;
                    string propertyTarget = operate.propertyTarget;
                    Type type = target.GetType();
                    MethodInfo propertyInfo = type.GetMethod(propertyTarget)
                        ?? throw new InvalidOperationException($"Method '{propertyTarget}' not found on type '{type.FullName}'.");
                    object value = operate.Value;
                    propertyInfo.Invoke(target, new object[] { value });
                    break;
                }
            }
        }


        


        



        public enum Flag
        {
            CollectionAdd,
            ItemPropertyChanged,
            CollectionRemove,
        }


        Flag banner = Flag.ItemPropertyChanged;
        object target;
        string propertyTarget = "";
        object value = null;

        public Operate(Flag banner, object target, string propertyTarget, object value)
        {
            this.banner = banner;
            this.target = target;
            this.propertyTarget = propertyTarget;
            this.value = value;
        }







        

        
        /// <summary>
        /// 标志
        /// </summary>
        public Flag Banner
        {
            get
            {
                return banner;
            }
            
        }


       
        /// <summary>
        /// 目标对象的属性名
        /// </summary>
        public string PropertyTarget
        {
            get
            {
                return propertyTarget;
            }


        }
        /// <summary>
        /// 值
        /// </summary>
        public object Value
        {
            get
            {
                return value;
            }

        }
        /// <summary>
        /// 目标对象
        /// </summary>
        public object Target
        {
            get
            {
                return target;
            }


        }

        
    }
}
