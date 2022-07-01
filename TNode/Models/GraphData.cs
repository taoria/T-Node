using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace TNode.Models{
    [Serializable]
    public class GraphData:ScriptableObject,ISerializationCallbackReceiver{
        [SerializeField]
        public Dictionary<string,NodeData> NodeDictionary = new Dictionary<string,NodeData>();

        [SerializeField]
        [HideInInspector]
        private string jsonObject;

        public void OnBeforeSerialize(){
            var serializedData  = JsonConvert.SerializeObject(NodeDictionary);
            jsonObject = serializedData;
        }

        public void OnAfterDeserialize(){
            var deserializedData = JsonConvert.DeserializeObject<Dictionary<string,NodeData>>(jsonObject);
            NodeDictionary = deserializedData;
        }
    }
}