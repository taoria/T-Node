﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TNode.Editor;
using TNode.Editor.Blackboard;
using TNode.Editor.EditorPersistence;
using TNode.Editor.Inspector;
using TNode.Editor.NodeGraphView;
using TNode.Editor.NodeViews;
using TNode.Editor.Search;
using TNode.Editor.Tools.NodeCreator;
using TNode.Models;
using TNodeGraphViewImpl.Editor.Cache;
using TNodeGraphViewImpl.Editor.GraphBlackboard;
using TNodeGraphViewImpl.Editor.GraphBlackboard.BlackboardProperty;
using TNodeGraphViewImpl.Editor.NodeViews;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace TNodeGraphViewImpl.Editor.NodeGraphView{
    public  abstract  class BaseDataGraphView<T>:GraphView,IDataGraphView<T> where T:GraphData{
        #region variables and properties
        private T _data;
        private bool _isInspectorOn;
        private NodeSearchWindowProvider _nodeSearchWindowProvider;
        private NodeInspector _nodeInspector;
        public GraphEditor<T> Owner;
        private Dictionary<string,Node> _nodeDict = new();
        private IBlackboardView _blackboard;
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
        #endregion
        #region event declarations
        public delegate void DataChangedEventHandler(object sender, DataChangedEventArgs<T> e);
        
        #endregion
     
        //A Constructor for the BaseDataGraphView ,never to override it


        #region construct default behaviour
        public BaseDataGraphView(){
            styleSheets.Add(Resources.Load<StyleSheet>("GraphViewBackground"));
            var grid = new GridBackground();
            Insert(0,grid);
            grid.StretchToParentSize();
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            RegisterDragEvent();
            OnInit();
        }
        private void ConstructDefaultBehaviour(){
            //Register a right click context menu
            ConstructViewContextualMenu();
        }

        public void ConstructViewContextualMenu(){
            RegisterCallback<ContextualMenuPopulateEvent>(evt => {
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
        public void RegisterDragEvent(){
            RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            RegisterCallback<DragPerformEvent>(OnDragPerform);
        }
        
        #endregion

        #region  event callbakc

        private void OnDragPerform(DragPerformEvent evt){
        
            if (DragAndDrop.GetGenericData("DragSelection") is List<ISelectable>{Count: > 0} data){
                var blackboardFields = data.OfType<BlackboardPropertyField >();
                foreach (var selectable in blackboardFields){
                    if(selectable is { } field) {
                        //Make a constructor of  BlackboardDragNodeData<field.PropertyType > by reflection
                        var specifiedType =
                            typeof(BlackboardDragNodeData<>).MakeGenericType(field.BlackboardProperty.PropertyType);
                        //Create a new instance of specified type
                        var dragNodeData = NodeCreator.InstantiateNodeData(specifiedType);
                        this.AddTNode(dragNodeData,new Rect(evt.mousePosition,new Vector2(200,200)));
                    }
                }
             
            }
        }

        private void OnDragUpdated(DragUpdatedEvent evt){
            Debug.Log(evt);
            
            //check if the drag data is BlackboardField

            if (DragAndDrop.GetGenericData("DragSelection") is List<ISelectable>{Count: > 0} data){
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
     
            }
            

        }

        #endregion



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

            foreach (var edge in _data.NodeLinks){
                var inputNode = _data.NodeDictionary[edge.inPort.nodeDataId];
                var outputNode = _data.NodeDictionary[edge.outPort.nodeDataId];
                var inputNodeView = _nodeDict[inputNode.id];
                var outputNodeView = _nodeDict[outputNode.id];
                Edge newEdge = new Edge(){
                 
                    input = inputNodeView.inputContainer.Q<Port>(edge.inPort.portName),
                    output = outputNodeView.outputContainer.Q<Port>(edge.outPort.portName)
                };

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

        public virtual void CreateMiniMap(Rect rect){
            var miniMap = new MiniMap();
            this.Add(miniMap);
            miniMap.SetPosition(rect);
        }


        private void BlackboardUpdate(){
            if (_data.blackboardData == null || _data.blackboardData.GetType()==(typeof(BlackboardData))){
                _data.blackboardData = NodeEditorExtensions.GetAppropriateBlackboardData(_data.GetType());

                if (_data.blackboardData == null) return;
                
            }
            _blackboard.SetBlackboardData(_data.blackboardData);
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
                if (node is IBaseNodeView nodeView){
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
                if (node is IBaseNodeView nodeView){
                    var nodeData = nodeView.GetNodeData();
                    if (!_data.NodeDictionary.ContainsKey(nodeData.id)){
                        _data.NodeDictionary.Add(nodeData.id, nodeData);
                    }
                }
            }
        }
        private void SaveEdge(){
            var links = new List<NodeLink>();
            foreach (var edge in edges){
                var inputNode = edge.input.node as IBaseNodeView;
                var outputNode = edge.output.node as IBaseNodeView;
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
                
            }
            
            _data.NodeLinks = links;
        }
        private void SaveGraphData(){
            _data.NodeDictionary.Clear();
            _data.NodeLinks.Clear();
            SaveNode();
            SaveEdge();
            SaveBlackboard();
            EditorUtility.SetDirty(_data);
        }

        private void SaveBlackboard(){
            if (_data.blackboardData == null){
                _data.blackboardData = NodeEditorExtensions.GetAppropriateBlackboardData(_data.GetType());
            }
        }


        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter){
            return ports.Where(x => x.portType == startPort.portType).ToList();
        }

        public virtual void OnGraphViewCreate(){
            
        }
        public virtual void OnGraphViewDestroy(){
            
        }
        ~BaseDataGraphView(){
            OnGraphViewDestroy();
        }

        #region implement interfaces
        public void AddTNode(NodeData nodeData, Rect rect){
            if (NodeEditorExtensions.CreateNodeViewFromNodeType(nodeData.GetType()) is Node nodeView){
                nodeView.SetPosition(rect);
                AddElement(nodeView);
                //Add a select callback to the nodeView
                nodeView.RegisterCallback<MouseDownEvent>(evt => {
                    if (evt.clickCount == 1){
                        if (_isInspectorOn){
                            _nodeInspector.Data = nodeData;
                            _nodeInspector.BaseNodeView = nodeView as IBaseNodeView;
                        }
                    }
                });
                if(nodeView is IBaseNodeView nodeViewInterface){
                    nodeViewInterface.SetNodeData(nodeData);
                }
                _nodeDict.Add(nodeData.id, nodeView);
                
                //register an callback ,when right click context menu
                nodeView.RegisterCallback<ContextClickEvent>(evt => {
                    var menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Delete"), false, () => {
                        RemoveElement(nodeView);
                        if (nodeView is IBaseNodeView tNodeView){
                            RemoveTNode(tNodeView.GetNodeData());
                        }
                    });
                    menu.ShowAsContext();
                });
                
            }
        }

        public void RemoveTNode(NodeData nodeData){
            
            _data.NodeDictionary.Remove(nodeData.id);
            var nodeView = _nodeDict[nodeData.id];
            _nodeDict.Remove(nodeData.id);
            //Break all edges connected to this node
            foreach (var edge in edges){
                if (edge.input.node == nodeView || edge.output.node == nodeView){
                    RemoveElement(edge);
                }
            }
            Owner.graphEditorData.graphElementsData.RemoveAll(x => x.guid == nodeData.id);
        }

        public void CreateBlackboard(){
            _blackboard = NodeEditorExtensions.CreateBlackboardWithGraphData(typeof(T));
            _blackboard.Setup(this,Owner);
      
            var castedBlackboard = _blackboard as Blackboard;
            
            Add(castedBlackboard);

            Rect blackboardPos = new Rect(0,0,300,700);
            castedBlackboard?.SetPosition(blackboardPos);
            
            
            OnDataChanged+= (sender, e) => { BlackboardUpdate(); };
        }

        public GraphData GetGraphData(){
            return _data;
        }


        public BlackboardData GetBlackboardData(){
            return this._data.blackboardData;
        }
        #endregion
    }


    public class DataChangedEventArgs<T>{
        public DataChangedEventArgs(T data){
            NewData = data;
        }

        public T NewData{ get; private set; }
        
    }
}