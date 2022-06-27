using System.Collections.Generic;
using TNode.Editor.Model;
using UnityEngine;

namespace TNode.Editor{
    
    [CreateAssetMenu(fileName = "NodeAttribute Editor Config", menuName = "TNode/NodeAttribute Editor Config")]
    public class GraphEditorData:ScriptableObject{
        public List<NodeEditorData> nodesData;
        
    }
}