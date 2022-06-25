using TNode.BaseViews;
using TNode.Models;

namespace TNode.Editor.BaseViews{
    public class NodeInspector:SimpleGraphSubWindow{
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
                RefreshInspector();
            }
        }

        private void RefreshInspector(){
            
        }
        
    }
}