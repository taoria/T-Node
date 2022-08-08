using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using TNode.TNodeGraphViewImpl.Editor.Search;
using TNodeCore.Editor.NodeGraphView;
using TNodeCore.Editor.Serialization;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Models;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.TNodeGraphViewImpl.Editor.GraphBlackboard{
    [ViewComponent]
    public class DefaultGraphBlackboardView:GraphBlackboardView<BlackboardData>{

        public DefaultGraphBlackboardView():base(){
            //the label and the field gap smaller
            styleSheets.Add( Resources.Load<StyleSheet>("GraphViewPropertyField"));
       
        }
        protected override void UpdateBlackboard(BlackboardData data){
            Clear();
            if (data == null) return;
            var serializedObject = new SerializedObject((BlackboardDataWrapper)data);
            var currentGraphView = graphView as IBaseDataGraphView;
            var isRuntimeGraph = currentGraphView?.IsRuntimeGraph ?? false;
            var blackboardGlobalSection = new BlackboardSection{
                title = "Global Data"
            };
            Add(blackboardGlobalSection);
            foreach (var field in data.GetType()
                         .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)){
                if(field.GetCustomAttributes(typeof(HideInBlackboard)).Count()!=0) continue;
                //if the field is MonoBehaviour,add a property field for blackboard 
                //skip if the field is a list or Ilist
                if (!typeof(IList).IsAssignableFrom(field.FieldType)&&!field.FieldType.IsArray){
                    CreateBlackboardDataEntry(field, serializedObject, isRuntimeGraph, blackboardGlobalSection);
                }
                else{
                    var blackboardList = new BlackboardSection{
                        title = field.Name
                    };
                    var foldout = new Foldout{
                        text = field.Name,
                    };
                    blackboardList.Add(foldout);
                    
                    Add(blackboardList);
                    if(field.GetValue(data)==null) continue;
                    if (field.GetValue(data) is IList list){
                        for (var i = 0; i < list.Count; i++){
                            CreateBlackboardDataEntryForListItem(field, serializedObject, isRuntimeGraph, blackboardList, i);
                        }
                    }
                    if (field.GetValue(data).GetType().IsArray){
                        var array = (Array)field.GetValue(data);
                        if(array==null) continue;
                        for (var i = 0; i < array.Length; i++){
                            CreateBlackboardDataEntryForListItem(field, serializedObject, isRuntimeGraph, blackboardList, i);
                        }
                    }
                }
            }
            addItemRequested = (sender) => {
                 var res = ScriptableObject.CreateInstance<BlackboardSearchWindowProvider>();
                 Debug.Log(res);
                 //Get right top corner of the blackboard
                
                 var blackboardPos = GetPosition().position+OwnerWindow.position.position;
                
                 var searchWindowContext = new SearchWindowContext(blackboardPos,200,200);
                 
          
                 //Call search window 
                 res.Setup(Owner.GetGraphData().GetType(),Owner,OwnerWindow,this);
                 
                 SearchWindow.Open(searchWindowContext, res);
                 
            };
        }

        private static void CreateBlackboardDataEntryForListItem(FieldInfo field, SerializedObject serializedObject,
            bool isRuntimeGraph,
            BlackboardSection blackboardSection, int index){
            var property = serializedObject.FindProperty("data");
                property =  property.FindPropertyRelative(field.Name).GetArrayElementAtIndex(index);
            
            BlackboardDataEntry entry = new BlackboardDataEntry(field.FieldType){
                propertyPath = field.Name+"."+index,
            };
            var drawer =
                new GraphBlackboardPropertyField(property,
                    isRuntimeGraph);
            drawer.Bind(serializedObject);
            entry.Add(drawer);
            var container = blackboardSection.Q<Foldout>();
            container.Add(entry);
           
        }

        
        private static void CreateBlackboardDataEntry(FieldInfo field, SerializedObject serializedObject, bool isRuntimeGraph,
            BlackboardSection blackboardSection){
            BlackboardDataEntry entry = new BlackboardDataEntry(field.FieldType){
                propertyPath = field.Name
            };
            //var propertyField = new BlackboardField(new BlackboardProperty.BlackboardProperty(field.Name,field.FieldType));
            var foldoutData = new Foldout{
            };
            var drawer =
                new GraphBlackboardPropertyField(serializedObject.FindProperty("data").FindPropertyRelative(field.Name),
                    isRuntimeGraph);
            drawer.Bind(serializedObject);
            foldoutData.Add(drawer);
            entry.Add(foldoutData);
            blackboardSection.Add(entry);
            Label visualElementOverlapFoldoutLabel = new Label(ObjectNames.NicifyVariableName(field.Name)){
                style ={
                    //put the label in the position that overlaps the foldout's right side to prevent click
                    position = Position.Absolute,
                    left = 20,
                    alignContent = new StyleEnum<Align>(Align.Center),
                    justifyContent = new StyleEnum<Justify>(Justify.Center),
                    unityTextAlign = new StyleEnum<TextAnchor>(TextAnchor.UpperCenter),
                    marginTop = 0,
                    paddingTop = 0

                }
            };
            entry.Add(visualElementOverlapFoldoutLabel);
            visualElementOverlapFoldoutLabel.BringToFront();
        }
    }
}