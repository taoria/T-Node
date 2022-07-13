﻿using System.Collections;
using System.Reflection;
using TNode.Attribute;
using TNode.Editor.NodeGraphView;
using TNode.Editor.Search;
using TNode.Editor.Serialization;
using TNode.Models;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNodeGraphViewImpl.Editor.GraphBlackboard{
    [ViewComponent]
    public class DefaultGraphBlackboardView:GraphBlackboardView<BlackboardData>{
        public DefaultGraphBlackboardView():base(){
            //the label and the field gap smaller
            styleSheets.Add( Resources.Load<StyleSheet>("GraphViewPropertyField"));
       
        }
        protected override void UpdateBlackboard(BlackboardData data){
            var serializedObject = new SerializedObject((BlackboardDataWrapper)data);
            foreach (var field in data.GetType()
                         .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)){
                //if the field is MonoBehaviour,add a property field for blackboard 
                //skip if the field is a list or Ilist
                if (!typeof(IList).IsAssignableFrom(field.FieldType)){
                    VisualElement visualElement = new VisualElement();
                    var propertyField = new BlackboardPropertyField(new BlackboardProperty.BlackboardProperty(field.Name,field.FieldType));
                    var foldoutData = new Foldout{
                        text = field.Name
                    };
                    var drawer = new PropertyField(serializedObject.FindProperty("data").FindPropertyRelative(field.Name),field.Name);
                    drawer.Bind(serializedObject);
                    foldoutData.Add(drawer);
                    visualElement.Add(propertyField);
                    visualElement.Add(foldoutData);
                    this.Add(visualElement);
                    
                }
                else{
                    var blackboardList = new BlackboardSection{
                        title = field.Name
                    };
                    this.Add(blackboardList);
                }
            }
            addItemRequested = (sender) => {
                var res = ScriptableObject.CreateInstance<BlackboardSearchWindowProvider>();
                
                //Get right top corner of the blackboard
                var blackboardPos = GetPosition().position+OwnerWindow.position.position;
                var searchWindowContext = new SearchWindowContext(blackboardPos,200,200);
                //Call search window 
                res.Setup(Owner.GetGraphData().GetType(),Owner,OwnerWindow);
                SearchWindow.Open(searchWindowContext, res);
            };
        }
        
    }
}