using System.Collections.Generic;
using System.Text.Json;

namespace Common.Extensions
{
    public static class ObjectToDictionaryExtensions { 
        public static IDictionary<string, object> ToDictionary(this object o) {
            var j = JsonSerializer.Serialize(o); 
            var value = JsonSerializer.Deserialize<IDictionary<string, object>>(j); 
            return value; 
        }
    }
}
