using System;
using System.Collections.Generic;
using System.Linq;
using TNode.TNodeCore.Runtime.Models;
using TNode.TNodeCore.Runtime.Tools;
using TNodeCore.Runtime.Models;

namespace TNodeCore.Runtime.RuntimeModels{
    public class StaticGraph:IRuntimeNodeGraph{
        private Dictionary<string,RuntimeNode> _nodes;
        private GraphTool _graphTool;
        private IEnumerator<RuntimeNode> BreathFirstEnumerator;

        private void ModifyLinks(NodeLink linkData){
            var outNodeId = linkData.outPort.nodeDataId;
            var outNode = _nodes[outNodeId];
            outNode.OutputLinks.Add(linkData);
            var inNodeId = linkData.inPort.nodeDataId;
            var inNode = _nodes[inNodeId];
            inNode.InputLinks.Add(linkData);
        }
        public StaticGraph(List<NodeData> nodes,List<NodeLink> links){
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
            BreathFirstEnumerator = _graphTool.BreathFirstSearch();
        }

        public void ResetState(){
            BreathFirstEnumerator = _graphTool.BreathFirstSearch();
        }

        public RuntimeNode GetRuntimeNode(NodeData nodeData){
            return _nodes[nodeData.id];
        }

        public RuntimeNode GetRuntimeNode(string id){
            return _nodes[id];
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
           BreathFirstEnumerator.MoveNext();
           return BreathFirstEnumerator.Current;
        }

        public RuntimeNode CurrentRuntimeNode(){
            if (BreathFirstEnumerator.Current == null){
                BreathFirstEnumerator.MoveNext();
            }
            return BreathFirstEnumerator.Current;
        }
    }


}