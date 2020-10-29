using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace DeltaX.Database
{
    public class Database<T> : DatabaseBase where T : IDbConnection, new()
    {
        public Database(string[] connectionStrings, ILogger<Database<T>> logger = null)
            : base(typeof(T), connectionStrings, logger) { }

        public new T DbConnection => (T)base.DbConnection;

        public new T GetConnection()
        {
            return (T)base.GetConnection();
        }

        public TResult RunSync<TResult>(Func<T, TResult> method)
        {
            return base.RunSync((dbConn) => method((T)dbConn));
        }

        public Task<TResult> RunAsync<TResult>(Func<T, Task<TResult>> method)
        {
            return base.RunAsync((dbConn) => method((T)dbConn));
        }

        public TResult Run<TResult>(Func<T, Task<TResult>> method)
        {
            return base.Run((dbConn) => method((T)dbConn));
        }

        public TResult Run<TResult>(Func<T, TResult> method)
        {
            return base.Run((dbConn) => method((T)dbConn));
        }
    }
}
