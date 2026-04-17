using LeadTurbo.Artemis;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace LeadTurbo.VirtualDatabase.Operations.Application
{
    /// <summary>
    /// PostgreSQL 数据库应用。
    /// 与 SQL Server 版的差异：
    ///   - PG 的"过程"是 CREATE FUNCTION，结果集要靠 SELECT * FROM fn(...) 读取，
    ///     所以这里统一用 CommandType.Text + 拼好的 SELECT 语句，而不是 StoredProcedure。
    ///   - 不做参数类型转换，Npgsql 原生支持 DateTime/Guid/Decimal/byte[] 等，仅做 null → DBNull 兜底。
    ///   - 函数名带 schema 限定（默认 "public"）。
    /// </summary>
    public class DataBasAppPostgreSQL : DataBasApp
    {
        /// <summary>
        /// 函数所在 schema，默认 "public"。
        /// </summary>
        public string Schema { get; set; } = "public";

        protected override DbConnection CreateDbConnection()
        {
            return new NpgsqlConnection();
        }

        public override int InsertEntityInDatabase(Entity entity)
        {
            string procedureName = Entity.Get_INSERTStoredProcedureName(entity);
            MemberData[] memberDatas = ProtobufNetHelper.GetMembersWithValues(entity);

            this.Param.Default();
            this.Param.CommandType = ExecuteType.SQL;
            this.Param.Command = BuildFunctionCallSql(procedureName, memberDatas.Length);
            foreach (MemberData memberData in memberDatas)
            {
                this.Param.AddParam(memberData.Value);
            }
            return this.ExecuteSQL();
        }

        public override int UpdateEntityInDatabase(Entity entity)
        {
            string procedureName = Entity.Get_UPDATEStoredProcedureName(entity);
            MemberData[] memberDatas = ProtobufNetHelper.GetMembersWithValues(entity);

            this.Param.Default();
            this.Param.CommandType = ExecuteType.SQL;
            this.Param.Command = BuildFunctionCallSql(procedureName, memberDatas.Length);
            foreach (MemberData memberData in memberDatas)
            {
                this.Param.AddParam(memberData.Value);
            }
            return this.ExecuteSQL();
        }

        public override int DeleteEntityInDatabase(Entity entity)
        {
            string procedureName = Entity.Get_DELStoredProcedureName(entity);
            this.Param.Default();
            this.Param.CommandType = ExecuteType.SQL;
            // Delete 函数只需 PK + Ver 两个参数（Ver 用于乐观并发校验）
            this.Param.Command = BuildFunctionCallSql(procedureName, 2);
            this.Param.AddParam(entity.PrimaryKey);
            this.Param.AddParam(entity.EditVer);
            return this.ExecuteSQL();
        }

        public override KeyValuePair<long, int>[] GetAllKeyAndVer(Entity entity)
        {
            string procedureName = Entity.Get_AllKeyAndVerProcedureName(entity);
            this.Param.Default();
            this.Param.CommandType = ExecuteType.SQL;
            this.Param.Command = BuildFunctionCallSql(procedureName, 0);

            List<KeyValuePair<long, int>> result = new List<KeyValuePair<long, int>>();
            using (DbCommand dbCommand = this.CreateTextReaderCommand())
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
            this.Param.CommandType = ExecuteType.SQL;
            this.Param.Command = BuildFunctionCallSql(procedureName, 1);
            this.Param.AddParam(primaryKey);

            using (DbCommand dbCommand = this.CreateTextReaderCommand())
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
            this.Param.CommandType = ExecuteType.SQL;
            this.Param.Command = BuildFunctionCallSql(procedureName, primaryKeys.Length);
            this.Param.AddRangeParam(primaryKeys);

            return ReadEntitiesByPrimaryKey(creator, primaryKeys);
        }

        public override Entity[] LoadEntitysFormDatabaseForInitializationResult(Func<Entity> creator, Function.MaxRangeKeyCount rangeKeyCount, long[] primaryKeys)
        {
            Entity entity = creator();
            AssertRangeKeyCountMatchesArray(rangeKeyCount, primaryKeys);
            string procedureName = Entity.GetDatabasInitializationResultProcedureName(rangeKeyCount, entity);

            this.Param.Default();
            this.Param.CommandType = ExecuteType.SQL;
            this.Param.Command = BuildFunctionCallSql(procedureName, primaryKeys.Length);
            this.Param.AddRangeParam(primaryKeys);

            return ReadEntitiesByPrimaryKey(creator, primaryKeys);
        }

        Entity[] ReadEntitiesByPrimaryKey(Func<Entity> creator, long[] primaryKeys)
        {
            Dictionary<long, Entity> keyValuePairs = new Dictionary<long, Entity>();

            using (DbCommand dbCommand = this.CreateTextReaderCommand())
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

        /// <summary>
        /// 拼一条 SELECT 调函数的 SQL：SELECT * FROM "schema"."fn"(@P_0, @P_1, ...)。
        /// </summary>
        string BuildFunctionCallSql(string procedureName, int paramCount)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"SELECT * FROM \"{Schema}\".\"{procedureName}\"(");
            for (int i = 0; i < paramCount; i++)
            {
                if (i > 0) sb.Append(", ");
                sb.Append($"@P_{i}");
            }
            sb.Append(")");
            return sb.ToString();
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
        /// PostgreSQL 参数：Npgsql 原生支持 DateTime / Guid / Decimal / byte[] 等，
        /// 不做类型转换，仅做 null → DBNull 兜底。
        /// 注：JSONB 列若用 string 传值，PG 不会隐式转换，需要在过程签名上声明为 jsonb 或调用方显式转换。
        /// </summary>
        public class PostgreSQLDataBasParam : DataBasParam
        {
            protected override object ParamConverter(object aParam)
            {
                return aParam ?? DBNull.Value;
            }
        }

        public override DataBasParam CreateDataBasParam()
        {
            return new PostgreSQLDataBasParam();
        }
    }
}
