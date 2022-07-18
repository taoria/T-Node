using TNodeCore.Models;
using TNodeCore.Runtime;
using UnityEngine;

namespace TNodeCore.Editor.NodeGraphView{
    public interface IBaseDataGraphView{
        public void AddTNode(NodeData nodeData, Rect rect);
        public void RemoveTNode(NodeData nodeData);

        
        public bool TestMode{ get; set; }
        public void CreateBlackboard();
        public GraphData GetGraphData();
        public BlackboardData GetBlackboardData();
        
        
        
        public bool IsRuntimeGraph{ get; set; }
        /// <summary>
        /// Null if it is not a runtime graph
        /// </summary>
        /// <returns></returns>
        public RuntimeGraph GetRuntimeGraph();

        public void SetGraphData(GraphData graph);
        
    }
}