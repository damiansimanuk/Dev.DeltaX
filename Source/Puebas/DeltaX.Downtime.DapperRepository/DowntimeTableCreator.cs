using Autofac;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace DeltaX.Downtime.DapperRepository
{
	public class DowntimeTableCreator : IStartable
	{
		private static string SQL = @"
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;
 
CREATE TABLE IF NOT EXISTS InterruptionHistory (
	Id INTEGER PRIMARY KEY AUTOINCREMENT, 
	ProcessHistoryId STRING NULL,  
	StartDateTime DATE NULL, 
	EndDateTime DATE NULL, 
	Description TEXT, 
	CreatedAt DATE DEFAULT (datetime('now', 'localtime')), 
	Enable BOOLEAN DEFAULT (1) );
 
CREATE TABLE IF NOT EXISTS ProcessHistory (
	Id CHAR(36) PRIMARY KEY,  
	StartProcessDateTime DATE NULL, 
	FinishProcessDateTime DATE NULL, 
	ProductSpecificationCode CHAR(100), 
	CreatedAt DATE DEFAULT (datetime('now', 'localtime')), 
	Enable BOOLEAN DEFAULT (1) );

CREATE TABLE IF NOT EXISTS ProductSpecification (
	Id INTEGER PRIMARY KEY AUTOINCREMENT, 
	Code  CHAR(100) NOT NULL, 
	StandarDuration INT NOT NULL, 
	ProductSpecificationCode CHAR(100), 
	CreatedAt DATE DEFAULT (datetime('now', 'localtime')), 
	Enable BOOLEAN DEFAULT (1) ); 

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
";

		private IDbConnection connection;
		private ILogger<DowntimeTableCreator> log;

		public DowntimeTableCreator(IDbConnection connection, ILogger<DowntimeTableCreator> log = null)
		{
			this.connection = connection;
			this.log = log;
		}

		public void Start()
		{
			log?.LogInformation("Executing CreateDatabase Script...");

			using (var objCommand = ((SqliteConnection)connection).CreateCommand())
			{
				objCommand.CommandText = SQL;
				var result = objCommand.ExecuteNonQuery();
				log?.LogInformation("CreateDatabase Execute result {result}", result);
			}
		}
	}
}
