using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TNodeCore.Runtime;
using TNodeCore.Runtime.Components;
using UnityEngine;

namespace TNode.TNodeCore.Runtime.Components{
    public class ConditionalGraph:RuntimeGraph{
        public ConditionalRuntimeNode EntryNode;
        public ConditionalRuntimeNode CurrentNode{ get; set; }
        public override void Build(){
            base.Build();
            var entry = GetRuntimeNodesOfType<EntryNode>();
            if (entry.Count > 1){
                Debug.LogError("There should be only one entry node in a conditional graph");
            }
            EntryNode = entry.FirstOrDefault() as ConditionalRuntimeNode;
        }
        public void Run(){
            var res = StepForward();
            while (StepForward().MoveNext()){
                
            }
            

        }
        
        public IEnumerator StepForward(){
            CurrentNode = EntryNode;
            while (CurrentNode != null){
                //First let's process the node
                CurrentNode.NodeData.Process();
                //Then check if there are conditional transitions
                var conditionalTransitions = CurrentNode.GetConditionalNextIds();
                var transitionNode = new List<RuntimeNode>();
                
                foreach (var conditionalTransition in conditionalTransitions){
                    transitionNode.Add(GetRuntimeNode(conditionalTransition));
                }
                foreach (var runtimeNode in transitionNode){
                    if (runtimeNode is ConditionalRuntimeNode == false){
                        runtimeNode.Process();
                    }
                }
                CurrentNode = transitionNode.FirstOrDefault(x => x is ConditionalRuntimeNode) as ConditionalRuntimeNode;
                yield return CurrentNode;
            }
        }
    }
}