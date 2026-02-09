
using LeadTurbo.Artemis;
using LeadTurbo.VirtualDatabase.ColumnEntitys;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace LeadTurbo.VirtualDatabase.Operations.Application
{
    public class DataBasAppSQLite : DataBasApp
    {
        protected override DbConnection CreateDbConnection()
        {
            return new SQLiteConnection();
        }

        /// <summary>
		/// 打开数据库
		/// </summary>
		public override void OpenDataBas()
        {
            SQLiteConnection sqliteConnection = (SQLiteConnection)DbConnection;
            sqliteConnection.Open();
            //sqliteConnection.EnableExtensions(true);
            //string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            //string filePhat = $"{baseDirectory}X64\\SQLite.Interop.dll";

            //if (!File.Exists(filePhat))
            //{
            //    filePhat = "SQLite.Interop.dll";
            //}
            //filePhat = Function.GetRuntimeDirectory(filePhat);
            //sqliteConnection.LoadExtension(filePhat, "sqlite3_fts5_init");

            //filePhat = $"{baseDirectory}X64\\uuid.dll";

            //if (!File.Exists(filePhat))
            //{
            //    filePhat = "uuid.dll";
            //}
            //filePhat = Function.GetRuntimeDirectory(filePhat);
            //sqliteConnection.LoadExtension(filePhat, "sqlite3_uuid_init");


        }




        public override int InsertEntityInDatabase(Entity entity)
        {
            string procedureName = Entity.Get_INSERTStoredProcedureName(entity);
            string sql = GetSQL(procedureName);
            this.Param.Default();
            this.Param.CommandType = ExecuteType.SQL;
            this.Param.Command = sql;
            MemberData[] memberDatas = ProtobufNetHelper.GetMembersWithValues(entity);
            foreach (MemberData memberData in memberDatas)
            {
                this.Param.AddParam(memberData.Value);
            }

            return this.ExecuteSQL();
        }






        public override int UpdateEntityInDatabase(Entity entity)
        {
            string procedureName = Entity.Get_UPDATEStoredProcedureName(entity);
            string sql = GetSQL(procedureName);
            this.Param.Default();
            this.Param.CommandType = ExecuteType.SQL;
            this.Param.Command = sql;
            MemberData[] memberDatas = ProtobufNetHelper.GetMembersWithValues(entity);
            foreach (MemberData memberData in memberDatas)
            {
                this.Param.AddParam(memberData.Value);
            }

            return this.ExecuteSQL();

        }

        public override int DeleteEntityInDatabase(Entity entity)
        {
            string procedureName = Entity.Get_DELStoredProcedureName(entity);
            string sql = GetSQL(procedureName);
            this.Param.Default();
            this.Param.CommandType = ExecuteType.SQL;
            this.Param.Command = sql;
            MemberData[] memberDatas = ProtobufNetHelper.GetMembersWithValues(entity);
            foreach (MemberData memberData in memberDatas)
            {
                this.Param.AddParam(memberData.Value);
            }

            return this.ExecuteSQL();
        }

        public override KeyValuePair<long, int>[] GetAllKeyAndVer(Entity entity)
        {

            string procedureName = Entity.Get_AllKeyAndVerProcedureName(entity);
            string sql = GetSQL(procedureName);
            this.Param.Default();
            this.Param.CommandType = ExecuteType.SQL;
            this.Param.Command = sql;

            List<KeyValuePair<long, int>> result = new List<KeyValuePair<long, int>>();
            using (DbDataReader dbDataReader = this.ExecuteReaderSQL())
            {
                while (dbDataReader.Read())
                {
                    KeyValuePair<long, int> keyValuePair = new KeyValuePair<long, int>(dbDataReader.GetInt64(0), dbDataReader.GetInt32(1));
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
            string sql = GetSQL(procedureName);
            this.Param.Default();
            this.Param.CommandType = ExecuteType.SQL;
            this.Param.Command = sql;
            this.Param.AddParam(primaryKey);

            using (DbDataReader dbDataReader = this.ExecuteReaderSQL())
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


            string procedureName = default;
            switch (rangeKeyCount)
            {
                case Function.MaxRangeKeyCount.Count1:
                {
                    procedureName = Entity.GetDatabasResultProcedureName(Function.MaxRangeKeyCount.Count1, entity);
                    break;
                }
                case Function.MaxRangeKeyCount.Count2:
                {
                    procedureName = Entity.GetDatabasResultProcedureName(Function.MaxRangeKeyCount.Count2, entity);
                    break;
                }
                case Function.MaxRangeKeyCount.Count5:
                {
                    procedureName = Entity.GetDatabasResultProcedureName(Function.MaxRangeKeyCount.Count5, entity);
                    break;
                }
                case Function.MaxRangeKeyCount.Count10:
                {
                    procedureName = Entity.GetDatabasResultProcedureName(Function.MaxRangeKeyCount.Count10, entity);
                    break;
                }
                case Function.MaxRangeKeyCount.Count20:
                {
                    procedureName = Entity.GetDatabasResultProcedureName(Function.MaxRangeKeyCount.Count20, entity);
                    break;
                }
                case Function.MaxRangeKeyCount.Count50:
                {
                    procedureName = Entity.GetDatabasResultProcedureName(Function.MaxRangeKeyCount.Count50, entity);
                    break;
                }
                default:
                {
                    throw new LeadTurbo.Exceptions.AssertException($"{rangeKeyCount} 不支持");
                }
            }

            switch (rangeKeyCount)
            {
                case Function.MaxRangeKeyCount.Count1:
                {
                    if (primaryKeys.Length != 1)
                    {
                        throw new LeadTurbo.Exceptions.AssertException($"primaryKeys.Length:{primaryKeys.Length} !=1");
                    }
                    break;
                }
                case Function.MaxRangeKeyCount.Count2:
                {
                    if (primaryKeys.Length != 2)
                    {
                        throw new LeadTurbo.Exceptions.AssertException($"primaryKeys.Length:{primaryKeys.Length} !=2");
                    }
                    break;
                }
                case Function.MaxRangeKeyCount.Count5:
                {
                    if (primaryKeys.Length != 5)
                    {
                        throw new LeadTurbo.Exceptions.AssertException($"primaryKeys.Length:{primaryKeys.Length} !=5");
                    }
                    break;
                }
                case Function.MaxRangeKeyCount.Count10:
                {
                    if (primaryKeys.Length != 10)
                    {
                        throw new LeadTurbo.Exceptions.AssertException($"primaryKeys.Length:{primaryKeys.Length} !=10");
                    }
                    break;
                }
                case Function.MaxRangeKeyCount.Count20:
                {
                    if (primaryKeys.Length != 20)
                    {
                        throw new LeadTurbo.Exceptions.AssertException($"primaryKeys.Length:{primaryKeys.Length} !=20");
                    }
                    break;
                }
                case Function.MaxRangeKeyCount.Count50:
                {
                    if (primaryKeys.Length != 50)
                    {
                        throw new LeadTurbo.Exceptions.AssertException($"primaryKeys.Length:{primaryKeys.Length} !=50");
                    }
                    break;
                }
                default:
                {
                    throw new LeadTurbo.Exceptions.AssertException($"{rangeKeyCount} 不支持");
                }

            }


            string sql = GetSQL(procedureName);
            this.Param.Default();
            this.Param.CommandType = ExecuteType.SQL;
            this.Param.Command = sql;
            this.Param.AddRangeParam(primaryKeys);

            Dictionary<long, Entity> keyValuePairs = new Dictionary<long, Entity>();

            using (DbDataReader dbDataReader = this.ExecuteReaderSQL())
            {
                while (dbDataReader.Read())
                {
                    entity = creator();
                    ProtobufNetHelper.ApplyMemberDataList(entity, dbDataReader);
                    keyValuePairs.Add(entity.PrimaryKey, entity);
                }
            }

            List<Entity> result = new List<Entity>();
            foreach (long key in primaryKeys)
            {
                Entity outEntity;
                if (keyValuePairs.TryGetValue(key, out outEntity))
                {
                    result.Add(outEntity);
                }
            }
            return result.ToArray();
        }

        public override Entity[] LoadEntitysFormDatabaseForInitializationResult(Func<Entity> creator, Function.MaxRangeKeyCount rangeKeyCount, long[] primaryKeys)
        {
            Entity entity = creator();


            string procedureName = default;
            switch (rangeKeyCount)
            {
                case Function.MaxRangeKeyCount.Count1:
                {
                    procedureName = Entity.GetDatabasInitializationResultProcedureName(Function.MaxRangeKeyCount.Count1, entity);
                    break;
                }
                case Function.MaxRangeKeyCount.Count2:
                {
                    procedureName = Entity.GetDatabasInitializationResultProcedureName(Function.MaxRangeKeyCount.Count2, entity);
                    break;
                }
                case Function.MaxRangeKeyCount.Count5:
                {
                    procedureName = Entity.GetDatabasInitializationResultProcedureName(Function.MaxRangeKeyCount.Count5, entity);
                    break;
                }
                case Function.MaxRangeKeyCount.Count10:
                {
                    procedureName = Entity.GetDatabasInitializationResultProcedureName(Function.MaxRangeKeyCount.Count10, entity);
                    break;
                }
                case Function.MaxRangeKeyCount.Count20:
                {
                    procedureName = Entity.GetDatabasInitializationResultProcedureName(Function.MaxRangeKeyCount.Count20, entity);
                    break;
                }
                case Function.MaxRangeKeyCount.Count50:
                {
                    procedureName = Entity.GetDatabasInitializationResultProcedureName(Function.MaxRangeKeyCount.Count50, entity);
                    break;
                }
                default:
                {
                    throw new LeadTurbo.Exceptions.AssertException($"{rangeKeyCount} 不支持");
                }
            }

            switch (rangeKeyCount)
            {
                case Function.MaxRangeKeyCount.Count1:
                {
                    if (primaryKeys.Length != 1)
                    {
                        throw new LeadTurbo.Exceptions.AssertException($"primaryKeys.Length:{primaryKeys.Length} !=1");
                    }
                    break;
                }
                case Function.MaxRangeKeyCount.Count2:
                {
                    if (primaryKeys.Length != 2)
                    {
                        throw new LeadTurbo.Exceptions.AssertException($"primaryKeys.Length:{primaryKeys.Length} !=2");
                    }
                    break;
                }
                case Function.MaxRangeKeyCount.Count5:
                {
                    if (primaryKeys.Length != 5)
                    {
                        throw new LeadTurbo.Exceptions.AssertException($"primaryKeys.Length:{primaryKeys.Length} !=5");
                    }
                    break;
                }
                case Function.MaxRangeKeyCount.Count10:
                {
                    if (primaryKeys.Length != 10)
                    {
                        throw new LeadTurbo.Exceptions.AssertException($"primaryKeys.Length:{primaryKeys.Length} !=10");
                    }
                    break;
                }
                case Function.MaxRangeKeyCount.Count20:
                {
                    if (primaryKeys.Length != 20)
                    {
                        throw new LeadTurbo.Exceptions.AssertException($"primaryKeys.Length:{primaryKeys.Length} !=20");
                    }
                    break;
                }
                case Function.MaxRangeKeyCount.Count50:
                {
                    if (primaryKeys.Length != 50)
                    {
                        throw new LeadTurbo.Exceptions.AssertException($"primaryKeys.Length:{primaryKeys.Length} !=50");
                    }
                    break;
                }
                default:
                {
                    throw new LeadTurbo.Exceptions.AssertException($"{rangeKeyCount} 不支持");
                }

            }


            string sql = GetSQL(procedureName);
            this.Param.Default();
            this.Param.CommandType = ExecuteType.SQL;
            this.Param.Command = sql;
            this.Param.AddRangeParam(primaryKeys);

            Dictionary<long, Entity> keyValuePairs = new Dictionary<long, Entity>();

            using (DbDataReader dbDataReader = this.ExecuteReaderSQL())
            {
                while (dbDataReader.Read())
                {
                    entity = creator();
                    ProtobufNetHelper.ApplyMemberDataList(entity, dbDataReader);
                    keyValuePairs.Add(entity.PrimaryKey, entity);
                }
            }

            List<Entity> result = new List<Entity>();
            foreach (long key in primaryKeys)
            {
                Entity outEntity;
                if (keyValuePairs.TryGetValue(key, out outEntity))
                {
                    result.Add(outEntity);
                }
            }
            return result.ToArray();
        }



        static ConcurrentDictionary<string, string> sqlCache = new ConcurrentDictionary<string, string>();
        string GetSQL(string procedureName)
        {
            procedureName = procedureName.ToUpper();


            string result = default;
            if (!sqlCache.TryGetValue(procedureName, out result))
            {
                this.Param.Default();
                this.Param.CommandType = ExecuteType.SQL;
                this.Param.Command = "select [sql] from [Procedures] where [Name]=@P_0 COLLATE NOCASE";
                this.Param.AddParam(procedureName);
                using (DbDataReader dbDataReader = this.ExecuteReaderSQL())
                {
                    if (dbDataReader.Read())
                    {
                        result = dbDataReader.GetString(0);
                        sqlCache[procedureName] = result;
                    }
                    else
                    {
                        throw new LeadTurbo.Exceptions.AssertException($"{procedureName} 对应的SQL不存在");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 进行数据库操作必须填写的数据;
        /// </summary>
        public class SQLiteDataBasParam : DataBasParam
        {
            protected override object ParamConverter(object aParam)
            {
                return aParam switch
                {
                    null => DBNull.Value,
                    System.DateTime dateTime => $"{dateTime:s}",
                    Guid guid => guid.ToString("N"),
                    _ => aParam
                };
            }
        }


        public override DataBasParam CreateDataBasParam()
        {
            return new SQLiteDataBasParam();
        }
    }
}