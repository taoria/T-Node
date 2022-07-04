using System;
using TNode.Attribute;
using TNode.Attribute.Ports;
using TNode.Editor.Inspector;
using TNode.Models;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;

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
                Debug.Log(propertyInfos);
                var attribute = propertyInfo.GetCustomAttributes(typeof(OutputAttribute),true);
                if (attribute.Length > 0){
                    Port port = InstantiatePort(Orientation.Horizontal, Direction.Output,Port.Capacity.Multi,propertyInfo.PropertyType);
                    this.outputContainer.Add(port);
                    port.portName = propertyInfo.Name;
                    port.name = propertyInfo.Name;
                }
            }
            foreach (var propertyInfo in propertyInfos){
                Debug.Log(propertyInfos);
                var attribute = propertyInfo.GetCustomAttributes(typeof(InputAttribute),true);
                if (attribute.Length > 0){
                    Port port = InstantiatePort(Orientation.Horizontal, Direction.Input,Port.Capacity.Multi,propertyInfo.PropertyType);
                    this.inputContainer.Add(port);
                    port.portName = propertyInfo.Name;
                    port.name = propertyInfo.Name;
                }
            }
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