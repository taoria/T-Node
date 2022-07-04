using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace TNode.JsonSerialize{
    public static class JsonSerializeTool{
        class WritablePropertiesOnlyResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
                return props.Where(p => p.Writable).ToList();
            }
        }
        public static JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings(){
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            Converters = new List<JsonConverter> { new Vector3Converter() },
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new WritablePropertiesOnlyResolver()

        };

        public static JsonSerializerSettings InternalJsonSerializerSettings = new JsonSerializerSettings(){
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        };
    }
}