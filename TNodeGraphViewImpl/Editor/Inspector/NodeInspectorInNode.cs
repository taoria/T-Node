using System.Reflection;
using TNodeCore.Attribute;
using TNodeCore.Editor.NodeGraphView;
using TNodeCore.Editor.Serialization;
using TNodeCore.Models;
using TNodeGraphViewImpl.Editor.NodeViews;
using UnityEditor;
using UnityEditor.UIElements;
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

        public NodeInspectorInNode():base(){
        }
        private void UpdateData(){
            if (_data != null){
                RefreshInspector();
            }
        }

        private void RefreshInspector(){
            //Set size
           
            Clear();
            //RefreshItems();
            RefreshPropertyDrawer();
        }

  
        private void RefreshPropertyDrawer(){
            //Check if the data's type is a generic type of  BlackboardDragNodeData<>
            if (_data.GetType().IsSubclassOf(typeof(BlackboardDragNodeData))){
                return;
            }
            var serializedObject = new SerializedObject((NodeDataWrapper)_data);
            foreach (var field in _data.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public|BindingFlags.NonPublic)){
                //Create corresponding property field
                //check if the field has ShowInNodeView attribute
                var showInNodeViewAttribute = field.GetCustomAttribute<ShowInNodeViewAttribute>() != null;
                if (!showInNodeViewAttribute)
                    continue;
                var drawer = new PropertyField(serializedObject.FindProperty("data").FindPropertyRelative(field.Name),field.Name);
                drawer.Bind(serializedObject);
                Add(drawer);

     
            }
            var globalTest = GetFirstAncestorOfType<IBaseNodeView>()?.BaseDataGraphView?.TestMode;
            if(globalTest??false){
                CreateTestButton();
            }
            else if (_data.isTest){
                //Add a test button for the node
                CreateTestButton();
            }
        }

        private void CreateTestButton(){
            var testButton = new Button(() => {
                var test = GetFirstAncestorOfType<IBaseDataGraphView>();
                if (test != null){
                    if(!test.IsRuntimeGraph) return;
                    var runtimeGraph = test.GetRuntimeGraph();
                    if (runtimeGraph != null){
                        runtimeGraph.ResolveDependency(_data);
                    }
                    _data.OnTest();
                }
            }){
                text = "Test"
            };

            Add(testButton);
     
        }
    }
}