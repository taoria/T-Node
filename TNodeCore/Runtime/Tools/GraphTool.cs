using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TNode.TNodeCore.Runtime.Components;
using TNodeCore.Runtime;
using TNodeCore.Runtime.Components;
using TNodeCore.Runtime.Extensions;
using TNodeCore.Runtime.Models;
using TNodeCore.Runtime.RuntimeModels;
using UnityEngine;

namespace TNode.TNodeCore.Runtime.Tools{
    /// <summary>
    /// Graph 
    /// </summary>
    public class GraphTool{
            
        /// <summary>
        /// Topological order of the graph nodes
        /// </summary>
        [NonSerialized]
        public readonly List<RuntimeNode> TopologicalOrder = new List<RuntimeNode>();

        public IRuntimeNodeGraph Parent;
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
                var links = node.InputLinks;
                foreach (var link in links){
                    HandlingLink(link);
                }
                node.NodeData.Process();
            }
        }
        //A IEnumerator version of the DirectlyTraversal,used to run the graph in a coroutine or somewhere you need
        public IEnumerator<RuntimeNode> IterateDirectlyTraversal(){
            if (TopologicalSorted==false){
                throw new Exception("The graph is not sorted,there may be a circular dependency,use another access method instead");
            }
            foreach (var node in TopologicalOrder){
                var links = node.InputLinks;
                foreach (var link in links){
                    HandlingLink(link);
                }
                node.NodeData.Process();
                yield return node;
            }
        }
        /// <summary>
        /// usually used in state transition 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<RuntimeNode> IterateNext(){
            var currentNode = NonDependencyNode.FirstOrDefault();
            if (currentNode == null){
                yield break;
            }

            currentNode.NodeData.Process();
            yield return currentNode;

            while(currentNode.OutputLinks.Any()){
                if (currentNode is ConditionalRuntimeNode conditionalRuntimeNode){
                    var id = conditionalRuntimeNode.GetNextNodeId();
                    if (id != null && id.Trim(' ').Length > 0){
                        Debug.Log(currentNode.NodeData+" is going to run "+id);
                        currentNode = RuntimeNodes[conditionalRuntimeNode.GetNextNodeId()];
                    }
                
                }
                else{
                    var link = currentNode.OutputLinks.FirstOrDefault();
                    if (link != null){
                        HandlingLink(link);
                        currentNode = RuntimeNodes[link.inPort.nodeDataId];     
                    }
                }
                currentNode.NodeData.Process();
                yield return currentNode;
            }

           
        }
        //Try to enable state transition from node to node.
        public IEnumerator<RuntimeNode> DeepFirstSearchWithCondition(){
            //Define the basic data structure for a traversal of the graph
            Stack<RuntimeNode> stack = new Stack<RuntimeNode>();
            HashSet<RuntimeNode> alreadyContained = new HashSet<RuntimeNode>();
            HashSet<RuntimeNode> visited = new HashSet<RuntimeNode>();
            foreach (var runtimeNode in NonDependencyNode){
                stack.Push(runtimeNode);
            }
            while (stack.Count > 0){
                var node = stack.Pop();
                
                visited.Add(node);
                if (node is ConditionalRuntimeNode conditionalRuntimeNode){
                    var ids = conditionalRuntimeNode.GetConditionalNextIds();
                    
                    var nextNodes =  ids.Select(id=>RuntimeNodes[id]).ToList();
                    
                    foreach (var runtimeNode in nextNodes){
                        AddToCollectionIfMeetCondition(alreadyContained, visited,runtimeNode, stack);
                    }
                }
                else{
                    foreach (var runtimeNode in node.OutputLinks.Select(link => RuntimeNodes[link.inPort.nodeDataId])){
                        AddToCollectionIfMeetCondition(alreadyContained, visited,runtimeNode, stack);
                    }
                }
                node.OutputLinks.ForEach(HandlingLink);
                node.NodeData.Process();
                yield return node;
            }
        }
        /// <summary>
        /// Breath first search for the graph.Not a standard BFS algorithm since all entries will be executed first.
        /// </summary>
        /// <returns>The IEnumerator to iterate the node</returns>
        public IEnumerator<RuntimeNode> BreathFirstSearch(){
            //Define the basic data structure for a traversal of the graph
            Queue<RuntimeNode> queue = new Queue<RuntimeNode>();
            //Already contained method to avoid duplicate traversal
            HashSet<RuntimeNode> alreadyContained = new HashSet<RuntimeNode>();
            //Visited method to avoid duplicate traversal
            HashSet<RuntimeNode> visited = new HashSet<RuntimeNode>();
            //Firstly add all entry node to the queue
            foreach (var runtimeNode in NonDependencyNode){
                queue.Enqueue(runtimeNode);
                alreadyContained.Add(runtimeNode);
            }
            //Iterate the queue to implement bfs
            while (queue.Count > 0){
                var node = queue.Dequeue();
                visited.Add(node);
                //Conditional node will be traversed in a special way,only links fit the condition will be traversed
                if (node is ConditionalRuntimeNode conditionalRuntimeNode){
                    var ids = conditionalRuntimeNode.GetConditionalNextIds();
       
                    var nextNodes =  ids.Select(id=>RuntimeNodes[id]).ToList();
      
                    foreach (var runtimeNode in nextNodes){
              
                        AddToCollectionIfMeetCondition(alreadyContained, visited,runtimeNode, queue);
                    }
                }
                else{
                    foreach (var runtimeNode in node.OutputLinks.Select(link => RuntimeNodes[link.inPort.nodeDataId])){
                        AddToCollectionIfMeetCondition(alreadyContained, visited,runtimeNode, queue);
                    }
                }
                node.NodeData.Process();
                //Handle the links of the node
                node.OutputLinks.ForEach(HandlingLink);
                yield return node;
            }
        }

        private void AddToCollectionIfMeetCondition(HashSet<RuntimeNode> alreadyContained,HashSet<RuntimeNode> visited, RuntimeNode runtimeNode, Queue<RuntimeNode> queue){
            //Check if the node is already contained in the queue or already visited
     
            if (visited.Contains(runtimeNode)) return;
            //the already contained guard is used to avoid duplicate traversal because the graph may start with multiple entries and all entry node should be run first.
            //Thus cause the same node could be add to the queue multiple times.
            if (alreadyContained.Contains(runtimeNode)) return;
 
            //Check if the visited node has all previous node of the node
            var dependentNodes = runtimeNode.GetDependentNodesId().Select(x => RuntimeNodes[x]);
            var allDependenciesVisited = dependentNodes.Aggregate(true, (a, b) =>
                alreadyContained.Contains(b) && a
            );
            //If the current node is not prepared,another routine will execute it when all is ready
            if (allDependenciesVisited == false) return;
            
            //If all conditions are met, add the node to the queue
            queue.Enqueue(runtimeNode);
            alreadyContained.Add(runtimeNode);
        }
        private void AddToCollectionIfMeetCondition(HashSet<RuntimeNode> alreadyContained,HashSet<RuntimeNode> visited, RuntimeNode runtimeNode, Stack<RuntimeNode> stack){
            //Check if the node is already contained in the stack
            if (alreadyContained.Contains(runtimeNode)) return;
            if (visited.Contains(runtimeNode)) return;
            //Check if the visited node has all previous node of the node
            var dependentNodes = runtimeNode.GetDependentNodesId().Select(x => RuntimeNodes[x]);
            var allDependenciesVisited = dependentNodes.Aggregate(true, (a, b) =>
                alreadyContained.Contains(b) && a
            );
            //If the current node is not prepared,run it dependently.
            if (allDependenciesVisited == false){
                RunNodeDependently(runtimeNode,0,false);
            }
            
            //If all conditions are met, add the node to the stack
            stack.Push(runtimeNode);
            alreadyContained.Add(runtimeNode);
        }
        private void AddNodeToStackIfMeetCondition(HashSet<RuntimeNode> alreadyContained, RuntimeNode runtimeNode, Stack<RuntimeNode> stack){
            //Check if the node is already contained in the queue
            if (alreadyContained.Contains(runtimeNode)) return;
            
            //Check if the visited node has all previous node of the node
            var dependentNodes = runtimeNode.GetDependentNodesId().Select(x => RuntimeNodes[x]);
            var allDependenciesVisited = dependentNodes.Aggregate(true, (a, b) =>
                alreadyContained.Contains(b) && a
            );
            if (allDependenciesVisited == false) return;
            
            //If all conditions are met, add the node to the queue
            stack.Push(runtimeNode);
            alreadyContained.Add(runtimeNode);
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
        /// <param name="processTargetNode">if the the node of the 0 level should be processed,which is the node you want to run,be processed by the method</param>
        public void RunNodeDependently(RuntimeNode runtimeNode,int dependencyLevel=0,bool processTargetNode=true){
            var links = runtimeNode.InputLinks;
            foreach (var link in links){
                var outputNode = RuntimeNodes[link.outPort.nodeDataId];
                if (outputNode is ConditionalRuntimeNode){
                    continue;
                }
                RunNodeDependently(outputNode,dependencyLevel+1);
                HandlingLink(link);
            }

            if (dependencyLevel > DependencyLevelMax){
                throw new Exception("Dependency anomaly detected,check if there is a loop in the graph");
            }

       
            //if the runtime node has no output ,it will not be processed
            if (runtimeNode.OutputLinks.Count == 0 && dependencyLevel != 0){
                return;
            }

            if (processTargetNode||dependencyLevel != 0){
                runtimeNode.NodeData.Process();
            }
            
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
            var outValue = OutputCached.ContainsKey(cachedKey) ? OutputCached[cachedKey] : outNode.GetOutput(nodeLink.outPort.portEntryName);
            Debug.Log(outValue);
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
        /// <param name="graph">The graph you want to build graph tool for</param>


        public GraphTool(IRuntimeNodeGraph graph){
            RuntimeNodes = graph.GetRuntimeNodesDictionary();
            var list = graph.GetRuntimeNodes();
            Parent = graph;
            if (Parent == null){
                
            }
            if (list == null) return;
            Queue<RuntimeNode> queue = new Queue<RuntimeNode>();
            Dictionary<string,int> inDegreeCounterForTopologicalSort = new Dictionary<string, int>();
            foreach (var runtimeNode in list){
                var id = runtimeNode.NodeData.id;
                if (!inDegreeCounterForTopologicalSort.ContainsKey(id)){
                    inDegreeCounterForTopologicalSort.Add(id,runtimeNode.InputLinks.Count);
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
                foreach (var outputLink in node.OutputLinks){
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