using TNode.Models;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;

namespace TNode.Editor.BaseViews{
    
    //A NodeAttribute monitor some type of node in the graph
    
    public abstract class NodeView<T> : Node,INodeView where T:NodeData,new(){
        protected T _data;
        public T Data{
            get => _data;
            set{
                _data = value;
                OnDataChanged?.Invoke(value);
            }
        }
        public sealed override string title{
            get => base.title;
            set => base.title = value;
        }
        public event System.Action<T> OnDataChanged;

        protected NodeView(){
            OnDataChanged+=OnDataChangedHandler;
        }

        private void OnDataChangedHandler(T obj){
            this.title = _data.nodeName;
        }

        public void SetNodeData(NodeData nodeData){
            Data = (T)nodeData;
        }

        public NodeData GetNodeData(){
            return _data;
        }

        public void OnDataModified(){
           Refresh();
        }

        public void Refresh(){
            title = _data.nodeName;
            
        }

    }

    public interface INodeView{
        public void SetNodeData(NodeData nodeData);
        public NodeData GetNodeData();
        
        public void OnDataModified();

    }
}