using Dialogue;
using TNode.Models;
using UnityEditor.Experimental.GraphView;

namespace TNode.BaseViews{
    
    //A Node monitor some type of node in the graph
    
    public abstract class NodeView<T> : Node where T:NodeData,new(){
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
        
        public NodeView(){
            
        }
    }
}