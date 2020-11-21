using DeltaX.Repository.Common.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DeltaX.LinqExpression
{

    public interface IQueryBuilder { public TableQueryBuilder Builder { get; } }
    public interface IQueryBuilder<T1> : IQueryBuilder { }
    public interface IQueryBuilder<T1, T2> : IQueryBuilder<T1> { }
    public interface IQueryBuilder<T1, T2, T3> : IQueryBuilder<T1, T2> { }
    public interface IQueryBuilder<T1, T2, T3, T4> : IQueryBuilder<T1, T2, T3> { }
    public interface IQueryBuilder<T1, T2, T3, T4, T5> : IQueryBuilder<T1, T2, T3, T4> { }
    public interface IQueryBuilder<T1, T2, T3, T4, T5, T6> : IQueryBuilder<T1, T2, T3, T4, T5> { }
    public interface IQueryBuilder<T1, T2, T3, T4, T5, T6, T7> : IQueryBuilder<T1, T2, T3, T4, T5, T6> { }


    public static class QueryBuilderExtension
    {
        public static IQueryBuilder<T1> Where2<TTable, T1>(this IQueryBuilder<T1> self, Expression<Func<TTable, bool>> whereCondition)
        {
            self.Builder.Where(whereCondition);
            return self;
        }

        public static IQueryBuilder Where<TTable>(this IQueryBuilder self, Expression<Func<TTable, bool>> whereCondition)
        {
            self.Builder.Where(whereCondition);
            return self;
        }
         



        //     public static IQueryBuilder<TData> Where<TData>(this IQueryBuilder<TData> builder, Expression<Func<TData, bool>> expression = null)
        //     {
        //         return builder as IQueryBuilder<TData>;
        //     }
        // 
        //     public static IQueryBuilder<TData> Select<TData>(this IQueryBuilder builder, Expression<Func<TData, object>> properties)
        //     {
        //         return builder as IQueryBuilder<TData>;
        //     }
        // 
        //     public static IQueryBuilder<TData> Select<TData>(this IQueryBuilder<TData> builder, Expression<Func<TData, object>> properties)
        //     {
        //         return builder as IQueryBuilder<TData>;
        //     }
        // 
        //     #region Join
        //     public static IQueryBuilder<T1, T2> Join<T1, T2>(this IQueryBuilder<T1> builder, Expression<Func<T1, T2, bool>> on)
        //     {
        //         return builder as IQueryBuilder<T1, T2>;
        //     }
        //     public static IQueryBuilder<T1, T2, T3> Join<T1, T2, T3>(this IQueryBuilder<T1, T2> builder, Expression<Func<T1, T2,T3,  bool>> on)
        //     {
        //         return builder as IQueryBuilder<T1, T2, T3>;
        //     }
        //     #endregion
        // 
    }


    public class TableConfig 
    {
        public Type TableType { get;  set; }
        public Expression ExpressionWhere { get; set; }
        public Expression ExpressionSelect { get; set; }
    }


    public class TableQueryBuilder
    {
        private List<TableConfig> Tables { get; set; }
         
        public TableQueryBuilder()
        {
            Tables = new List<TableConfig>();
        }
         
        public void AddTable<T>()
        {
            Tables.Add(new TableConfig { TableType = typeof(T) });
        }

        public TableConfig GetTableConfig<T>()
        {
            return Tables.FirstOrDefault(t => t.TableType == typeof(T))
                 ?? throw new ArgumentException($"Table for Type {typeof(T)} is not configurated!");
        }

        public void Where<T>(Expression<Func<T, bool>> whereCondition)
        {
            var table = GetTableConfig<T>();

            table.ExpressionWhere = whereCondition;
            new QueryParser(whereCondition);
        }

        public void Select<T>(Expression<Func<T, object>> properties)
        {
            var table = GetTableConfig<T>();

            table.ExpressionSelect = properties;
        }
    }

    public class QueryBuilder : IQueryBuilder
    {  
        public TableQueryBuilder Builder { get; protected set; }

        public QueryBuilder(TableQueryBuilder builder = null) 
        {
            Builder = builder ?? new TableQueryBuilder(); 
        } 

        public void AddTable<T>()
        {
            Builder.AddTable<T>(); 
        }

        public TableConfig GetTableConfig<T>()
        {
            return Builder.GetTableConfig<T>();
        }

        // public void Where<T>(Expression<Func<T, bool>> whereCondition)
        // {
        //     Builder.Where(whereCondition);
        // }
        // 
        // public void Select<T>(Expression<Func<T, object>> properties)
        // {
        //     Builder.Select(properties);
        // }
    }

    public class QueryBuilder<T1> : QueryBuilder, IQueryBuilder<T1>
    {
        public QueryBuilder(TableQueryBuilder builder = null) : base(builder)
        {
            AddTable<T1>();
        }

        public QueryBuilder<T1, T2> Join<T2>(Expression<Func<T1, T2, bool>> joinOn)
        {
            return new QueryBuilder<T1, T2>(Builder);
        }

        public IQueryBuilder<T1> Select(Expression<Func<T1, object>> properties)
        {
            Builder.Select(properties);
            return this;
        }  
    }

    public class QueryBuilder<T1, T2> : QueryBuilder<T1>, IQueryBuilder<T1, T2>
    {
        public QueryBuilder(TableQueryBuilder builder = null) :base(builder)
        {  
            Builder.AddTable<T2>();
        }

        public QueryBuilder<T1, T2, T3> Join<T3>(Expression<Func<T1, T2, T3, bool>> joinOn)
        {
            return new QueryBuilder<T1, T2, T3>(Builder);
        }

        public IQueryBuilder<T1, T2> Select(
            Expression<Func<T1, object>> properties1, 
            Expression<Func<T2, object>> properties2)
        {
            Builder.Select<T1>(properties1);
            Builder.Select<T2>(properties2);
            return this;
        }
    }

    public class QueryBuilder<T1, T2, T3> : QueryBuilder<T1, T2>, IQueryBuilder<T1, T2, T3>
    {
        public QueryBuilder(TableQueryBuilder builder = null) : base(builder)
        {
            Builder.AddTable<T3>();
        }

        public QueryBuilder<T1, T2, T3, T4> Join<T4>(Expression<Func<T1, T2, T3, T4, bool>> joinOn)
        {
            return new QueryBuilder<T1, T2, T3, T4>(Builder);
        }

        public IQueryBuilder<T1, T2, T3> Select(
            Expression<Func<T1, object>> properties1,
            Expression<Func<T2, object>> properties2,
            Expression<Func<T3, object>> properties3)
        {
            Builder.Select(properties1);
            Builder.Select(properties2);
            Builder.Select(properties3);
            return this;
        }
    }

    public class QueryBuilder<T1, T2, T3, T4> : QueryBuilder<T1, T2, T3>, IQueryBuilder<T1, T2, T3, T4>
    {
        public QueryBuilder(TableQueryBuilder builder = null) : base(builder)
        {
            Builder.AddTable<T4>();
        }

        public IQueryBuilder<T1, T2, T3, T4> Select(
          Expression<Func<T1, object>> properties1,
          Expression<Func<T2, object>> properties2,
          Expression<Func<T3, object>> properties3,
          Expression<Func<T4, object>> properties4)
        {
            Builder.Select(properties1);
            Builder.Select(properties2);
            Builder.Select(properties3);
            Builder.Select(properties4);
            return this;
        }
    }

    ///  public class QueryBuilder<T1, T2> : QueryBuilder<T1, T2, NullTable, NullTable, NullTable, NullTable> { }

    //   // public class QueryBuilder<T1,T2> : QueryBuilder<T1, T2, NullTable, NullTable, NullTable, NullTable> { }
    //     public class QueryBuilder<T1,T2, T3> : QueryBuilder<T1, T2, T3, NullTable, NullTable, NullTable> { }
    //     public class QueryBuilder<T1,T2, T3,T4> : QueryBuilder<T1, T2, T3, T4, NullTable, NullTable> { }
    //     public class QueryBuilder<T1,T2, T3,T4, T5> : QueryBuilder<T1, T2, T3, T4, T5, NullTable> { }  
    // 


    public class ParserStream
    {
        StringBuilder sql = new StringBuilder();
        Dictionary<string, object> Parameters = new Dictionary<string, object>();
        Dictionary<ITableConfiguration, HashSet<ColumnConfiguration>> tablesConfigured = new Dictionary<ITableConfiguration, HashSet<ColumnConfiguration>>();

        TableQueryFactory tableFactory;
        public ParserStream(TableQueryFactory tableFactory = null)
        {
            this.tableFactory = tableFactory ?? TableQueryFactory.GetInstance();
        }

        int _nextParameter = 0;
        private string GetParamId() => string.Format("arg_{0}", _nextParameter++);

        public IDictionary<string, object> GetParameters()
        {
            return Parameters;
        }

        public IDictionary<ITableConfiguration, HashSet<ColumnConfiguration>> GetTableColumns()
        {
            return tablesConfigured;
        }

        public bool IsConfiguredTable(Type tableType)
        {
            return tableFactory.IsConfiguredTable(tableType);
        }

        public void AddOperator(string op)
        {
            sql.Append($" {op} ");
        }

        public bool AddColumn(Type tableType, string columnName)
        {
            if (!tableFactory.IsConfiguredTable(tableType))
            {
                return false;
            }

            var table = tableFactory.GetTable(tableType); 
            if (!tablesConfigured.ContainsKey(table))
            {
                tablesConfigured.Add(table, new HashSet<ColumnConfiguration>());
            }
            var column = table.Columns.FirstOrDefault(c => c.DtoFieldName == columnName);
            if (column != null)
            {
                tablesConfigured[table].Add(column);
                sql.Append(tableFactory.DialectQuery.Encapsulation(column.DbColumnName, table.Identifier));
            }
            return column != null;
        }

        public void AddParameter(object val)
        {
            if (val == null)
            {
                sql.Append("NULL");
                return;
            }

            var param = GetParamId();
            Parameters.Add(param, val);

            sql.Append($"@{param}");
        }

        public override string ToString()
        {
            return sql.ToString();
        }
    }

    public class QueryParser : ExpressionVisitor 
    {
        ParserStream stream;

        public QueryParser(Expression expression = null, TableQueryFactory tableFactory = null)
        {
            stream = new ParserStream(tableFactory); 
            base.Visit(expression);

            var sql = stream.ToString();

        }

        public string GetSql()
        {
            return stream.ToString();
        }
          
        public IDictionary<string, object> GetParameters()
        {
            return stream.GetParameters();
        }

        public IDictionary<ITableConfiguration, HashSet<ColumnConfiguration>> GetTableColumns()
        {
            return stream.GetTableColumns();
        }

        protected override Expression VisitNew(NewExpression node)
        {
            return base.VisitNew(node);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            return base.VisitNewArray(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            base.Visit(node.Left);
            stream.AddOperator(QueryHelper.GetOperator(node));
            base.Visit(node.Right);

            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            return base.VisitUnary(node);
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return base.VisitBlock(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (stream.IsConfiguredTable(node.Expression.Type))
            {
                if (stream.AddColumn(node.Expression.Type, node.Member.Name))
                {
                    return node;
                }
            }

            if (QueryHelper.IsVariable(node))
            {
                stream.AddParameter(node);
                return node;
            }  

            return base.VisitMember(node);
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            return base.VisitCatchBlock(node);
        }

        

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            // QueryHelper.GetValueFromExpression(node.Value);            
            stream.AddParameter(node.Value);
            
            return base.VisitConstant(node);
        }
    }

    public class QueryHelper
    {
        public static object GetValueFromExpression(Expression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        internal static bool IsVariable(Expression expr)
        {
            return (expr is MemberExpression) && (((MemberExpression)expr).Expression is ConstantExpression);
        }

        internal static string GetOperator(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return (IsBoolean(b.Left.Type)) ? "AND" : "&";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return (IsBoolean(b.Left.Type) ? "OR" : "|");
                default:
                    return GetOperator(b.NodeType);
            }
        }

        internal static string GetOperator(ExpressionType exprType)
        {
            switch (exprType)
            {
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Modulo:
                    return "%";
                case ExpressionType.ExclusiveOr:
                    return "^";
                case ExpressionType.LeftShift:
                    return "<<";
                case ExpressionType.RightShift:
                    return ">>";
                default:
                    return "";
            }
        }
        internal static bool IsBoolean(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }

    }
}
