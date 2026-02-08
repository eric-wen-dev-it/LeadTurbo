using LeadTurbo.Artemis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Text;


namespace LeadTurbo.VirtualDatabase.Operations.Application
{
    /// <summary>
    /// 数据库使用对象
    /// </summary>
    public abstract class DataBasAPP : IDisposable
    {
        DbConnection dbConnection;
        DbTransaction dbTransaction;

        // 指示资源是否已释放
        private bool disposed = false;
        // 实现 IDisposable 接口的 Dispose 方法
        public void Dispose()
        {
            Dispose(true);
            // 防止终结器调用
            GC.SuppressFinalize(this);
        }

        // 受保护的虚拟 Dispose 方法
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {


                if (dbConnection != null)
                {
                    dbConnection.Dispose();
                }


            }



            disposed = true;
        }

        // 析构函数（终结器）
        ~DataBasAPP()
        {
            Dispose(false);
        }






        protected abstract DbConnection CreateDbConnection();

        protected DbCommand CreateDbCommand()
        {
            return dbConnection.CreateCommand();
        }

        protected DbParameter CreateDbParameter(DbCommand dbCommand)
        {
            return dbCommand.CreateParameter();
        }


        public DataBasAPP()
        {
            dbConnection = CreateDbConnection();
            dataBasParam = CreateDataBasParam();

        }

        public abstract DataBasParam CreateDataBasParam();

        public string ConnectionString
        {
            get
            {
                return dbConnection.ConnectionString;
            }

            set
            {

                dbConnection.ConnectionString = value;
            }
        }


        /// <summary>
		/// 打开数据库
		/// </summary>
		public virtual void OpenDataBas()
        {
            dbConnection.Open();
        }
        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void CloseDataBas()
        {
            dbConnection.Close();
        }
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns>事务对象</returns>
        public void BeginTransaction()
        {
            if (dbTransaction == null)
            {
                dbTransaction = dbConnection.BeginTransaction();
            }
            else
            {
                throw new Exception("系统断言失败，数据库事务未完成，不可以开启一个新的事物。");
            }


        }

        /// <summary>
        /// 提交事务
        /// </summary>
        /// <returns></returns>
        public void CommitTransaction()
        {
            dbTransaction.Commit();
            dbTransaction.Dispose();
            dbTransaction = null;
        }


        /// <summary>
        /// 回滚事务
        /// </summary>
        /// <returns></returns>
        public void RollbackTransaction()
        {
            if (dbTransaction != null)
            {
                dbTransaction.Rollback();
                dbTransaction.Dispose();
                dbTransaction = null;
            }
        }





        /// <summary>
        /// 执行类型
        /// </summary>
        public enum ExecuteType
        {
            /// <summary>
            /// SQL命令
            /// </summary>
            SQL,
            /// <summary>
            /// 存储过程
            /// </summary>
            Procedure
        }
        /// <summary>
        /// 进行数据库操作必须填写的数据;
        /// </summary>
        public abstract class DataBasParam
        {
            readonly List<object> paramArray = new List<object>();
            /// <summary>
            /// 构造
            /// </summary>
            public DataBasParam()
            {

                Default();
            }
            /// <summary>
            /// 操作的命令
            /// </summary>
            public string Command;
            /// <summary>
            /// 操作的超时设定
            /// </summary>
            public int CommandTimeout;
            /// <summary>
            /// 操作的类型
            /// </summary>
            public ExecuteType CommandType;

            protected abstract object ParamConverter(object aParam);


            public void AddParam(object aParam)
            {
                object paramConverter = ParamConverter(aParam);
                paramArray.Add(paramConverter);

            }

            public void AddRangeParam(IEnumerable collection)
            {
                foreach (object aParam in collection)
                {
                    AddParam(aParam);
                }

            }


            public object[] GetParamArray()
            {
                return paramArray.ToArray();

            }

            /// <summary>
            /// 添加一批参数
            /// </summary>
            public void SetParamArray(object[] Param)
            {
                paramArray.CopyTo(Param);
            }

            /// <summary>
            /// 恢复数据状态到默认值,供下次查询
            /// </summary>
            public void Default()
            {
                Command = default;
                CommandTimeout = 300;
                CommandType = ExecuteType.Procedure;
                paramArray.Clear();
            }
        }

        DataBasParam dataBasParam = null;
        /// <summary>
        /// 测试录入参数是否符合要求
        /// </summary>
        private void DataBasParamTest()
        {
            if (dataBasParam == null)
            {
                throw new Exception("操作参数没有初始化,操作不能继续");
            }

            if (dataBasParam.Command == null)
            {
                throw new Exception("操作命令为空,操作不能继续");
            }
        }

        /// <summary>
		/// 执行返回结果的SQL
		/// </summary>
		protected DbDataReader ExecuteReaderSQL()
        {
            using (DbCommand dbCommand = CreateDbCommand())
            {

                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = this.Param.Command;
                dbCommand.Transaction = dbTransaction;
                dbCommand.CommandTimeout = this.Param.CommandTimeout;

                object[] paramArray = Param.GetParamArray();
                for (int a = 0; a < paramArray.Length; a++)
                {

                    if (paramArray[a] is DbParameter dbParameter)
                    {
                        if (dbParameter.Value == null)
                        {
                            dbParameter.Value = DBNull.Value;
                        }

                        dbCommand.Parameters.Add(dbParameter);
                    }
                    else
                    {
                        if (paramArray[a] == null)
                        {
                            paramArray[a] = DBNull.Value;
                        }

                        dbParameter = CreateDbParameter(dbCommand);
                        dbParameter.ParameterName = string.Format("@P_{0}", a);
                        dbParameter.Value = paramArray[a];
                        dbCommand.Parameters.Add(dbParameter);
                    }
                }
                return dbCommand.ExecuteReader();
            }
        }

        /// <summary>
		/// 执行返回结果的存储过程
		/// </summary>
		protected DbDataReader ExecuteReaderProcedure()
        {
            using (DbCommand dbCommand = CreateDbCommand())
            {

                dbCommand.CommandType = CommandType.StoredProcedure;
                dbCommand.CommandText = this.Param.Command;
                dbCommand.Transaction = dbTransaction;
                dbCommand.CommandTimeout = this.Param.CommandTimeout;



                object[] paramArray = Param.GetParamArray();
                for (int a = 0; a < paramArray.Length; a++)
                {

                    if (paramArray[a] is DbParameter dbParameter)
                    {
                        if (dbParameter.Value == null)
                        {
                            dbParameter.Value = DBNull.Value;
                        }

                        dbCommand.Parameters.Add(dbParameter);
                    }
                    else
                    {
                        if (paramArray[a] == null)
                        {
                            paramArray[a] = DBNull.Value;
                        }

                        dbParameter = CreateDbParameter(dbCommand);
                        dbParameter.ParameterName = string.Format("@P_{0}", a);
                        dbParameter.Value = paramArray[a];
                        dbCommand.Parameters.Add(dbParameter);
                    }
                }
                return dbCommand.ExecuteReader();
            }

        }

        /// <summary>
		/// 执行不返回结果的SQL
		/// </summary>
		/// <returns>影响数量</returns>
		protected int ExecuteSQL()
        {
            using (DbCommand dbCommand = CreateDbCommand())
            {
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = this.Param.Command;
                dbCommand.Transaction = dbTransaction;
                dbCommand.CommandTimeout = this.Param.CommandTimeout;

                object[] paramArray = Param.GetParamArray();
                for (int a = 0; a < paramArray.Length; a++)
                {

                    if (paramArray[a] is DbParameter dbParameter)
                    {
                        if (dbParameter.Value == null)
                        {
                            dbParameter.Value = DBNull.Value;
                        }

                        dbCommand.Parameters.Add(dbParameter);
                    }
                    else
                    {
                        if (paramArray[a] == null)
                        {
                            paramArray[a] = DBNull.Value;
                        }

                        dbParameter = CreateDbParameter(dbCommand);
                        dbParameter.ParameterName = string.Format("@P_{0}", a);
                        dbParameter.Value = paramArray[a];
                        dbCommand.Parameters.Add(dbParameter);
                    }
                }
                return dbCommand.ExecuteNonQuery();
            }
        }


        /// <summary>
		/// 执行不返回结果的存储过程
		/// </summary>
		/// <returns>影响数量</returns>
		protected int ExecuteProcedure()
        {
            using (DbCommand dbCommand = CreateDbCommand())
            {
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbCommand.CommandText = this.Param.Command;
                dbCommand.Transaction = dbTransaction;
                dbCommand.CommandTimeout = this.Param.CommandTimeout;

                object[] paramArray = Param.GetParamArray();
                for (int a = 0; a < paramArray.Length; a++)
                {

                    if (paramArray[a] is DbParameter dbParameter)
                    {
                        if (dbParameter.Value == null)
                        {
                            dbParameter.Value = DBNull.Value;
                        }

                        dbCommand.Parameters.Add(dbParameter);
                    }
                    else
                    {
                        if (paramArray[a] == null)
                        {
                            paramArray[a] = DBNull.Value;
                        }

                        dbParameter = CreateDbParameter(dbCommand);
                        dbParameter.ParameterName = string.Format("@P_{0}", a);
                        dbParameter.Value = paramArray[a];
                        dbCommand.Parameters.Add(dbParameter);
                    }
                }
                return dbCommand.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// 结果转化为动态数组
        /// </summary>
        /// <returns>数组</returns>
        protected ArrayList ToArrayList(DbDataReader dbDataReader)
        {
            int numcells = dbDataReader.FieldCount;
            ArrayList al1 = new ArrayList();
            ArrayList al2 = new ArrayList();
            for (int i = 0; i < numcells; i++)
            {
                al2.Add(dbDataReader.GetName(i));
            }
            al1.Add(al2);

            while (dbDataReader.Read())
            {
                al2 = new ArrayList();
                for (int i = 0; i < numcells; i++)
                {
                    al2.Add(dbDataReader.GetValue(i));
                }
                al1.Add(al2);
            }
            dbDataReader.Close();
            return al1;
        }
        /// <summary>
		/// 转化为字符串数组
		/// </summary>
		/// <returns>字符数组</returns>
		protected string[] ToStringArray(DbDataReader dbDataReader)
        {
            string bs1 = ToAString(dbDataReader);
            return bs1.Split('\n');
        }
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>字符串</returns>
        protected string ToAString(DbDataReader dbDataReader)
        {
            System.Text.StringBuilder sb1 = new System.Text.StringBuilder();
            int numcells = dbDataReader.FieldCount;
            for (int i = 0; i < numcells; i++)
            {
                string bs1 = dbDataReader.GetName(i);
                bs1 = bs1.Replace("\n", " ");
                bs1 = bs1.Replace("	", " ");
                if (i == 0)
                {
                    sb1.Append(bs1);
                }
                else
                {
                    sb1.Append("	" + bs1);
                }
            }
            while (dbDataReader.Read())
            {
                sb1.Append("\n");
                for (int i = 0; i < numcells; i++)
                {
                    string bs1 = dbDataReader.GetValue(i).ToString();
                    bs1 = bs1.Replace("\n", " ");
                    bs1 = bs1.Replace("	", " ");
                    if (i == 0)
                    {
                        sb1.Append(bs1);
                    }
                    else
                    {
                        sb1.Append("	" + bs1);
                    }
                }
            }
            dbDataReader.Close();
            return sb1.ToString();
        }


        /// <summary>
        /// 转化为HTML字符
        /// </summary>
        /// <returns>HTML</returns>
        protected string ToHTML(DbDataReader dbDataReader)
        {
            System.Text.StringBuilder sb1 = new System.Text.StringBuilder();
            int numcells = dbDataReader.FieldCount;
            sb1.Append("<TABLE  style=\"BORDER-COLLAPSE: collapse\" borderColor=\"#111111\" cellSpacing=\"0\" cellPadding=\"0\" border=\"1\">");
            sb1.Append("<tr>");
            for (int i = 0; i < numcells; i++)
            {
                string bs1 = dbDataReader.GetName(i);
                sb1.Append("<td>" + bs1 + "</td>");
            }
            sb1.Append("</tr>");
            while (dbDataReader.Read())
            {
                sb1.Append("<tr>");
                for (int i = 0; i < numcells; i++)
                {
                    string bs1 = dbDataReader.GetValue(i).ToString();
                    sb1.Append("<td>" + bs1 + "</td>");
                }
                sb1.Append("</tr>");
            }
            sb1.Append("</TABLE>");
            dbDataReader.Close();
            return sb1.ToString();
        }
        /// <summary>
        /// 结果转化为XML字符串
        /// </summary>
        /// <returns>XML字符串</returns>
        protected string ToXML(DbDataReader dbDataReader)
        {
            return ToHTML(dbDataReader);
        }
        /// <summary>
        /// 运行操作的第一行,第一列的信息
        /// </summary>
        /// <returns>对象</returns>
        protected object Top1Row1(DbDataReader dbDataReader)
        {
            if (dbDataReader.Read() != true)
            {
                throw new Exception("没有查询结果可以返回");
            }
            if (dbDataReader.FieldCount < 1)
            {
                throw new Exception("此行中不存在第一值.目前值的数量是:" + dbDataReader.FieldCount.ToString());
            }
            return dbDataReader.GetValue(0);
        }

        /// <summary>
        /// 转化为DataTable对象
        /// </summary>
        /// <returns></returns>
        protected DataTable ToDataTable(DbDataReader dbDataReader)
        {
            DataTable Return = new DataTable();
            Return.Load(dbDataReader);
            return Return;
        }


        protected IEnumerable<object[]> ToEnumerable(DbDataReader dbDataReader)
        {
            int numcells = dbDataReader.FieldCount;
            while (dbDataReader.Read())
            {
                List<object> list = new List<object>();
                for (int i = 0; i < numcells; i++)
                {
                    list.Add(dbDataReader.GetValue(i));
                }
                yield return list.ToArray();
            }
            dbDataReader.Close();
        }


        /// <summary>
        /// 返回一个迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object[]> ReturnEnumerable()
        {
            DataBasParamTest();
            if (dataBasParam.CommandType == ExecuteType.Procedure)
            {

                using (DbDataReader dbDataReader = ExecuteReaderProcedure())
                {
                    dataBasParam.Default();
                    return ToEnumerable(dbDataReader);
                }
            }
            else
            {
                using (DbDataReader dbDataReader = ExecuteReaderSQL())
                {
                    dataBasParam.Default();
                    return ToEnumerable(dbDataReader);
                }
            }


        }


        /// <summary>
        /// 运行操作返回数组对象
        /// </summary>
        /// <returns>数组对象</returns>
        public ArrayList ReturnToArrayList()
        {
            DataBasParamTest();
            if (dataBasParam.CommandType == ExecuteType.Procedure)
            {
                using (DbDataReader dbDataReader = ExecuteReaderProcedure())
                {
                    dataBasParam.Default();
                    return ToArrayList(dbDataReader);
                }
            }
            else
            {
                using (DbDataReader dbDataReader = ExecuteReaderSQL())
                {
                    dataBasParam.Default();
                    return ToArrayList(dbDataReader);
                }
            }

        }
        /// <summary>
        /// 运行操作返回字符串数组对象
        /// </summary>
        /// <returns>字符串数组</returns>
        public string[] ReturnToStringArray()
        {
            DataBasParamTest();
            if (dataBasParam.CommandType == ExecuteType.Procedure)
            {
                using (DbDataReader dbDataReader = ExecuteReaderProcedure())
                {
                    dataBasParam.Default();
                    return ToStringArray(dbDataReader);
                }
            }
            else
            {
                using (DbDataReader dbDataReader = ExecuteReaderSQL())
                {
                    dataBasParam.Default();
                    return ToStringArray(dbDataReader);
                }
            }

        }
        /// <summary>
        /// 运行操作返回字符串数组对象
        /// </summary>
        /// <returns>字符串数组</returns>
        public string ReturnToString()
        {
            DataBasParamTest();
            if (dataBasParam.CommandType == ExecuteType.Procedure)
            {
                using (DbDataReader dbDataReader = ExecuteReaderProcedure())
                {
                    dataBasParam.Default();
                    return ToAString(dbDataReader);
                }
            }
            else
            {
                using (DbDataReader dbDataReader = ExecuteReaderSQL())
                {
                    dataBasParam.Default();
                    return ToAString(dbDataReader);
                }
            }

        }
        /// <summary>
        /// 运行操作返回HTML字符串
        /// </summary>
        /// <returns>HTML字符串</returns>
        public string ReturnToHTML()
        {
            DataBasParamTest();
            if (dataBasParam.CommandType == ExecuteType.Procedure)
            {
                using (DbDataReader dbDataReader = ExecuteReaderProcedure())
                {
                    dataBasParam.Default();
                    return ToHTML(dbDataReader);
                }
            }
            else
            {
                using (DbDataReader dbDataReader = ExecuteReaderSQL())
                {
                    dataBasParam.Default();
                    return ToHTML(dbDataReader);
                }
            }

        }
        /// <summary>
        /// 运行操作返回XML字符串
        /// </summary>
        /// <returns>XML字符串</returns>
        public string ReturnToXML()
        {
            DataBasParamTest();
            if (dataBasParam.CommandType == ExecuteType.Procedure)
            {
                using (DbDataReader dbDataReader = ExecuteReaderProcedure())
                {
                    dataBasParam.Default();
                    return ToXML(dbDataReader);
                }
            }
            else
            {
                using (DbDataReader dbDataReader = ExecuteReaderSQL())
                {
                    dataBasParam.Default();
                    return ToXML(dbDataReader);
                }
            }

        }

        /// <summary>
        /// 运行操作返回DataTable
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable ReturnToDataTable()
        {
            DataBasParamTest();
            if (dataBasParam.CommandType == ExecuteType.Procedure)
            {
                using (DbDataReader dbDataReader = ExecuteReaderProcedure())
                {
                    dataBasParam.Default();
                    return ToDataTable(dbDataReader);
                }
            }
            else
            {
                using (DbDataReader dbDataReader = ExecuteReaderSQL())
                {
                    dataBasParam.Default();
                    return ToDataTable(dbDataReader);
                }
            }

        }
        /// <summary>
        /// 运行操作的第一行,第一列的信息
        /// </summary>
        /// <returns>对象</returns>
        public object ReturnTop1Row1()
        {
            DataBasParamTest();
            if (dataBasParam.CommandType == ExecuteType.Procedure)
            {
                using (DbDataReader dbDataReader = ExecuteReaderProcedure())
                {
                    dataBasParam.Default();
                    return Top1Row1(dbDataReader);
                }
            }
            else
            {
                using (DbDataReader dbDataReader = ExecuteReaderSQL())
                {
                    dataBasParam.Default();
                    return Top1Row1(dbDataReader);
                }
            }

        }
        /// <summary>
        /// 执行操作返回受此操作影响的行数
        /// </summary>
        /// <returns>影响的行数</returns>
        public int Return()
        {
            DataBasParamTest();
            int bi1 = 0;
            if (dataBasParam.CommandType == ExecuteType.Procedure)
            {
                bi1 = ExecuteProcedure();
            }
            else
            {
                bi1 = ExecuteSQL();
            }
            dataBasParam.Default();
            return bi1;
        }
        /// <summary>
        /// 执行参数
        /// </summary>
        public DataBasParam Param
        {
            get
            {
                return dataBasParam;
            }
        }

        protected DbConnection DbConnection
        {
            get
            {
                return dbConnection;
            }

        }


        public abstract int InsertEntityInDatabase(Entity entity);
        public abstract int UpdateEntityInDatabase(Entity entity);

        public abstract int DeleteEntityInDatabase(Entity entity);

        public abstract KeyValuePair<long, int>[] GetAllKeyAndVer(Entity entity);
        public abstract bool TryLoadEntityFormDatabase<T>(long primaryKey, out T entity) where T : Entity, new();

        public abstract Entity[] LoadEntitysFormDatabase(Func<Entity> creator, Function.MaxRangeKeyCount rangeKeyCount, long[] primaryKeys);

        public abstract Entity[] LoadEntitysFormDatabaseForInitializationResult(Func<Entity> creator, Function.MaxRangeKeyCount rangeKeyCount, long[] primaryKeys);
    }
}
