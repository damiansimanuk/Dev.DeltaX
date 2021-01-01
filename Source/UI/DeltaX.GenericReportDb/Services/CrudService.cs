namespace DeltaX.GenericReportDb.Services
{
    using Dapper;
    using DeltaX.Database;
    using DeltaX.GenericReportDb.Configuration;
    using DeltaX.GenericReportDb.Controllers;
    using DeltaX.GenericReportDb.Controllers.Models;
    using DeltaX.GenericReportDb.Utils;
    using Microsoft.Data.Sqlite;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class CrudService
    {
        private DatabaseBase db;
        private readonly ILogger<CrudService> logger;
        public CrudService(EndpointConfiguration configuration, ILogger<CrudService> logger = null)
        {
            this.logger = logger;
            SetConfiguration(configuration);
        }

        public EndpointConfiguration Configuration { get; private set; }
        public DateTime ReloadAt { get; private set; }

        public void SetConfiguration(EndpointConfiguration configuration)
        {
            this.Configuration = configuration;
            this.ReloadAt = DateTime.Now;

            // Instance of Db 
            switch (Configuration.DatabaseType.ToUpper())
            {
                case "SQLITE":
                    db = DatabaseBase.Build<SqliteConnection>(new[] { Configuration.ConnectionString });
                    break;

                    // case "MSSQL":
                    // case "SQLSERVER":
                    //     db = DatabaseBase.Build<SqlConnection>(new[] { Config.ConnectionString });
                    //     break;
                    // 
                    // case "MARIADB":
                    // case "MYSQL":
                    //     db = DatabaseBase.Build<MySqlConnection>(new[] { Config.ConnectionString });
                    //     break;
                    // 
                    // case "NODE":
                    // case "NODEJS":
                    //     nodeService = new CrudNodeService(config.Name);
                    //     break;
            }
        }

        public bool ContainPrefix(EndpointFunction functions, string prefix)
        {
            if (Configuration.Endpoints.TryGetValue(prefix, out var ep))
            {
                switch (functions)
                {
                    case EndpointFunction.GetItem: return ep.GetItem.Length > 0;
                    case EndpointFunction.UpdateItem: return ep.UpdateItem.Length > 0;
                    case EndpointFunction.DeleteItem: return ep.DeleteItem.Length > 0;
                    case EndpointFunction.UploadFile: return ep.UploadFile.Length > 0;
                    case EndpointFunction.DownloadFile: return ep.DownloadFile.Length > 0;
                    case EndpointFunction.GetList: return ep.GetList.Length > 0;
                    case EndpointFunction.SearchList: return ep.SearchList.Length > 0;
                    case EndpointFunction.InsertList: return ep.InsertList.Length > 0;
                }
            }
            return false;
        }

        public Dictionary<string, object> ParseSchemaFields(string[] fieldsNames, Dictionary<string, JsonElement> data)
        {
            var param = new Dictionary<string, object>();
            if (fieldsNames != null)
            {
                foreach (var fName in fieldsNames)
                {
                    var kv = data.FirstOrDefault(d => d.Key.Equals(fName, StringComparison.InvariantCultureIgnoreCase));
                    object value = null;

                    if (!string.IsNullOrEmpty(kv.Key))
                    {
                        value = kv.Value.ToObject();
                    }

                    param.Add(fName, value);

                }
            }
            return param;
        }

        public Dictionary<string, object> GetParsePrimaryKeys(string[] PrimaryKeys, string primaryKeys)
        {
            var keys = new Dictionary<string, object>();

            int index = 0;
            foreach (var value in primaryKeys.Split(Configuration.PrimaryKeysDelimiter, StringSplitOptions.RemoveEmptyEntries))
            {
                var fieldName = PrimaryKeys[index++];
                var field = Configuration.FieldsSchema
                    .FirstOrDefault(f => f.Field.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase));

                if (field != null)
                {
                    switch (field.Type.ToUpper())
                    {
                        case "BOOL":
                        case "INTEGER":
                        case "INT":
                            keys[fieldName] = Convert.ToInt64(value);
                            break;
                        case "NUMBER":
                        case "FLOAT":
                        case "DOUBLE":
                            keys[fieldName] = Convert.ToDouble(value);
                            break;
                        default:
                            keys[fieldName] = value;
                            break;
                    }
                }
                else
                {
                    keys[fieldName] = value;
                }
            }
            return keys;
        }

        public object GetList(string prefixList, int perPage = 10, int page = 1)
        {
            var param = new Dictionary<string, object>();
            param["PerPage"] = perPage;
            param["Page"] = page;
            param["Offset"] = (page - 1) * perPage;

            var ep = Configuration.Endpoints.GetValueOrDefault(prefixList);

            var sqlOrMethod = string.Join("\n", ep.GetList);
            var sqlCount = string.Join("\n", ep.CountList);

            return db.Run(conn =>
            {
                var totalRows = 0;
                if (!string.IsNullOrEmpty(sqlCount))
                    totalRows = conn.QueryFirstOrDefault<int>(sqlCount);
                var rows = conn.Query(sqlOrMethod, param).ToList();

                return new { totalRows, perPage, page, rows };
            });
        }


        public object SearchList(string prefixList, Dictionary<string, JsonElement> data)
        {
            var ep = Configuration.Endpoints.GetValueOrDefault(prefixList);

            var param = ParseSchemaFields(ep.SearchFields, data);
            var sqlOrMethod = string.Join("\n", ep.SearchList);

            return db.Run(conn =>
            {
                var rows = conn.Query(sqlOrMethod, param).ToList();
                return new { rows };
            });
        }

        public int InsertList(string prefixList, Dictionary<string, JsonElement> data)
        {
            var ep = Configuration.Endpoints.GetValueOrDefault(prefixList);
            var param = ParseSchemaFields(ep.InsertFields, data);

            var sqlOrMethod = string.Join("\n", ep.InsertList);

            return db.Run(async conn =>
            {
                try
                {
                    return await conn.ExecuteAsync(sqlOrMethod, param);
                }
                catch (Exception e)
                {
                    Console.WriteLine("InsertList Exception {0}", e);
                    throw new HttpResponseException(HttpStatusCode.Conflict, e);
                }
            });
        }

        public object GetItem(string prefixItem, string primaryKeys)
        {
            var ep = Configuration.Endpoints.GetValueOrDefault(prefixItem);
            var param = GetParsePrimaryKeys(ep.PrimaryKeys, primaryKeys);

            var sqlOrMethod = string.Join("\n", ep.GetItem);

            return db.Run(conn =>
            {
                return conn.QueryFirst(sqlOrMethod, param);
            });
        }

        public async Task<int> UpdateItemAsync(string prefixItem, string primaryKeys, Dictionary<string, JsonElement> data)
        {
            var ep = Configuration.Endpoints.GetValueOrDefault(prefixItem);
            var param = ParseSchemaFields(ep.UpdateFields, data);

            // Merge with primary keys paramas
            foreach (var p in GetParsePrimaryKeys(ep.PrimaryKeys, primaryKeys))
            {
                param[p.Key] = p.Value;
            }

            var sqlOrMethod = string.Join("\n", ep.UpdateItem);

            return await db.RunAsync(conn =>
            {
                return conn.ExecuteAsync(sqlOrMethod, param);
            });
        }

        public int DeleteItem(string prefixItem, string primaryKeys)
        {
            var ep = Configuration.Endpoints.GetValueOrDefault(prefixItem);
            var param = GetParsePrimaryKeys(ep.PrimaryKeys, primaryKeys);

            var sqlOrMethod = string.Join("\n", ep.DeleteItem);

            return db.Run(conn =>
            {
                return conn.ExecuteAsync(sqlOrMethod, param);
            });
        }


        public int UploadFile(string prefixItem, string primaryKeys, string FieldName, FileModel file)
        {
            var ep = Configuration.Endpoints.GetValueOrDefault(prefixItem);

            var param = GetParsePrimaryKeys(ep.PrimaryKeys, primaryKeys);
            param["ConfigurationName"] = Configuration.Name;
            param["Endpoint"] = prefixItem;
            param["PrimaryKey"] = primaryKeys;
            param["FieldName"] = FieldName;
            param["FileName"] = file.FileName;
            param["ContentType"] = file.ContentType;
            param["FileContents"] = file.FileContents;

            var sqlOrMethod = string.Join("\n", ep.UploadFile);

            // sqlOrMethod = "REPLACE INTO FilesDemo (ConfigName, Endpoint, PrimaryKey, Field, FileName, FileType, FileData) " +
            //     "VALUES (@ConfigName, @Endpoint, @PrimaryKey, @FieldName, @FileName, @ContentType, @FileContents)";

            return db.Run(conn =>
            {
                return conn.Execute(sqlOrMethod, param);
            });
        }


        public FileModel DownloadFile(string prefixItem, string primaryKeys, string FieldName, string fileName = null)
        {
            var ep = Configuration.Endpoints.GetValueOrDefault(prefixItem);

            var param = GetParsePrimaryKeys(ep.PrimaryKeys, primaryKeys);
            param["ConfigurationName"] = Configuration.Name;
            param["Endpoint"] = prefixItem;
            param["PrimaryKey"] = primaryKeys;
            param["FieldName"] = FieldName;
            param["FileName"] = fileName;

            var sqlOrMethod = string.Join("\n", ep.DownloadFile);

            // sqlOrMethod = "SELECT FileName, FileType as ContentType, FileData as FileContents " +
            //   "FROM FilesDemo " +
            //   "WHERE ConfigName=@ConfigName " +
            //   "AND Endpoint=@Endpoint " +
            //   "AND PrimaryKey=@PrimaryKey " +
            //   "AND Field=@FieldName "; 

            return db.Run(conn =>
            {
                return conn.QueryFirst<FileModel>(sqlOrMethod, param);
            });
        }
    }
}
