using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LeadTurbo.Edit.Operate;

namespace LeadTurbo.Edit
{
    /// <summary>
    /// 编辑记录类
    /// </summary>
    public class EditRecord
    {
        Stack<Operate[]> operates = new Stack<Operate[]>();
        public void Push(Flag banner, object target, string propertyTarget, object value)
        {
            operates.Push(new Operate[] { new Operate(banner, target, propertyTarget, value) });
        }

        public void Push(Operate operate)
        {
            operates.Push(new Operate[] { operate });
        }

        public void Push(Operate[] operate)
        {
            operates.Push(operate);

        }

        public int Count
        {
            get
            {
                return operates.Count;
            }

        }



        public void Clear()
        {
            operates.Clear();

        }



        /// <summary>
        /// 获得所有编辑对象
        /// </summary>
        /// <returns></returns>
        public Operate[] AllEditData()
        {
            Operate[][] operateArray = operates.ToArray();
            Dictionary<object, Operate.Flag> test = new Dictionary<object, Flag>();


            List<object> orderList = new List<object>();

            foreach (Operate[] operate in operateArray)
            {
                foreach (Operate item in operate)
                {
                    Operate.Flag flag = item.Banner;
                    object target = item.Target;

                    if (item.Banner== Flag.CollectionRemove || item.Banner==Flag.CollectionAdd)
                    {
                        target = item.Value;
                    }


                    if (!test.TryGetValue(target, out flag))
                    {
                        test.Add(target, item.Banner);
                        orderList.Add(target);
                    }

                    if (item.Banner == Flag.CollectionAdd)
                    {
                        test[target] = item.Banner;
                    }

                    if (item.Banner == Flag.CollectionRemove)
                    {
                        test[target] = item.Banner;
                    }
                }
            }

            List<Operate> Return = new List<Operate>();
            foreach(object obj in orderList)
            {
                Operate.Flag flag;
                if(test.TryGetValue(obj,out flag))
                {
                    Operate operate = new Operate(flag, obj, null, null);
                    Return.Add(operate);
                }
            }
            Return.Reverse();

            return Return.ToArray();





        }



        public Operate[] Pop()
        {
            return operates.Pop();

        }


    }
}
