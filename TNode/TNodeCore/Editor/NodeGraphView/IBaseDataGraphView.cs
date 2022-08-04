using System;
using TNodeCore.Runtime;
using TNodeCore.Runtime.Components;
using TNodeCore.Runtime.Models;
using UnityEngine;

namespace TNodeCore.Editor.NodeGraphView{
    public interface IBaseDataGraphView{
        public void AddTNode(NodeData nodeData, Rect rect);
        public void RemoveTNode(NodeData nodeData);

        public void AddLink(NodeLink nodeLink);
        
        public void RemoveLink(NodeLink nodeLink);

        
        public bool AutoUpdate{ get; set; }
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


        public Action AfterGraphResolved{ get; set; }
        void AfterEditorLoadGraphView();
        
        //todo remove it later ,keep it now
        void NotifyRuntimeUpdate();

    }
}