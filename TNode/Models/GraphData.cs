using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TNode.Editor;
using TNode.JsonSerialize;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Serialization;

namespace TNode.Models{
    [Serializable]
    public class GraphData:ScriptableObject,ISerializationCallbackReceiver{
        [SerializeField]
        public Dictionary<string,NodeData> NodeDictionary = new Dictionary<string,NodeData>();
        public List<NodeLink> nodeLinks = new();
        public BlackboardData blackboardData = new();
        [TextArea(1,10)]
        [SerializeField]
        //[HideInInspector]
        private string jsonObject;
        [TextArea(1,10)]
        [SerializeField]
        private string jsonBlackboard;
        public void OnBeforeSerialize(){
            jsonObject = JsonConvert.SerializeObject(NodeDictionary,JsonSerializeTool.JsonSerializerSettings);
            jsonBlackboard = JsonConvert.SerializeObject(blackboardData,JsonSerializeTool.JsonSerializerSettings);
        }
        public void OnAfterDeserialize(){
            //Deserialize node dictionary
            var deserializedData = JsonConvert.DeserializeObject<Dictionary<string,NodeData>>(jsonObject,JsonSerializeTool.JsonSerializerSettings);
            NodeDictionary = deserializedData;
            //Deserialize blackboard data
            var deserializedBlackboard = JsonConvert.DeserializeObject<BlackboardData>(jsonBlackboard,JsonSerializeTool.JsonSerializerSettings);
            blackboardData = deserializedBlackboard;
            
        }
    }
}