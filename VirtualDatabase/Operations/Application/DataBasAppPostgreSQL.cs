using LeadTurbo.Artemis;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace LeadTurbo.VirtualDatabase.Operations.Application
{
    public class DataBasAppPostgreSQL : DataBasApp
    {
        public override DataBasParam CreateDataBasParam()
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
            throw new NotImplementedException();
        }
    }
}
