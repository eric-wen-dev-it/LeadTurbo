using LeadTurbo.Edit;
using LeadTurbo.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using static LeadTurbo.Edit.Operate;

namespace LeadTurbo
{

    
    public class ObservableCollectionAndItems<T> : ObservableCollection<T>, IEdit where T : INotifyPropertyChanged, INotifyPropertyChanging
    {

        bool isChanged = false;

        readonly Guid primaryKey = Guid.NewGuid();



        public T[] ToArray()
        {
            T[] Return = new T[this.Count];
            this.CopyTo(Return, 0);
            return Return;
        }

        public ObservableCollectionAndItems(IEnumerable<T> collection) : base(collection)
        {
            foreach (T item in collection)
            {
                if (item != null)
                {
                    item.PropertyChanging += Item_PropertyChanging;
                    item.PropertyChanged += Item_PropertyChanged;
                }
                //else
                {
                    //    throw new Jiamparts.Exceptions.AssertException("集合中不可以存在Null值");

                }

            }
            if (this.Count > 0)
            {
                this.current = this[0];
            }
            else
            {
                this.current = default(T);
            }

            this.CollectionChanged += ObservableCollectionAndItems_CollectionChanged;


        }



        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                this.Add(item);
            }
        }

        public ObservableCollectionAndItems(List<T> list) : base(list)
        {
            foreach (T item in list)
            {
                item.PropertyChanging += Item_PropertyChanging;
                item.PropertyChanged += Item_PropertyChanged;

            }
            if (this.Count > 0)
            {
                this.current = this[0];
            }
            this.CollectionChanged += ObservableCollectionAndItems_CollectionChanged;

        }



        public ObservableCollectionAndItems() : base()
        {
            this.CollectionChanged += ObservableCollectionAndItems_CollectionChanged;

        }



        public event PropertyChangedEventHandler ItemPropertyChanged;

        T current = default(T);


        /// <summary>
        /// 当前对象。
        /// </summary>
        
        public T Current
        {
            get
            {
                return current;
            }
            set
            {
                if (value != null)
                {
                    if (this.IndexOf(value) < 0)
                    {
                        //throw new AssertException("设置当前对象非本集合对象。");
                        current = default(T);
                    }
                    else
                    {
                        current = value;
                    }

                    OnPropertyChanged(new PropertyChangedEventArgs("Current"));
                }



            }
        }

        /// <summary>
        /// 是否可以将当前向下移动一个
        /// </summary>
        public bool IsCurrentMoveNext
        {
            get
            {
                bool _return = false;
                int index = this.IndexOf(this.Current);
                if (index < this.Count - 1)
                {
                    _return = true;
                }
                return _return;
            }
        }


        /// <summary>
        /// 将当前移动到下一个
        /// </summary>
        /// <returns></returns>
        public bool CurrentMoveNext()
        {
            if (IsCurrentMoveNext)
            {
                int index = this.IndexOf(this.Current);
                T current = this[(index + 1)];
                this.Current = current;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否可以将当前向上移动一个
        /// </summary>
        public bool IsCurrentMovePrevious
        {
            get
            {
                bool _return = false;
                int index = this.IndexOf(this.Current);
                if (index > 0)
                {
                    _return = true;
                }
                return _return;
            }
        }


        /// <summary>
        /// 将当前移动到上一个
        /// </summary>
        /// <returns></returns>
        public bool CurrentMovePrevious()
        {
            if (IsCurrentMovePrevious)
            {
                int index = this.IndexOf(this.Current);
                T current = this[(index - 1)];
                this.Current = current;
                return true;
            }
            return false;
        }



        List<T> selectedItems = new List<T>();
        public IList SelectedItems
        {
            get
            {
                return selectedItems;
            }
            set
            {
                selectedItems = new List<T>();
                foreach (T item in value)
                {
                    if (this.IndexOf(item) < 0)
                    {
                        throw new AssertException("设置当前对象非本集合对象。");
                    }
                    selectedItems.Add(item);
                }
                OnPropertyChanged(new PropertyChangedEventArgs("SelectedItems"));
            }

        }
        /// <summary>
        /// 自集合建立以来内容是否发生过变化
        /// </summary>
        public bool IsChanged
        {
            get
            {
                return isChanged;
            }

        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            isChanged = true;
        }





        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);




            IList oldlist = e.OldItems;
            if (oldlist != null)
            {
                foreach (T item in oldlist)
                {
                    item.PropertyChanging -= Item_PropertyChanging;
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }

            IList newlist = e.NewItems;

            if (newlist != null)
            {
                Current = (T)newlist[0];

                foreach (T item in newlist)
                {
                    if (item != null)
                    {
                        item.PropertyChanging += Item_PropertyChanging;
                        item.PropertyChanged += Item_PropertyChanged;
                    }
                }
            }




            isChanged = true;
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ItemPropertyChanged != null)
            {
                ItemPropertyChanged(sender, e);
            }

            isChanged = true;

        }

        EditRecord undoEditRecord = new EditRecord();


        /// <summary>
        /// 获得所有编辑对象
        /// </summary>
        /// <returns></returns>
        public Operate[] AllEditData()
        {
            return undoEditRecord.AllEditData();
        }


        public void UndoEditClear()
        {
            undoEditRecord.Clear();

        }


        private void Item_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            undoEditRecord.Push(Operate.OperateGet(sender, e.PropertyName));
        }

        private void ObservableCollectionAndItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<object> oldlistPush = new List<object>();
            if (e.OldItems != null)
            {
                oldlistPush = new List<object>(e.OldItems.OfType<object>());
            }
            List<object> newlistPush = new List<object>();
            if (e.NewItems != null)
            {
                newlistPush = new List<object>(e.NewItems.OfType<object>());
            }
            if (newlistPush.Count > 0 && oldlistPush.Count > 0)
            {
                undoEditRecord.Push(Operate.OperateGet(this, oldlistPush.ToArray(), newlistPush.ToArray()));
            }
        }

        public bool IsUndo
        {
            get
            {
                if (undoEditRecord.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }

        public Guid PrimaryKey
        {
            get
            {
                return primaryKey;
            }
        }

        /// <summary>
        /// 撤销
        /// </summary>
        public void Undo()
        {
            if (undoEditRecord.Count > 0)
            {

                Operate[] operate = undoEditRecord.Pop();


                foreach (Operate item in operate)
                {
                    if (item.Banner == Flag.ItemPropertyChanged)
                    {
                        INotifyPropertyChanging notifyPropertyChanging = (INotifyPropertyChanging)item.Target;
                        notifyPropertyChanging.PropertyChanging -= Item_PropertyChanging;
                    }
                }
                this.CollectionChanged -= ObservableCollectionAndItems_CollectionChanged;

                foreach (Operate item in operate)
                {
                    IEdit edit = item as IEdit;
                    if (edit != null)
                    {
                        edit.Undo();
                    }
                    else
                    {
                        Operate.OperateSet(item);
                    }
                }


                foreach (Operate item in operate)
                {
                    if (item.Banner == Flag.ItemPropertyChanged)
                    {
                        INotifyPropertyChanging notifyPropertyChanging = (INotifyPropertyChanging)item.Target;
                        notifyPropertyChanging.PropertyChanging += Item_PropertyChanging;
                    }
                }

                this.CollectionChanged += ObservableCollectionAndItems_CollectionChanged;



            }



        }

        /// <summary>
        /// 重做
        /// </summary>
        public void Redo()
        {
            throw new NotImplementedException();
        }
    }
}
