using System;
using System.Collections.Generic;
using System.Reflection;
using TNode.Editor.NodeViews;
using TNodeCore.Runtime.Models;
using TNodeGraphViewImpl.Editor.NodeGraphView;
using TNodeGraphViewImpl.Editor.NodeViews;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector{
    public class NodeInspector:SimpleGraphSubWindow{
        private NodeData _data;
        public NodeData Data{
            get => _data;
            set{
                _data = value;
                UpdateData();
                
            }
        }

        public IBaseNodeView BaseNodeView;
        private void UpdateData(){
            Debug.Log(_data);
            if (_data != null){
                RefreshInspector();
            }
        }
        public NodeInspector(){
            this.capabilities |= Capabilities.Resizable;
            style.position = new StyleEnum<Position>(Position.Absolute);
            var visualTreeAsset = Resources.Load<VisualTreeAsset>("NodeInspector");
            Debug.Log(visualTreeAsset);
            ConstructWindowBasicSetting();
            BuildWindow(visualTreeAsset);
        }


        private void RefreshInspector(){
            //iterate field of data and get name of every fields,create a new inspector item of appropriate type and add it to the inspector for each field
            var body = this.Q("InspectorBody");
            body.Clear();
            body.StretchToParentSize();
            // foreach (var field in _data.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)){
            //     var bindingPath = field.Name;
            //     var type = field.FieldType;
            //     InspectorItemFactory inspectorItemFactory = new InspectorItemFactory();
            //     //Invoke generic function Create<> of default inspector item factory to create an inspector item of appropriate type by reflection
            //     var createdItem = inspectorItemFactory.Create(type);
            //     if (createdItem is { } castedItem){
            //         castedItem.BindingNodeData = _data;
            //         castedItem.BindingPath = bindingPath;
            //     }
            //     Add((VisualElement)createdItem);
            // }
        }
    }
}