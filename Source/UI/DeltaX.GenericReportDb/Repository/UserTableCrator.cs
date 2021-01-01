namespace DeltaX.GenericReportDb.Repository
{
	using Microsoft.Data.Sqlite;
	using Microsoft.Extensions.Logging; 
	using System.Data; 

	public class UserTableCrator
	{
		private IDbConnection connection;
		private ILogger log;

		public UserTableCrator(IDbConnection connection, ILogger log = null)
		{
			this.connection = connection;
			this.log = log;
		}

		public void Start()
		{
			log?.LogInformation("Executing CreateDatabase Script...");

			using (var objCommand = ((SqliteConnection)connection).CreateCommand())
			{
				objCommand.CommandText = SqlQueries.ScriptCreateTables;
				var result = objCommand.ExecuteNonQuery();
				log?.LogInformation("CreateDatabase Execute result {result}", result);
			}
		}
	}
}