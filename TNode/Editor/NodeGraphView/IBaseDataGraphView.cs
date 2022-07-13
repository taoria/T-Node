﻿using TNode.Models;
using UnityEngine;

namespace TNode.Editor.NodeGraphView{
    public interface IBaseDataGraphView{
        public void AddTNode(NodeData nodeData, Rect rect);
        public void RemoveTNode(NodeData nodeData);

        public void CreateBlackboard();
        public GraphData GetGraphData();
        public BlackboardData GetBlackboardData();
        
        
    }
}