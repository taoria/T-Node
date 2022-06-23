using System.Collections.Generic;
using TNode.Editor.Model;
using UnityEngine;

namespace TNode.Editor{
    
    [CreateAssetMenu(fileName = "Node Editor Config", menuName = "TNode/Node Editor Config")]
    public class GraphEditorData:ScriptableObject{
        public List<NodeEditorData> nodesData;
        
    }
}