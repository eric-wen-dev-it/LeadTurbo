using LeadTurbo.VirtualDatabase.ColumnEntitys;
using LeadTurbo.VirtualDatabase.Operations.Application;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Metrics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;
using static LeadTurbo.Function;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LeadTurbo.VirtualDatabase
{
    public class SQLForBuildingASQLite
    {
        public void Building(Database database)
        {
            DataBasAPP dataBasAPP=new DataBasAppSQLite();
            dataBasAPP.ConnectionString = database.ConnectString;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("CREATE TABLE IF NOT EXISTS [Procedures]");
            stringBuilder.AppendLine("(");
            stringBuilder.AppendLine("\tName text PRIMARY KEY,");
            stringBuilder.AppendLine("\tSql text");
            stringBuilder.AppendLine(")");


            try
            {
                dataBasAPP.OpenDataBas();
                dataBasAPP.Param.CommandType = DataBasAPP.ExecuteType.SQL;
                dataBasAPP.Param.Command = stringBuilder.ToString();
                dataBasAPP.Return();
            }
            finally
            {
                dataBasAPP.CloseDataBas();
            }



            StringBuilder sb = new StringBuilder();
            foreach (Table table in database.Tables)
            {
                BuildingTableAll(table, dataBasAPP);
            }
        }

        public void BuildingTableAll(Table table, DataBasAPP dataBasAPP)
        {
            BuildingTable(table, dataBasAPP);
            string sql = BuildingInsertProcedure(table);
            string name = $"{table.Name}_Insert";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingUpdateProcedure(table);
            name = $"{table.Name}_Update";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingDeleteProcedure(table);
            name = $"{table.Name}_Delete";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingAllMainKeysProcedure(table);
            name = $"{table.Name}_SELECT_ALL";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingAllKeyAndVerProcedure(table);
            name = $"{table.Name}_SELECT_ALLKEY_AND_VER";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingInitializationResultProcedure(table, MaxRangeKeyCount.Count1);
            name = $"{table.Name}_INITIALIZATION_SELECT_KEY";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingInitializationResultProcedure(table, MaxRangeKeyCount.Count2);
            name = $"{table.Name}_INITIALIZATION_SELECT_KEY_2";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingInitializationResultProcedure(table, MaxRangeKeyCount.Count5);
            name = $"{table.Name}_INITIALIZATION_SELECT_KEY_5";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingInitializationResultProcedure(table, MaxRangeKeyCount.Count10);
            name = $"{table.Name}_INITIALIZATION_SELECT_KEY_10";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingInitializationResultProcedure(table, MaxRangeKeyCount.Count20);
            name = $"{table.Name}_INITIALIZATION_SELECT_KEY_20";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingInitializationResultProcedure(table, MaxRangeKeyCount.Count50);
            name = $"{table.Name}_INITIALIZATION_SELECT_KEY_50";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingResultProcedure(table, MaxRangeKeyCount.Count1);
            name = $"{table.Name}_SELECT_KEY";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingResultProcedure(table, MaxRangeKeyCount.Count2);
            name = $"{table.Name}_SELECT_KEY_2";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingResultProcedure(table, MaxRangeKeyCount.Count5);
            name = $"{table.Name}_SELECT_KEY_5";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingResultProcedure(table, MaxRangeKeyCount.Count10);
            name = $"{table.Name}_SELECT_KEY_10";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingResultProcedure(table, MaxRangeKeyCount.Count20);
            name = $"{table.Name}_SELECT_KEY_20";
            SaveProcedure(name, sql, dataBasAPP);

            sql = BuildingResultProcedure(table, MaxRangeKeyCount.Count50);
            name = $"{table.Name}_SELECT_KEY_50";
            SaveProcedure(name, sql, dataBasAPP);
        }




        public ColumnEntity[] GetPrimaryKeys(Table table)
        {
            List<ColumnEntity> primaryKeys = new List<ColumnEntity>();
            foreach (ColumnEntity column in table.Columns)
            {
                if (column is ColumnEntitys.PrimaryKey)
                {
                    primaryKeys.Add(column);
                }
            }
            return primaryKeys.ToArray();

        }

        public ColumnEntity[] GetVers(Table table)
        {
            List<ColumnEntity> vers = new List<ColumnEntity>();
            foreach (ColumnEntity column in table.Columns)
            {
                if (column is ColumnEntitys.Ver)
                {
                    vers.Add(column);
                }
            }
            return vers.ToArray();

        }


        public void BuildingTable(Table table, DataBasAPP dataBasAPP)
        {
            ColumnEntity[] primaryKeys = GetPrimaryKeys(table);


            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"BEGIN TRANSACTION;");
            sb.AppendLine($"DROP TABLE IF EXISTS [{table.Name}];");
            sb.AppendLine($"CREATE TABLE [{table.Name}] (");
            foreach (ColumnEntity column in table.Columns)
            {
                sb.AppendLine($"\t{BuildingColumn(column).Trim()},");
            }

            foreach (ColumnEntity column in primaryKeys)
            {
                sb.AppendLine($"\tPRIMARY KEY(\"{column.Name}\"),");
            }
            sb.Remove(sb.Length - 3, 3);
            sb.AppendLine(");");



            if (table.HasSequence())
            {

                sb.AppendLine($"DROP TABLE IF EXISTS [{table.Name}_Sequence_AutoIncrement];");
                sb.AppendLine($"CREATE TRIGGER [{table.Name}_Sequence_AutoIncrement]");
                sb.AppendLine($"AFTER INSERT ON [{table.Name}]");
                sb.AppendLine($"WHEN NEW.Sequence = 0");
                sb.AppendLine($"BEGIN");
                sb.AppendLine($"UPDATE [{table.Name}]");
                sb.AppendLine($"SET Sequence = (");
                sb.AppendLine($"SELECT IFNULL(MAX(Sequence), 0) + 1");
                sb.AppendLine($"FROM [{table.Name}]");
                sb.AppendLine($")");
                sb.AppendLine($"WHERE PrimaryKey = NEW.PrimaryKey;");
                sb.AppendLine($"END;");
            }




            sb.AppendLine("COMMIT;");

            try
            {
                dataBasAPP.OpenDataBas();
                dataBasAPP.Param.CommandType = DataBasAPP.ExecuteType.SQL;
                dataBasAPP.Param.Command = sb.ToString();
                dataBasAPP.Return();
            }
            finally
            {
                dataBasAPP.CloseDataBas();
            }



        }


        void SaveProcedure(string name, string sql, DataBasAPP dataBasAPP)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"BEGIN TRANSACTION;");
            sb.AppendLine($"Delete From [Procedures]");
            sb.AppendLine($"where");
            sb.AppendLine($"Name='{name}';");
            sb.AppendLine($"INSERT INTO [Procedures] (");
            sb.AppendLine($"\tName,");
            sb.AppendLine($"\tSql");
            sb.AppendLine($")");
            sb.AppendLine($"VALUES(");
            sb.AppendLine($"\t@P_0,");
            sb.AppendLine($"\t@P_1");
            sb.AppendLine($");");
            sb.AppendLine("COMMIT;");

            try
            {
                dataBasAPP.OpenDataBas();
                dataBasAPP.Param.CommandType = DataBasAPP.ExecuteType.SQL;
                dataBasAPP.Param.Command = sb.ToString();
                dataBasAPP.Param.AddParam(name);
                dataBasAPP.Param.AddParam(sql);
                dataBasAPP.Return();
            }
            finally
            {
                dataBasAPP.CloseDataBas();
            }
        }






        public string BuildingColumn(ColumnEntity column)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"[{column.Name}] ");
            sb.Append($"{TypeNameToSQLType(column)} ");
            if (column.IsUnique)
            {
                sb.Append($"UNIQUE ");
            }

            if (column.PermitNull)
            {
                sb.Append("NOT NULL");
            }

            return sb.ToString();

        }


        public string BuildingInsertProcedure(Table table)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine($"INSERT INTO [{table.Name}] (");
            foreach (ColumnEntity column in table.Columns)
            {
                sql.AppendLine($"\t{column.Name},");
            }
            sql.Remove(sql.Length - 3, 3);
            sql.AppendLine($")");
            sql.AppendLine($"VALUES(");

            for (int i = 0; i < table.Columns.Count; i++)
            {
                sql.AppendLine($"@P_{i},");
            }
            sql.Remove(sql.Length - 3, 3);
            sql.AppendLine($");");

            return sql.ToString();
        }

        public string BuildingUpdateProcedure(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Update [{table.Name}]");
            sb.AppendLine($"Set");
            int count = 0;

            List<KeyValuePair<string, int>> primaryKeys = new List<KeyValuePair<string, int>>();



            foreach (ColumnEntity column in table.Columns)
            {
                if (column is PrimaryKey)
                {
                    primaryKeys.Add(new KeyValuePair<string, int>(column.Name, count));
                }
                sb.AppendLine($"\t{column.Name}=@P_{count},");
                count++;
            }
            sb.Remove(sb.Length - 3, 3);
            sb.AppendLine();
            sb.AppendLine($"where");

            foreach (KeyValuePair<string, int> item in primaryKeys)
            {
                sb.AppendLine($"\t{item.Key}=@P_{item.Value} and");
            }

            sb.Remove(sb.Length - 6, 6);

            return sb.ToString();
        }

        public string BuildingDeleteProcedure(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Delete From [{table.Name}]");
            sb.AppendLine($"where");
            ColumnEntity[] primaryKeys = GetPrimaryKeys(table);
            int count = 0;
            foreach (ColumnEntity item in primaryKeys)
            {
                sb.AppendLine($"\t{item.Name}=@P_{count} and");
                count++;
            }

            sb.Remove(sb.Length - 6, 6);

            return sb.ToString();

        }


        public string BuildingAllMainKeysProcedure(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"SELECT [PRIMARYKEY] FROM [{table.Name}]");
            return sb.ToString();
        }

        public string BuildingAllKeyAndVerProcedure(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"SELECT [PRIMARYKEY],[EDITVER] FROM [{table.Name}]");
            return sb.ToString();
        }


        public string BuildingResultProcedure(Table table, MaxRangeKeyCount maxRangeKeyCount)
        {
            ColumnEntity[] primaryKeys = GetPrimaryKeys(table);


            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Select");
            foreach (ColumnEntity column in table.Columns)
            {
                sb.AppendLine($"\t{column.Name},");
            }
            sb.Remove(sb.Length - 3, 3);
            sb.AppendLine();
            sb.AppendLine($"From [{table.Name}]");
            
            sb.AppendLine("where");

            int count = 0;

            switch (maxRangeKeyCount)
            {

                case MaxRangeKeyCount.Count50:
                {
                    count = 50;
                    break;
                }
                case MaxRangeKeyCount.Count20:
                {
                    count = 20;
                    break;
                }
                case MaxRangeKeyCount.Count10:
                {
                    count = 10;
                    break;
                }
                case MaxRangeKeyCount.Count5:
                {
                    count = 5;
                    break;
                }
                case MaxRangeKeyCount.Count2:
                {
                    count = 2;
                    break;
                }
                case MaxRangeKeyCount.Count1:
                {
                    count = 1;
                    break;
                }
                default:
                {
                    throw new NotImplementedException(Enum.GetName(typeof(MaxRangeKeyCount), maxRangeKeyCount));
                }
            }

            int count1 = 0;
            for (int i = 0; i < count; i++)
            {

                StringBuilder sb1 = new StringBuilder();
                foreach (ColumnEntity column in primaryKeys)
                {
                    sb1.Append($"{column.Name}=@P_{count1} and ");
                    count1++;
                }
                sb1.Remove(sb1.Length - 4, 4);
                sb.AppendLine($"\t{sb1.ToString()} or");
            }

            sb.Remove(sb.Length - 4, 4);

            return sb.ToString();

        }




        public string BuildingInitializationResultProcedure(Table table, MaxRangeKeyCount maxRangeKeyCount)
        {
            ColumnEntity[] primaryKeys = GetPrimaryKeys(table);
            ColumnEntity[] vers = GetVers(table);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Select");
            foreach (ColumnEntity column in table.Columns)
            {
                sb.AppendLine($"\t{column.Name},");
            }
            sb.Remove(sb.Length - 3, 3);
            sb.AppendLine();
            sb.AppendLine($"From [{table.Name}]");

            sb.AppendLine("where");

            int count = 0;

            switch (maxRangeKeyCount)
            {

                case MaxRangeKeyCount.Count50:
                {
                    count = 50;
                    break;
                }
                case MaxRangeKeyCount.Count20:
                {
                    count = 20;
                    break;
                }
                case MaxRangeKeyCount.Count10:
                {
                    count = 10;
                    break;
                }
                case MaxRangeKeyCount.Count5:
                {
                    count = 5;
                    break;
                }
                case MaxRangeKeyCount.Count2:
                {
                    count = 2;
                    break;
                }
                case MaxRangeKeyCount.Count1:
                {
                    count = 1;
                    break;
                }
                default:
                {
                    throw new NotImplementedException(Enum.GetName(typeof(MaxRangeKeyCount), maxRangeKeyCount));
                }
            }

            int count1 = 0;
            for (int i = 0; i < count; i++)
            {

                StringBuilder sb1 = new StringBuilder();
                foreach (ColumnEntity column in primaryKeys)
                {
                    sb1.Append($"{column.Name}=@P_{count1} and ");
                    count1++;
                }
                sb1.Remove(sb1.Length - 4, 4);
                sb.AppendLine($"\t{sb1.ToString()} or");
            }

            sb.Remove(sb.Length - 4, 4);

            return sb.ToString();

        }





        /// <summary>
        /// 根据类型名获得SQL对应类型
        /// </summary>
        /// <param name="colProperty"></param>
        /// <returns></returns>
        public string TypeNameToSQLType(ColumnEntity colProperty)
        {
            return colProperty.GetType() switch
            {
                Type t when t == typeof(ColumnEntitys.ArtemisEntity) => "[Text]",
                Type t when t == typeof(ColumnEntitys.Binary) => "[BLOB]",
                Type t when t == typeof(ColumnEntitys.Boolean) => "[NUMERIC]",
                Type t when t == typeof(ColumnEntitys.Char) => $"[Char]({((ColumnEntitys.Char)colProperty).Length})",
                Type t when t == typeof(ColumnEntitys.DataRowName) => "[TEXT]",
                Type t when t == typeof(ColumnEntitys.DataRowNameTextSection) => "[TEXT]",
                Type t when t == typeof(ColumnEntitys.Date) => "[DateTime]",
                Type t when t == typeof(ColumnEntitys.DateTime) => "[Datetime]",
                Type t when t == typeof(ColumnEntitys.Decimal) => "[REAL]",
                Type t when t == typeof(ColumnEntitys.Double) => "[REAL]",
                Type t when t == typeof(ColumnEntitys.Enumerate) => "[INTEGER]",
                Type t when t == typeof(ColumnEntitys.Integer32) => "[INTEGER]",
                Type t when t == typeof(ColumnEntitys.Integer64) => "[INTEGER]",
                Type t when t == typeof(ColumnEntitys.Json) => "[TEXT]",
                Type t when t == typeof(ColumnEntitys.PrimaryKey) => "[INTEGER]",
                Type t when t == typeof(ColumnEntitys.Sequence) => "[INTEGER]",
                Type t when t == typeof(ColumnEntitys.Uuid) => "[TEXT]",
                Type t when t == typeof(ColumnEntitys.VarBinary) => "[BLOB]",
                Type t when t == typeof(ColumnEntitys.VarChar) => $"[VarChar]({((VarChar)colProperty).Length})",
                Type t when t == typeof(ColumnEntitys.Ver) => "[INTEGER]",
                Type t when t == typeof(ColumnEntitys.Xml) => "[TEXT]",
                

                _ => throw new NotImplementedException(colProperty.GetType().Name)
            };

        }
    }
}
