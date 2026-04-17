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
    /// 为 PostgreSQL 生成建表脚本和"存储过程"（CREATE FUNCTION）的工具。
    /// 与 SQL Server 版的差异：
    ///   - 用 CREATE FUNCTION（dollar-quoted body）；标识符用双引号。
    ///   - DML（Update/Delete）函数返回 integer（受影响行数，便于乐观并发判断）。
    ///   - 行集合查询用 RETURNS SETOF "tbl" 或 RETURNS TABLE(...)。
    ///   - Sequence 自动填充改用 BEFORE INSERT 触发器（PG 风格）。
    /// </summary>
    public class SQLForBuildingAPostgreSQL : SQLForBuildingDatabase
    {
        /// <summary>
        /// 默认 schema，可在使用方修改。
        /// </summary>
        public string Schema { get; set; } = "public";

        public override void Building(Database database)
        {
            DataBasApp dataBasAPP = new DataBasAppPostgreSQL();
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
                $"DROP TABLE IF EXISTS \"{Schema}\".\"{table.Name}\" CASCADE;",
                dataBasAPP);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"CREATE TABLE \"{Schema}\".\"{table.Name}\" (");

            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append($"\t{BuildingColumn(table.Columns[i]).Trim()}");
                bool needComma = (i < table.Columns.Count - 1) || primaryKeys.Length > 0;
                sb.AppendLine(needComma ? "," : string.Empty);
            }

            if (primaryKeys.Length > 0)
            {
                sb.Append($"\tCONSTRAINT \"PK_{table.Name}\" PRIMARY KEY (");
                for (int i = 0; i < primaryKeys.Length; i++)
                {
                    sb.Append($"\"{primaryKeys[i].Name}\"");
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
                    $"CREATE UNIQUE INDEX IF NOT EXISTS \"IX_{table.Name}_Sequence\" ON \"{Schema}\".\"{table.Name}\" (\"Sequence\");",
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
            sb.Append($"\"{column.Name}\" ");
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
            sb.AppendLine($"DROP FUNCTION IF EXISTS \"{Schema}\".\"{table.Name}_Insert\";");
            sb.AppendLine($"CREATE FUNCTION \"{Schema}\".\"{table.Name}_Insert\"(");

            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append($"\tp_{i} {TypeNameToSQLType(table.Columns[i])}");
                sb.AppendLine(i < table.Columns.Count - 1 ? "," : string.Empty);
            }
            sb.AppendLine(")");
            sb.AppendLine("RETURNS void");
            sb.AppendLine("LANGUAGE sql");
            sb.AppendLine("AS $$");

            sb.Append($"\tINSERT INTO \"{Schema}\".\"{table.Name}\" (");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append($"\"{table.Columns[i].Name}\"");
                if (i < table.Columns.Count - 1) sb.Append(", ");
            }
            sb.AppendLine(")");

            sb.Append("\tVALUES (");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (table.Columns[i] is Sequence)
                {
                    // Sequence 列：忽略调用方传值，自动取 MAX+1
                    sb.Append($"COALESCE((SELECT MAX(\"Sequence\") FROM \"{Schema}\".\"{table.Name}\"), 0) + 1");
                }
                else
                {
                    sb.Append($"p_{i}");
                }
                if (i < table.Columns.Count - 1) sb.Append(", ");
            }
            sb.AppendLine(");");

            sb.AppendLine("$$;");
            return sb.ToString();
        }

        public override string BuildingUpdateProcedure(Table table)
        {
            List<KeyValuePair<string, int>> primaryKeys = new List<KeyValuePair<string, int>>();
            List<KeyValuePair<string, int>> vers = new List<KeyValuePair<string, int>>();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"DROP FUNCTION IF EXISTS \"{Schema}\".\"{table.Name}_Update\";");
            sb.AppendLine($"CREATE FUNCTION \"{Schema}\".\"{table.Name}_Update\"(");

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
                sb.Append($"\tp_{i} {TypeNameToSQLType(col)}");
                sb.AppendLine(i < table.Columns.Count - 1 ? "," : string.Empty);
            }

            sb.AppendLine(")");
            sb.AppendLine("RETURNS integer");
            sb.AppendLine("LANGUAGE plpgsql");
            sb.AppendLine("AS $$");
            sb.AppendLine("DECLARE");
            sb.AppendLine("\tv_count integer;");
            sb.AppendLine("BEGIN");
            sb.AppendLine($"\tUPDATE \"{Schema}\".\"{table.Name}\"");
            sb.AppendLine("\tSET");

            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append($"\t\t\"{table.Columns[i].Name}\" = p_{i}");
                sb.AppendLine(i < table.Columns.Count - 1 ? "," : string.Empty);
            }

            sb.AppendLine("\tWHERE");
            int totalWhere = primaryKeys.Count + vers.Count;
            int written = 0;
            foreach (KeyValuePair<string, int> item in primaryKeys)
            {
                sb.Append($"\t\t\"{item.Key}\" = p_{item.Value}");
                written++;
                sb.AppendLine(written < totalWhere ? " AND" : ";");
            }
            foreach (KeyValuePair<string, int> item in vers)
            {
                sb.Append($"\t\t\"{item.Key}\" = p_{item.Value} - 1");
                written++;
                sb.AppendLine(written < totalWhere ? " AND" : ";");
            }

            sb.AppendLine("\tGET DIAGNOSTICS v_count = ROW_COUNT;");
            sb.AppendLine("\tRETURN v_count;");
            sb.AppendLine("END;");
            sb.AppendLine("$$;");
            return sb.ToString();
        }

        public override string BuildingDeleteProcedure(Table table)
        {
            ColumnEntity[] primaryKeys = GetPrimaryKeys(table);
            ColumnEntity[] vers = GetVers(table);
            int totalParams = primaryKeys.Length + vers.Length;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"DROP FUNCTION IF EXISTS \"{Schema}\".\"{table.Name}_Delete\";");
            sb.AppendLine($"CREATE FUNCTION \"{Schema}\".\"{table.Name}_Delete\"(");

            int paramIdx = 0;
            foreach (ColumnEntity item in primaryKeys)
            {
                sb.Append($"\tp_{paramIdx} {TypeNameToSQLType(item)}");
                paramIdx++;
                sb.AppendLine(paramIdx < totalParams ? "," : string.Empty);
            }
            foreach (ColumnEntity item in vers)
            {
                sb.Append($"\tp_{paramIdx} {TypeNameToSQLType(item)}");
                paramIdx++;
                sb.AppendLine(paramIdx < totalParams ? "," : string.Empty);
            }

            sb.AppendLine(")");
            sb.AppendLine("RETURNS integer");
            sb.AppendLine("LANGUAGE plpgsql");
            sb.AppendLine("AS $$");
            sb.AppendLine("DECLARE");
            sb.AppendLine("\tv_count integer;");
            sb.AppendLine("BEGIN");
            sb.AppendLine($"\tDELETE FROM \"{Schema}\".\"{table.Name}\"");
            sb.AppendLine("\tWHERE");

            paramIdx = 0;
            int written = 0;
            foreach (ColumnEntity item in primaryKeys)
            {
                sb.Append($"\t\t\"{item.Name}\" = p_{paramIdx}");
                paramIdx++;
                written++;
                sb.AppendLine(written < totalParams ? " AND" : ";");
            }
            foreach (ColumnEntity item in vers)
            {
                sb.Append($"\t\t\"{item.Name}\" = p_{paramIdx} - 1");
                paramIdx++;
                written++;
                sb.AppendLine(written < totalParams ? " AND" : ";");
            }

            sb.AppendLine("\tGET DIAGNOSTICS v_count = ROW_COUNT;");
            sb.AppendLine("\tRETURN v_count;");
            sb.AppendLine("END;");
            sb.AppendLine("$$;");
            return sb.ToString();
        }

        public override string BuildingAllMainKeysProcedure(Table table)
        {
            ColumnEntity[] primaryKeys = GetPrimaryKeys(table);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"DROP FUNCTION IF EXISTS \"{Schema}\".\"{table.Name}_SELECT_ALL\";");
            sb.Append($"CREATE FUNCTION \"{Schema}\".\"{table.Name}_SELECT_ALL\"() RETURNS TABLE(");
            for (int i = 0; i < primaryKeys.Length; i++)
            {
                // 输出列名用大写以与 SQL Server 版结果列名一致；
                // 与表里的 "PrimaryKey" 因大小写不同不会冲突。
                sb.Append($"\"{primaryKeys[i].Name.ToUpperInvariant()}\" {TypeNameToSQLType(primaryKeys[i])}");
                if (i < primaryKeys.Length - 1) sb.Append(", ");
            }
            sb.AppendLine(")");
            sb.AppendLine("LANGUAGE sql");
            sb.AppendLine("AS $$");
            sb.Append("\tSELECT ");
            for (int i = 0; i < primaryKeys.Length; i++)
            {
                sb.Append($"\"{primaryKeys[i].Name}\"");
                if (i < primaryKeys.Length - 1) sb.Append(", ");
            }
            sb.AppendLine($" FROM \"{Schema}\".\"{table.Name}\";");
            sb.AppendLine("$$;");
            return sb.ToString();
        }

        public override string BuildingAllKeyAndVerProcedure(Table table)
        {
            ColumnEntity[] primaryKeys = GetPrimaryKeys(table);
            ColumnEntity[] vers = GetVers(table);
            ColumnEntity[] all = primaryKeys.Concat(vers).ToArray();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"DROP FUNCTION IF EXISTS \"{Schema}\".\"{table.Name}_SELECT_ALLKEY_AND_VER\";");
            sb.Append($"CREATE FUNCTION \"{Schema}\".\"{table.Name}_SELECT_ALLKEY_AND_VER\"() RETURNS TABLE(");
            for (int i = 0; i < all.Length; i++)
            {
                sb.Append($"\"{all[i].Name.ToUpperInvariant()}\" {TypeNameToSQLType(all[i])}");
                if (i < all.Length - 1) sb.Append(", ");
            }
            sb.AppendLine(")");
            sb.AppendLine("LANGUAGE sql");
            sb.AppendLine("AS $$");
            sb.Append("\tSELECT ");
            for (int i = 0; i < all.Length; i++)
            {
                sb.Append($"\"{all[i].Name}\"");
                if (i < all.Length - 1) sb.Append(", ");
            }
            sb.AppendLine($" FROM \"{Schema}\".\"{table.Name}\";");
            sb.AppendLine("$$;");
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
            sb.AppendLine($"DROP FUNCTION IF EXISTS \"{Schema}\".\"{procedureName}\";");
            sb.AppendLine($"CREATE FUNCTION \"{Schema}\".\"{procedureName}\"(");

            int paramIdx = 0;
            for (int r = 0; r < rangeCount; r++)
            {
                for (int k = 0; k < primaryKeys.Length; k++)
                {
                    sb.Append($"\tp_{paramIdx} {TypeNameToSQLType(primaryKeys[k])}");
                    paramIdx++;
                    sb.AppendLine(paramIdx < totalParams ? "," : string.Empty);
                }
            }

            sb.AppendLine(")");
            sb.AppendLine($"RETURNS SETOF \"{Schema}\".\"{table.Name}\"");
            sb.AppendLine("LANGUAGE sql");
            sb.AppendLine("AS $$");
            sb.AppendLine("\tSELECT");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sb.Append($"\t\t\"{table.Columns[i].Name}\"");
                sb.AppendLine(i < table.Columns.Count - 1 ? "," : string.Empty);
            }
            sb.AppendLine($"\tFROM \"{Schema}\".\"{table.Name}\"");
            sb.AppendLine("\tWHERE");

            paramIdx = 0;
            for (int r = 0; r < rangeCount; r++)
            {
                StringBuilder clause = new StringBuilder();
                for (int k = 0; k < primaryKeys.Length; k++)
                {
                    clause.Append($"\"{primaryKeys[k].Name}\" = p_{paramIdx}");
                    if (k < primaryKeys.Length - 1) clause.Append(" AND ");
                    paramIdx++;
                }
                sb.Append($"\t\t({clause})");
                sb.AppendLine(r < rangeCount - 1 ? " OR" : ";");
            }
            sb.AppendLine("$$;");
            return sb.ToString();
        }

        /// <summary>
        /// 列实体类型映射到 PostgreSQL 数据类型。
        /// JSON 用 JSONB（二进制存储，可建索引），UUID/XML 用原生类型；
        /// 主键/外键/序列用 BIGINT；二进制统一 BYTEA（PG 没有定长二进制）。
        /// </summary>
        public override string TypeNameToSQLType(ColumnEntity column)
        {
            return column switch
            {
                ColumnEntitys.ArtemisEntity => "TEXT",
                ColumnEntitys.Xml => "XML",
                ColumnEntitys.Json => "JSONB",
                ColumnEntitys.Uuid => "UUID",
                ColumnEntitys.Date => "DATE",
                ColumnEntitys.DateTime => "TIMESTAMP",
                ColumnEntitys.Char charColumn => $"CHAR({charColumn.Length})",
                VarChar varCharColumn => $"VARCHAR({varCharColumn.Length})",
                Binary => "BYTEA",
                VarBinary => "BYTEA",
                VarBinaryMax => "BYTEA",
                ColumnEntitys.Boolean => "BOOLEAN",
                ColumnEntitys.Decimal decimalColumn => $"NUMERIC(18, {decimalColumn.Precision})",
                ColumnEntitys.Double => "DOUBLE PRECISION",
                ForeignKey => "BIGINT",
                PrimaryKey => "BIGINT",
                Sequence => "BIGINT",
                Ver => "INTEGER",
                Integer32 => "INTEGER",
                Integer64 => "BIGINT",
                _ => throw new NotImplementedException(column.GetType().Name)
            };
        }
    }
}
