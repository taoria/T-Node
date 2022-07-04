using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TNode.JsonSerialize{
    public class JsonSerializeTool{
        public static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings(){
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            Converters = new List<JsonConverter> { new Vector3Converter() },
            TypeNameHandling = TypeNameHandling.Auto
          
        };

        public static JsonSerializerSettings InternalJsonSerializerSettings = new JsonSerializerSettings(){
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };
    }
}