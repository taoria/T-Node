using System.Collections;
using System.Reflection;
using TNode.Attribute;
using TNode.Editor.NodeGraphView;
using TNode.Editor.Search;
using TNode.Models;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TNodeGraphViewImpl.Editor.GraphBlackboard{
    [ViewComponent]
    public class DefaultGraphBlackboardView:GraphBlackboardView<BlackboardData>{
        protected override void UpdateBlackboard(BlackboardData data){
            
          
            foreach (var field in data.GetType()
                         .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)){
                //if the field is MonoBehaviour,add a property field for blackboard 
                //skip if the field is a list or Ilist
                if (!typeof(IList).IsAssignableFrom(field.FieldType)){
                    var propertyField = new BlackboardPropertyField(new BlackboardProperty.BlackboardProperty(field.Name,field.FieldType));
                    this.Add(propertyField);
                }
            }
            this.addItemRequested = (sender) => {
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