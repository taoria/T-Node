using System;
using System.Collections.Generic;
using System.Linq;
using TNodeCore.Models;
using TNodeCore.Runtime;
using UnityEngine;

namespace TNodeCore.Components{
    public class RuntimeGraph:MonoBehaviour{
        public GraphData graphData;
        public List<SceneNodeData> sceneNodes;

        public readonly Dictionary<string, RuntimeNode> RuntimeNodes = new Dictionary<string, RuntimeNode>();
        private GraphTool _graphTool;
        
        private class GraphTool{
            [NonSerialized]
            public readonly List<RuntimeNode> TopologicalOrder = new List<RuntimeNode>();
            public readonly List<RuntimeNode> EntryNodes = new List<RuntimeNode>();
            /// <summary>
            /// elements are read only ,do not modify them
            /// </summary>
            public readonly Dictionary<string, RuntimeNode> RuntimeNodes;
            public void DirectlyTraversal(){
                foreach (var node in TopologicalOrder){
                    var links = node.InputLink;
                    foreach (var link in links){
                        HandlingLink(link);
                    }
                    node.NodeData.Process();
                }
            }
            public void DependencyTraversal(RuntimeNode runtimeNode){
                var links = runtimeNode.InputLink;
                foreach (var link in links){
                    var outputNode = RuntimeNodes[link.outPort.nodeDataId];
                    DependencyTraversal(outputNode);
                    HandlingLink(link);
                }
                runtimeNode.NodeData.Process();
            }
            
            public void HandlingLink(NodeLink nodeLink){
                var inNode = RuntimeNodes[nodeLink.inPort.nodeDataId];
                var outNode = RuntimeNodes[nodeLink.outPort.nodeDataId];
                
                //out node is node output data
                //in node is node receive data
                var outValue = outNode.GetOutput(nodeLink.outPort.portEntryName);
    
                inNode.SetInput(nodeLink.inPort.portEntryName, outValue);
            }
            public GraphTool(List<RuntimeNode> list, Dictionary<string, RuntimeNode> graphNodes){
                RuntimeNodes = graphNodes;
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
        [SerializeReference]
        public BlackboardData runtimeBlackboardData;
        [NonSerialized]
        private bool _build = false;
        public void Build(){
            
            var link = graphData.NodeLinks;
            //iterate links and create runtime nodes
            foreach (var linkData in link){
                ModifyOrCreateInNode(linkData);
                ModifyOrCreateOutNode(linkData);
            }
            Debug.Log("hi");
            var nodeList = RuntimeNodes.Values;
            _graphTool = new GraphTool(nodeList.ToList(),RuntimeNodes);
            var sceneNodes = RuntimeNodes.Values.Where(x => x.NodeData is SceneNodeData).Select(x => x.NodeData as SceneNodeData);
            foreach (var sceneNode in sceneNodes){
                if (sceneNode != null) sceneNode.BlackboardData = runtimeBlackboardData;
            }
            _build = true;
        }

        public RuntimeNode Get(NodeData nodeData){
            if(!_build)
                Build();
            if(RuntimeNodes.ContainsKey(nodeData.id)){
                return RuntimeNodes[nodeData.id];
            }
            return null;
        }

        public RuntimeNode Get(string id){
            if (RuntimeNodes.ContainsKey(id)){
                return RuntimeNodes[id];
            }
            return null;
        }
        //DFS search for resolving dependency
        public bool ResolveDependency(NodeData startNode){
            if(!_build)
                Build();
            if (_graphTool == null)
                return false;
            _graphTool.DependencyTraversal(Get(startNode));
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
            return RuntimeNodes.Values.Where(x => x.NodeType == typeof(T)).ToList();
        }
        public  List<RuntimeNode> GetRuntimeNodesOfType(Type type){
            return RuntimeNodes.Values.Where(x => x.NodeType == type).ToList();
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
                    runtimeBlackboardData = graphData.blackboardData.Clone() as BlackboardData;
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