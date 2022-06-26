using System.Collections.Generic;
using TNode.BaseViews;
using TNode.Cache;
using TNode.Models;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.Editor.BaseViews{
    /*
       public class DialogueGraphView : DataGraphView<DialogueGraph>{
        public Action<DialogueNodeView> onNodeAdded;
        public Action<DialogueNodeView> onNodeSelected;
        public Action<DialogueNodeView> onNodeRemoved;
        public Action<DialogueNodeView> onNodeUnselected;
        // public DialogueGraphView(DialogueGraph graph):base(){
        //     this.Data = graph;
        //
        //     //Set background to a bit of darker
        //     
        //     
        //     //Register a data context change callback
        //     
        // }

        public override void OnGraphViewCreate(){
            AddNode(GenerateEntryPoint());
            RegisterCallback<ContextualMenuPopulateEvent>(evt => {
                var pos = evt.mousePosition;
        
                evt.menu.AppendAction("Add Node", (dropMenuAction) => {
                    DialogueNodeView nodeView = new DialogueNodeView{
                        GUID = Guid.NewGuid().ToString(),
                        title = "New Node"
                    };
                    // make it a 200x100 box
                    nodeView.SetPosition(new Rect(pos.x - 100, pos.y - 50, 200, 100));
        
        
                    AddNode(nodeView);
                }, DropdownMenuAction.AlwaysEnabled);
            });
            this.OnDataChanged += OnOnDataChanged;
        }
        private void OnOnDataChanged(object sender, DataChangedEventArgs<DialogueGraph> e){
            //clean all nodes from the graphview
            foreach (var graphViewNode in nodes){
               RemoveElement(graphViewNode);
            }

            foreach (var edge in edges){
                RemoveElement(edge);
            }
            //add all nodes from the new graph
            foreach (var node in e.NewData.nodes){
                //AddNode(node);
            }
        }

        public void AddNode(DialogueNodeData dialogueNodeData){
            var res = InstantiateFromDialogueNodeData(dialogueNodeData);
            AddNode(res);
        }
        public void AddNode(DialogueNodeView nodeView){
            AddElement(nodeView);
            onNodeAdded?.Invoke(nodeView);
            //Register nodeView selection callback
            nodeView.RegisterCallback<MouseDownEvent>(evt => {
                if (evt.clickCount == 1){
                    onNodeSelected?.Invoke(nodeView);
                }
            });
            nodeView.OnUnselect += () => { onNodeUnselected?.Invoke(nodeView); };
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter) => this.ports.ToList()
            .Where(x => x != startPort &&
                        x.direction != startPort.direction).ToList();

        public DialogueNodeView GenerateEntryPoint(){
            var entryPoint = new DialogueNodeView{
                title = "Entry Point",
                GUID = Guid.NewGuid().ToString(),
                EntryPoint = true
            };
            //Add output port to the nodeView 
            entryPoint.AddPort(Orientation.Horizontal, Direction.Output, "Next");
            //Set nodeView position to top center side of screen
            entryPoint.SetPosition(new Rect(this.layout.width / 2 - 100, 0, 200, 200));
            return entryPoint;
        }
        protected DialogueNodeView InstantiateFromDialogueNodeData(DialogueNodeData dialogueNodeData){
            var node = new DialogueNodeView();
            node.title = dialogueNodeData.nodeName;
            node.GUID = Guid.NewGuid().ToString();
            //TODO:after completing the separation of the node data and the node editor data,this should be switch to the node editor data
            //node.SetPosition(dialogueNodeData.rect);
            this.AddNode(node);
            return node;
        }
    }
     */
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
            
            styleSheets.Add(Resources.Load<StyleSheet>("GraphViewBackground"));
            var grid = new GridBackground();
            Insert(0,grid);
            grid.StretchToParentSize();
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
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
        //rewrite function of the derived class in the comment  on the top of this script file in this class
        // public abstract override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter);
        //
        // public void AddNode(NodeData nodeData){
        //     
        // }
      
        
    }

    public class DataChangedEventArgs<T>{
        public DataChangedEventArgs(T data){
            NewData = data;
        }

        public T NewData{ get; private set; }
        
    }
}