using TNode.Editor.Search;
using TNodeCore.Editor.Blackboard;
using TNodeCore.Editor.NodeGraphView;
using TNodeCore.Models;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TNodeGraphViewImpl.Editor.GraphBlackboard{
    /// <summary>
    /// Implement this class to create graph black board for specified graph
    /// </summary>
    public class GraphBlackboardView<T>:Blackboard,IBlackboardView<T> where T:BlackboardData{
        protected IBaseDataGraphView Owner;
        protected EditorWindow OwnerWindow;
        private T _data;

        public void Setup(IBaseDataGraphView graphView,EditorWindow ownerWindow){
            Owner = graphView;
            OwnerWindow = ownerWindow;
        }



        public new void SetPosition(Rect rect){
            
        }
        
        protected virtual void UpdateBlackboard(BlackboardData data){

        }
        public  T Data{
            get => (T) _data;

            set{
                _data = value;
                UpdateBlackboard(value);
            } 
        }
        public BlackboardData GetBlackboardData(){
            return _data;
        }

        public void SetBlackboardData(BlackboardData data){
            Data = (T) data;
        }

        public void AddItem(){
           
        }
    }
}