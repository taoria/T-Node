using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TNodeCore.Editor.NodeGraphView;
using TNodeCore.Editor.Serialization;
using TNodeCore.Runtime;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.Models;
using TNodeCore.Runtime.RuntimeCache;
using TNodeGraphViewImpl.Editor.Inspector;
using TNodeGraphViewImpl.Editor.Ports;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Direction = UnityEditor.Experimental.GraphView.Direction;

namespace TNodeGraphViewImpl.Editor.NodeViews{
    
    public abstract class BaseNodeView<T> : Node,INodeView<T> where T:NodeData,new(){
        protected T _data;
        private readonly NodeInspectorInNode _nodeInspectorInNode;
        private NodeViewLogger _viewLogger;

        private class NodeViewLogger:INodeLoggerImpl{
            public BaseNodeView<T> NodeView { get; set; }
            public void Log(string message){
                var loggerAreaParent = NodeView.extensionContainer;
                if (loggerAreaParent == null){
                    return;
                }
                var loggerArea = loggerAreaParent.Q<TextField>("loggerArea");
                if(loggerArea == null){
                    loggerArea = new TextField();
                    loggerArea.name = "loggerArea";
                    loggerArea.AddToClassList("loggerArea");
                    loggerAreaParent.Add(loggerArea);
                }

                loggerArea.multiline = true;
                loggerArea.value += message;
            }

            public void LogTexture(Texture2D texture2D){
                
            }
        }

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
            if (BaseDataGraphView == null) return;
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
            if(obj.id==null||obj.id.Trim(' ').Length==0){
                Debug.LogWarning("You should not use a node data without id");
                return;
            }
            this.title = _data.nodeName;
            if (_nodeInspectorInNode != null){
                _nodeInspectorInNode.Data = obj;
            }

            _viewLogger ??= new NodeViewLogger{NodeView = this};
            if (NodeLogger.Loggers.ContainsKey(obj.id)){
                NodeLogger.Loggers[obj.id] = _viewLogger;
            }
            else{
                NodeLogger.Loggers.Add(obj.id,_viewLogger);
            }
            BuildInputAndOutputPort();
            this.expanded = true;
            this.RefreshExpandedState();
        }

        protected virtual string BuildPortName(PortAttribute portAttribute,MemberInfo memberInfo,params object[] args){
            switch (portAttribute.NameHandling){
                case PortNameHandling.Auto:
                    return portAttribute.Name.Trim(' ').Length>0?portAttribute.Name:memberInfo.Name;
                case PortNameHandling.Manual:
                    return portAttribute.Name;
                case PortNameHandling.MemberName:
                    return memberInfo.Name;
                case PortNameHandling.Format:
                    return String.Format(memberInfo.Name, args);
                case PortNameHandling.MemberType:
                    return memberInfo.MemberPortType().Name;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Type GetDataType(string path){
            //Access the member by given path
            var memberInfo =  _data.GetType().GetMember(path).FirstOrDefault();
       
            if (memberInfo == null){
                return null;
            }
            if (memberInfo is PropertyInfo propertyInfo){
             
                return propertyInfo.GetValue(_data) as Type;
            }
        
            //Check if the member is a field
            if (memberInfo is FieldInfo fieldInfo){
                return _data.GetType().GetField(path)?.GetValue(_data) as Type;
            }
      
            throw new Exception("Member is not a property or field");
        }
        protected virtual Type BuildPortType(PortAttribute portAttribute,MemberInfo propertyInfo){
            
            switch (portAttribute.TypeHandling){
                    case TypeHandling.Declared :
                        return propertyInfo.MemberPortType();
                    case TypeHandling.Implemented:
                        return propertyInfo.GetPortValue(_data)?.GetType();
                    case TypeHandling.Specified:
                        return portAttribute.HandledType??typeof(object);
                    case TypeHandling.Path:
                        var type = GetDataType(portAttribute.TypePath);
                        return type;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }


        protected virtual Type BuildGroupPortType(PortAttribute portAttribute,MemberInfo propertyInfo,int index){
            IList iList = null;
            if (propertyInfo is PropertyInfo propertyInfo1){
                iList = propertyInfo1.GetValue(_data) as IList;
            }
            else if (propertyInfo is MethodInfo methodInfo){
               iList = methodInfo.Invoke(_data,null) as IList;
            }
            if (iList is Array array){
                switch (portAttribute.TypeHandling){
                    case TypeHandling.Declared:
                        return array.GetType().GetElementType();
                    case TypeHandling.Implemented:
                        return array.GetValue(index).GetType();
                    case TypeHandling.Specified:
                        return portAttribute.HandledType??typeof(object);
                    case TypeHandling.Path:
                        return GetDataType(portAttribute.TypePath);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (iList != null && iList.GetType().IsGenericType){
                var genericType = iList.GetType().GetGenericArguments()[0];
                switch (portAttribute.TypeHandling){
                    case TypeHandling.Declared:
                        return genericType;
                    case TypeHandling.Implemented:
                        return iList[index].GetType();
                    case TypeHandling.Specified:
                        return portAttribute.HandledType??typeof(object);
                    case TypeHandling.Path:
                        return GetDataType(portAttribute.TypePath);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return null;
        }
        
        /// <summary>
        /// of course you can override this method to build your own port builder
        /// </summary>
        protected virtual void BuildInputAndOutputPort(){
            var propertyInfos = _data.GetType().GetProperties();
            var methodInfos = _data.GetType().GetMethods();
            foreach (var propertyInfo in propertyInfos){
                if (propertyInfo.GetCustomAttributes(typeof(OutputAttribute),true).FirstOrDefault() is OutputAttribute attribute){
                    BuildPortFromAttributeAndPropertyInfo(attribute,propertyInfo,Direction.Output);

                }
            }
            foreach (var propertyInfo in propertyInfos){
                if(propertyInfo.GetCustomAttributes(typeof(InputAttribute),true).FirstOrDefault() is InputAttribute attribute){
                    BuildPortFromAttributeAndPropertyInfo(attribute, propertyInfo);
                }
            }

            foreach (var method in methodInfos){
                if (method.GetCustomAttribute<PortAttribute>() != null){
                    var portAtt = method.GetCustomAttribute<PortAttribute>();
                    if (portAtt is InputAttribute input){
                        BuildPortFromAttributeAndPropertyInfo(input,method);
                    }

                    if (portAtt is OutputAttribute output){
                        BuildPortFromAttributeAndPropertyInfo(output,method,Direction.Output);
                    }
                    
                }
                
            }
        }

        private void BuildPortFromAttributeAndPropertyInfo(PortAttribute attribute, MemberInfo propertyInfo,Direction direction = Direction.Input){
            if (attribute.Group == false){
                Port port = new CustomPort
                (Orientation.Horizontal,
                    direction, attribute.Multiple ? Port.Capacity.Multi : Port.Capacity.Single,
                    BuildPortType(attribute, propertyInfo));
                BuildPort(port, attribute, propertyInfo, direction==Direction.Input?inputContainer:outputContainer);
            }
            else{
                var propertyValue = propertyInfo.GetPortValue(_data);
         
                if (propertyValue is IList list){
                    for (var i = 0; i < list.Count; i++){
                        var port = new CustomPort(Orientation.Horizontal, direction,
                            attribute.Multiple ? Port.Capacity.Multi : Port.Capacity.Single,
                            BuildGroupPortType(attribute, propertyInfo, i));
                        BuildGroupPort(port, attribute, propertyInfo, direction==Direction.Input?inputContainer:outputContainer, list[i], i);
                    }
                }
            }
        }


        private void BuildPort(Port port, PortAttribute attribute, MemberInfo memberInfo,VisualElement portContainer){
            portContainer.Add(port);
            var portName = ObjectNames.NicifyVariableName(BuildPortName(attribute, memberInfo));
            port.portName = portName;
            port.name = memberInfo.Name;
            PortColorAttribute colorAtt = null;
            colorAtt =  memberInfo.MemberPortType().GetCustomAttribute<PortColorAttribute>();
            if (colorAtt != null){
                var color = colorAtt.Color;
                port.portColor = color;
            }
        }
        private void BuildGroupPort(Port port, PortAttribute attribute, MemberInfo propertyInfo,
            VisualElement portContainer,object currentElement,int index = 0){
            portContainer.Add(port);
            port.name = propertyInfo.Name + ":" + index;
            if (currentElement is string str){
                if (attribute.NameHandling == PortNameHandling.Auto){
                    port.portName = str;
                }
                else{
                    port.portName = ObjectNames.NicifyVariableName(BuildGroupPortName(attribute, propertyInfo, index));
                }
            }
        }
        private string BuildGroupPortName(PortAttribute attribute, MemberInfo memberInfo, int index){
            switch (attribute.NameHandling){
                case PortNameHandling.Auto:
                    return (attribute.Name.Trim(' ').Length>0?attribute.Name:memberInfo.Name)+":"+index;
                case PortNameHandling.Manual:
                    return attribute.Name+":"+index;;
                case PortNameHandling.MemberName:
                    return memberInfo.Name+":"+index;
                case PortNameHandling.Format:
                    throw new NotImplementedException();
                case PortNameHandling.MemberType:
                    return memberInfo.MemberPortType().Name + ":" + index;
                default:
                    throw new ArgumentOutOfRangeException();
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

        public override void SetPosition(Rect newPos){
            var graphView = (GraphView)BaseDataGraphView;
            //Cast newPos s position to global space
            _data.positionInView.position = newPos.position;
            base.SetPosition(newPos);
        }

        public  void InitializePosition(Rect pos){
            base.SetPosition(pos);
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
        
        public void InitializePosition(Rect pos);
    }

    public interface INodeView<T>:IBaseNodeView where T:NodeData,new(){
        public T Data{ get; set; }
        
    }
}