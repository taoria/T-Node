using System;
using System.Collections.Generic;
using TNodeCore.Runtime;
using TNodeCore.Runtime.Components;
using TNodeCore.Runtime.Models;

namespace TNode.TNodeCore.Runtime.Tools{
    public class GraphTool{
            
        /// <summary>
        /// Topological order of the graph nodes
        /// </summary>
        [NonSerialized]
        public readonly List<RuntimeNode> TopologicalOrder = new List<RuntimeNode>();

        public RuntimeGraph Parent;
        public bool TopologicalSorted = false;
            
        /// <summary>
        /// Entry nodes of the graph. These are the nodes that has no input.
        /// </summary>
        [NonSerialized]
        public readonly List<RuntimeNode> NonDependencyNode = new List<RuntimeNode>();
        /// <summary>
        /// Cached data for Dependency traversal.
        /// </summary>
        public readonly Dictionary<string, object> OutputCached = new Dictionary<string, object>();
            
        /// <summary>
        /// Ssed to detect if the graph tool is caching the output data of the node
        /// </summary>
        private bool _isCachingOutput = false;
        /// <summary>
        /// elements are read only ,do not modify them
        /// </summary>
        public readonly Dictionary<string, RuntimeNode> RuntimeNodes;
        //Traverse and process all nodes in a topological order,dependency of the node is already resolved.if you want to run specific node,you can use RunNodeDependently instead
        public void DirectlyTraversal(){
            foreach (var node in TopologicalOrder){
                var links = node.InputLink;
                foreach (var link in links){
                    HandlingLink(link);
                }
                node.NodeData.Process();
            }
        }
        
        /// <summary>
        /// Cache out port data in the graph tool so that we can directly access the output.
        /// The two function assume there will be no change happens in scene nodes or blackboard referenced data during the running,so in a dependency traversal for some
        /// batch of nodes.the nodes could directly access the output data in the graph tool instead of waiting dependency traversal resolve the result of the output.
        /// </summary>
        public void StartCachingPort(){
            _isCachingOutput = true;
        }
        public void EndCachingPort(){
            _isCachingOutput = false;
            OutputCached.Clear();
        }
        /// <summary>
        /// Resolve dependencies by a deep first search,the depended nodes will be processed to satisfy the need of the the given runtime node
        /// Note it's a recursive function.if you want directly traverse all nodes with dependency resolved ,use DirectlyTraversal() instead.
        /// </summary>
        /// <param name="runtimeNode">The node you want to resolve dependency</param>
        /// <param name="dependencyLevel">search depth,no need provide a number when use outside</param>
        public void RunNodeDependently(RuntimeNode runtimeNode,int dependencyLevel=0){
            var links = runtimeNode.InputLink;
            foreach (var link in links){
                var outputNode = RuntimeNodes[link.outPort.nodeDataId];
                RunNodeDependently(outputNode,dependencyLevel+1);
                HandlingLink(link);
            }

            if (dependencyLevel > DependencyLevelMax){
                throw new Exception("Dependency anomaly detected,check if there is a loop in the graph");
            }

       
            //if the runtime node has no output ,it will not be processed
            if (runtimeNode.OutputLink.Count == 0 && dependencyLevel != 0){
                return;
            }
            runtimeNode.NodeData.Process();
            Parent.StartCoroutine(runtimeNode.NodeData.AfterProcess());
        }
        /// <summary>
        /// Max depth of dependency traversal,in case of some special situation. the dependency level bigger than this number will be considered as a loop.
        /// </summary>
        private const int DependencyLevelMax = 1145;
        /// <summary>
        /// Handling a node link to transfer data from it's output side to the input side
        /// </summary>
        /// <param name="nodeLink">Link you want to process</param>
        public void HandlingLink(NodeLink nodeLink){
            //out node is node output data
            //in node is node receive data
            var inNode = RuntimeNodes[nodeLink.inPort.nodeDataId];
            var outNode = RuntimeNodes[nodeLink.outPort.nodeDataId];
                
                
            //TODO looks like this string would be too long to make a cache
                
            var cachedKey = $"{outNode.NodeData.id}-{nodeLink.inPort.portEntryName}";
            var outValue = OutputCached.ContainsKey(cachedKey) ? OutputCached[cachedKey] : outNode.GetOutput(nodeLink.outPort.portEntryName);;
            if (_isCachingOutput){
                OutputCached[cachedKey] = outValue;
            }
            inNode.SetInput(nodeLink.inPort.portEntryName, outValue);
        }
        /// <summary>
        /// Constructor of the graph tool,it will traverse the graph and build the topological order of the graph.
        /// </summary>
        /// <param name="list">List of nodes you need to traversal to build graph tool</param>
        /// <param name="graphNodes">Map stores the mapping of node data id to runtime node</param>
        
        public GraphTool(List<RuntimeNode> list, Dictionary<string, RuntimeNode> graphNodes,RuntimeGraph graph){
            RuntimeNodes = graphNodes;
            Parent = graph;
            if (list == null) return;
            Queue<RuntimeNode> queue = new Queue<RuntimeNode>();
            Dictionary<string,int> inDegreeCounterForTopologicalSort = new Dictionary<string, int>();
            foreach (var runtimeNode in list){
                var id = runtimeNode.NodeData.id;
                if (!inDegreeCounterForTopologicalSort.ContainsKey(id)){
                    inDegreeCounterForTopologicalSort.Add(id,runtimeNode.InputLink.Count);
                }
                if (inDegreeCounterForTopologicalSort[id] == 0){
                    queue.Enqueue(runtimeNode);
                    NonDependencyNode.Add(runtimeNode);
                }
            }
                
            //Topological sort
            while (queue.Count > 0){
                var node = queue.Dequeue();
                TopologicalOrder.Add(node);
                foreach (var outputLink in node.OutputLink){
                    inDegreeCounterForTopologicalSort[outputLink.inPort.nodeDataId]--;
                    if (inDegreeCounterForTopologicalSort[outputLink.inPort.nodeDataId] == 0){
                        queue.Enqueue(RuntimeNodes[outputLink.inPort.nodeDataId]);
                    }
                }
            }

            TopologicalSorted = TopologicalOrder.Count != list.Count;
            
            inDegreeCounterForTopologicalSort.Clear();
            queue.Clear();
        }
            
            
    }
}