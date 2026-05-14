using LeadTurbo.VirtualDatabase.ColumnEntitys;
using LeadTurbo.VirtualDatabase.Operations.Application;
using System.Text;

using static LeadTurbo.Function;

namespace LeadTurbo.VirtualDatabase
{
    public class SQLForBuildingASQLite : SQLForBuildingDatabase
    {
        public override void Building(Database database)
        {
            DataBasApp dataBasAPP=new DataBasAppSQLite();
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
                dataBasAPP.Param.CommandType = DataBasApp.ExecuteType.SQL;
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

        public override void BuildingTableAll(Table table, DataBasApp dataBasAPP)
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




        public override void BuildingTable(Table table, DataBasApp dataBasAPP)
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
                // 不再使用 trigger，改为在 INSERT 时直接 (SELECT MAX+1)；
                // UNIQUE 索引兼顾两件事：MAX() 查询走索引很快，并发冲突时由唯一约束自然失败，由调用方重试
                sb.AppendLine($"CREATE UNIQUE INDEX IF NOT EXISTS [IX_{table.Name}_Sequence] ON [{table.Name}] ([Sequence]);");
            }




            sb.AppendLine("COMMIT;");

            try
            {
                dataBasAPP.OpenDataBas();
                dataBasAPP.Param.CommandType = DataBasApp.ExecuteType.SQL;
                dataBasAPP.Param.Command = sb.ToString();
                dataBasAPP.Return();
            }
            finally
            {
                dataBasAPP.CloseDataBas();
            }



        }


        void SaveProcedure(string name, string sql, DataBasApp dataBasAPP)
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
                dataBasAPP.Param.CommandType = DataBasApp.ExecuteType.SQL;
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






        public override string BuildingColumn(ColumnEntity column)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"[{column.Name}] ");
            sb.Append($"{TypeNameToSQLType(column)} ");
            if (column.IsUnique)
            {
                sb.Append($"UNIQUE ");
            }

            if (!column.PermitNull)
            {
                sb.Append("NOT NULL");
            }

            return sb.ToString();

        }


        public override string BuildingInsertProcedure(Table table)
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
                if (table.Columns[i] is Sequence)
                {
                    // Sequence 列：忽略调用方传值，自动取 MAX+1
                    sql.AppendLine($"(SELECT IFNULL(MAX([Sequence]), 0) + 1 FROM [{table.Name}]),");
                }
                else
                {
                    sql.AppendLine($"@P_{i},");
                }
            }
            sql.Remove(sql.Length - 3, 3);
            sql.AppendLine($");");

            return sql.ToString();
        }

        public override string BuildingUpdateProcedure(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Update [{table.Name}]");
            sb.AppendLine($"Set");
            int count = 0;

            List<KeyValuePair<string, int>> primaryKeys = new List<KeyValuePair<string, int>>();
            List<KeyValuePair<string, int>> vers = new List<KeyValuePair<string, int>>();

            foreach (ColumnEntity column in table.Columns)
            {
                if (column is PrimaryKey)
                {
                    primaryKeys.Add(new KeyValuePair<string, int>(column.Name, count));
                }
                if (column is Ver)
                {
                    vers.Add(new KeyValuePair<string, int>(column.Name, count));
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

            foreach (KeyValuePair<string, int> item in vers)
            {
                sb.AppendLine($"\t{item.Key}=@P_{item.Value} -1 and");
            }

            sb.Remove(sb.Length - 6, 6);

            return sb.ToString();
        }

        public override string BuildingDeleteProcedure(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Delete From [{table.Name}]");
            sb.AppendLine($"where");
            ColumnEntity[] primaryKeys = GetPrimaryKeys(table);
            ColumnEntity[] vers = GetVers(table);
            int count = 0;
            foreach (ColumnEntity item in primaryKeys)
            {
                sb.AppendLine($"\t{item.Name}=@P_{count} and");
                count++;
            }

            foreach (ColumnEntity item in vers)
            {
                sb.AppendLine($"\t{item.Name}=@P_{count} -1 and");
                count++;
            }

            sb.Remove(sb.Length - 6, 6);

            return sb.ToString();

        }


        public override string BuildingAllMainKeysProcedure(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"SELECT [PRIMARYKEY] FROM [{table.Name}]");
            return sb.ToString();
        }

        public override string BuildingAllKeyAndVerProcedure(Table table)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"SELECT [PRIMARYKEY],[EDITVER] FROM [{table.Name}]");
            return sb.ToString();
        }


        public override string BuildingResultProcedure(Table table, MaxRangeKeyCount maxRangeKeyCount)
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

            int count = MaxRangeKeyCountToInt(maxRangeKeyCount);

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




        public override string BuildingInitializationResultProcedure(Table table, MaxRangeKeyCount maxRangeKeyCount)
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

            int count = MaxRangeKeyCountToInt(maxRangeKeyCount);

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
        public override string TypeNameToSQLType(ColumnEntity colProperty)
        {
            return colProperty switch
            {
                ColumnEntitys.ArtemisEntity => "TEXT",
                ColumnEntitys.Xml => "TEXT",
                ColumnEntitys.Json => "TEXT",
                ColumnEntitys.Uuid => "TEXT",
                ColumnEntitys.Date => "TEXT",
                ColumnEntitys.DateTime => "TEXT",
                ColumnEntitys.Char charColumn => $"CHAR({charColumn.Length})",
                VarChar varCharColumn => $"VARCHAR({varCharColumn.Length})",
                Binary => "BLOB",
                VarBinary => "BLOB",
                VarBinaryMax => "BLOB",
                ColumnEntitys.Boolean => "INTEGER",
                ColumnEntitys.Decimal => "NUMERIC",
                ColumnEntitys.Double => "REAL",
                ForeignKey => "INTEGER",
                PrimaryKey => "INTEGER",
                Sequence => "INTEGER",
                Ver => "INTEGER",
                Integer32 => "INTEGER",
                Integer64 => "INTEGER",
                _ => throw new NotImplementedException(colProperty.GetType().Name)
            };

        }
    }
}
