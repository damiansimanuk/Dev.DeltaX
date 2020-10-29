using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DeltaX.Repository.Common.Table
{
    public interface ITableConfiguration
    {
        public string Name { get; }
        public string Schema { get; }
        public IEnumerable<ColumnConfiguration> Columns { get; }

        public ColumnConfiguration GetIdentityColumn();

        public IEnumerable<ColumnConfiguration> GetPrimaryKeysColumn();
        public IEnumerable<ColumnConfiguration> GetSelectColumns();
        public IEnumerable<ColumnConfiguration> GetSelectColumnsList();
        public IEnumerable<ColumnConfiguration> GetInsertColumns();
        public IEnumerable<ColumnConfiguration> GetUpdateColumns();
    }

    public class TableConfiguration<TTable> : ITableConfiguration
        where TTable : ITableDto
    {

        private List<ColumnConfiguration> columns;

        public TableConfiguration(string name, string schema)
        {
            Name = name;
            Schema = schema;
            Table = Activator.CreateInstance<TTable>();
            columns = new List<ColumnConfiguration>();
        }
        public TTable Table { get; private set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public IEnumerable<ColumnConfiguration> Columns => columns;

        public void AddColumn(ColumnConfiguration config)
        {
            config.TableDto ??= Table;
            columns.Add(config);
        }

        public void AddColumn(string dtoFieldName, string dbColumnName = null,
            bool isIdentity = false, bool isPrimaryKey = false,
            Action<ColumnConfiguration> configColumn = null)
        {
            var config = new ColumnConfiguration(Table, dtoFieldName, dbColumnName, isIdentity, isPrimaryKey);
            configColumn?.Invoke(config);
            AddColumn(config);
        }

        public void AddColumn(string dtoFieldName, Action<ColumnConfiguration> configColumn)
        {
            AddColumn(dtoFieldName, null, false, false, configColumn);
        }

        public void AddColumn<TProperty>(Expression<Func<TTable, TProperty>> property,
            string dbColumnName = null, bool isIdentity = false, bool isPrimaryKey = false,
            Action<ColumnConfiguration> configColumn = null)
        {
            PropertyInfo propInfo = GetPropertyInfo(property);
            var dtoFieldName = propInfo?.Name;

            AddColumn(dtoFieldName, dbColumnName, isIdentity, isPrimaryKey, configColumn);
        }

        public void AddColumn<TProperty>(Expression<Func<TTable, TProperty>> property, Action<ColumnConfiguration> configColumn)
        {
            AddColumn(property, null, false, false, configColumn);
        }
                

        public void SetIdentity<TProperty>(Expression<Func<TTable, TProperty>> property)
        {
            PropertyInfo propInfo = GetPropertyInfo(property);
            var dtoFieldName = propInfo?.Name;
            var type = propInfo.PropertyType;

            columns.ForEach(c => c.IsIdentity = false);
            var column = columns.FirstOrDefault(c => c.DtoFieldName == dtoFieldName);
            _ = column ?? throw new ArgumentException("Column 'dtoFieldName' cannot be null", nameof(property));
            column.IsIdentity = true;
        } 

        public PropertyInfo GetPropertyInfo<TProperty>(Expression<Func<TTable, TProperty>> property)
        {
            MemberExpression member = property?.Body as MemberExpression;
            return member?.Member as PropertyInfo;
        }

        public void InvalidatePk()
        {
            var pks = columns.Where(c => c.IsPrimaryKey || c.IsIdentity);
            if (pks.Any())
            {
                return;
            }

            pks = columns.Where(c => c.DtoFieldName == "Id" || $"{Name}Id" == c.DbColumnName);
            foreach (var pk in pks)
            {
                pk.IsPrimaryKey = true;
            }
        }

        public ColumnConfiguration GetIdentityColumn() 
        { 
            return  Columns.FirstOrDefault(c => c.IsIdentity);
        }

        public IEnumerable<ColumnConfiguration> GetPrimaryKeysColumn()
        { 
            return Columns.Where(c => c.IsIdentity || c.IsPrimaryKey).ToArray();
        }

        public IEnumerable<ColumnConfiguration> GetSelectColumns()
        {
            return Columns.Where(c => !c.IgnoreSelect).ToArray();            
        }

        public IEnumerable<ColumnConfiguration> GetSelectColumnsList()
        {
            return Columns.Where(c => !c.IgnoreSelect && !c.IgnoreSelectList).ToArray();
        }

        public IEnumerable<ColumnConfiguration> GetInsertColumns()
        { 
            return Columns.Where(c => !c.IgnoreInsert && !c.IsIdentity).ToArray();
        }

        public IEnumerable<ColumnConfiguration> GetUpdateColumns()
        { 
            return Columns.Where(c => !c.IgnoreUpdate && !c.IsIdentity && !c.IsPrimaryKey).ToArray();
        }
    }
}
