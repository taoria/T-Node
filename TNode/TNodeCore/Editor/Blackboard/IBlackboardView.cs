using TNodeCore.Editor.NodeGraphView;
using TNodeCore.Runtime.Models;
using UnityEditor;

namespace TNodeCore.Editor.Blackboard{
    public interface IBlackboardView{
        BlackboardData GetBlackboardData();
        void SetBlackboardData(BlackboardData data);
        
        void AddItem();

        void Setup(IBaseDataGraphView graphView,EditorWindow ownerWindow);
        void NotifyUpdate();
    }
    public interface IBlackboardView<T> : IBlackboardView where T : BlackboardData{
        
        T Data{ get; set; }
    }
}