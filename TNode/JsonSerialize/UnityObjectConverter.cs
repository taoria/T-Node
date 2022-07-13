using System;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TNode.JsonSerialize{
    public class UnityObjectConverter:JsonConverter<Object>{
        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer){
            //Holding the object reference in a string
            var go = value;
            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(go));
            writer.WriteValue(value.GetInstanceID().ToString());
        }

        public override Object ReadJson(JsonReader reader, Type objectType, Object existingValue, bool hasExistingValue,
            JsonSerializer serializer){
            //Reading the object reference from the string
            var guid = reader.Value.ToString();
            var go = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid));
            return go;
        }
    }
}