using System.Collections.Generic;

namespace DeltaX.GenericReportDb.Configuration
{
    public class SchemaFieldConfig
    {
        public string Field { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
    }

    public class EndpointQuery
    {
        // Item Mode 
        public string[] PrimaryKeys { get; set; } = new string[] { };
        public string[] UpdateFields { get; set; } = new string[] { };
        public string[] GetItem { get; set; } = new string[] { };
        public string[] UpdateItem { get; set; } = new string[] { };
        public string[] DeleteItem { get; set; } = new string[] { };
        public string[] UploadFile { get; set; } = new string[] { };
        public string[] DownloadFile { get; set; } = new string[] { };

        // List Mode 
        public string[] InsertFields { get; set; } = new string[] { };
        public string[] InsertList { get; set; } = new string[] { };
        public string[] CountList { get; set; } = new string[] { };
        public string[] GetList { get; set; } = new string[] { };
        public string[] SearchFields { get; set; } = new string[] { };
        public string[] SearchList { get; set; } = new string[] { };
    }

    public class EndpointConfiguration
    {
        public string DisplayName { get; set; }  
        public string Name { get; set; } 
        public string PrefixItem { get; set; }
        public string PrefixList { get; set; }
        public bool Enable { get; set; }
        public string DatabaseType { get; set; }
        public string ConnectionString { get; set; }
        public string PrimaryKeysDelimiter { get; set; } = ";"; 
        public string[] PermissionsRoles { get; set; } = new string[] { };
        public Dictionary<string, object> Widgets { get; set; }
        public object[] WidgetsOnCreate { get; set; } = new object[] { };
        public object[] WidgetsOnEdit { get; set; } = new object[] { };
        public object[] ListFields { get; set; } = new string[] { };
        public SchemaFieldConfig[] FieldsSchema { get; set; } = new SchemaFieldConfig[] { };
        public Dictionary<string, EndpointQuery> Endpoints { get; set; } = new Dictionary<string, EndpointQuery>();
    }
}
