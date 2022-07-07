using System;
using TNode.Attribute;
using TNode.Attribute.Ports;
using TNode.Editor.Inspector;
using TNode.Models;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.Editor.BaseViews{
    
    //A NodeAttribute monitor some type of node in the graph
    
    public abstract class NodeView<T> : Node,INodeView where T:NodeData,new(){
        protected T _data;
        private readonly NodeInspectorInNode _nodeInspectorInNode;
        
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

        private void OnDataValueChanged(NodeDataWrapper obj){
            Refresh();
        }

        public sealed override string title{
            get => base.title;
            set => base.title = value;
        }
        public event System.Action<T> OnDataChanged;

        protected NodeView(){
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

        private void BuildInputAndOutputPort(){
            var propertyInfos = _data.GetType().GetProperties();
   
            foreach (var propertyInfo in propertyInfos){
                var attribute = propertyInfo.GetCustomAttributes(typeof(OutputAttribute),true);
                if (attribute.Length > 0){
                    Port port = InstantiatePort(Orientation.Horizontal, Direction.Output,Port.Capacity.Multi,propertyInfo.PropertyType);
                    this.outputContainer.Add(port);
                    port.portName = propertyInfo.Name;
                    port.name = propertyInfo.Name;
                }
            }
            foreach (var propertyInfo in propertyInfos){
                var attribute = propertyInfo.GetCustomAttributes(typeof(InputAttribute),true);
                if (attribute.Length > 0){
                    Port port = InstantiatePort(Orientation.Horizontal, Direction.Input,Port.Capacity.Multi,propertyInfo.PropertyType);
                    this.inputContainer.Add(port);
                    port.portName = propertyInfo.Name;
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

    public interface INodeView{
        public void SetNodeData(NodeData nodeData);
        public NodeData GetNodeData();
        
        public void OnDataModified();

    }
}