using System.Collections.Generic;
using UnityEngine;

namespace TNodeCore.Editor.EditorPersistence{
    
    [CreateAssetMenu(fileName = "Graph Editor Data", menuName = "TNode/Graph Editor Data")]
    public class GraphEditorData:ScriptableObject{
        public List<GraphElementEditorData> graphElementsData;
    }
}