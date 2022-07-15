using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TNodeGraphViewImpl.Editor.GraphBlackboard{
    public class GraphBlackboardPropertyField:PropertyField{
        public GraphBlackboardPropertyField(SerializedProperty findPropertyRelative, string fieldName):base(findPropertyRelative, fieldName){
            
        }

        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            base.ExecuteDefaultActionAtTarget(evt);
            if (this.Q<ObjectField>() != null){
                this.Q<ObjectField>().allowSceneObjects = false;
            }
        }
     
        
    }
}