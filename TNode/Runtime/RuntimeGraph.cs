using System.Collections;
using System.Collections.Generic;
using TNode.Models;
using UnityEngine;

namespace TNode.Runtime{
    public class RuntimeGraph:MonoBehaviour{
        public GraphData graphData;
        public SortedSet<RuntimeNode> _sortedSet;
        
        public void StartProcessNode(ProcessingStrategy strategy){

            
        }
        
    }

    public enum ProcessingStrategy{
        BreadthFirst,
        DepthFirst
    }
}