using LeadTurbo.VirtualDatabase.ColumnEntitys;
using LeadTurbo.VirtualDatabase.Operations.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static LeadTurbo.Function;

namespace LeadTurbo.VirtualDatabase
{
    /// <summary>
    /// 为 SQL Server 生成建表脚本和真实存储过程的工具。
    /// 与 SQLite 版的差异：SQL Server 有真实存储过程，因此每个 Building*Procedure
    /// 直接返回完整的 CREATE OR ALTER PROCEDURE 脚本并就地执行，
    /// 而不是写进 [Procedures] 表。
    /// </summary>
    public class SQLForBuildingASQLServer : SQLForBuildingDatabase
    {
        /// <summary>
        /// 默认 schema，可在使用方修改。
        /// </summary>
        public string Schema { get; set; } = "dbo";

        public override void Building(Database database)
        {
            DataBasApp dataBasAPP = new DataBasAppSQLServer();
            dataBasAPP.ConnectionString = database.ConnectString;

            foreach (Table table in database.Tables)
            {
                BuildingTableAll(table, dataBasAPP);
            }
        }

        public override void BuildingTableAll(Table table, DataBasApp dataBasAPP)
        {
            BuildingTable(table, dataBasAPP);

            ExecuteSql(BuildingInsertProcedure(table), dataBasAPP);
            ExecuteSql(BuildingUpdateProcedure(table), dataBasAPP);
            ExecuteSql(BuildingDeleteProcedure(table), dataBasAPP);
            ExecuteSql(BuildingAllMainKeysProcedure(table), dataBasAPP);
            ExecuteSql(BuildingAllKeyAndVerProcedure(table), dataBasAPP);

            ExecuteSql(BuildingInitializationResultProcedure(table, MaxRangeKeyCount.Count1), dataBasAPP);
            ExecuteSql(BuildingInitializationResultProcedure(table, MaxRangeKeyCount.Count2), dataBasAPP);
            ExecuteSql(BuildingInitializationResultProcedure(table, MaxRangeKeyCount.Count5), dataBasAPP);
            ExecuteSql(BuildingInitializationResultProcedure(table, MaxRangeKeyCount.Count10), dataBasAPP);
            ExecuteSql(BuildingInitializationResultProcedure(table, MaxRangeKeyCount.Count20), dataBasAPP);
            ExecuteSql(BuildingInitializationResultProcedure(table, MaxRangeKeyCount.Count50), dataBasAPP);

            ExecuteSql(BuildingResultProcedure(table, MaxRangeKeyCount.Count1), dataBasAPP);
            ExecuteSql(BuildingResultProcedure(table, MaxRangeKeyCount.Count2), dataBasAPP);
            ExecuteSql(BuildingResultProcedure(table, MaxRangeKeyCount.Count5), dataBasAPP);
            ExecuteSql(BuildingResultProcedure(table, MaxRangeKeyCount.Count10), dataBasAPP);
            ExecuteSql(BuildingResultProcedure(table, MaxRangeKeyCount.Count20), dataBasAPP);
            ExecuteSql(BuildingResultProcedure(table, MaxRangeKeyCount.Count50), dataBasAPP);
        }

        public override void BuildingTable(Table table, DataBasApp dataBasAPP)
        {
            ColumnEntity[] primaryKeys = GetPrimaryKeys(table);

            ExecuteSql(
                $"IF OBJECT_ID('[{Schema}].[{table.Name}]', 'U') IS NOT NULL DROP TABLE [{Schema}].[{table.Name}];",
                dataBasAPP);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CREATE TABLE [{Schema}].[{table.Name}] (");

            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append($"\t{BuildingColumn(table.Columns[i]).Trim()}");
                bool needComma = (i < table.Columns.Count - 1) || primaryKeys.Length > 0;
                sb.AppendLine(needComma ? "," : string.Empty);
            }

            if (primaryKeys.Length > 0)
            {
                sb.Append($"\tCONSTRAINT [PK_{table.Name}] PRIMARY KEY (");
                for (int i = 0; i < primaryKeys.Length; i++)
                {
                    sb.Append($"[{primaryKeys[i].Name}]");
                    if (i < primaryKeys.Length - 1) sb.Append(", ");
                }
                sb.AppendLine(")");
            }
            sb.AppendLine(");");

            ExecuteSql(sb.ToString(), dataBasAPP);

            if (table.HasSequence())
            {
                // 不再使用 trigger，改为在 INSERT 时直接 (SELECT MAX+1)；
                // UNIQUE 索引兼顾两件事：MAX() 查询走索引很快，并发冲突时由唯一约束自然失败，由调用方重试
                ExecuteSql(
                    $"CREATE UNIQUE INDEX [IX_{table.Name}_Sequence] ON [{Schema}].[{table.Name}] ([Sequence]);",
                    dataBasAPP);
            }
        }

        void ExecuteSql(string sql, DataBasApp dataBasAPP)
        {
            try
            {
                dataBasAPP.OpenDataBas();
                dataBasAPP.Param.CommandType = DataBasApp.ExecuteType.SQL;
                dataBasAPP.Param.Command = sql;
                dataBasAPP.Return();
            }
            finally
            {
                dataBasAPP.CloseDataBas();
            }
        }

        public override string BuildingColumn(ColumnEntity column)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"[{column.Name}] ");
            sb.Append($"{TypeNameToSQLType(column)} ");
            if (column.IsUnique && !(column is PrimaryKey))
            {
                sb.Append("UNIQUE ");
            }
            sb.Append(column.PermitNull ? "NULL" : "NOT NULL");
            return sb.ToString();
        }

        public override string BuildingInsertProcedure(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CREATE OR ALTER PROCEDURE [{Schema}].[{table.Name}_Insert]");

            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append($"\t@P_{i} {TypeNameToSQLType(table.Columns[i])}");
                sb.AppendLine(i < table.Columns.Count - 1 ? "," : string.Empty);
            }

            sb.AppendLine("AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine("\tSET NOCOUNT ON;");

            sb.Append($"\tINSERT INTO [{Schema}].[{table.Name}] (");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append($"[{table.Columns[i].Name}]");
                if (i < table.Columns.Count - 1) sb.Append(", ");
            }
            sb.AppendLine(")");

            sb.Append("\tVALUES (");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (table.Columns[i] is Sequence)
                {
                    // Sequence 列：忽略调用方传值，自动取 MAX+1
                    sb.Append($"ISNULL((SELECT MAX([Sequence]) FROM [{Schema}].[{table.Name}]), 0) + 1");
                }
                else
                {
                    sb.Append($"@P_{i}");
                }
                if (i < table.Columns.Count - 1) sb.Append(", ");
            }
            sb.AppendLine(");");

            sb.AppendLine("END");
            return sb.ToString();
        }

        public override string BuildingUpdateProcedure(Table table)
        {
            List<KeyValuePair<string, int>> primaryKeys = new List<KeyValuePair<string, int>>();
            List<KeyValuePair<string, int>> vers = new List<KeyValuePair<string, int>>();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CREATE OR ALTER PROCEDURE [{Schema}].[{table.Name}_Update]");

            for (int i = 0; i < table.Columns.Count; i++)
            {
                ColumnEntity col = table.Columns[i];
                if (col is PrimaryKey)
                {
                    primaryKeys.Add(new KeyValuePair<string, int>(col.Name, i));
                }
                if (col is Ver)
                {
                    vers.Add(new KeyValuePair<string, int>(col.Name, i));
                }
                sb.Append($"\t@P_{i} {TypeNameToSQLType(col)}");
                sb.AppendLine(i < table.Columns.Count - 1 ? "," : string.Empty);
            }

            sb.AppendLine("AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine("\tSET NOCOUNT ON;");
            sb.AppendLine($"\tUPDATE [{Schema}].[{table.Name}]");
            sb.AppendLine("\tSET");

            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append($"\t\t[{table.Columns[i].Name}] = @P_{i}");
                sb.AppendLine(i < table.Columns.Count - 1 ? "," : string.Empty);
            }

            sb.AppendLine("\tWHERE");
            int totalWhere = primaryKeys.Count + vers.Count;
            int written = 0;
            foreach (KeyValuePair<string, int> item in primaryKeys)
            {
                sb.Append($"\t\t[{item.Key}] = @P_{item.Value}");
                written++;
                sb.AppendLine(written < totalWhere ? " AND" : ";");
            }
            foreach (KeyValuePair<string, int> item in vers)
            {
                sb.Append($"\t\t[{item.Key}] = @P_{item.Value} - 1");
                written++;
                sb.AppendLine(written < totalWhere ? " AND" : ";");
            }

            sb.AppendLine("END");
            return sb.ToString();
        }

        public override string BuildingDeleteProcedure(Table table)
        {
            ColumnEntity[] primaryKeys = GetPrimaryKeys(table);
            ColumnEntity[] vers = GetVers(table);
            int totalParams = primaryKeys.Length + vers.Length;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CREATE OR ALTER PROCEDURE [{Schema}].[{table.Name}_Delete]");

            int paramIdx = 0;
            foreach (ColumnEntity item in primaryKeys)
            {
                sb.Append($"\t@P_{paramIdx} {TypeNameToSQLType(item)}");
                paramIdx++;
                sb.AppendLine(paramIdx < totalParams ? "," : string.Empty);
            }
            foreach (ColumnEntity item in vers)
            {
                sb.Append($"\t@P_{paramIdx} {TypeNameToSQLType(item)}");
                paramIdx++;
                sb.AppendLine(paramIdx < totalParams ? "," : string.Empty);
            }

            sb.AppendLine("AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine("\tSET NOCOUNT ON;");
            sb.AppendLine($"\tDELETE FROM [{Schema}].[{table.Name}]");
            sb.AppendLine("\tWHERE");

            paramIdx = 0;
            int written = 0;
            foreach (ColumnEntity item in primaryKeys)
            {
                sb.Append($"\t\t[{item.Name}] = @P_{paramIdx}");
                paramIdx++;
                written++;
                sb.AppendLine(written < totalParams ? " AND" : ";");
            }
            foreach (ColumnEntity item in vers)
            {
                sb.Append($"\t\t[{item.Name}] = @P_{paramIdx} - 1");
                paramIdx++;
                written++;
                sb.AppendLine(written < totalParams ? " AND" : ";");
            }
            sb.AppendLine("END");
            return sb.ToString();
        }

        public override string BuildingAllMainKeysProcedure(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CREATE OR ALTER PROCEDURE [{Schema}].[{table.Name}_SELECT_ALL]");
            sb.AppendLine("AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine("\tSET NOCOUNT ON;");
            sb.AppendLine($"\tSELECT [PRIMARYKEY] FROM [{Schema}].[{table.Name}];");
            sb.AppendLine("END");
            return sb.ToString();
        }

        public override string BuildingAllKeyAndVerProcedure(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CREATE OR ALTER PROCEDURE [{Schema}].[{table.Name}_SELECT_ALLKEY_AND_VER]");
            sb.AppendLine("AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine("\tSET NOCOUNT ON;");
            sb.AppendLine($"\tSELECT [PRIMARYKEY], [EDITVER] FROM [{Schema}].[{table.Name}];");
            sb.AppendLine("END");
            return sb.ToString();
        }

        public override string BuildingResultProcedure(Table table, MaxRangeKeyCount maxRangeKeyCount)
        {
            return BuildSelectByKeyProcedure(
                table,
                maxRangeKeyCount,
                ProcedureNameForResult(table, maxRangeKeyCount));
        }

        public override string BuildingInitializationResultProcedure(Table table, MaxRangeKeyCount maxRangeKeyCount)
        {
            return BuildSelectByKeyProcedure(
                table,
                maxRangeKeyCount,
                ProcedureNameForInitializationResult(table, maxRangeKeyCount));
        }

        string ProcedureNameForResult(Table table, MaxRangeKeyCount c)
        {
            int n = MaxRangeKeyCountToInt(c);
            return n == 1 ? $"{table.Name}_SELECT_KEY" : $"{table.Name}_SELECT_KEY_{n}";
        }

        string ProcedureNameForInitializationResult(Table table, MaxRangeKeyCount c)
        {
            int n = MaxRangeKeyCountToInt(c);
            return n == 1 ? $"{table.Name}_INITIALIZATION_SELECT_KEY" : $"{table.Name}_INITIALIZATION_SELECT_KEY_{n}";
        }

        string BuildSelectByKeyProcedure(Table table, MaxRangeKeyCount maxRangeKeyCount, string procedureName)
        {
            ColumnEntity[] primaryKeys = GetPrimaryKeys(table);
            int rangeCount = MaxRangeKeyCountToInt(maxRangeKeyCount);
            int totalParams = rangeCount * primaryKeys.Length;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CREATE OR ALTER PROCEDURE [{Schema}].[{procedureName}]");

            int paramIdx = 0;
            for (int r = 0; r < rangeCount; r++)
            {
                for (int k = 0; k < primaryKeys.Length; k++)
                {
                    sb.Append($"\t@P_{paramIdx} {TypeNameToSQLType(primaryKeys[k])}");
                    sb.AppendLine(paramIdx < totalParams - 1 ? "," : string.Empty);
                    paramIdx++;
                }
            }

            sb.AppendLine("AS");
            sb.AppendLine("BEGIN");
            sb.AppendLine("\tSET NOCOUNT ON;");
            sb.AppendLine("\tSELECT");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append($"\t\t[{table.Columns[i].Name}]");
                sb.AppendLine(i < table.Columns.Count - 1 ? "," : string.Empty);
            }
            sb.AppendLine($"\tFROM [{Schema}].[{table.Name}]");
            sb.AppendLine("\tWHERE");

            paramIdx = 0;
            for (int r = 0; r < rangeCount; r++)
            {
                StringBuilder clause = new StringBuilder();
                for (int k = 0; k < primaryKeys.Length; k++)
                {
                    clause.Append($"[{primaryKeys[k].Name}] = @P_{paramIdx}");
                    if (k < primaryKeys.Length - 1) clause.Append(" AND ");
                    paramIdx++;
                }
                sb.Append($"\t\t({clause})");
                sb.AppendLine(r < rangeCount - 1 ? " OR" : ";");
            }

            sb.AppendLine("END");
            return sb.ToString();
        }

        /// <summary>
        /// 根据列实体类型映射到 SQL Server 数据类型。
        /// 字符串使用 N 系列（Unicode），主键/外键/序列使用 BIGINT。
        /// </summary>
        public override string TypeNameToSQLType(ColumnEntity column)
        {
            return column switch
            {
                ColumnEntitys.ArtemisEntity => "NVARCHAR(MAX)",
                ColumnEntitys.Xml => "XML",
                ColumnEntitys.Json => "NVARCHAR(MAX)",
                ColumnEntitys.Uuid => "UNIQUEIDENTIFIER",
                ColumnEntitys.Date => "DATE",
                ColumnEntitys.DateTime => "DATETIME2",
                ColumnEntitys.Char charColumn => $"NCHAR({charColumn.Length})",
                VarChar varCharColumn => $"NVARCHAR({varCharColumn.Length})",
                Binary binaryColumn => $"BINARY({binaryColumn.Length})",
                VarBinary varBinaryColumn => $"VARBINARY({varBinaryColumn.Length})",
                VarBinaryMax => "VARBINARY(MAX)",
                ColumnEntitys.Boolean => "BIT",
                ColumnEntitys.Decimal decimalColumn => $"DECIMAL(18, {decimalColumn.Precision})",
                ColumnEntitys.Double => "FLOAT",
                ForeignKey => "BIGINT",
                PrimaryKey => "BIGINT",
                Sequence => "BIGINT",
                Ver => "INT",
                Integer32 => "INT",
                Integer64 => "BIGINT",
                _ => throw new NotImplementedException(column.GetType().Name)
            };
        }
    }
}
