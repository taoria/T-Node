using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;

namespace TNode.Models{
    [Serializable]
    public class GraphData:ScriptableObject,ISerializationCallbackReceiver{
        public Dictionary<string,NodeData> NodeDictionary = new Dictionary<string,NodeData>();
        
        [SerializeReference]
        public List<NodeData> nodeList = new List<NodeData>();
        
        [SerializeField]
        protected List<NodeLink> nodeLinks;
        [SerializeReference]
        public BlackboardData blackboardData;

        public List<NodeLink> NodeLinks{
            get{
                return nodeLinks ??= new List<NodeLink>();
                
            }
            set => nodeLinks = value;
        }
    
  
        public void OnBeforeSerialize(){
     
   
            nodeList.Clear();
            foreach(var node in NodeDictionary.Values){
                nodeList.Add(node);
            }
        }
        public void OnAfterDeserialize(){
            NodeDictionary.Clear();
            foreach(var node in nodeList){
                NodeDictionary.Add(node.id,node);
            }
        }
        
    }
}