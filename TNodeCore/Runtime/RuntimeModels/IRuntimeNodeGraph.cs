using System;
using System.Collections.Generic;
using TNodeCore.Runtime.Models;

namespace TNodeCore.Runtime.RuntimeModels{
    public enum AccessMethod{
        //Iterate all nodes by breadth first search,start with the entry nodes and resolve the dependencies
        Bfs,
        //Iterate all nodes by deep first search,but run the dependencies that are not ready first
        Dfs,
        ///Start from the entry node,if multiple entry nodes exist,run first of them ,and then ,from this entry node,transit to nodes that met the condition,if 
        ///there is no node that met the condition ,stay in the state ,if there are multiple nodes that met the condition,run first of them
        /// If the run node depends on other nodes,run the dependencies first.
        StateTransition,
        /// <summary>
        /// Iterate all nodes by  a topological order 
        /// </summary>
        Dependency,
        
        
    }
    
    public interface IRuntimeNodeGraph{
        public AccessMethod AccessMethod{ get; set; }
        
        public RuntimeNode GetRuntimeNode(NodeData nodeData);
        public RuntimeNode GetRuntimeNode(string id);
        public BlackboardData GetBlackboardData();
        public List<RuntimeNode> GetRuntimeNodes();
        public Dictionary<string,RuntimeNode> GetRuntimeNodesDictionary();
        
        
        public NodeData GetNode(string id);
        List<RuntimeNode> GetRuntimeNodesOfType(Type type);
        List<RuntimeNode> GetRuntimeNodesOfType<T>();

        /// <summary>
        /// Return a node if there is a node is concerned 
        /// </summary>
        /// <returns></returns>
        public void ResetState();


        RuntimeNode MoveNext();
        NodeData CurrentNode();
        RuntimeNode CurrentRuntimeNode();
    }
}