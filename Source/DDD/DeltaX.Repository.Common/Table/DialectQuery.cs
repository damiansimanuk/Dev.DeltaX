using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeltaX.Repository.Common.Table
{
    public enum Dialect
    {
        SQLServer,
        PostgreSQL,
        SQLite,
        MySQL,
    }

    public class DialectQuery
    {
        public string EncapsulationSql { get; private set; }
        public string IdentityQueryFormatSql { get; private set; }
        public string PagedListQueryFormatSql { get; private set; }
        public string SingleQueryFormatSql { get; private set; }
        public string InsertQueryFormatSql { get; private set; }
        public string DeleteQueryFormatSql { get; private set; }
        public string UpdateQueryFormatSql { get; private set; }
        public string CountQueryFormatSql { get; private set; }
        public Dialect Dialect { get; private set; }

        public DialectQuery(Dialect dialect)
        {
            DefaultInitialization();
            Dialect = dialect;

            switch (dialect)
            {
                case Dialect.PostgreSQL:
                    IdentityQueryFormatSql = "SELECT LASTVAL() AS id";
                    break;
                case Dialect.SQLite:
                    IdentityQueryFormatSql = "SELECT LAST_INSERT_ROWID() AS id";
                    break;
                case Dialect.MySQL:
                    EncapsulationSql = "`{0}`";
                    IdentityQueryFormatSql = "SELECT LAST_INSERT_ID() AS id";
                    PagedListQueryFormatSql = "SELECT \n" +
                        "\t{SelectColumns} \n" +
                        "FROM {TableName} \n" +
                        "{WhereClause} \n" +
                        "{OrderByClause} \n" +
                        "LIMIT {SkipCount},{RowsPerPage} ";
                    break;
                case Dialect.SQLServer:
                    EncapsulationSql = "[{0}]";
                    IdentityQueryFormatSql = "SELECT SCOPE_IDENTITY() AS [id]";
                    PagedListQueryFormatSql = "SELECT \n" +
                       "\t{SelectColumns} \n" +
                       "FROM {TableName} \n" +
                       "{WhereClause} \n" +
                       "{OrderByClause} \n" +
                       "OFFSET {SkipCount} ROWS FETCH FIRST {RowsPerPage} ROWS ONLY ";
                    break;
            }
        }

        private void DefaultInitialization()
        {
            EncapsulationSql = "\"{0}\"";
            IdentityQueryFormatSql = "SELECT LAST_INSERT_ROWID() AS id";
            PagedListQueryFormatSql = "SELECT \n" +
                "\t{SelectColumns} \n" +
                "FROM {TableName} \n" +
                "{WhereClause} \n" +
                "{OrderByClause} \n" +
                "LIMIT {RowsPerPage} OFFSET {SkipCount} ";
            SingleQueryFormatSql = "SELECT \n" +
                "\t{SelectColumns} \n" +
                "FROM {TableName} \n" +
                "{WhereClause} \n";
            InsertQueryFormatSql = "INSERT INTO {TableName} \n" +
                "\t({InsertColumns}) " +
                "VALUES\n" +
                "\t({InsertValues}) ";
            DeleteQueryFormatSql = "DELETE FROM {TableName} \n{WhereClause}";
            UpdateQueryFormatSql = "UPDATE {TableName} SET\n\t {SetColumns} \n{WhereClause}";
            CountQueryFormatSql = "SELECT count(*) as Count FROM {TableName} \n{WhereClause}";
        }

        public string GetTableName(ITableConfiguration table)
        {
            return string.IsNullOrEmpty(table.Schema) ? table.Name : $"{table.Schema}.{table.Name}";
        }

        public string Encapsulation(string dbWord)
        {
            return string.Format(EncapsulationSql, dbWord);
        }

        public string GetWhereClausePK(ITableConfiguration table)
        {
            var pks = table.GetPrimaryKeysColumn();
            return "WHERE " + string.Join(" AND ", pks.Select(c => $"{Encapsulation(c.DbColumnName)} = @{c.DtoFieldName}"));
        }

        public string GetSelectColumns(ITableConfiguration table)
        {
            var columns = table.GetSelectColumns();
            return string.Join("\n\t, ", columns.Select(c =>
                c.DbAlias == null
                ? Encapsulation(c.DbColumnName)
                : $"{Encapsulation(c.DbColumnName)} as {Encapsulation(c.DbAlias)}"));
        }

        public string GetSelectColumnsList(ITableConfiguration table)
        {
            var columns = table.GetSelectColumnsList();
            return string.Join("\n\t, ", columns.Select(c =>
                c.DbAlias == null
                ? Encapsulation(c.DbColumnName)
                : $"{Encapsulation(c.DbColumnName)} as {Encapsulation(c.DbAlias)}"));
        }

        public string GetInsertColumns(ITableConfiguration table, IEnumerable<string> fieldsToInsert = null)
        {
            var columns = table.GetInsertColumns();

            if (fieldsToInsert != null && fieldsToInsert.Any())
            {
                columns = columns.Where(c => fieldsToInsert.Contains(c.DtoFieldName) || fieldsToInsert.Contains(c.DbColumnName));
            }

            return string.Join(", ", columns.Select(c => Encapsulation(c.DbColumnName)));
        }

        public string GetInsertValues(ITableConfiguration table, IEnumerable<string> fieldsToInsert = null)
        {
            var columns = table.GetInsertColumns();

            if (fieldsToInsert != null && fieldsToInsert.Any())
            {
                columns = columns.Where(c => fieldsToInsert.Contains(c.DtoFieldName) || fieldsToInsert.Contains(c.DbColumnName));
            }

            return string.Join(", ", columns.Select(c => $"@{c.DtoFieldName}"));
        }

        public string GetSetColumns(ITableConfiguration table, IEnumerable<string> fieldsToSet = null)
        {
            var columns = table.GetUpdateColumns();

            if (fieldsToSet != null && fieldsToSet.Any())
            {
                columns = table.Columns.Where(c => fieldsToSet.Contains(c.DtoFieldName) || fieldsToSet.Contains(c.DbColumnName));
            }

            return string.Join("\n\t, ", columns.Select(c => $"{Encapsulation(c.DbColumnName)} = @{c.DtoFieldName}"));
        }

    }
}
