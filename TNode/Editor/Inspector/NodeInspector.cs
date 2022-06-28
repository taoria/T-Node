using System.Reflection;
using TNode.BaseViews;
using TNode.Models;
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

        private void UpdateData(){
            if (_data != null){
                RefreshInspector();
            }
        }
        public NodeInspector(){
            var visualTreeAsset = Resources.Load<VisualTreeAsset>("NodeInspector");
            
            Debug.Log(visualTreeAsset);
            ConstructWindowBasicSetting();
            BuildWindow(visualTreeAsset);
        }

        private void RefreshInspector(){
            //iterate field of data and get name of every fields,create a new inspector item of appropriate type and add it to the inspector for each field
            foreach (var field in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)){
                var bindingPath = field.Name;
                var type = field.FieldType;
                DefaultInspectorItemFactory defaultInspectorItemFactory = new DefaultInspectorItemFactory();
                //Invoke generic function Create<> of default inspector item factory to create an inspector item of appropriate type by reflection
                MethodInfo methodInfo = defaultInspectorItemFactory.GetType().GetMethod("Create", BindingFlags.Instance | BindingFlags.Public);
                if (methodInfo != null){
                    var genericMethod = methodInfo.MakeGenericMethod(type);
                    var createdInspector  = genericMethod.Invoke(defaultInspectorItemFactory,null) as VisualElement;
                    Add(createdInspector);
                    if (createdInspector is INodeDataBindingBase castedInspector){
                        castedInspector.BindingNodeData = _data;
                        castedInspector.BindingPath = bindingPath;
                    }
                }
            }
            
        }
        
    }
}