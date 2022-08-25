using System;
using System.Collections.Generic;
using System.Linq;
using TNode.TNodeCore.Runtime.Models;
using TNode.TNodeCore.Runtime.Tools;
using TNodeCore.Runtime.Models;
using UnityEngine;

namespace TNodeCore.Runtime.RuntimeModels{
    public class StaticGraph:IRuntimeNodeGraph{
        private Dictionary<string,RuntimeNode> _nodes;
        private IEnumerator<RuntimeNode> _runtimeNodeEnumerator;
        
        private readonly GraphTool _graphTool;
        private readonly GraphData _originalData;

        private void ModifyLinks(NodeLink linkData){
            var outNodeId = linkData.outPort.nodeDataId;
            var outNode = _nodes[outNodeId];
            outNode.OutputLinks.Add(linkData);
         
            var inNodeId = linkData.inPort.nodeDataId;
            var inNode = _nodes[inNodeId];
            Debug.Log($"{inNode},{outNode}");
            inNode.InputLinks.Add(linkData);
        }
        public StaticGraph(GraphData graphData){
            _originalData = graphData;
            var nodes = graphData.NodeDictionary.Values.ToList();
            var links = graphData.NodeLinks;
            _nodes = new Dictionary<string, RuntimeNode>();
    

            foreach (var nodeData in nodes){
                if(_nodes.ContainsKey(nodeData.id)) continue;
                
                if (nodeData is ConditionalNode conditionalNode){
                    ConditionalRuntimeNode conditionalRuntimeNode = new ConditionalRuntimeNode(conditionalNode);
                    _nodes.Add(conditionalNode.id,conditionalRuntimeNode);
                }
                else{
                    _nodes.Add(nodeData.id,new RuntimeNode(nodeData));
                }
                RuntimeNode currentNode = _nodes[nodeData.id];
                currentNode.InputLinks = new List<NodeLink>();
                currentNode.OutputLinks = new List<NodeLink>();
                
            }
            foreach (var link in links){
                ModifyLinks(link);
                
            }
            _graphTool = new GraphTool(this);
            _runtimeNodeEnumerator = _graphTool.BreathFirstSearch();
        }

        public void ResetState(){
            switch (AccessMethod){
                case AccessMethod.Bfs:
                    _runtimeNodeEnumerator = _graphTool.BreathFirstSearch();
                    break;
                case AccessMethod.Dfs:
                    _runtimeNodeEnumerator = _graphTool.DeepFirstSearchWithCondition();
                    break;
                case AccessMethod.StateTransition:
                    _runtimeNodeEnumerator = _graphTool.IterateNext();
                    break;
                case AccessMethod.Dependency:
                    _runtimeNodeEnumerator = _graphTool.IterateDirectlyTraversal();
                    break;
            }

        }

        public AccessMethod AccessMethod{ get; set; } = AccessMethod.Bfs;

        public RuntimeNode GetRuntimeNode(NodeData nodeData){
            return _nodes[nodeData.id];
        }

        public RuntimeNode GetRuntimeNode(string id){
            return _nodes[id];
        }

        public BlackboardData GetBlackboardData(){
            return _originalData.blackboardData;
        }

        public List<RuntimeNode> GetRuntimeNodes(){
            return _nodes.Values.ToList();
        }

        public Dictionary<string, RuntimeNode> GetRuntimeNodesDictionary(){
            return _nodes;
        }

        public NodeData GetNode(string id){
            return _nodes[id].NodeData;
        }

        public List<RuntimeNode> GetRuntimeNodesOfType(Type type){
            return _nodes.Where(x=>x.Value.NodeType == type).Select(x=>x.Value).ToList();
        }

        public List<RuntimeNode> GetRuntimeNodesOfType<T>(){
            return _nodes.Where(x=>x.Value.NodeType == typeof(T)).Select(x=>x.Value).ToList();
        }

        public NodeData CurrentNode(){

            return CurrentRuntimeNode().NodeData;
        }

        public RuntimeNode MoveNext(){
           _runtimeNodeEnumerator.MoveNext();
           return _runtimeNodeEnumerator.Current;
        }

        public RuntimeNode CurrentRuntimeNode(){
            if (_runtimeNodeEnumerator.Current == null){
                _runtimeNodeEnumerator.MoveNext();
            }
            return _runtimeNodeEnumerator.Current;
        }
    }


}