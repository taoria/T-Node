using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TNode.TNodeCore.Editor.Models;
using TNode.TNodeGraphViewImpl.Editor.Cache;
using TNode.TNodeGraphViewImpl.Editor.GraphBlackboard;
using TNode.TNodeGraphViewImpl.Editor.Inspector;
using TNode.TNodeGraphViewImpl.Editor.NodeViews;
using TNode.TNodeGraphViewImpl.Editor.Placemats;
using TNode.TNodeGraphViewImpl.Editor.Search;
using TNodeCore.Editor;
using TNodeCore.Editor.Blackboard;
using TNodeCore.Editor.EditorPersistence;
using TNodeCore.Editor.Models;
using TNodeCore.Editor.NodeGraphView;

using TNodeCore.Editor.Tools.NodeCreator;
using TNodeCore.Runtime.Components;
using TNodeCore.Runtime.Models;
using TNodeCore.Runtime.RuntimeCache;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using BlackboardField = TNode.TNodeGraphViewImpl.Editor.GraphBlackboard.BlackboardField;
using Edge = UnityEditor.Experimental.GraphView.Edge;

namespace TNode.TNodeGraphViewImpl.Editor.NodeGraphView{
    public   class BaseDataGraphView<T>:GraphView,IDataGraphView<T> where T:GraphData{
        #region const 
        public const float RefreshRate = 1f;
        #endregion
        #region variables and properties
        private T _data;
        private RuntimeGraph _runtimeGraph;
        private bool _isInspectorOn;
        private NodeSearchWindowProvider _nodeSearchWindowProvider;
        private NodeInspector _nodeInspector;
        private Dictionary<string,Node> _nodeDict = new Dictionary<string,Node>();
        private IBlackboardView _blackboard;
        private bool _loaded;
        private GraphViewData _graphViewData;
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

        public GraphEditor<T> Owner{ get; set; }
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


        /// <summary>
        /// Probably reusable in later GTFs version
        /// </summary>
        private void WaitingForAGraph(){
            VisualElement visualElement = new VisualElement();
            //Set background color to white
            visualElement.style.backgroundColor = new StyleColor(new Color(0.1f, 0.1f, 0.1f, 1));
            
            visualElement.StretchToParentSize();
            visualElement.name = "WaitingForAGraph";
            Add(visualElement);
            visualElement.BringToFront();
        
    
            //Add a label at the center of the created element
            Label label = new Label("drag a graph item here"){
                style ={
                    position = Position.Absolute
                },
                name = "HintLabel"
            };
            visualElement.RegisterCallback<DragPerformEvent>((evt) => {
                //check if the dragged object is a graph data or a Game Object contains a runtime graph
                var res = DragAndDrop.objectReferences;
                foreach (var obj in res){
                    if (obj is T graphData){
                        IsRuntimeGraph = false;
                        Data = graphData;
                    }
                    else{
                        if (obj is GameObject gameObject){
                            if (gameObject.GetComponent<RuntimeGraph>() != null){
                                if (gameObject.GetComponent<RuntimeGraph>().graphData != null){
                                    _runtimeGraph = gameObject.GetComponent<RuntimeGraph>();
                                    IsRuntimeGraph = true;
                                    BuildRuntimeGraphBehaviour();
                                    Data = gameObject.GetComponent<RuntimeGraph>().graphData as T;
                                    if(Data==null){
                                        Debug.LogError($"Dragged a wrong graph data to editor,expected {typeof(T)} but got {gameObject.GetComponent<RuntimeGraph>().graphData.GetType()}");
                                    }
                                }
                            }
                        }
                    }
                }
            });
            visualElement.RegisterCallback<DragUpdatedEvent>((evt) => {
                //check if the dragged object is a graph data or a Game Object contains a runtime graph
                var res = DragAndDrop.objectReferences;
                foreach (var obj in res){
                    if (obj is GraphData graphData){
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    }
                    else{
                        if (obj is GameObject gameObject){
                            if (gameObject.GetComponent<RuntimeGraph>() != null){
                                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                            }
                        }
                    }
                }

            });
            visualElement.Add(label);
            OnDataChanged += (sender, e) => {
                if (Data != null){
                    visualElement.RemoveFromHierarchy();
                }
                CreateMenu();
            };
        }

        private void BuildRuntimeGraphBehaviour(){
            //EditorApplication.update+= UpdateRuntimeGraphBehaviour;
            UpdateRuntimeGraphBehaviourInTime();
            
        }

        private async void UpdateRuntimeGraphBehaviourInTime(){
            
            while (_loaded){
                await Task.Delay(TimeSpan.FromSeconds(RefreshRate));
                if(_runtimeGraph != null){
                    if (AutoUpdate){
                        _runtimeGraph.TraverseAll();
                        AfterGraphResolved?.Invoke();
                    }
                }
            }
        }
 
        private void CheckDataAfterInit(){
            if(Data == null){
                WaitingForAGraph();
            }
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
                    var targetPos = this.viewTransform.matrix.inverse.MultiplyPoint(dma.eventInfo.localMousePosition);
                    searchWindow.Setup(typeof(T),this,Owner,targetPos);
             
                    Debug.Log(targetPos);
                    SearchWindow.Open(searchWindowContext, searchWindow);
                });
                evt.menu.AppendAction("Create PlacematData",dma=> {
                    //find placemat container 
                    var placematContainer = GetPlacematContainer();
                    var targetPos = this.viewTransform.matrix.inverse.MultiplyPoint(dma.eventInfo.localMousePosition);
                    var dmaPosRect = new Rect(targetPos,new Vector2(500,500));
                    var placemat = placematContainer.CreatePlacemat<PlacematView>(dmaPosRect,1,"Title");
                    var placematData = new PlacematData{
                        title = "Title",
                        positionInView = dmaPosRect
                    };
                    placemat.PlacematData = placematData;
                    AddPlacemat(placematData);

                });
            });
        }

        private void AddPlacemat(PlacematData data){
            _data.EditorModels.Add(data);
        }

        private PlacematContainer GetPlacematContainer(){
            if (this.placematContainer == null){
                PlacematContainer container = new PlacematContainer(this){
                    name = "PlacematContainer"
                };
                this.Add(container);
            }
            return this.placematContainer;
        }

        private void OnInit(){
            ConstructDefaultBehaviour();
            
            CheckDataAfterInit();
            
            OnGraphViewCreate();

            BuildUndo();

            _loaded = true;

            SetDetachedFromPanel();
            
        }

        private void SetDetachedFromPanel(){
            this.RegisterCallback<DetachFromPanelEvent>(evt => {
                _loaded = false;
            });
        }

        private void BuildUndo(){
            Undo.undoRedoPerformed+=UndoRedoPerformed;
        }

        private void UndoRedoPerformed(){
            ResetGraphView();
        }

        public virtual void AfterEditorLoadGraphView(){
            
        }
        
        protected void CreateMenu(){
            if (this.Q("TopMenu") != null) return;
            var visualElement = new VisualElement{
                name = "TopMenu"
            };
            visualElement.style.position = Position.Absolute;
            visualElement.style.top = 0;
            visualElement.style.backgroundColor = new StyleColor(new Color(0.1f, 0.1f, 0.1f, 1));
            Add(visualElement);
            visualElement.style.flexDirection = FlexDirection.Row;
            
            
            //Add a toggle button to toggle test mode
            var autoUpdateToggle = new Toggle{
                name = "TestModeToggle",
                label = "Test Mode",
                value = AutoUpdate
            };
            autoUpdateToggle.RegisterValueChangedCallback(evt => {
                if (evt.newValue){
                    AutoUpdate = true;
                }
                else{
                    AutoUpdate = false;
                }
            });
            visualElement.Add(autoUpdateToggle);
            
            var runButton = new Button{
                name = "RunButton",
                text = "Run Once"
            };
            runButton.RegisterCallback<ClickEvent>(evt => {
                if (IsRuntimeGraph){
                    _runtimeGraph.TraverseAll();
                    AfterGraphResolved?.Invoke();
                }
            });
            visualElement.Add(runButton);
            
            var blackboardButton = new Button{
                name = "blackboardButton",
                text = "Blackboard"
            };
            blackboardButton.RegisterCallback<ClickEvent>(evt => {
                if(_blackboard==null)
                    CreateBlackboard();
            });
            visualElement.Add(blackboardButton);
        }
        
        public void RegisterDragEvent(){
            RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            RegisterCallback<DragPerformEvent>(OnDragPerform);
        }
        
        #endregion

        #region  event callbakc

        private void OnDragPerform(DragPerformEvent evt){
            var realPos = viewTransform.matrix.inverse.MultiplyPoint(evt.localMousePosition);
        
            if (DragAndDrop.GetGenericData("DragSelection") is List<ISelectable> data){
                if (data.Count == 0){
                    return;
                }
                var blackboardFields = data.OfType<BlackboardField >();
                foreach (var selectable in blackboardFields){
                    if(selectable is { } field) {
                        //Make a constructor of  BlackboardDragNodeData<field.PropertyType > by reflection
                        var dragNodeData = NodeCreator.InstantiateNodeData<BlackboardDragNodeData>();
                        dragNodeData.BlackboardData = GetBlackboardData();
                        dragNodeData.BlackDragData = field.BlackboardProperty.PropertyName;
                        AddTNode(dragNodeData,new Rect(realPos,new Vector2(200,200)));
                    }
                }

                var blackboardEntries = data.OfType<BlackboardDataEntry>();
                foreach (var selectable in blackboardEntries){
                    if(selectable is { } entry) {
                        //Make a constructor of  BlackboardDragNodeData<field.PropertyType > by reflection
                        var dragNodeData = NodeCreator.InstantiateNodeData<BlackboardDragNodeData>();
                        dragNodeData.BlackboardData = GetBlackboardData();
                        dragNodeData.BlackDragData = entry.propertyPath;
                        AddTNode(dragNodeData,new Rect(realPos,new Vector2(200,200)));
                    }
                }

            }
        }

        private void OnDragUpdated(DragUpdatedEvent evt){
            //check if the drag data is BlackboardField
            if (DragAndDrop.GetGenericData("DragSelection") is List<ISelectable> data){
                if (data.Count <= 0) return;
                DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                //high light the 
            }
          
        }
        #endregion

        public void ClearAll(){
            foreach (var node in nodes.ToList()){
                RemoveElement(node);
            }
            foreach (var edge in edges.ToList()){
                RemoveElement(edge);
            }
            foreach (var placemat in placematContainer.Placemats.ToList()){
                RemoveElement(placemat);
            }
        }
        public void ResetGraphView(){
            ClearAll();

            LoadPersistentGraphViewData();
            
            
            if (_nodeDict == null) throw new ArgumentNullException(nameof(_nodeDict));
            
            foreach (var dataNode in _data.NodeDictionary.Values){
                if(dataNode==null)
                    continue;
                //Get the derived type of NodeAttribute View from the node type
                if (dataNode is SceneNodeData runtimeNodeData){
                    if (runtimeNodeData is  BlackboardDragNodeData){
                        runtimeNodeData.BlackboardData = GetBlackboardData();
                        AddPersistentNode(runtimeNodeData);
                    }
                    else{
            
                        var node = _runtimeGraph.Get(runtimeNodeData.id).NodeData as SceneNodeData;
                        AddPersistentNode(node);
                    }
                }
                else{
                    AddPersistentNode(dataNode);
                }
  
            }
            foreach (var edge in _data.NodeLinks){
                var inputNode = _data.NodeDictionary[edge.inPort.nodeDataId];
                var outputNode = _data.NodeDictionary[edge.outPort.nodeDataId];
                var inputNodeView = _nodeDict[inputNode.id];
                var outputNodeView = _nodeDict[outputNode.id];
                Edge newEdge = new Edge(){
                 
                    input = inputNodeView.inputContainer.Q<Port>(edge.inPort.portEntryName),
                    output = outputNodeView.outputContainer.Q<Port>(edge.outPort.portEntryName)
                };
                newEdge.input?.Connect(newEdge);
                newEdge.output?.Connect(newEdge);
                AddElement(newEdge);
            }
            var placemats = _data.EditorModels.OfType<PlacematData>();
            foreach (var placemat in placemats){
                var container = GetPlacematContainer();
                var res = container.CreatePlacemat<PlacematView>(placemat.positionInView, 0, placemat.title);
                res.PlacematData = placemat;

            }
            _nodeDict.Clear();
        }

        private void LoadPersistentGraphViewData(){
            var r= _data.GraphViewData;
            this.viewTransformChanged-=OnViewTransformChanged;
            this.UpdateViewTransform(r.persistOffset,new Vector3(r.persistScale,r.persistScale,1));
            this.viewTransformChanged+=OnViewTransformChanged;

        }

        private void OnViewTransformChanged(GraphView graphview){
            if (_data == null) return;
            _data.GraphViewData.persistOffset = graphview.viewTransform.position;
            _data.GraphViewData.persistScale = graphview.viewTransform.scale.x;
        }

        private void AddPersistentNode(NodeData dataNode){
            var nodePos = dataNode.positionInView;
            AddTNode(dataNode, nodePos);
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
            Add(miniMap);
            miniMap.SetPosition(rect);
        }


        private void UpdateBlackboardData(){
            if (_data == null) return;
   
            if (_data.blackboardData == null || _data.blackboardData.GetType()==(typeof(BlackboardData))){
      
                _data.blackboardData = NodeEditorExtensions.GetAppropriateBlackboardData(_data.GetType());
                Debug.Log(_data.blackboardData);
                if (_data.blackboardData == null) return;
            }
            _blackboard.SetBlackboardData(GetBlackboardData());
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
            //To do something later
        }
        
        public  void SaveWithEditorData(GraphEditorData graphEditorData){
            SaveEditorData(graphEditorData);
            SaveGraphData();
        }

        private void SaveNode(){
        
            foreach (var node in nodes.ToList()){
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
            foreach (var edge in edges.ToList()){
                var inputNode = edge.input.node as IBaseNodeView;
                var outputNode = edge.output.node as IBaseNodeView;
                if (inputNode != null && outputNode != null){
                    var inputNodeData = inputNode.GetNodeData();
                    var outputNodeData = outputNode.GetNodeData();
                    var newNodeLink = new NodeLink(new PortInfo(){
                        nodeDataId = inputNodeData.id,
                        portEntryName = edge.input.name,

                    }, new PortInfo(){
                        nodeDataId = outputNodeData.id,
                        portEntryName = edge.output.name
                    });
                    links.Add(newNodeLink);
                }
                
            }
            
            _data.NodeLinks = links;
        }
        private void SaveGraphData(){
            _data.NodeDictionary.Clear();
            _data.NodeLinks.Clear();
            _data.EditorModels.Clear();
            SaveNode();
            SaveEdge();
            SaveBlackboard();
            SaveEditorModels();
            EditorUtility.SetDirty(_data);
        }

        private void SaveEditorModels(){
            var placemats = placematContainer.Placemats.ToList();
            Debug.Log(placemats.Count);
            foreach (var placemat in placemats){
                if (placemat is PlacematView placematView){
                    _data.EditorModels.Add(placematView.PlacematData);
                }
            }
        }

        private void SaveBlackboard(){
            if (_data.blackboardData == null){
                _data.blackboardData = NodeEditorExtensions.GetAppropriateBlackboardData(_data.GetType());
            }
        }
        //TODO:Handling implicit conversion when two port types are different but compatible
        private static bool HasImplicitConversion(Type baseType, Type targetType)
        {
            return baseType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(mi => mi.Name == "op_Implicit" && mi.ReturnType == targetType)
                .Any(mi => {
                    ParameterInfo pi = mi.GetParameters().FirstOrDefault();
                    return pi != null && pi.ParameterType == baseType;
                });
        }
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter){
            
         
            var compatiblePorts = ports.ToList().Where(x => startPort != x &&
                                                   (x.portType == startPort.portType ||
                                                    x.portType.IsAssignableFrom(startPort.portType)
                                                   )).ToList();
            if(startPort.direction==Direction.Input){
                //Search output to find ports with type that have implicit conversion or define converter that convert to type of the startPort
                var outputPorts = ports.ToList().Where(x => x.direction == Direction.Output).ToList();
                foreach (var outputPort in outputPorts){
                    //Want a port type that can convert to to the type of the startPort
                    if (HasImplicitConversion(outputPort.portType,startPort.portType)){
                        compatiblePorts.Add(outputPort);
                    }
                    if (RuntimeCache.Instance.GetSupportedTypes(outputPort.portType).Contains(startPort.portType)){
                        compatiblePorts.Add(outputPort);
                    }
                }
            }
            else{
                var inputPorts = ports.ToList().Where(x => x.direction == Direction.Input).ToList();
                
                foreach (var inputPort in inputPorts){
                    //check if start port could implicitly convert to input port type
                    if (HasImplicitConversion(startPort.portType,inputPort.portType)){
                        compatiblePorts.Add(inputPort);
                    }
                    //Check if input port type is supported by output port type
                    if (RuntimeCache.Instance.GetSupportedTypes(startPort.portType).Contains(inputPort.portType)){
                        compatiblePorts.Add(inputPort);
                    }
                }
            }
            
            
   
            
            
            
            return compatiblePorts;


        }

        public virtual void OnGraphViewCreate(){
            
        }
        public virtual void OnGraphViewDestroy(){
            
        }
        ~BaseDataGraphView(){
            OnGraphViewDestroy();
        }

        #region implement interfaces
        /// <summary>
        /// Simultaneously add the node to the graph data and add the node to the graph view
        /// </summary>
        /// <param name="nodeData"></param>
        /// <param name="rect">The give rect is the actual position in the graph view</param>
        public void AddTNode(NodeData nodeData, Rect rect){
            if (NodeEditorExtensions.CreateNodeViewFromNodeType(nodeData.GetType()) is Node nodeView){
                //convert rect at graph space
        
                rect.position = rect.position;
                if(nodeView is IBaseNodeView nodeViewInterface){
                    nodeViewInterface.SetNodeData(nodeData);
                }
                AddElement(nodeView);
                ((IBaseNodeView)nodeView).InitializePosition(rect);
                //Add a select callback to the nodeView
                nodeView.RegisterCallback<MouseDownEvent>(evt => {
                    if (evt.clickCount == 1){
                        if (_isInspectorOn){
                            _nodeInspector.Data = nodeData;
                            _nodeInspector.BaseNodeView = nodeView as IBaseNodeView;
                        }
                    }
                });
                
                

                if(_nodeDict.ContainsKey(nodeData.id)==false) 
                    _nodeDict.Add(nodeData.id, nodeView);
                if (_data.NodeDictionary.ContainsKey(nodeData.id) == false){
                    Undo.RegisterCompleteObjectUndo(_data,"Node Creation");
                    _data.NodeDictionary.Add(nodeData.id,nodeData);
                }

                
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
        /// <summary>
        /// Remove node from 
        /// </summary>
        /// <param name="nodeData"></param>

        public void RemoveTNode(NodeData nodeData){
            Undo.RegisterCompleteObjectUndo(_data,"Node deletion");
            _data.NodeDictionary.Remove(nodeData.id);
            var nodeView = _nodeDict[nodeData.id];
            _nodeDict.Remove(nodeData.id);
            //Break all edges connected to this node
            foreach (var edge in edges.ToList()){
                if (edge.input.node == nodeView || edge.output.node == nodeView){
                    RemoveElement(edge);
                }
            }
            //TODO: rect x,y move to node region
            Owner.graphEditorData.graphElementsData.RemoveAll(x => x.guid == nodeData.id);
        }

        public void AddLink(NodeLink nodeLink){
            Undo.RegisterCompleteObjectUndo(_data,"node linked");
           _data.NodeLinks.Add(nodeLink);
        }

        public void RemoveLink(NodeLink nodeLink){
            Undo.RegisterCompleteObjectUndo(_data,"node unlinked");
            _data.NodeLinks.Remove(nodeLink);
        }

        public bool AutoUpdate{
            get=>Owner.graphEditorData.autoUpdate; set=>Owner.graphEditorData.autoUpdate = value;
        }
        
        
        public override EventPropagation DeleteSelection(){
            Undo.RegisterCompleteObjectUndo(_data,"Delete Selection");
            var res = base.DeleteSelection();
            SaveGraphData();
            ResetGraphView();
            return res;
        }

        public void CreateBlackboard(){
            _blackboard = NodeEditorExtensions.CreateBlackboardWithGraphData(typeof(T)) ;
            _blackboard.Setup(this,Owner);
            var castedBlackboard = _blackboard as Blackboard;
            Add(castedBlackboard);
            Rect blackboardPos = new Rect(0,0,300,700);
            castedBlackboard?.SetPosition(blackboardPos);
            UpdateBlackboardData();
            OnDataChanged+= (sender, e) => { UpdateBlackboardData(); };
            if(_data.blackboardData!=null){
                _data.GraphViewData.isBlackboardOn = true;
            }
        }

        public GraphData GetGraphData(){
            return _data;
        }


        public BlackboardData GetBlackboardData(){
            if (IsRuntimeGraph){
                return _runtimeGraph.runtimeBlackboardData;
            }
            return _data.blackboardData;
        }

        public bool IsRuntimeGraph{ get; set; }

        public RuntimeGraph GetRuntimeGraph(){
            return _runtimeGraph;
        }

        public void SetGraphData(GraphData graph){
            Data = graph as T;
        }

        public void NotifyRuntimeUpdate(){
            
        }
        
        public Action AfterGraphResolved{ get; set; }

        #endregion
    }


    public class DataChangedEventArgs<T>{
        public DataChangedEventArgs(T data){
            NewData = data;
        }

        public T NewData{ get; private set; }
        
    }
}