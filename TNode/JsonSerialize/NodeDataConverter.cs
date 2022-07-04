using System;
using Newtonsoft.Json;
using TNode.Models;
using UnityEngine;

namespace TNode.JsonSerialize{
    public class NodeDataConverter:JsonConverter<NodeData>{
        public override void WriteJson(JsonWriter writer, NodeData value, JsonSerializer serializer){
            //Write node data with type information
            writer.WriteStartObject();
            writer.WritePropertyName("type");
            Debug.Log(value.GetType().ToString());
            writer.WriteValue(value.GetType().Name);
            writer.WritePropertyName("data");
            serializer.Serialize(writer, value, value.GetType());
            
            
            writer.WriteEndObject();
            

        }

        public override NodeData ReadJson(JsonReader reader, Type objectType, NodeData existingValue, bool hasExistingValue,
            JsonSerializer serializer){
            //Load type info
            reader.Read();
       
            if (reader.Value != null){
                var type = reader.Value.ToString();
                if (type.Trim().Length==0){
                    Debug.LogError(type);
                    throw new JsonSerializationException("Type name is empty");
                }
                reader.Read();
                //Load data
                var data = serializer.Deserialize(reader, Type.GetType(type));
                return (NodeData) data;
            }
            return null;
        }
    }
}