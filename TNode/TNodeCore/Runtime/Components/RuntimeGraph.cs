using System;
using System.Collections.Generic;
using System.Linq;
using TNodeCore.Runtime.Models;
using UnityEngine;

namespace TNodeCore.Runtime.Components{
    public class RuntimeGraph:MonoBehaviour{
        /// <summary>
        /// Graph data reference to be used in runtime
        /// </summary>
        public GraphData graphData;
        /// <summary>
        /// Runtime copy of scene node data to hold references to scene objects
        /// </summary>
        public List<SceneNodeData> sceneNodes;
        
        /// <summary>
        /// Map of node id to runtime node
        /// </summary>
        [NonSerialized]
        public readonly Dictionary<string, RuntimeNode> RuntimeNodes = new Dictionary<string, RuntimeNode>();
        ///<summary>
        /// The graph tool the current runtime graph is using
        /// </summary>
        [NonSerialized]
        private GraphTool _graphTool;
        /// <summary>
        /// Inner graph tool to help with graph operations
        /// </summary>
        private class GraphTool{
            
            /// <summary>
            /// Topological order of the graph nodes
            /// </summary>
            [NonSerialized]
            public readonly List<RuntimeNode> TopologicalOrder = new List<RuntimeNode>();

            public RuntimeGraph Parent;
            
            /// <summary>
            /// Entry nodes of the graph. These are the nodes that has no input.
            /// </summary>
            [NonSerialized]
            public readonly List<RuntimeNode> EntryNodes = new List<RuntimeNode>();
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
                        EntryNodes.Add(runtimeNode);
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
                if(TopologicalOrder.Count!= list.Count){
                    throw new Exception("Topological sort failed,circular dependency detected");
                }
             
                inDegreeCounterForTopologicalSort.Clear();
                queue.Clear();
            }
            
            
        }
        /// <summary>
        /// Holding the reference of the blackboard ,but it will be override by the runtime graph
        /// </summary>
        [SerializeReference]
        public BlackboardData runtimeBlackboardData;
        /// <summary>
        /// Check if the runtime graph is build .a built graph has a graph tool set up
        /// </summary>
        [NonSerialized]
        private bool _build = false;
        
        /// <summary>
        /// Build the graph tool and other dependencies for the runtime graph
        /// </summary>
        public void Build(){
            if (_build) return;
            
            var link = graphData.NodeLinks;
            //iterate links and create runtime nodes
            foreach (var linkData in link){
                ModifyOrCreateInNode(linkData);
                ModifyOrCreateOutNode(linkData);
            }
            var nodeList = RuntimeNodes.Values;
            _graphTool = new GraphTool(nodeList.ToList(),RuntimeNodes,this);
            var sceneNodes = RuntimeNodes.Values.Where(x => x.NodeData is SceneNodeData).Select(x => x.NodeData as SceneNodeData);
            foreach (var sceneNode in sceneNodes){
                if (sceneNode != null) sceneNode.BlackboardData = runtimeBlackboardData;
            }
            _build = true;
        }

        /// <summary>
        /// Cast the node data to a runtime node
        /// </summary>
        /// <param name="nodeData">Node data you provided</param>
        /// <returns></returns>
        public RuntimeNode Get(NodeData nodeData){
            if(!_build)
                Build();
            if(RuntimeNodes.ContainsKey(nodeData.id)){
                return RuntimeNodes[nodeData.id];
            }
            return null;
        }
        /// <summary>
        /// Get the runtime node from an id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RuntimeNode Get(string id){
            if (RuntimeNodes.ContainsKey(id)){
                return RuntimeNodes[id];
            }
            return null;
        }
        //DFS search to run a node.
        public bool RunOnDependency(NodeData startNode){
            if(!_build)
                Build();
            if (_graphTool == null)
                return false;
            _graphTool.RunNodeDependently(Get(startNode));
            return true;
        }
        public bool ResolveDependency(){
            if(!_build)
                Build();
            if (_graphTool == null)
                return false;
            _graphTool.DirectlyTraversal();
            return true;
        }
        private void ModifyOrCreateInNode(NodeLink linkData){
            var inNodeId = linkData.inPort.nodeDataId;
            var inNode = graphData.NodeDictionary[inNodeId];
            if (!RuntimeNodes.ContainsKey(inNode.id)){
                var runtimeInNode = new RuntimeNode(inNode);
                RuntimeNodes.Add(inNode.id,runtimeInNode);
            }
            RuntimeNodes[inNode.id].InputLink.Add(linkData);
            
        }
        public List<RuntimeNode> GetRuntimeNodesOfType<T>(){
            return RuntimeNodes.Values.Where(x => typeof(T).IsAssignableFrom(x.NodeType)).ToList();
        }
        public  List<RuntimeNode> GetRuntimeNodesOfType(Type type){
            return RuntimeNodes.Values.Where(x => type.IsAssignableFrom(type)).ToList();
        }
        public void RunNodesOfType(Type t,bool isCaching= false){
            var nodes = GetRuntimeNodesOfType(t);
            if(isCaching)
                _graphTool.StartCachingPort();
            foreach (var runtimeNode in nodes){
                RunOnDependency(runtimeNode.NodeData);
            }
            if(isCaching)
                _graphTool.EndCachingPort();
        }

        /// <summary>
        /// Run some nodes ,if the node is not in the graph ,then pass
        /// </summary>
        /// <param name="runtimeNodes"></param>
        /// <param name="isCaching"></param>
        public void RunNodes(List<RuntimeNode> runtimeNodes,bool isCaching= false){
            if (isCaching){
                _graphTool.StartCachingPort();
            }
            foreach (var runtimeNode in runtimeNodes){
                if(!RuntimeNodes.ContainsKey(runtimeNode.NodeData.id)){
                    continue;
                }
                RunOnDependency(runtimeNode.NodeData);
            }
            if (isCaching){
                _graphTool.EndCachingPort();
            }
        }
        private void ModifyOrCreateOutNode(NodeLink linkData){
            var outNodeId = linkData.outPort.nodeDataId;
            var outNode = graphData.NodeDictionary[outNodeId];
            if(!RuntimeNodes.ContainsKey(outNode.id)){
                var runtimeOutNode = new RuntimeNode(outNode);
                RuntimeNodes.Add(outNode.id,runtimeOutNode);
            }
            RuntimeNodes[outNode.id].OutputLink.Add(linkData);
        }
        
        public void OnValidate(){
            if(runtimeBlackboardData==null||runtimeBlackboardData.GetType()==typeof(BlackboardData)){
                if (graphData != null)
                    runtimeBlackboardData = graphData.blackboardData?.Clone() as BlackboardData;
            }
        }

        public void OnDisable(){
            RuntimeNodes.Clear();
            _build = false;
        }

        public void OnDestroy(){
            RuntimeNodes.Clear();
            _build = false;
        }

        public void Start(){
            Build();
          
        }
        public virtual void RuntimeExecute(){
            _graphTool.DirectlyTraversal();
        }
       
    }

    public enum ProcessingStrategy{
        BreadthFirst,
        DepthFirst
    }
}