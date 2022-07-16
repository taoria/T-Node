using TNodeCore.Models;
using UnityEngine;

namespace TNodeCore.Editor.NodeGraphView{
    public interface IBaseDataGraphView{
        public void AddTNode(NodeData nodeData, Rect rect);
        public void RemoveTNode(NodeData nodeData);

        public void CreateBlackboard();
        public GraphData GetGraphData();
        public BlackboardData GetBlackboardData();
        
        
    }
}