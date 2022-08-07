using System;
using System.Reflection;
using TNodeCore.Runtime.Attributes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.TNodeGraphViewImpl.Editor.GraphBlackboard{
    public class BlackboardDataEntry:GraphElement{
        public Type propertyType;
        public string propertyPath;
        private Color _convertedColor;
        public BlackboardDataEntry(Type type){
            propertyType = type;
            if (typeof(Component).IsAssignableFrom(propertyType)){
                this.AddToClassList("typeComponent");
            }else if (typeof(GameObject).IsAssignableFrom(propertyType)){
                this.AddToClassList("gameObject");
            }else{
                this.AddToClassList(propertyType.Name);
            }


            this.capabilities |= Capabilities.Selectable | Capabilities.Deletable | Capabilities.Droppable | Capabilities.Renamable;
            this.AddManipulator(new SelectionDropper());
            var styleSheet =  Resources.Load<StyleSheet>("BlackboardDataEntry");
            this.styleSheets.Add(styleSheet);
            
            if (type.GetCustomAttribute<PortColorAttribute>() is {} portColorAttribute){
                _convertedColor = portColorAttribute.Color;
            }
            this.RegisterCallback<MouseEnterEvent>((evt) => {
                style.borderBottomColor=style.borderRightColor=style.borderLeftColor=style.borderTopColor=new Color(1,1,1,1);
            });
            this.RegisterCallback<MouseLeaveEvent>((evt) => {
                style.borderBottomColor = style.borderRightColor =
                    style.borderLeftColor = style.borderTopColor = _convertedColor==default?StyleKeyword.Null:new StyleColor(_convertedColor);

            });
        }
    }
}