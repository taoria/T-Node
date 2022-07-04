using System.Collections;
using System.Collections.Generic;
using TNode.Models;
using UnityEngine;

namespace TNode.Runtime{
    public class RuntimeGraph:MonoBehaviour{
        public GraphData graphData;
        public SortedSet<RuntimeNode> _sortedSet;

        public void Start(){
            //iterate through all nodes and add them to the sorted set
            foreach (var node in graphData.NodeDictionary.Values){
                
            }
        }
        
        public void StartProcessNode(ProcessingStrategy strategy, RuntimeNode entry){
            
        }
        
    }

    public enum ProcessingStrategy{
        BreadthFirst,
        DepthFirst
    }
}