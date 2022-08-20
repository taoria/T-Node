using System;
using TNodeCore.Runtime;
using TNodeCore.Runtime.Components;
using TNodeCore.Runtime.Models;
using TNodeCore.Runtime.RuntimeModels;
using UnityEngine;

namespace TNodeCore.Editor.NodeGraphView{
    /// <summary>
    /// Base data graph view interface for all data graph views.its define the most behaviour of a data graph view.
    /// </summary>
    public interface IBaseDataGraphView{
        /// <summary>
        /// To add a node to the graph view
        /// </summary>
        /// <param name="nodeData">The node data you want to add to graph view</param>
        /// <param name="rect">The position you want to add in. In most situation, calculated by the implementation.</param>
        
        public void AddTNode(NodeData nodeData, Rect rect);
        /// <summary>
        /// Remove the node from the graph view.since Experimental graph view holds the reference itself.Currently no use of the this function
        /// </summary>
        /// <param name="nodeData"></param>
        public void RemoveTNode(NodeData nodeData);
        /// <summary>
        /// Add a link to the graphview
        /// </summary>
        /// <param name="nodeLink"></param>

        public void AddLink(NodeLink nodeLink);
        /// <summary>
        /// Remove link from a graph view.
        /// </summary>
        /// <param name="nodeLink"></param>
        
        public void RemoveLink(NodeLink nodeLink);
        
        /// <summary>
        /// Is the graph view gives a auto update feature.St to true to call periodically the update function.
        /// </summary>

        
        public bool AutoUpdate{ get; set; }
        
        /// <summary>
        /// CreateProp the blackboard view for the graph view.
        /// </summary>
        public void CreateBlackboard();
        /// <summary>
        /// Get the inspected graph data.
        /// </summary>
        /// <returns></returns>
        public GraphData GetGraphData();
        /// <summary>
        /// Get the inspected blackboard data.Differs in runtime graph and static graph.
        /// </summary>
        /// <returns></returns>
        public BlackboardData GetBlackboardData();
        
        /// <summary>
        ///  Check if it's used by a runtime graph.
        /// </summary>
        
        public bool IsRuntimeGraph{ get; set; }
        /// <summary>
        /// Null if it is not a runtime graph
        /// </summary>
        /// <returns></returns>
        public IRuntimeNodeGraph GetRuntimeNodeGraph();
        /// <summary>
        /// Edit a graph data.
        /// </summary>
        /// <param name="graph">The graph you want to edit,watch,update,modify</param>

        public void SetGraphData(GraphData graph);
        /// <summary>
        /// call after a runtime graph is finished its execution
        /// </summary>

        public Action AfterGraphResolved{ get; set; }
        
        /// <summary>
        /// call After the editor load this current graphview
        /// </summary>
        void AfterEditorLoadGraphView();
        
        //todo remove it later ,keep it now
        void NotifyRuntimeUpdate();

    }
}