using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TNode.JsonSerialize;
using UnityEngine.Serialization;

namespace TNode.Models{
    [Serializable]
    public class GraphData:ScriptableObject,ISerializationCallbackReceiver{
        [SerializeField]
        public Dictionary<string,NodeData> NodeDictionary = new Dictionary<string,NodeData>();
        public List<NodeLink> nodeLinks = new List<NodeLink>();
        

        [SerializeField]
        [TextArea]
        //[HideInInspector]
        private string jsonObject;

        public void OnBeforeSerialize(){
            var serializedData  = JsonConvert.SerializeObject(NodeDictionary,JsonSerializeTool.JsonSerializerSettings);
       
            jsonObject = serializedData;
        }

        public void OnAfterDeserialize(){
            var deserializedData = JsonConvert.DeserializeObject<Dictionary<string,NodeData>>(jsonObject,JsonSerializeTool.JsonSerializerSettings);
            var deserializedData2 = JsonUtility.FromJson<Dictionary<string,NodeData>>(jsonObject);
            NodeDictionary = deserializedData;
            Debug.Log("hi");
        }
    }
}