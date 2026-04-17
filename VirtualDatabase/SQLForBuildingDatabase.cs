using LeadTurbo.VirtualDatabase.ColumnEntitys;
using LeadTurbo.VirtualDatabase.Operations.Application;
using System;
using System.Collections.Generic;
using static LeadTurbo.Function;

namespace LeadTurbo.VirtualDatabase
{
    /// <summary>
    /// 不同数据库引擎共用的"建库脚本生成器"基类。
    /// 抽掉跨引擎共有的辅助逻辑（PK/Ver 提取、MaxRangeKeyCount 映射），
    /// 方言部分（建表、过程语法、类型映射）由子类实现。
    /// </summary>
    public abstract class SQLForBuildingDatabase
    {
        /// <summary>
        /// 支持的数据库引擎类型。
        /// </summary>
        public enum DatabaseType
        {
            SQLite,
            SQLServer,
            PostgreSQL
        }

        /// <summary>
        /// 工厂方法：按引擎类型创建对应实现。
        /// </summary>
        public static SQLForBuildingDatabase Create(DatabaseType type) => type switch
        {
            DatabaseType.SQLite => new SQLForBuildingASQLite(),
            DatabaseType.SQLServer => new SQLForBuildingASQLServer(),
            DatabaseType.PostgreSQL => new SQLForBuildingAPostgreSQL(),
            _ => throw new NotSupportedException($"未知的数据库类型: {type}")
        };

        /// <summary>
        /// 根据 Database 配置建库（建表 + 全部存储过程/函数）。
        /// </summary>
        public abstract void Building(Database database);

        /// <summary>
        /// 给单个表建表 + 全部相关存储过程/函数。
        /// </summary>
        public abstract void BuildingTableAll(Table table, DataBasApp dataBasAPP);

        /// <summary>
        /// 建表（DROP + CREATE，可能含触发器）。
        /// </summary>
        public abstract void BuildingTable(Table table, DataBasApp dataBasAPP);

        /// <summary>
        /// 单个列定义（不含逗号）。
        /// </summary>
        public abstract string BuildingColumn(ColumnEntity column);

        public abstract string BuildingInsertProcedure(Table table);
        public abstract string BuildingUpdateProcedure(Table table);
        public abstract string BuildingDeleteProcedure(Table table);
        public abstract string BuildingAllMainKeysProcedure(Table table);
        public abstract string BuildingAllKeyAndVerProcedure(Table table);
        public abstract string BuildingResultProcedure(Table table, MaxRangeKeyCount maxRangeKeyCount);
        public abstract string BuildingInitializationResultProcedure(Table table, MaxRangeKeyCount maxRangeKeyCount);

        /// <summary>
        /// 列实体到引擎数据类型的映射。
        /// </summary>
        public abstract string TypeNameToSQLType(ColumnEntity column);

        public ColumnEntity[] GetPrimaryKeys(Table table)
        {
            List<ColumnEntity> primaryKeys = new List<ColumnEntity>();
            foreach (ColumnEntity column in table.Columns)
            {
                if (column is PrimaryKey)
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
                if (column is Ver)
                {
                    vers.Add(column);
                }
            }
            return vers.ToArray();
        }

        /// <summary>
        /// MaxRangeKeyCount 枚举转对应整数。
        /// </summary>
        protected static int MaxRangeKeyCountToInt(MaxRangeKeyCount c) => c switch
        {
            MaxRangeKeyCount.Count1 => 1,
            MaxRangeKeyCount.Count2 => 2,
            MaxRangeKeyCount.Count5 => 5,
            MaxRangeKeyCount.Count10 => 10,
            MaxRangeKeyCount.Count20 => 20,
            MaxRangeKeyCount.Count50 => 50,
            _ => throw new NotImplementedException(Enum.GetName(typeof(MaxRangeKeyCount), c))
        };
    }
}
