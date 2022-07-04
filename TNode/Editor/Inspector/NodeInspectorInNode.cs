using System.Reflection;
using TNode.Attribute;
using TNode.Models;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.Editor.Inspector{
    public class NodeInspectorInNode:VisualElement{
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

        private void RefreshInspector(){
            Clear();
            InspectorItemFactory inspectorItemFactory = new InspectorItemFactory();
            foreach (var field in _data.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)){
                var bindingPath = field.Name;
                var type = field.FieldType;
                //check if the field has ShowInNodeView attribute
                var showInNodeViewAttribute = field.GetCustomAttribute<ShowInNodeViewAttribute>()!=null;
                if(!showInNodeViewAttribute)
                    continue;
                //Invoke generic function Create<> of default inspector item factory to create an inspector item of appropriate type by reflection
                MethodInfo methodInfo = inspectorItemFactory.GetType().GetMethod("Create", BindingFlags.Instance | BindingFlags.Public);
                if (methodInfo != null){
                    var genericMethod = methodInfo.MakeGenericMethod(type);
                    var createdItem  = genericMethod.Invoke(inspectorItemFactory,null) as VisualElement;
                    Add(createdItem);
                    if (createdItem is INodeDataBindingBase castedItem){
                        castedItem.BindingNodeData = _data;
                        castedItem.BindingPath = bindingPath;
                    }
                }
            }
        }
    }
}