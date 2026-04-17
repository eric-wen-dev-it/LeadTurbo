using LeadTurbo.VirtualDatabase.ColumnEntitys;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeadTurbo.VirtualDatabase
{
    public class Columns : ObservableCollectionAndItems<ColumnEntity>
    {
       
        
        
        
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            HashSet<string> oldNames = new HashSet<string>();
            HashSet<string> allNames = new HashSet<string>();

            if (e.NewItems != null)
            {
                if (e.OldItems != null)
                {
                    foreach (ColumnEntity item in e.OldItems)
                    {
                        oldNames.Add(item.Name.ToUpper());
                    }
                }

                foreach (ColumnEntity item in this)
                {
                    foreach (ColumnEntity newItem in e.NewItems)
                    {
                        if (item != newItem)
                        {
                            allNames.Add(item.Name.ToUpper());
                        }
                    }
                }

                allNames.ExceptWith(oldNames);


                foreach (ColumnEntity item in e.NewItems)
                {
                    if (item != null)
                    {

                        ConstraintName(item, allNames);
                    }
                }



            }


            int index = 0;
            foreach (ColumnEntity item in this)
            {
                if (item != null)
                {
                    item.Order = index;
                    index++;
                }
            }

            foreach (ColumnEntity item in this)
            {
                if (item != null)
                {
                    item.ManualPropertyChanged(nameof(item.Order));
                }
                
            }

            base.OnCollectionChanged(e);

            

        }

        void ConstraintName(ColumnEntity constraintValue, HashSet<string> allNames)
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

        public int GetVerIndex()
        {
            int index = 0;
            foreach (ColumnEntity columnEntity in this)
            {
                index++;
                if (columnEntity is Ver)
                {
                    return index;
                }
            }
            return -1;
        }



    }
}