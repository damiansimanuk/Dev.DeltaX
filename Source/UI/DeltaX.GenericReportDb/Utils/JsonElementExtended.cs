using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DeltaX.GenericReportDb.Utils
{
    public static class JsonElementExtended
    {
        public static object ToObject(this JsonElement element)
        {
            object result = null;

            switch (element.ValueKind)
            {
                case JsonValueKind.Null:
                    result = null;
                    break;
                case JsonValueKind.Number:
                    result = element.GetDouble();
                    break;
                case JsonValueKind.False:
                    result = false;
                    break;
                case JsonValueKind.True:
                    result = true;
                    break;
                case JsonValueKind.Undefined:
                    result = null;
                    break;
                case JsonValueKind.String:
                    result = element.GetString();
                    break;
                case JsonValueKind.Object:
                    element.EnumerateObject().Select(o => new KeyValuePair<string, object>(o.Name, o.Value.ToObject()));
                    break;
                case JsonValueKind.Array:
                    result = element.EnumerateArray()
                        .Select(o => o.ToObject())
                        .ToArray();
                    break;
            }

            return result;
        }


    } 
}
