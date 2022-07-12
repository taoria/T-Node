using TNode.Models;
using UnityEditor.Experimental.GraphView;

namespace TNode.Editor.GraphBlackboard{
    /// <summary>
    /// Implement this class to create graph black board for specified graph
    /// </summary>
    public class GraphBlackboardView<T>:Blackboard where T:BlackboardData{
        public T BlackboardData;

        public GraphBlackboardView() : base(){
            
        }
    }
}