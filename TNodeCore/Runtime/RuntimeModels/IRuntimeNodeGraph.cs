using System;
using System.Collections.Generic;
using TNodeCore.Runtime.Models;

namespace TNodeCore.Runtime.RuntimeModels{
    public interface IRuntimeNodeGraph{
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