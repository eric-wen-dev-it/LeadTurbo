using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Data.Common;
using LeadTurbo.Artemis;

namespace LeadTurbo.VirtualDatabase.Operations.Application
{
    /// <summary>
    /// SQL SERVER数据库应用。
    /// </summary>
    public class DataBasAppSQL : DataBasApp
    {
        public override DataBasApp.DataBasParam CreateDataBasParam()
        {
            throw new NotImplementedException();
        }

        public override int DeleteEntityInDatabase(Entity entity)
        {
            throw new NotImplementedException();
        }

        public override KeyValuePair<long, int>[] GetAllKeyAndVer(Entity entity)
        {
            throw new NotImplementedException();
        }

        public override int InsertEntityInDatabase(Entity entity)
        {
            throw new NotImplementedException();
        }

        public override Entity[] LoadEntitysFormDatabase(Func<Entity> creator, Function.MaxRangeKeyCount rangeKeyCount, long[] primaryKeys)
        {
            throw new NotImplementedException();
        }

        public override Entity[] LoadEntitysFormDatabaseForInitializationResult(Func<Entity> creator, Function.MaxRangeKeyCount rangeKeyCount, long[] primaryKeys)
        {
            throw new NotImplementedException();
        }

        public override bool TryLoadEntityFormDatabase<T>(long primaryKey, out T entity)
        {
            throw new NotImplementedException();
        }

        public override int UpdateEntityInDatabase(Entity entity)
        {
            throw new NotImplementedException();
        }

        protected override DbConnection CreateDbConnection()
        {
            return new SqlConnection();
        }


    }
}
