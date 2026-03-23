using LeadTurbo.VirtualDatabase.Operations.Application;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Artemis
{
    public abstract class EntitieSets
    {
        FrozenDictionary<string, EntitySet> dictionary = null;

        readonly string databaseConnection;
        readonly DatabasType databasType = DatabasType.Nothing;




        protected DataBasApp CreateDataBasAPP()
        {
            switch (databasType)
            {
                case DatabasType.SQLite:
                {
                    DataBasApp app = new DataBasAppSQLite();
                    app.ConnectionString = databaseConnection;
                    return app;
                }
                case DatabasType.SQLServer:
                {
                    DataBasApp app = new DataBasAppSQLServer();
                    app.ConnectionString = databaseConnection;
                    return app;
                }
                case DatabasType.Nothing:
                {
                    throw new LeadTurbo.Exceptions.AssertException("没有赋值DatabasType");

                }
                default:
                {
                    throw new NotImplementedException();
                }

            }
        }


        protected abstract Dictionary<string, EntitySet> InitializeEntitieDictionary();
        

        public EntitieSets(string databaseConnection, DatabasType databasType)
        {
            this.databaseConnection = databaseConnection;
            this.databasType = databasType;
            var entitieSets = InitializeEntitieDictionary();
            dictionary = entitieSets.ToFrozenDictionary();
        }

        public EntitySet this[string key]
        {
            get
            {
                return dictionary[key];
            }
        }



        public async Task InitializeAsync()
        {
            List<Task> tasks = new List<Task>();
            foreach (EntitySet entitySet in dictionary.Values)
            {
                DataBasApp dataBasAPP = CreateDataBasAPP();
                dataBasAPP.ConnectionString = databaseConnection;
                tasks.Add(entitySet.InitializeAsync(dataBasAPP));
            }
            await Task.WhenAll(tasks);
        }


        public void SaveEntity(SaveEntityData saveEntityData)
        {
            DataBasApp dataBasAPP = CreateDataBasAPP();

            EntitySet entitySet = this[saveEntityData.TypeNameOfTargetEntity];




            try
            {
                dataBasAPP.OpenDataBas();
                switch (saveEntityData.Banner)
                {
                    case SaveEntityData.Operation.Insert:
                    {
                        entitySet.Insert(saveEntityData.TargetEntity, dataBasAPP);
                        break;
                    }
                    case SaveEntityData.Operation.Update:
                    {
                        entitySet.Updated(saveEntityData.TargetEntity, dataBasAPP);
                        break;
                    }
                    case SaveEntityData.Operation.Delete:
                    {
                        entitySet.Remove(saveEntityData.TargetEntity, dataBasAPP);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                throw;
            }
            finally
            {
                dataBasAPP.CloseDataBas();
            }
        }

        public T[] SelectEntity<T>(KeyValuePair<long, int>[] keys) where T: Entity
        {
            DataBasApp dataBasAPP = CreateDataBasAPP();
            EntitySet entitySet = this[typeof(T).Name];
            try
            {
                dataBasAPP.OpenDataBas();
                return entitySet.Select<T>(keys,dataBasAPP);
            }
            finally
            {
                dataBasAPP.CloseDataBas();
            }
        }




    }
}
