namespace DeltaX.Database
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Data;

    public class DatabaseFactory<TDbConnection> where TDbConnection : IDbConnection, new()
    {
        private ILogger logger;
        private string[] connectionStrings;

        public DatabaseFactory(string[] connectionStrings, ILogger logger = null)
        {
            this.connectionStrings = connectionStrings;
            this.logger = logger;
        }

        /// <summary>
        /// Get a new connection based on connectionStrings array
        /// </summary>
        /// <returns></returns>
        public TDbConnection GetConnection()
        {
            Exception exception = new ArgumentNullException("ConnectionStrings List Error");
            for (int idx = 0; idx < connectionStrings.Length; idx++)
            {
                try
                {
                    var dbConn = Connect(connectionStrings[idx]);
                    if (dbConn.State.HasFlag(ConnectionState.Open))
                    {
                        return dbConn;
                    }

                    // Close if not openned!
                    dbConn.Close();
                }
                catch (Exception e) { exception = e; }
            }

            throw exception;
        }

        public TDbConnection Connect(string connectionString)
        {
            try
            {
                logger?.LogDebug("Database try ConnectionString: {0}", connectionString);

                TDbConnection dbConn = Activator.CreateInstance<TDbConnection>();

                dbConn.ConnectionString = connectionString;
                dbConn.Open();

                logger?.LogDebug("Database State: {0}", dbConn.State);

                if (dbConn.State.HasFlag(ConnectionState.Open))
                {
                    logger?.LogDebug("Database Connected ConnectionString: {0}", connectionString);
                }

                return dbConn;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Database Connect excepcion");
                throw ex;
            }
        }
    }
}
