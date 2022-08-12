using System;
using System.Collections.Generic;
using TNodeCore.Editor.EditorPersistence;
using TNodeCore.Editor.NodeGraphView;
using TNodeCore.Runtime.Models;
using UnityEngine;
using UnityEngine.Serialization;

namespace TNode.TNodeCore.Editor.EditorPersistence{
    /// <summary>
    /// Graph Editor Data hold the config of a type of graph.
    /// it's used by a graph editor to determine which implementation to use and some other config may be stored in here some days later.
    /// </summary>
    [CreateAssetMenu(fileName = "Graph Editor Data", menuName = "TNode/Graph Editor Data")]
    public class GraphEditorData:ScriptableObject{
        /// <summary>
        /// The implementation of a graph view.experimental graphview or GTF.
        /// </summary>
        public GraphImplType graphImplType = GraphImplType.GraphViewImpl;
        /// <summary>
        /// Cached global static function to create a graph view by the given type of the graph data.it's injected by the implementation side.
        /// </summary>
        public static Func<Type,IBaseDataGraphView> GraphViewImplCreator;
        public static Func<Type,IBaseDataGraphView> GtfImplCreator;
        
        /// <summary>
        /// set this to true to maintain the graph in auto update mode,an auto update mode only applies certain types of graph.
        /// TODO: move to graph data side later.
        /// </summary>
        [FormerlySerializedAs("testMode")] public bool autoUpdate;
        /// <summary>
        /// Get the implementation of a graphview by the given type of the graph.
        /// </summary>
        /// <typeparam name="T">The type of the graph you want to create a view to inspect it</typeparam>
        /// <returns>the corresponding graph view for the given graph data</returns>
        /// <exception cref="NotImplementedException"></exception>
        public IDataGraphView<T> GetGraphView<T> () where T:GraphData{
            switch (graphImplType){
                case GraphImplType.GraphViewImpl:{
                    return (IDataGraphView<T>)GraphViewImplCreator.Invoke(typeof(T));
                    
                }
                case GraphImplType.GraphToolsFoundationImpl:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
        
    }
    /// <summary>
    /// The possible implementation of a graph view by default.
    /// </summary>
    public enum GraphImplType{
        GraphViewImpl,
        GraphToolsFoundationImpl
    }
}