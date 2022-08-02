using System;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.TNodeGraphViewImpl.Editor.GraphBlackboard{
    public class BlackboardDataEntry:GraphElement{
        public Type propertyType;
        public string propertyPath;
        public BlackboardDataEntry(Type type){
            propertyType = type;
            if (typeof(Component).IsAssignableFrom(propertyType)){
                this.AddToClassList("typeComponent");
            }
            if (typeof(GameObject).IsAssignableFrom(propertyType)){
                this.AddToClassList("gameObject");
            }
            if (typeof(Vector2).IsAssignableFrom(propertyType)){
                this.AddToClassList("vector");
            }
            if (typeof(Vector2Int).IsAssignableFrom(propertyType)){
                this.AddToClassList("vector");
            }
            if (typeof(IList).IsAssignableFrom(propertyType)){
                this.AddToClassList("list");
            }
            this.capabilities |= Capabilities.Selectable | Capabilities.Deletable | Capabilities.Droppable | Capabilities.Renamable;
            this.AddManipulator(new SelectionDropper());
            var styleSheet =  Resources.Load<StyleSheet>("BlackboardDataEntry");
            this.styleSheets.Add(styleSheet);
            
            this.RegisterCallback<MouseEnterEvent>((evt) => {
                style.borderBottomColor=style.borderRightColor=style.borderLeftColor=style.borderTopColor=new Color(1,1,1,1);
            });
            this.RegisterCallback<MouseLeaveEvent>((evt) => {
                style.borderBottomColor = style.borderRightColor =
                    style.borderLeftColor = style.borderTopColor = StyleKeyword.Null;

            });
   
        }
    }
}