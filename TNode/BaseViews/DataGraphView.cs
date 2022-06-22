
using TNode.Tools;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.BaseViews{
    public  abstract  class DataGraphView<T>:GraphView where T:GraphData{
        private T _data;
        
        public T Data{
            get{ return _data; }
            set{
                _data = value;
                if(OnDataChanged != null){
                    OnDataChanged(this, new DataChangedEventArgs<T>(_data));
                }
                ResetGraphView();
            }
        }
     
        public void ResetGraphView(){
            //Clear all nodes
            foreach (var node in nodes){
                RemoveElement(node);
            }
            foreach (var edge in edges){
                RemoveElement(edge);
            }
            foreach (var dataNode in _data.nodes){
                if(dataNode==null)
                    continue;
                
                //Get the node type
                var nodeType = dataNode.GetType();
                //Get the derived type of Node View from the node type
                var nodeViewType = typeof(NodeView<>).MakeGenericType(nodeType);
                
                //Fetch the node view from the node view type
                var nodeView = NodeEditorExtensions.CreateInstance(nodeViewType);
                
                //Cast the node view to the nodeViewType
                AddElement((Node)nodeView);

            }
        }
        //A Constructor for the DataGraphView ,never to override it
        public DataGraphView(){
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            
            //Default Color
            style.backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1);
            // ReSharper disable once VirtualMemberCallInConstructor
            OnInit();
        }
        //OnDataChanged event
        public event DataChangedEventHandler OnDataChanged;
        public delegate void DataChangedEventHandler(object sender, DataChangedEventArgs<T> e);

        private void OnInit(){
            OnGraphViewCreate();
        }
        public virtual void OnGraphViewCreate(){
            
        }
        public virtual void OnGraphViewDestroy(){
            
        }
        ~DataGraphView(){
            OnGraphViewDestroy();
        }
    }

    public class DataChangedEventArgs<T>{
        public DataChangedEventArgs(T data){
            NewData = data;
        }

        public T NewData{ get; private set; }
        
    }
}