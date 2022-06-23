using TNode.Models;
using UnityEditor.Experimental.GraphView;

namespace TNode.BaseViews{
    public class NodeInspector<T>:SimpleGraphSubWindow{
        private NodeData _data;

        public NodeData Data{
            get => _data;
            set{
                _data = value;
                UpdateData();
            }
        }

        private void UpdateData(){
            if (_data != null){
                
            }
        }
    }
}