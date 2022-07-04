using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TNode.BaseViews;
using TNode.Cache;
using TNode.Editor.Inspector;
using TNode.Editor.Model;
using TNode.Models;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

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
        
                evt.menu.AppendAction("Add NodeAttribute", (dropMenuAction) => {
                    DialogueNodeView nodeView = new DialogueNodeView{
                        GUID = Guid.NewGuid().ToString(),
                        title = "New NodeAttribute"
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
    public  abstract  class DataGraphView<T>:GraphView,IDataGraphView where T:GraphData{
        private T _data;
        private bool _isInspectorOn;
        
        private NodeSearchWindowProvider _nodeSearchWindowProvider;
        private NodeInspector _nodeInspector;
        public GraphEditor<T> Owner;
        private Dictionary<string,Node> _nodeDict = new();
        
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
        public event DataChangedEventHandler OnDataChanged;
        public delegate void DataChangedEventHandler(object sender, DataChangedEventArgs<T> e);
     
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
        private void ConstructDefaultBehaviour(){
            //Register a right click context menu
            ConstructViewContextualMenu();
        }

        public void ConstructViewContextualMenu(){
            
            //Rebuild the contextual menu
            
            this.RegisterCallback<ContextualMenuPopulateEvent>(evt => {
                Vector2 editorPosition = Owner==null?Vector2.zero:Owner.position.position;
                //Remove all the previous menu items
                evt.menu.MenuItems().Clear();
                evt.menu.AppendAction("Create Node", dma => {
                    var dmaPos = dma.eventInfo.mousePosition+editorPosition;
                    SearchWindowContext searchWindowContext = new SearchWindowContext(dmaPos,200,200);
                    var searchWindow = ScriptableObject.CreateInstance<NodeSearchWindowProvider>();
                    searchWindow.Setup(typeof(T),this,Owner);
                    SearchWindow.Open(searchWindowContext, searchWindow);
                });
 
            });
           
        }

        private void OnInit(){
            ConstructDefaultBehaviour();
            OnGraphViewCreate();
        }
        public void ResetGraphView(){
            //Clear all nodes
            foreach (var node in nodes){
                RemoveElement(node);
            }
            foreach (var edge in edges){
                RemoveElement(edge);
            }
      
            if (_nodeDict == null) throw new ArgumentNullException(nameof(_nodeDict));
            foreach (var dataNode in _data.NodeDictionary.Values){
                if(dataNode==null)
                    continue;
                
                //Get the node type
                var nodeType = dataNode.GetType();
                //Get the derived type of NodeAttribute View from the node type
           
                var nodePos = Owner.graphEditorData.graphElementsData.
                    FirstOrDefault(x => x.guid == dataNode.id)?.pos??new Rect(0,0,200,200);
                
                AddTNode(dataNode,nodePos);
            }

            foreach (var edge in _data.nodeLinks){
                var inputNode = _data.NodeDictionary[edge.inPort.nodeDataId];
                var outputNode = _data.NodeDictionary[edge.outPort.nodeDataId];
                var inputNodeView = _nodeDict[inputNode.id];
                var outputNodeView = _nodeDict[outputNode.id];
                Edge newEdge = new Edge(){
                 
                    input = inputNodeView.inputContainer.Q<Port>(edge.inPort.portName),
                    output = outputNodeView.outputContainer.Q<Port>(edge.outPort.portName)
                };
                Debug.Log(edge.inPort.portName);
                Debug.Log(edge.outPort.portName);
                newEdge.input?.Connect(newEdge);
                newEdge.output?.Connect(newEdge);
                AddElement(newEdge);
            }
            _nodeDict.Clear();
        }
        //OnDataChanged event


        


        public virtual void CreateInspector(){
            NodeInspector nodeInspector = new NodeInspector();
            this.Add(nodeInspector);
            _nodeInspector = nodeInspector;
            _isInspectorOn = true;
        }

        public void CreateMiniMap(Rect rect){
            var miniMap = new MiniMap();
            this.Add(miniMap);
            miniMap.SetPosition(rect);
        }

        public void CreateBlackboard(){
            var blackboard = new Blackboard();
            //Blackboard add "Add Node" button
            // blackboard.Add(new BlackboardSection(){
            //     title = "Hello World",
            // });
            // blackboard.addItemRequested = (item) => {
            //     //Create a sub window for the blackboard to show the selection
            //     var subWindow = ScriptableObject.CreateInstance<NodeSearchWindowProvider>();
            // };
            //
            //Set black board to left side of the view
            blackboard.SetPosition(new Rect(0,0,200,600));
            Add(blackboard);
            //Check the type of the blackboard
            
            OnDataChanged+= (sender, e) => {
              
                if (_data.blackboardData==null||_data.blackboardData.GetType()==typeof(BlackboardData)){
                    _data.blackboardData = NodeEditorExtensions.GetAppropriateBlackboardData(_data.GetType());
          
                    if(_data.blackboardData==null) return;

                }  
                Debug.Log(_data.blackboardData);
                //Iterate field of the blackboard and add a button for each field
                foreach (var field in _data.blackboardData.GetType()
                             .GetFields(BindingFlags.Public|BindingFlags.NonPublic | BindingFlags.Instance)){
                    Debug.Log(field);
                    //if the field is MonoBehaviour,add a property field for blackboard
                    if(typeof(UnityEngine.Object).IsAssignableFrom(field.FieldType)){
                        var propertyField = new BlackboardField(null,field.Name,null){
                            
                        };
                        blackboard.Add(propertyField);
                    }
                    if(typeof(string).IsAssignableFrom(field.FieldType)){
                        var propertyField = new BlackboardField(null,field.Name,null){
                            
                        };
                        blackboard.Add(propertyField);
                    }
                }
            };

        }
        public virtual void DestroyInspector(){
            if(_nodeInspector!=null){
                this.Remove(_nodeInspector);
                _nodeInspector = null;
            }
            _isInspectorOn = false;
        }

        public virtual void SetInspector(NodeInspector nodeInspector){
            _nodeInspector = nodeInspector;
            if (!_isInspectorOn){
                _isInspectorOn = true;
            }
        }

        public void SaveEditorData(GraphEditorData graphEditorData){
            graphEditorData.graphElementsData?.Clear();
            //iterator nodes
            if (graphEditorData.graphElementsData == null){
                graphEditorData.graphElementsData = new List<GraphElementEditorData>();
            }
            foreach (var node in this.nodes){
                var nodeEditorData = new GraphElementEditorData{
                    pos = node.GetPosition(),
                };
                if (node is INodeView nodeView){
                    nodeEditorData.guid = nodeView.GetNodeData().id;
                }
                graphEditorData.graphElementsData.Add(nodeEditorData);
                EditorUtility.SetDirty(graphEditorData);
            }
        }
        
        public  void SaveWithEditorData(GraphEditorData graphEditorData){
            SaveEditorData(graphEditorData);
            SaveGraphData();
        }

        private void SaveNode(){
        
            foreach (var node in nodes){
                if (node is INodeView nodeView){
                    var nodeData = nodeView.GetNodeData();
                    if (!_data.NodeDictionary.ContainsKey(nodeData.id)){
                        _data.NodeDictionary.Add(nodeData.id, nodeData);
                    }
                }
            }
        }
        private void SaveEdge(){
            foreach (var edge in edges){
                var inputNode = edge.input.node as INodeView;
                var outputNode = edge.output.node as INodeView;
                var links = new List<NodeLink>();
                Debug.Log($"Edge{inputNode},{outputNode}");
                if (inputNode != null && outputNode != null){
                    var inputNodeData = inputNode.GetNodeData();
                    var outputNodeData = outputNode.GetNodeData();
                    var newNodeLink = new NodeLink(new PortInfo(){
                        nodeDataId = inputNodeData.id,
                        portName = edge.input.portName,

                    }, new PortInfo(){
                        nodeDataId = outputNodeData.id,
                        portName = edge.output.portName
                    });
                    links.Add(newNodeLink);
                }

                _data.nodeLinks = links;
                
            }
        }
        private void SaveGraphData(){
            _data.NodeDictionary.Clear();
  
            SaveNode();
            SaveEdge();
   
            EditorUtility.SetDirty(_data);
        }

   

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter){
            return ports.Where(x => x.portType == startPort.portType).ToList();
        }

        public virtual void OnGraphViewCreate(){
            
        }
        public virtual void OnGraphViewDestroy(){
            
        }
        ~DataGraphView(){
            OnGraphViewDestroy();
        }

        public void AddTNode(NodeData nodeData, Rect rect){
            if (NodeEditorExtensions.CreateNodeViewFromNodeType(nodeData.GetType()) is Node nodeView){
                nodeView.SetPosition(rect);
                AddElement(nodeView);
                //Add a select callback to the nodeView
                nodeView.RegisterCallback<MouseDownEvent>(evt => {
                    Debug.Log("NodeView Selected");
                    if (evt.clickCount == 1){
                        if (_isInspectorOn){
                            _nodeInspector.Data = nodeData;
                            _nodeInspector.NodeView = nodeView as INodeView;
                        }
                    }
                });
                
                if(nodeView is INodeView nodeViewInterface){
                    nodeViewInterface.SetNodeData(nodeData);
                }
                _nodeDict.Add(nodeData.id, nodeView);
                
                //register an callback ,when right click context menu
                nodeView.RegisterCallback<ContextClickEvent>(evt => {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () => {
                        RemoveElement(nodeView);
                        if (nodeView is INodeView tNodeView){
                            var nodeData1 = tNodeView.GetNodeData();
                            _data.NodeDictionary.Remove(nodeData1.id);
                            _nodeDict.Remove(nodeData1.id);
                            //Break all edges connected to this node
                            foreach (var edge in edges){
                                if (edge.input.node == nodeView || edge.output.node == nodeView){
                                    RemoveElement(edge);
                                }
                            }
                            Owner.graphEditorData.graphElementsData.RemoveAll(x => x.guid == nodeData1.id);
                        }
                    });
                    menu.ShowAsContext();
                });
                
                
                
   
                
            }
        }

        public void RemoveTNode(NodeData nodeData){
            throw new NotImplementedException();
        }
    }

    public interface IDataGraphView{
        public void AddTNode(NodeData nodeData, Rect rect);
        public void RemoveTNode(NodeData nodeData);
    }

    public class DataChangedEventArgs<T>{
        public DataChangedEventArgs(T data){
            NewData = data;
        }

        public T NewData{ get; private set; }
        
    }
}