using System;
using System.Collections.Generic;
using TNodeCore.Editor.Models;
using UnityEditor;
using UnityEngine;

namespace TNodeCore.Runtime.Models{
    [Serializable]
    public class GraphData:ScriptableObject,ISerializationCallbackReceiver{
        [NonSerialized]
        public Dictionary<string,NodeData> NodeDictionary = new Dictionary<string,NodeData>();
        
        [SerializeReference]
        public List<NodeData> nodeList = new List<NodeData>();
        
        [SerializeField]
        protected List<NodeLink> nodeLinks;
        [SerializeReference]
        public BlackboardData blackboardData;

        [HideInInspector] 
        public string sceneReference;

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
        
        #if UNITY_EDITOR
        [SerializeReference] public List<EditorModel> EditorModels = new List<EditorModel>();

#endif

    }

 
}