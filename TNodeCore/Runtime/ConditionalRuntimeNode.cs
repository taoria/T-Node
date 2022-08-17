using System;
using System.Collections.Generic;
using System.Linq;
using TNode.TNodeCore.Runtime.Models;
using TNodeCore.Runtime.Models;
using UnityEngine;

namespace TNodeCore.Runtime{
    public class ConditionalRuntimeNode:RuntimeNode{
        private readonly List<Tuple<string,Func<bool>>> PossibleTransition;
        public ConditionalRuntimeNode(NodeData nodeData) : base(nodeData){
            if (nodeData is ConditionalNode conditionalNode){
                var transitionPort = GetPortsOfType<bool>();
                PossibleTransition = new List<Tuple<string,Func<bool>>>();
                foreach (var port in transitionPort){
                    if(GetPortDirection(port)==Direction.Input) continue;
                    PossibleTransition.Add(new Tuple<string, Func<bool>>(port,() => (bool)GetOutput(port)) );
                }
            }
            else{
                Debug.LogError("The Conditional runtime node must be applied on a Conditional node");
            }
        }

        public string[] GetConditionalNextIds(){
            var ports = PossibleTransition.Where(x => x.Item2());
            var portNames = ports.Select(x => x.Item1);
            //Search output links to found the link contains portNames as outport's name
            var outputLinks = OutputLink.Where(x => portNames.Contains(x.outPort.portEntryName));
            return outputLinks.Select(x => x.inPort.nodeDataId).ToArray();
        }
        
    }
}