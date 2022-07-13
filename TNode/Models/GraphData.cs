using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TNode.JsonSerialize;
using UnityEngine.Serialization;

namespace TNode.Models{
    [Serializable]
    public class GraphData:ScriptableObject,ISerializationCallbackReceiver{
        public Dictionary<string,NodeData> NodeDictionary = new Dictionary<string,NodeData>();
        
        
        [SerializeField]
        protected List<NodeLink> nodeLinks;
        [TextArea(1,10)]
        [SerializeField]
        //[HideInInspector]
        private string jsonNodeData;
        [TextArea(1,10)]
        [SerializeField]
        private string jsonBlackboard;
        
        
        public BlackboardData blackboardData;

        public List<NodeLink> NodeLinks{
            get{
                return nodeLinks ??= new List<NodeLink>();
                
            }
            set => nodeLinks = value;
        }
    
  
        public void OnBeforeSerialize(){
            if (nodeLinks != null){
                jsonNodeData = JsonConvert.SerializeObject(NodeDictionary,JsonSerializeTool.JsonSerializerSettings);

            }

            if (jsonBlackboard != null){
                jsonBlackboard = JsonConvert.SerializeObject(blackboardData,typeof(object),JsonSerializeTool.JsonSerializerSettings);

            }
        }
        public void OnAfterDeserialize(){
            //Deserialize node dictionary
            var deserializedData = JsonConvert.DeserializeObject<Dictionary<string,NodeData>>(jsonNodeData,JsonSerializeTool.JsonSerializerSettings);
            NodeDictionary = deserializedData;
            //Deserialize blackboard data
            // var deserializedBlackboard =
            //     JsonConvert.DeserializeObject(jsonBlackboard,JsonSerializeTool.JsonSerializerSettings);
            // blackboardData = deserializedBlackboard as BlackboardData;
            // Debug.Log(deserializedBlackboard);
        }

        public void OnEnable(){
            var deserializedBlackboard =
                JsonConvert.DeserializeObject(jsonBlackboard,JsonSerializeTool.JsonSerializerSettings);
            blackboardData = deserializedBlackboard as BlackboardData;
            Debug.Log(deserializedBlackboard);
        }
    }
}