using TNodeCore.Editor.NodeGraphView;
using TNodeCore.Runtime.Models;
using UnityEditor;

namespace TNodeCore.Editor.Blackboard{
    /// <summary>
    /// View of the blackboard,different in each implementation,but the same in the interface.
    /// </summary>
    public interface IBlackboardView{
        /// <summary>
        /// Get the blackboard data model watched by this view.
        /// </summary>
        /// <returns></returns>
        BlackboardData GetBlackboardData();
        /// <summary>
        /// Set the blackboard data model watched by this view.
        /// </summary>
        /// <param name="data"></param>
        void SetBlackboardData(BlackboardData data);
        
        /// <summary>
        /// Add a new entry for the blackboard.currently no such use.
        /// </summary>
        
        void AddItem();
        /// <summary>
        /// Setup the blackboard view from the given Editor and graphview
        /// </summary>
        /// <param name="graphView"></param>
        /// <param name="ownerWindow"></param>
        void Setup(IBaseDataGraphView graphView,EditorWindow ownerWindow);
        
        /// <summary>
        /// Notify update the blackboard view's content by the watched blackboard data.
        /// </summary>
        void NotifyUpdate();
    }
    //A generic implementation of the blackboard view.
    public interface IBlackboardView<T> : IBlackboardView where T : BlackboardData{
        
        T Data{ get; set; }
    }
}