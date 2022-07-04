using System;
using System.Numerics;
using Newtonsoft.Json;
namespace TNode.JsonSerialize{

    public class Vector3Converter:JsonConverter<Vector3>{
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer){
            writer.WriteStartArray();
            writer.WriteValue(value.X);
            writer.WriteValue(value.Y);
            writer.WriteValue(value.Z);
            writer.WriteEndArray();
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer){
            if (reader.TokenType == JsonToken.Null){
                return default(Vector3);
            }
            else{
                var array = serializer.Deserialize<float[]>(reader);

                if (array != null) return new Vector3(array[0], array[1], array[2]);
            }
            return default(Vector3);
            
        }
    }
}