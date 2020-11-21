using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace DeltaX.Downtime.Console
{
    public static class Class1
    {
        public static bool Like(this string src, string comp)
        {
            return src.Contains(comp);
        }

        public static PropertyInfo GetPropertyInfo<TTable, TProperty>(Expression<Func<TTable, TProperty>> property)
        {
            dynamic body = property?.Body; 

            var param = property.Parameters[0];
            var parT = param.Type;
            var table = parT.Name; 

            var whereClause = Parser(body);
            System.Console.WriteLine("body   =>{0}\n", body);
            System.Console.WriteLine($"result => SELECT * FROM {table} WHERE {whereClause}");

            MemberExpression member = property?.Body as MemberExpression;
            return member?.Member as PropertyInfo;
        }

        public static string Parser(dynamic exp)
        {
            ExpressionType nodeType = exp.NodeType;

            switch (nodeType)
            {
                case ExpressionType.Call:
                    if(exp.Method.Name == "Like")
                    { 
                        return $"({Parser(exp.Arguments[0])} LIKE {Parser(exp.Arguments[1])})";
                    }
                    break;
                case ExpressionType.Constant:
                    return object.Equals(exp.Value, null) ? null : $"{exp.Value}".Trim();
                case ExpressionType.MemberAccess:
                    return "" + exp.Member.Name;
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    return $"({Parser(exp.Left)} AND {Parser(exp.Right)})";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return $"({Parser(exp.Left)} OR {Parser(exp.Right)})";
                case ExpressionType.Equal:
                    var node = exp as BinaryExpression; 
                    var l = Parser(exp.Left);
                    var r = Parser(exp.Right);
                    if (l == null)
                        return $"{r} IS NULL";
                    if (r == null)
                        return $"{l} IS NULL";
                    if (r.StartsWith("%") && r.EndsWith("%"))
                        return $"({l} LIKE {r})";
                    return $"({l} = {r})";
                case ExpressionType.NotEqual:
                    return $"({Parser(exp.Left)} != {Parser(exp.Right)})";
                case ExpressionType.LessThan:
                    return $"({Parser(exp.Left)} < {Parser(exp.Right)})";
                case ExpressionType.LessThanOrEqual:
                    return $"({Parser(exp.Left)} <= {Parser(exp.Right)})";
                case ExpressionType.GreaterThan:
                    return $"({Parser(exp.Left)} > {Parser(exp.Right)})";
                case ExpressionType.GreaterThanOrEqual:
                    return $"({Parser(exp.Left)} >= {Parser(exp.Right)})";
                case ExpressionType.Not:
                    return $"({Parser(exp.Left)} >= {Parser(exp.Right)})";
            }
            throw new AggregateException("Expresion incrorrecta");
        }

    }
}
