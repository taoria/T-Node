using System;
using System.Collections.Generic;
using Dialogue;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TNode.Models{
    [Serializable]
    public class GraphData:ScriptableObject{
        [SerializeReference] 
        public List<NodeData> nodes;

        [SerializeReference] 
        public List<NodeLink> nodeLinks;
        [HideInInspector]
        [SerializeReference]
        public NodeData entryNode;
        
    }
}