using System.Collections.Generic;
using TNodeCore.Models;
using UnityEngine;

namespace TNodeCore.Runtime{
    public class RuntimeGraph:MonoBehaviour{
        public GraphData graphData;
        public SortedSet<RuntimeNode> _sortedSet;
        [SerializeReference]
        public BlackboardData runtimeBlackboardData;
        
        public void OnValidate(){
            if(runtimeBlackboardData==null||runtimeBlackboardData.GetType()==typeof(BlackboardData)){
                runtimeBlackboardData = graphData?.blackboardData;
            }
        }
    }

    public enum ProcessingStrategy{
        BreadthFirst,
        DepthFirst
    }
}