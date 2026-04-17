using LeadTurbo.Artemis;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace LeadTurbo.VirtualDatabase.Operations.Application
{
    /// <summary>
    /// SQL SERVER 数据库应用。
    /// 与 SQLite 版的差异：
    ///   - 直接调用真实存储过程（CommandType.StoredProcedure），不再从 [Procedures] 表里取 SQL 文本；
    ///   - 不做参数类型转换，SqlClient 原生支持 DateTime/Guid/Decimal 等，仅做 null → DBNull 兜底。
    /// </summary>
    public class DataBasAppSQLServer : DataBasApp
    {
        protected override DbConnection CreateDbConnection()
        {
            return new SqlConnection();
        }

        public override int InsertEntityInDatabase(Entity entity)
        {
            string procedureName = Entity.Get_INSERTStoredProcedureName(entity);
            this.Param.Default();
            this.Param.CommandType = ExecuteType.Procedure;
            this.Param.Command = procedureName;
            MemberData[] memberDatas = ProtobufNetHelper.GetMembersWithValues(entity);
            foreach (MemberData memberData in memberDatas)
            {
                this.Param.AddParam(memberData.Value);
            }
            return this.ExecuteProcedure();
        }

        public override int UpdateEntityInDatabase(Entity entity)
        {
            string procedureName = Entity.Get_UPDATEStoredProcedureName(entity);
            this.Param.Default();
            this.Param.CommandType = ExecuteType.Procedure;
            this.Param.Command = procedureName;
            MemberData[] memberDatas = ProtobufNetHelper.GetMembersWithValues(entity);
            foreach (MemberData memberData in memberDatas)
            {
                this.Param.AddParam(memberData.Value);
            }
            return this.ExecuteProcedure();
        }

        public override int DeleteEntityInDatabase(Entity entity)
        {
            string procedureName = Entity.Get_DELStoredProcedureName(entity);
            this.Param.Default();
            this.Param.CommandType = ExecuteType.Procedure;
            this.Param.Command = procedureName;
            // Delete 过程只需 PK + Ver 两个参数（Ver 用于乐观并发校验）
            this.Param.AddParam(entity.PrimaryKey);
            this.Param.AddParam(entity.EditVer);
            return this.ExecuteProcedure();
        }

        public override KeyValuePair<long, int>[] GetAllKeyAndVer(Entity entity)
        {
            string procedureName = Entity.Get_AllKeyAndVerProcedureName(entity);
            this.Param.Default();
            this.Param.CommandType = ExecuteType.Procedure;
            this.Param.Command = procedureName;

            List<KeyValuePair<long, int>> result = new List<KeyValuePair<long, int>>();
            using (DbCommand dbCommand = this.CreateProcedureReaderCommand())
            using (DbDataReader dbDataReader = dbCommand.ExecuteReader())
            {
                while (dbDataReader.Read())
                {
                    KeyValuePair<long, int> keyValuePair = new KeyValuePair<long, int>(
                        dbDataReader.GetInt64(0),
                        dbDataReader.GetInt32(1));
                    result.Add(keyValuePair);
                }
            }
            return result.ToArray();
        }

        public override bool TryLoadEntityFormDatabase<T>(long primaryKey, out T entity)
        {
            bool result = false;
            entity = new T();

            string procedureName = Entity.GetDatabasResultProcedureName(Function.MaxRangeKeyCount.Count1, entity);
            this.Param.Default();
            this.Param.CommandType = ExecuteType.Procedure;
            this.Param.Command = procedureName;
            this.Param.AddParam(primaryKey);

            using (DbCommand dbCommand = this.CreateProcedureReaderCommand())
            using (DbDataReader dbDataReader = dbCommand.ExecuteReader())
            {
                if (dbDataReader.Read())
                {
                    ProtobufNetHelper.ApplyMemberDataList(entity, dbDataReader);
                    result = true;
                }
            }
            return result;
        }

        public override Entity[] LoadEntitysFormDatabase(Func<Entity> creator, Function.MaxRangeKeyCount rangeKeyCount, long[] primaryKeys)
        {
            Entity entity = creator();
            AssertRangeKeyCountMatchesArray(rangeKeyCount, primaryKeys);
            string procedureName = Entity.GetDatabasResultProcedureName(rangeKeyCount, entity);

            this.Param.Default();
            this.Param.CommandType = ExecuteType.Procedure;
            this.Param.Command = procedureName;
            this.Param.AddRangeParam(primaryKeys);

            return ReadEntitiesByPrimaryKey(creator, primaryKeys);
        }

        public override Entity[] LoadEntitysFormDatabaseForInitializationResult(Func<Entity> creator, Function.MaxRangeKeyCount rangeKeyCount, long[] primaryKeys)
        {
            Entity entity = creator();
            AssertRangeKeyCountMatchesArray(rangeKeyCount, primaryKeys);
            string procedureName = Entity.GetDatabasInitializationResultProcedureName(rangeKeyCount, entity);

            this.Param.Default();
            this.Param.CommandType = ExecuteType.Procedure;
            this.Param.Command = procedureName;
            this.Param.AddRangeParam(primaryKeys);

            return ReadEntitiesByPrimaryKey(creator, primaryKeys);
        }

        Entity[] ReadEntitiesByPrimaryKey(Func<Entity> creator, long[] primaryKeys)
        {
            Dictionary<long, Entity> keyValuePairs = new Dictionary<long, Entity>();

            using (DbCommand dbCommand = this.CreateProcedureReaderCommand())
            using (DbDataReader dbDataReader = dbCommand.ExecuteReader())
            {
                while (dbDataReader.Read())
                {
                    Entity entity = creator();
                    ProtobufNetHelper.ApplyMemberDataList(entity, dbDataReader);
                    keyValuePairs.Add(entity.PrimaryKey, entity);
                }
            }

            List<Entity> result = new List<Entity>();
            foreach (long key in primaryKeys)
            {
                if (keyValuePairs.TryGetValue(key, out Entity outEntity))
                {
                    result.Add(outEntity);
                }
            }
            return result.ToArray();
        }

        static void AssertRangeKeyCountMatchesArray(Function.MaxRangeKeyCount rangeKeyCount, long[] primaryKeys)
        {
            int expected = rangeKeyCount switch
            {
                Function.MaxRangeKeyCount.Count1 => 1,
                Function.MaxRangeKeyCount.Count2 => 2,
                Function.MaxRangeKeyCount.Count5 => 5,
                Function.MaxRangeKeyCount.Count10 => 10,
                Function.MaxRangeKeyCount.Count20 => 20,
                Function.MaxRangeKeyCount.Count50 => 50,
                _ => throw new LeadTurbo.Exceptions.AssertException($"{rangeKeyCount} 不支持")
            };

            if (primaryKeys.Length != expected)
            {
                throw new LeadTurbo.Exceptions.AssertException($"primaryKeys.Length:{primaryKeys.Length} !={expected}");
            }
        }

        /// <summary>
        /// SQL Server 参数：SqlClient 原生支持 DateTime / Guid / Decimal / byte[] 等，
        /// 不做类型转换，仅做 null → DBNull 兜底。
        /// </summary>
        public class SQLServerDataBasParam : DataBasParam
        {
            protected override object ParamConverter(object aParam)
            {
                return aParam ?? DBNull.Value;
            }
        }

        public override DataBasParam CreateDataBasParam()
        {
            return new SQLServerDataBasParam();
        }
    }
}
