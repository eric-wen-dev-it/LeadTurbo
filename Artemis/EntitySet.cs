using LeadTurbo.Artemis.Attributes;
using LeadTurbo.Artemis.IndexFeatures;
using LeadTurbo.Exceptions;
using LeadTurbo.VirtualDatabase.ColumnEntitys;
using LeadTurbo.VirtualDatabase.Operations.Application;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static LeadTurbo.Artemis.Entity;
using static LeadTurbo.Function;

namespace LeadTurbo.Artemis
{
    public abstract class EntitySet : IDisposable
    {
       


        


       


        /// <summary>
        /// 同步锁
        /// </summary>
        protected readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
        /// <summary>
        /// 版本号索引，所有数据
        /// </summary>
        protected readonly Dictionary<long, int> verIndex = new Dictionary<long, int>();
        /// <summary>
        /// 创建实体Cache
        /// </summary>
        protected readonly EntityCache entityCache = new EntityCache();
        protected abstract Entity CreateEntity();
        protected abstract EntityIndexSet CreateEntityIndexSet();

        protected readonly EntityIndexSet indexSet;

        protected readonly string entityName;

        protected readonly Type entityType;

        protected readonly FrozenDictionary<string, PropertyInfo> nameToPropertyInfo;

        /// <summary>
        /// 本Set管理的实体对象名称。
        /// </summary>
        public string EntityName
        {
            get
            {
                return entityName;
            }
        }

        public EntitySet()
        {
           
            indexSet = CreateEntityIndexSet();
            indexSet.Initialize();
            Entity entity = CreateEntity();
            entityType = entity.GetType();
            entityName = entityType.Name;
            PropertyInfo[] propertyInfos = entityType.GetProperties();
            Dictionary<string, PropertyInfo> nameToPropertyInfoTemp = new Dictionary<string, PropertyInfo>(propertyInfos.Length, StringComparer.Ordinal);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (!propertyInfo.IsDefined(typeof(NotIndexPropertyAttribute), false))
                {
                    nameToPropertyInfoTemp.Add(propertyInfo.Name, propertyInfo);
                }
            }
            nameToPropertyInfo = nameToPropertyInfoTemp.ToFrozenDictionary();
        }
        /// <summary>
        /// 按主键返回当前版本
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public virtual int this[long primaryKey]
        {
            get
            {
                readerWriterLock.EnterReadLock();
                try
                {
                    if (!verIndex.TryGetValue(primaryKey, out int ver))
                    {
                        throw new AssertException("未找到对应版本。");
                    }
                    return ver;
                }
                finally
                {
                    readerWriterLock.ExitReadLock();
                }
            }
        }


        /// <summary>
        /// 按主键返回当前版本
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public virtual bool TryGetVer(long primaryKey, out int ver)
        {
            readerWriterLock.EnterReadLock();
            try
            {
                return verIndex.TryGetValue(primaryKey, out ver);
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }
        }



        public virtual KeyValuePair<long, int>[] GetAllKeyAndVer(DataBasApp dataBasAPP)
        {
            readerWriterLock.EnterReadLock();
            try
            {
                return verIndex.ToArray();
            }
            finally
            {
                readerWriterLock.ExitReadLock();
            }
        }

        /// <summary>
        /// 按属性名称查询
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        public KeyValuePair<long, int>[] FindProperty(string propertyName, object feature)
        {
            KeyValuePair<long, int>[] result = Array.Empty<KeyValuePair<long, int>>();
            PropertyInfo propertyInfo;
            if (nameToPropertyInfo.TryGetValue(propertyName, out propertyInfo))
            {
                IndexFeatureBase indexFeatureBase;
                if (IndexFeatureBase.TryCreate(entityType, propertyInfo, feature, out indexFeatureBase))
                {
                    result = Find(indexFeatureBase);
                }
            }
            return result;
        }


        /// <summary>
        /// 按一组属性名称查询,返回交集
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        public KeyValuePair<long, int>[] FindPropertysIntersection(KeyValuePair<string, object>[] parameters)
        {
            Entity entity = CreateEntity();
            List<HashSet<long>> findResults = new List<HashSet<long>>();
            readerWriterLock.EnterReadLock();
            try
            {

                foreach (KeyValuePair<string, object> item in parameters)
                {
                    PropertyInfo propertyInfo;
                    if (nameToPropertyInfo.TryGetValue(item.Key, out propertyInfo))
                    {
                        IndexFeatureBase indexFeatureBase;
                        if (IndexFeatureBase.TryCreate(entityType, propertyInfo, item.Value, out indexFeatureBase))
                        {
                            long[] keys = indexSet.Get(indexFeatureBase);
                            findResults.Add(new HashSet<long>(keys));
                        }
                    }
                }

                HashSet<long>? temp = null;
                foreach (HashSet<long> guids in findResults)
                {
                    if (temp != null)
                    {
                        temp.IntersectWith(guids);
                    }
                    else
                    {
                        temp = guids;
                    }
                }

                List<KeyValuePair<long, int>> _return = new List<KeyValuePair<long, int>>();
                if (temp == null)
                    return _return.ToArray();
                foreach (long guid in temp)
                {
                    int ver;
                    if (verIndex.TryGetValue(guid, out ver))
                    {
                        KeyValuePair<long, int> temp1 = new KeyValuePair<long, int>(guid, ver);
                        _return.Add(temp1);
                    }
                }
                return _return.ToArray();
            }
            finally
            {

                readerWriterLock.ExitReadLock();
            }
        }

        /// <summary>
        /// 单一属性 In 查询
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public KeyValuePair<long, int>[] FindPropertyIn(string propertyName, object[] feature)
        {
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            foreach (object obj in feature)
            {
                parameters.Add(new KeyValuePair<string, object>(propertyName, obj));

            }
            return FindPropertysUnion(parameters.ToArray());
        }



        /// <summary>
        /// 按一组属性名称查询,返回合集
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        public KeyValuePair<long, int>[] FindPropertysUnion(KeyValuePair<string, object>[] parameters)
        {
            Entity entity = CreateEntity();
            List<HashSet<long>> findResults = new List<HashSet<long>>();
            readerWriterLock.EnterReadLock();
            try
            {

                foreach (KeyValuePair<string, object> item in parameters)
                {
                    PropertyInfo propertyInfo;
                    if (nameToPropertyInfo.TryGetValue(item.Key, out propertyInfo))
                    {
                        IndexFeatureBase indexFeatureBase;
                        if (IndexFeatureBase.TryCreate(entityType, propertyInfo, item.Value, out indexFeatureBase))
                        {
                            long[] keys = indexSet.Get(indexFeatureBase);
                            findResults.Add(new HashSet<long>(keys));
                        }
                    }
                }

                HashSet<long>? temp = null;
                foreach (HashSet<long> guids in findResults)
                {
                    if (temp != null)
                    {
                        temp.UnionWith(guids);
                    }
                    else
                    {
                        temp = guids;
                    }
                }

                List<KeyValuePair<long, int>> _return = new List<KeyValuePair<long, int>>();
                if (temp == null)
                    return _return.ToArray();
                foreach (long guid in temp)
                {
                    int ver;
                    if (verIndex.TryGetValue(guid, out ver))
                    {
                        KeyValuePair<long, int> temp1 = new KeyValuePair<long, int>(guid, ver);
                        _return.Add(temp1);
                    }
                }
                return _return.ToArray();
            }
            finally
            {

                readerWriterLock.ExitReadLock();
            }
        }




        /// <summary>
        /// 按一组属性名称查询,返回交集
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        public KeyValuePair<long, int>[] FindPropertysIntersection(string propertyName1, object feature1,
            string propertyName2, object feature2)
        {
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(propertyName1, feature1));
            parameters.Add(new KeyValuePair<string, object>(propertyName2, feature2));

            return FindPropertysIntersection(parameters.ToArray());


        }


        /// <summary>
        /// 按一组属性名称查询,返回交集
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        public KeyValuePair<long, int>[] FindPropertysIntersection(string propertyName1, object feature1,
            string propertyName2, object feature2,
            string propertyName3, object feature3)
        {
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(propertyName1, feature1));
            parameters.Add(new KeyValuePair<string, object>(propertyName2, feature2));
            parameters.Add(new KeyValuePair<string, object>(propertyName3, feature3));

            return FindPropertysIntersection(parameters.ToArray());


        }

        /// <summary>
        /// 按一组属性名称查询,返回交集
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        public KeyValuePair<long, int>[] FindPropertysIntersection(string propertyName1, object feature1,
            string propertyName2, object feature2,
            string propertyName3, object feature3,
            string propertyName4, object feature4)
        {
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(propertyName1, feature1));
            parameters.Add(new KeyValuePair<string, object>(propertyName2, feature2));
            parameters.Add(new KeyValuePair<string, object>(propertyName3, feature3));
            parameters.Add(new KeyValuePair<string, object>(propertyName4, feature4));

            return FindPropertysIntersection(parameters.ToArray());


        }

        /// <summary>
        /// 按一组属性名称查询,返回交集
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        public KeyValuePair<long, int>[] FindPropertysIntersection(string propertyName1, object feature1,
            string propertyName2, object feature2,
            string propertyName3, object feature3,
            string propertyName4, object feature4,
            string propertyName5, object feature5)
        {
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(propertyName1, feature1));
            parameters.Add(new KeyValuePair<string, object>(propertyName2, feature2));
            parameters.Add(new KeyValuePair<string, object>(propertyName3, feature3));
            parameters.Add(new KeyValuePair<string, object>(propertyName4, feature4));
            parameters.Add(new KeyValuePair<string, object>(propertyName5, feature5));
            return FindPropertysIntersection(parameters.ToArray());


        }

        /// <summary>
        /// 按一组属性名称查询,返回交集
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        public KeyValuePair<long, int>[] FindPropertysIntersection(string propertyName1, object feature1,
            string propertyName2, object feature2,
            string propertyName3, object feature3,
            string propertyName4, object feature4,
            string propertyName5, object feature5,
            string propertyName6, object feature6)
        {
            List<KeyValuePair<string, object>> parameters = new List<KeyValuePair<string, object>>();
            parameters.Add(new KeyValuePair<string, object>(propertyName1, feature1));
            parameters.Add(new KeyValuePair<string, object>(propertyName2, feature2));
            parameters.Add(new KeyValuePair<string, object>(propertyName3, feature3));
            parameters.Add(new KeyValuePair<string, object>(propertyName4, feature4));
            parameters.Add(new KeyValuePair<string, object>(propertyName5, feature5));
            parameters.Add(new KeyValuePair<string, object>(propertyName6, feature6));
            return FindPropertysIntersection(parameters.ToArray());


        }

        public KeyValuePair<long, int>[] Find(IndexFeatureBase indexFeatureBase)
        {
            readerWriterLock.EnterReadLock();
            try
            {

                long[] keys = indexSet.Get(indexFeatureBase);
                List<KeyValuePair<long, int>> Return = new List<KeyValuePair<long, int>>();
                foreach (long guid in keys)
                {
                    int ver;
                    if (verIndex.TryGetValue(guid, out ver))
                    {
                        KeyValuePair<long, int> temp = new KeyValuePair<long, int>(guid, ver);
                        Return.Add(temp);
                    }
                }
                return Return.ToArray();


            }
            finally
            {

                readerWriterLock.ExitReadLock();
            }


        }



        /// <summary>
        /// 实体的数量
        /// </summary>
        public int Count
        {
            get
            {
                readerWriterLock.EnterReadLock();
                try
                {

                    return verIndex.Count;
                }
                finally
                {
                    readerWriterLock.ExitReadLock();
                }
            }
        }





        /// <summary>
        /// 未启用线程同步的插入方法
        /// </summary>
        protected virtual void InsertBase(Entity entity, DataBasApp dataBasAPP)
        {
            int ver = 0;
            if (!verIndex.TryGetValue(entity.PrimaryKey, out ver))
            {
                Entity savedEntity = InsertDatabase(entity, dataBasAPP);
                indexSet.Set(savedEntity);
                //新建实体不进行Cache，防止自动编号没有刷新
                //InsertCache(savedEntity);
                verIndex.Add(savedEntity.PrimaryKey, savedEntity.EditVer);
            }
            else
            {
                throw new AssertException("'Insert(Entity entity)' entity对象不可以插入,系统以存在相同PrimaryKey的对象");
            }

        }




        /// <summary>
        /// 插入实体.
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(Entity entity, DataBasApp dataBasAPP)
        {

            if (entity.EditVer != 0)
            {
                throw new AssertException($"'Insert(Entity entity)' entity对象编辑版本必须为0. TYPE:{this.EntityName}");
            }

            //增加编辑版本号
            entity.Upgrade();
            readerWriterLock.EnterWriteLock();
            try
            {

                InsertBase(entity, dataBasAPP);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Updated(Entity entity, DataBasApp dataBasAPP)
        {

            //增加编辑版本号
            entity.Upgrade();
            try
            {
                readerWriterLock.EnterWriteLock();
                int ver = 0;
                if (verIndex.TryGetValue(entity.PrimaryKey, out ver))
                {
                    if (entity.EditVer > ver)
                    {
                        UpdatedDatabase(entity, dataBasAPP);
                        indexSet.Set(entity);
                        UpdatedCache(entity);
                        verIndex[entity.PrimaryKey] = entity.EditVer;
                    }
                    else
                    {
                        throw new AssertException($"'Updated(Entity entity)'试图升版对象版本比系统存在版本旧,升版操作失败.Type:{entity.GetType().FullName} PrimaryKey:{entity.PrimaryKey}。");
                    }
                }
                else
                {
                    throw new AssertException($"Updated(Entity entity)'试图升版不存在的对象,系统不存在相同PrimaryKey的对象.{this.EntityName} {entity.PrimaryKey} {entity.EditVer}");
                }

            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 移除实体
        /// </summary>
        /// <param name="entity"></param>
        public virtual bool Remove(Entity entity, DataBasApp dataBasAPP)
        {

            entity.Upgrade();
            readerWriterLock.EnterWriteLock();
            try
            {

                int ver = 0;
                if (verIndex.TryGetValue(entity.PrimaryKey, out ver))
                {
                    if (entity.EditVer > ver)
                    {
                        RemoveDatabase(entity, dataBasAPP);
                        indexSet.Remove(entity);
                        RemoveCache(entity);
                        verIndex.Remove(entity.PrimaryKey);
                        return true;
                    }
                    else
                    {
                        throw new AssertException("'Delete({0} {1})'试图移除对象版本比系统存在版本旧,移除操作失败。", this.EntityName, entity.PrimaryKey);
                    }
                }
                else
                {
                    return false;
                }

            }
            finally
            {
                readerWriterLock.ExitWriteLock();
                OnRemoved(entity);
            }



        }

        public event EventHandler<EntityRemovedEventArgs> Removed;

        protected virtual void OnRemoved(Entity removedEntity)
        {
            Removed?.Invoke(this, new EntityRemovedEventArgs(removedEntity));
        }




        /// <summary>
        /// 本集合Cache最大缓存数量。
        /// </summary>
        public int MaxCacheCount
        {
            get
            {
                return entityCache.MaxCacheCount;
            }

            set
            {
                entityCache.MaxCacheCount = value;
            }
        }



        /// <summary>
        /// 索引组
        /// </summary>
        public EntityIndexSet IndexSet
        {
            get
            {
                return indexSet;
            }
        }

        

        public Entity[] SelectAll(DataBasApp dataBasAPP)
        {
            return Select(verIndex.ToArray(), dataBasAPP);
        }


        public Entity[] Select(KeyValuePair<long, int>[] keyValuePairs, DataBasApp dataBasAPP)
        {
            List<long> keys = new List<long>();
            foreach (KeyValuePair<long, int> keyValuePair in keyValuePairs)
            {
                keys.Add(keyValuePair.Key);
            }

            return Select(keys.ToArray(), dataBasAPP);
        }





        public T[] Select<T>(KeyValuePair<long, int>[] keyValuePairs, DataBasApp dataBasAPP) where T : Entity
        {
            Entity[] entities = Select(keyValuePairs, dataBasAPP);
            List<T> list = new List<T>();
            foreach (T item in entities)
            {
                list.Add(item);
            }
            return list.ToArray();
        }

        public T[] Select<T>(long[] primaryKeys, DataBasApp dataBasAPP) where T : Entity
        {
            Entity[] entities = Select(primaryKeys, dataBasAPP);
            List<T> list = new List<T>();
            foreach (T item in entities)
            {
                list.Add(item);
            }
            return list.ToArray();
        }

        public T Select<T>(long primaryKey, DataBasApp dataBasAPP) where T : Entity
        {
            Entity[] entities = Select(new long[] { primaryKey }, dataBasAPP);

            if (entities.Length == 1)
            {
                return entities[0] as T;
            }
            else if (entities.Length < 1)
            {
                return null;
            }
            else
            {
                throw new AssertException("竟然返回多于一个的实体");

            }

        }





        /// <summary>
        /// 按主Key挑选数据
        /// </summary>
        /// <param name="primaryKeys"></param>
        /// <returns></returns>
        public virtual Entity[] Select(long[] primaryKeys, DataBasApp dataBasAPP)
        {


            HashSet<long> keys = new HashSet<long>(primaryKeys);
            readerWriterLock.EnterUpgradeableReadLock();
            try
            {

                Entity[] entitysCache = SelectCache(primaryKeys);

                foreach (Entity item in entitysCache)
                {
                    keys.Remove(item.PrimaryKey);
                }

                Entity[] entitysDb = new Entity[0];

                if (keys.Count > 0)
                {
                    readerWriterLock.EnterWriteLock();
                    try
                    {
                        entitysDb = SelectDatabase(keys.ToArray(), dataBasAPP);
                        if (entitysDb.Length > 0)
                        {
                            foreach (Entity item in entitysDb)
                            {
                                InsertCache(item);
                            }
                        }

                    }
                    finally
                    {

                        readerWriterLock.ExitWriteLock();
                    }



                }

                Dictionary<long, Entity> dictionary = new Dictionary<long, Entity>();

                foreach (Entity item in entitysCache)
                {
                    if (!dictionary.ContainsKey(item.PrimaryKey))
                    {
                        dictionary.Add(item.PrimaryKey, item);
                    }
                }

                foreach (Entity item in entitysDb)
                {
                    if (!dictionary.ContainsKey(item.PrimaryKey))
                    {
                        dictionary.Add(item.PrimaryKey, item);
                    }
                }

                List<Entity> Return = new List<Entity>();
                foreach (long key in primaryKeys)
                {
                    Entity temp;
                    if (dictionary.TryGetValue(key, out temp))
                    {
                        Return.Add(temp);
                    }
                }

                return Return.ToArray();



            }
            finally
            {
                readerWriterLock.ExitUpgradeableReadLock();
            }

        }

        protected void InsertCache(Entity entity)
        {
            if (entityCache.MaxCacheCount > 0)
            {
                entityCache.Add(entity);
            }

        }

        protected void UpdatedCache(Entity entity)
        {
            if (entityCache.MaxCacheCount > 0)
            {
                entityCache.Updated(entity);
            }
        }

        protected void RemoveCache(Entity entity)
        {
            entityCache.Remove(entity);
        }

        protected Entity[] SelectCache(long[] primaryKey)
        {
            List<Entity> entities = new List<Entity>();
            foreach (long key in primaryKey)
            {
                Entity temp;
                if (entityCache.TryGet(key, out temp))
                {
                    entities.Add(temp);
                }

            }

            return entities.ToArray();

        }




        protected virtual Entity InsertDatabase(Entity entity, DataBasApp dataBasApp)
        {
            if (!(entity is View))
            {
                dataBasApp.InsertEntityInDatabase(entity);

                Entity[] entities = SelectDatabase(new long[] { entity.PrimaryKey }, dataBasApp);
                if (entities != null && entities.Length == 1)
                {
                    return entities[0];
                }
                else
                {
                    throw new AssertException("长度竟然不是1");
                }
            }
            else
            {
                throw new AssertException("View 不可以Insert");
            }
        }


        protected virtual void RemoveDatabase(Entity entity, DataBasApp dataBasApp)
        {
            if (!(entity is View))
            {
                dataBasApp.DeleteEntityInDatabase(entity);
            }
            else
            {
                throw new AssertException("View 不可以Remove");
            }

        }


        protected virtual void UpdatedDatabase(Entity entity, DataBasApp dataBasApp)
        {


            if (!(entity is View))
            {
                dataBasApp.UpdateEntityInDatabase(entity);
            }
            else
            {
                throw new AssertException("View 不可以Updated");
            }

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dataBasApp"></param>
        /// 



        Queue<Entity> initializeQueue = new Queue<Entity>();
        bool IsfillDBItemEnd = false;
        public virtual void Initialize(DataBasApp dataBasApp)
        {
            Entity entity = CreateEntity();

            KeyValuePair<long,int>[] keyValues= dataBasApp.GetAllKeyAndVer(entity);

            readerWriterLock.EnterWriteLock();
            try
            {
                //安装所有主键和版本。

                foreach (KeyValuePair<long, int> item in keyValues)
                {
                    if (!verIndex.ContainsKey(item.Key))
                    {
                        verIndex.Add(item.Key,item.Value);
                    }
                    else
                    {
                        throw new AssertException("竟然有重复主键。");
                    }

                }



                int maxCacheCount = this.MaxCacheCount;
                this.MaxCacheCount = 0;//下面装载索引，禁用Chache.

                List<List<long>> keys = Function.RangeKey<long>(new List<long>(verIndex.Keys), Function.MaxRangeKeyCount.Count50);

                int count = 0;
                Debug.WriteLine($"{this.EntityName} Begin");

                foreach (List<long> list in keys)
                {
                    Entity[] temp = InitializeFillDBItem(dataBasApp, list.ToArray());

                    foreach (Entity item in temp)
                    {
                        InitializeIndexSet(item);
                        count++;
                    }
                }



                this.MaxCacheCount = maxCacheCount;//恢复Chache功能。

                Debug.WriteLine($"{this.EntityName} end");

            }
            finally
            {
                readerWriterLock.ExitWriteLock();
            }
        }

        public async Task InitializeAsync(DataBasApp dataBasApp)
        {
            await Task.Run(() =>
            {
                try
                {
                    dataBasApp.OpenDataBas();
                    Initialize(dataBasApp);
                }
                finally
                {
                    dataBasApp.CloseDataBas();
                }
            });
        }




        protected virtual void InitializeIndexSet(Entity entity)
        {
            indexSet.Set(entity);


        }







        /// <summary>
        /// 初始化填充主key
        /// </summary>
        /// <param name="dataBasApp"></param>
        /// <param name="mainKeys"></param>
        /// <returns></returns>
        Entity[] InitializeFillDBItem(DataBasApp dataBasApp, long[] mainKeys)
        {

                Entity[] entitys = dataBasApp.LoadEntitysFormDatabaseForInitializationResult(CreateEntity, ((MaxRangeKeyCount)Enum.Parse(typeof(MaxRangeKeyCount), string.Format("Count{0}", mainKeys.Length))), mainKeys);


                Dictionary<long, Entity> tempData = new Dictionary<long, Entity>();
                foreach (Entity entity in entitys)
                {
                    if (!tempData.ContainsKey(entity.PrimaryKey))
                    {
                        tempData.Add(entity.PrimaryKey, entity);
                    }
                }

                List<Entity> _return = new List<Entity>();
                foreach (long key in mainKeys)
                {
                    Entity outEntity;
                    if (tempData.TryGetValue(key, out outEntity))
                    {
                        _return.Add(outEntity);

                    }

                }

                return _return.ToArray();
        }





        /// <summary>
        /// 填充主key
        /// </summary>
        /// <param name="dataBasApp"></param>
        /// <param name="mainKeys"></param>
        /// <returns></returns>
        Entity[] FillDBItem(DataBasApp dataBasApp, long[] mainKeys)
        {
            return dataBasApp.LoadEntitysFormDatabase(CreateEntity, ((MaxRangeKeyCount)Enum.Parse(typeof(MaxRangeKeyCount), string.Format("Count{0}", mainKeys.Length))), mainKeys);


        }

        Task DbDataToEntityAsync(object[][] scr, ConcurrentDictionary<long, Entity> collect)
        {
            return Task.Run(() => DbDataToEntity(scr, collect));
        }

        void DbDataToEntity(object[][] scr, ConcurrentDictionary<long, Entity> collect)
        {
            Parallel.ForEach(scr, item =>
            {
                var entity = CreateEntity();
                ProtobufNetHelper.ApplyMemberDataList(entity, item);
                collect.GetOrAdd(entity.PrimaryKey, entity);
            });
        }



        protected virtual Entity[] SelectDatabase(long[] primaryKey, DataBasApp dataBasApp)
        {
            List<List<long>> keys = Function.RangeKey<long>(new List<long>(primaryKey), Function.MaxRangeKeyCount.Count50);
            List<Entity> Return = new List<Entity>();

            foreach (List<long> list in keys)
            {
                Entity[] temp = FillDBItem(dataBasApp, list.ToArray());
                Return.AddRange(temp);
            }
            return Return.ToArray();



        }

        public static HashSet<long> ToHashSet(KeyValuePair<long, int>[] keyValuePairs)
        {
            HashSet<long> _return = new HashSet<long>();
            foreach (KeyValuePair<long, int> item in keyValuePairs)
            {
                _return.Add(item.Key);

            }
            return _return;
        }

        public static Guid[] ToArray(KeyValuePair<Guid, int>[] keyValuePairs)
        {
            List<Guid> _return = new List<Guid>();
            foreach (KeyValuePair<Guid, int> item in keyValuePairs)
            {
                _return.Add(item.Key);

            }
            return _return.ToArray();
        }

        public static object[] ToObjArray(KeyValuePair<Guid, int>[] keyValuePairs)
        {
            List<object> _return = new List<object>();
            foreach (KeyValuePair<Guid, int> item in keyValuePairs)
            {
                _return.Add(item.Key);

            }
            return _return.ToArray();
        }


        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    entityCache.Dispose();
                    readerWriterLock.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~EntitySet() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion





    }
}
