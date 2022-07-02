using System;
using System.Collections.Generic;
using System.Linq;
using TNode.BaseViews;
using TNode.Cache;
using TNode.Editor.Inspector;
using TNode.Editor.Model;
using TNode.Models;
using UnityEditor;
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
        private SearchWindowProvider _searchWindowProvider;
        private bool _isInspectorOn;
        private NodeInspector _nodeInspector;
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
            Dictionary<string,Node> nodeDict = new Dictionary<string, Node>();
            if (nodeDict == null) throw new ArgumentNullException(nameof(nodeDict));
            foreach (var dataNode in _data.NodeDictionary.Values){
                if(dataNode==null)
                    continue;
                
                //Get the node type
                var nodeType = dataNode.GetType();
                //Get the derived type of NodeAttribute View from the node type
                var nodeViewType = typeof(NodeView<>).MakeGenericType(nodeType);
                
                //Fetch the node view from the node view type
                var nodeView = NodeEditorExtensions.CreateInstance(nodeViewType);
                
                //Cast the node view to the nodeViewType
                AddElement((Node)nodeView);
                
                ((INodeView)nodeView).SetNodeData(dataNode);
                
                //Add the node view to the node dictionary
                nodeDict.Add(dataNode.id, (Node)nodeView);
            }

            foreach (var edge in _data.NodeLinks){
                var inputNode = _data.NodeDictionary[edge.inPort.nodeDataId];
                var outputNode = _data.NodeDictionary[edge.outPort.nodeDataId];
                var inputNodeView = nodeDict[inputNode.id];
                var outputNodeView = nodeDict[outputNode.id];
                Edge newEdge = new Edge(){
                    input = inputNodeView.inputContainer.Q<Port>(edge.inPort.portName),
                    output = outputNodeView.outputContainer.Q<Port>(edge.outPort.portName)
                };
                newEdge.input?.Connect(newEdge);
                newEdge.output?.Connect(newEdge);
                AddElement(newEdge);
            }
            nodeDict.Clear();
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

        
        private void ConstructDefaultBehaviour(){
            //Register a right click context menu
            //ConstructContextualMenuOption();
        }

        public void ConstructViewContextualMenu(EventCallback<ContextualMenuPopulateEvent> callback){
            RegisterCallback<ContextualMenuPopulateEvent>(callback);
        }

        private void OnInit(){
            ConstructDefaultBehaviour();
            OnGraphViewCreate();
        }

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

        public void CreateBlackBoard(){
            var blackboard = new Blackboard();
            //Set black board to left side of the view
            blackboard.SetPosition(new Rect(0,0,200,600));
            this.Add(blackboard);
            
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
            graphEditorData.nodesData.Clear();
            //iterator nodes
       
            foreach (var node in this.nodes){
                var nodeEditorData = new NodeEditorData{
                    nodePos = node.GetPosition(),
                };
                if (node is INodeView nodeView){
                    nodeEditorData.nodeGuid = nodeView.GetNodeData().id;
                }
                graphEditorData.nodesData.Add(nodeEditorData);
                EditorUtility.SetDirty(graphEditorData);
            }
        }
        public void LoadEditorData(GraphEditorData graphEditorData){
            //Load node position
            foreach (var nodeEditorData in graphEditorData.nodesData){

                var node = this.nodes.Select(x => x as INodeView).First(x=>x?.GetNodeData().id==nodeEditorData.nodeGuid);
                
                if (node != null){
                    ((GraphElement)node).SetPosition(nodeEditorData.nodePos);
                }
            }
        }


        public  void SaveWithEditorData(GraphEditorData graphEditorData){
            SaveEditorData(graphEditorData);
            SaveGraphData();
        }

        private void SaveGraphData(){
            foreach (var node in nodes){
                if (node is INodeView nodeView){
                    var nodeData = nodeView.GetNodeData();
                    if (!_data.NodeDictionary.ContainsKey(nodeData.id)){
                        _data.NodeDictionary.Add(nodeData.id, nodeData);
                    }
                }
            }
            //force edge to write as links
            foreach (var edge in edges){
                var inputNode = edge.input.node as INodeView;
                var outputNode = edge.output.node as INodeView;
                var links = new List<NodeLink>();
                if (inputNode != null && outputNode != null){
                    var inputNodeData = inputNode.GetNodeData();
                    var outputNodeData = outputNode.GetNodeData();
                    var newNodeLink = new NodeLink(new PortInfo(){
                        nodeDataId = inputNodeData.id,
                        portName = edge.input.name

                    }, new PortInfo(){
                        nodeDataId = outputNodeData.id,
                        portName = edge.output.name
                    });
                }

                _data.NodeLinks = links;
                
            }
            EditorUtility.SetDirty(_data);
            AssetDatabase.SaveAssets();
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
            if (NodeEditorExtensions.CreateNodeViewFromNodeType(nodeData.GetType()) is GraphElement nodeView){
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