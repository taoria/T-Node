using System.Collections.Generic;
using TNode.Editor.EditorPersistence;
using TNode.Editor.Model;
using UnityEngine;

namespace TNode.Editor{
    
    [CreateAssetMenu(fileName = "NodeAttribute Editor Config", menuName = "TNode/NodeAttribute Editor Config")]
    public class GraphEditorData:ScriptableObject{
        public List<NodeEditorData> nodesData;
        public List<NodeEditorData> subWindowPos;
        
        public List<IGraphViewPersistence> graphViewPersistence;
    }
}