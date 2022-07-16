﻿using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace TNodeGraphViewImpl.Editor.GraphBlackboard{
    public class GraphBlackboardPropertyField:PropertyField{
        private readonly bool _runtime;

        public GraphBlackboardPropertyField(SerializedProperty findPropertyRelative, string fieldName,bool runtime):base(findPropertyRelative, fieldName){
            _runtime = runtime;
        }
        
        protected override void ExecuteDefaultActionAtTarget(EventBase evt)
        {
            
            base.ExecuteDefaultActionAtTarget(evt);
            if (this.Q<ObjectField>() != null){
                this.Q<ObjectField>().allowSceneObjects = _runtime;

            }
        }
     
        
    }
}