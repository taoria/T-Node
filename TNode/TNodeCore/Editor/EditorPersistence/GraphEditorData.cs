using System;
using System.Collections.Generic;
using TNodeCore.Editor.NodeGraphView;
using TNodeCore.Runtime.Models;
using UnityEngine;

namespace TNodeCore.Editor.EditorPersistence{
    
    [CreateAssetMenu(fileName = "Graph Editor Data", menuName = "TNode/Graph Editor Data")]
    public class GraphEditorData:ScriptableObject{
        [HideInInspector]
        public List<GraphElementEditorData> graphElementsData;
        
        public GraphImplType graphImplType = GraphImplType.GraphViewImpl;
        public static Func<Type,IBaseDataGraphView> GraphViewImplCreator;
        public static Func<Type,IBaseDataGraphView> GtfImplCreator;
        
        public IDataGraphView<T> GetGraphView<T> () where T:GraphData{
            switch (graphImplType){
                case GraphImplType.GraphViewImpl:
                    return (IDataGraphView<T>)GraphViewImplCreator.Invoke(typeof(T));
                case GraphImplType.GraphToolsFoundationImpl:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
        }
        
    }

    public enum GraphImplType{
        GraphViewImpl,
        GraphToolsFoundationImpl
    }
}