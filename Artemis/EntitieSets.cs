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




        protected DataBasAPP CreateDataBasAPP()
        {
            switch (databasType)
            {
                case DatabasType.SQLite:
                {
                    DataBasAPP app = new DataBasAppSQLite();
                    app.ConnectionString = databaseConnection;
                    return app;
                }
                case DatabasType.SQLServer:
                {
                    DataBasAPP app = new DataBasAppSQL();
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



        public void Initialize()
        {
            foreach (EntitySet entitySet in dictionary.Values)
            {
                DataBasAPP dataBasAPP = CreateDataBasAPP();
                dataBasAPP.ConnectionString = databaseConnection;

                Task task=entitySet.InitializeAsync(dataBasAPP);

            }
        }


        public void SaveEntity(SaveEntityData saveEntityData)
        {
            DataBasAPP dataBasAPP = CreateDataBasAPP();

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
            DataBasAPP dataBasAPP = CreateDataBasAPP();
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
