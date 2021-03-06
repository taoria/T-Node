using System;
using System.Linq;
using System.Reflection;
using TNode.TNodeGraphViewImpl.Editor.Inspector;
using TNode.TNodeGraphViewImpl.Editor.Ports;
using TNodeCore.Editor.NodeGraphView;
using TNodeCore.Editor.Serialization;
using TNodeCore.Runtime;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.Models;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.TNodeGraphViewImpl.Editor.NodeViews{
    
    public abstract class BaseNodeView<T> : Node,INodeView<T> where T:NodeData,new(){
        protected T _data;
        private readonly NodeInspectorInNode _nodeInspectorInNode;

        public IBaseDataGraphView BaseDataGraphView{
            get{
                var visualElement = this.GetFirstAncestorOfType<IBaseDataGraphView>();
                return visualElement;
            }
        }
        public T Data{
            get => _data;
            set{
                if(_data!=null)
                    ((NodeDataWrapper)_data).OnValueChanged -= OnDataValueChanged;
                _data = value;
                OnDataChanged?.Invoke(value);
                if(_data!=null)
                    ((NodeDataWrapper)_data).OnValueChanged += OnDataValueChanged;
   
            }
        }

        private void OnDataValueChanged(DataWrapper<NodeDataWrapper, NodeData> obj){
            Refresh();
            if (BaseDataGraphView.IsRuntimeGraph){
                BaseDataGraphView.NotifyRuntimeUpdate();
            }
        }
        public sealed override string title{
            get => base.title;
            set => base.title = value;
        }
        public event System.Action<T> OnDataChanged;

        protected BaseNodeView(){
            OnDataChanged+=OnDataChangedHandler;
            
            _nodeInspectorInNode = new NodeInspectorInNode(){
                name = "nodeInspectorInNode"
            };
            this.extensionContainer.Add(_nodeInspectorInNode);
            
            BuildDoubleClickRename();
        }
        private void OnDataChangedHandler(T obj){
            this.title = _data.nodeName;
            if (_nodeInspectorInNode != null){
                _nodeInspectorInNode.Data = obj;
            }
            BuildInputAndOutputPort();
            this.expanded = true;
            this.RefreshExpandedState();
        }

        protected virtual string BuildPortName(PortAttribute portAttribute,PropertyInfo propertyInfo,params object[] args){
            switch (portAttribute.NameHandling){
                case PortNameHandling.Auto:
                    return portAttribute.Name.Trim(' ').Length>0?portAttribute.Name:propertyInfo.Name;
                case PortNameHandling.Manual:
                    return portAttribute.Name;
                case PortNameHandling.MemberName:
                    return propertyInfo.Name;
                case PortNameHandling.Format:
                    return String.Format(propertyInfo.Name, args);
                case PortNameHandling.MemberType:
                    return propertyInfo.PropertyType.Name;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        protected virtual Type BuildPortType(PortAttribute portAttribute,PropertyInfo propertyInfo){
            switch (portAttribute.TypeHandling){
                case TypeHandling.Declared :
                    return propertyInfo.PropertyType;
                case TypeHandling.Implemented:
                    return propertyInfo.GetValue(_data)?.GetType();
                case TypeHandling.Specified:
                    return portAttribute.HandledType??typeof(object);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        /// <summary>
        /// of course you can override this method to build your own port builder
        /// </summary>
        protected virtual void BuildInputAndOutputPort(){
            var propertyInfos = _data.GetType().GetProperties();
   
            foreach (var propertyInfo in propertyInfos){
                if (propertyInfo.GetCustomAttributes(typeof(OutputAttribute),true).FirstOrDefault() is OutputAttribute attribute){

                    Port port = new CustomPort(Orientation.Horizontal, Direction.Output,
                        attribute.Multiple ? Port.Capacity.Multi : Port.Capacity.Single,
                        BuildPortType(attribute, propertyInfo));
                
                    this.outputContainer.Add(port);
                    var portName = ObjectNames.NicifyVariableName(BuildPortName(attribute,propertyInfo));
                    port.portName = portName;
                    port.name = propertyInfo.Name;
                    
                }
            }
            foreach (var propertyInfo in propertyInfos){
                if(propertyInfo.GetCustomAttributes(typeof(InputAttribute),true).FirstOrDefault() is InputAttribute attribute){
                    Port port = new CustomPort(Orientation.Horizontal, Direction.Input,attribute.Multiple?Port.Capacity.Multi:Port.Capacity.Single,BuildPortType(attribute,propertyInfo));
                    this.inputContainer.Add(port);
                    var portName = BuildPortName(attribute,propertyInfo);
                    port.portName = portName;
                    port.name = propertyInfo.Name;
                }
            }
        }

        public void StartARenameTitleTextField(){
            var textField = new TextField{
                value = title,
                style ={
                    //Make the text filed overlap the title container
                    position = Position.Absolute,
                    left = 0,
                    top = 0,
                    width = titleContainer.layout.width,
                    height = titleContainer.layout.height
                }
            };
            textField.StretchToParentSize();
            textField.RegisterValueChangedCallback(evt2 => {
                title = evt2.newValue;
            });
            textField.RegisterCallback<FocusOutEvent>(evt2 => {
                title = textField.text;
                ((NodeDataWrapper)_data).SetValue("nodeName",textField.text);
                textField.RemoveFromHierarchy();
            });
            //if enter is pressed ,set the title and remove the text field
            textField.RegisterCallback<KeyDownEvent>(evt2 => {
                if (evt2.keyCode == KeyCode.Return){
                    title = textField.text;
                    ((NodeDataWrapper)_data).SetValue("nodeName",textField.text);
                    textField.RemoveFromHierarchy();
                }
            });
                    
            titleContainer.Add(textField);
            textField.Focus();
        }
        private void BuildDoubleClickRename(){
            //when double click titleContainer ,create a textfield to rename the node
            titleContainer.RegisterCallback<MouseDownEvent>(evt => {
                if (evt.clickCount == 2){
                  StartARenameTitleTextField();
                }
            });
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

    public interface IBaseNodeView{
        public void SetNodeData(NodeData nodeData);
        public NodeData GetNodeData();
        public void OnDataModified();

        IBaseDataGraphView BaseDataGraphView{ get; }
    }

    public interface INodeView<T>:IBaseNodeView where T:NodeData,new(){
        public T Data{ get; set; }
        
    }
}