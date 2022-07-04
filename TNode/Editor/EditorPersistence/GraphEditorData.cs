using System.Collections.Generic;
using TNode.Editor.EditorPersistence;
using TNode.Editor.Model;
using UnityEngine;
using UnityEngine.Serialization;

namespace TNode.Editor{
    
    [CreateAssetMenu(fileName = "Graph Editor Data", menuName = "TNode/Graph Editor Data")]
    public class GraphEditorData:ScriptableObject{
        public List<GraphElementEditorData> graphElementsData;
    }
}