using System;
using System.Collections.Generic;
using System.Linq;
using TNode.TNodeCore.Runtime.Models;
using TNodeCore.Runtime.Models;
using UnityEngine;

namespace TNodeCore.Runtime{
    public class ConditionalRuntimeNode:RuntimeNode{
        private readonly List<Tuple<string,Func<TransitionCondition>>> _possibleTransition;
        public ConditionalRuntimeNode(NodeData nodeData) : base(nodeData){
        
            if (nodeData is ConditionalNode conditionalNode){
                var transitionPort = GetPortsOfType<TransitionCondition>();
                _possibleTransition = new List<Tuple<string,Func<TransitionCondition>>>();
                var allOutput = GetPortsOfType<object>().Where(x => GetPortDirection(x) == Direction.Output);
                var enumerable = allOutput as string[] ?? allOutput.ToArray();
                if (enumerable.Count() != transitionPort.Length){
                    Debug.LogError($"Conditional node should only have output port with type of" +
                                   $" TransitionCondition found {transitionPort.Count()} output port with" +
                                   $" type of TransitionCondition but totally {enumerable.Count()} output port found");
                }
                foreach (var port in transitionPort){
                    if(GetPortDirection(port)==Direction.Input) continue;
                    _possibleTransition.Add(new Tuple<string, Func<TransitionCondition>>(port,() => (TransitionCondition)GetOutput(port)) );
                }
            }
            else{
                Debug.LogError("The Conditional runtime node must be applied on a Conditional node");
            }
        }

        public string[] GetConditionalNextIds(){
            var ports = _possibleTransition.Where(x => x.Item2().Condition);
            var portNames = ports.Select(x => x.Item1);
            //Search output links to found the link contains portNames as outport's name
            var outputLinks = OutputLinks.Where(x => portNames.Contains(x.outPort.portEntryName));
            return outputLinks.Select(x => x.inPort.nodeDataId).ToArray();
        }

        public string GetNextNodeId(){
            List<Tuple<string,TransitionCondition>> possibleCondition = _possibleTransition
                .Select(x=>new Tuple<string,TransitionCondition>(x.Item1,x.Item2()))
                .Where(x=>x.Item2.Condition).ToList();
             possibleCondition.Sort((a, b) => {
                 var compareTo = b.Item2.Priority.CompareTo(a.Item2.Priority);
                 return compareTo;
             });
             var portName = possibleCondition.FirstOrDefault()?.Item1;
             return OutputLinks.Where(x => x.outPort.portEntryName == portName).Select(x => x.inPort.nodeDataId).FirstOrDefault();
        }
        
    }


}