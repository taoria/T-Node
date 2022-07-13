using TNode.Editor.NodeGraphView;
using TNode.Models;
using UnityEditor;
using UnityEngine;

namespace TNode.Editor.Blackboard{
    public interface IBlackboardView{
        public BlackboardData GetBlackboardData();
        public void SetBlackboardData(BlackboardData data);
        
        public void AddItem();

        void Setup(IBaseDataGraphView graphView,EditorWindow ownerWindow);
    }
    public interface IBlackboardView<T> : IBlackboardView where T : BlackboardData{
        
        public  T Data{ get; set; }
    }
}