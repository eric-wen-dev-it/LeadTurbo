using LeadTurbo.VirtualDatabase.ColumnEntitys;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeadTurbo.VirtualDatabase
{
    public class Tables : ObservableCollectionAndItems<Table>
    {
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            HashSet<string> oldNames = new HashSet<string>();
            HashSet<string> allNames = new HashSet<string>();

            if (e.NewItems != null)
            {
                if (e.OldItems != null)
                {
                    foreach (Table item in e.OldItems)
                    {
                        oldNames.Add(item.Name.ToUpper());
                    }
                }

                foreach (Table item in this)
                {
                    foreach (Table newItem in e.NewItems)
                    {
                        if (item != newItem)
                        {
                            allNames.Add(item.Name.ToUpper());
                        }
                    }
                }

                allNames.ExceptWith(oldNames);


                foreach (Table item in e.NewItems)
                {
                    ConstraintName(item, allNames);
                }
            }
            base.OnCollectionChanged(e);
        }

        void ConstraintName(Table constraintValue, HashSet<string> allNames)
        {
            string pattern = @"\d+$";
            int index = 0;
            while (allNames.Contains(constraintValue.Name.ToUpper()))
            {
                index++;
                string result = Regex.Replace(constraintValue.Name, pattern, "");
                constraintValue.Name = $"{result}{index:0}";
            }


        }
    }
}
