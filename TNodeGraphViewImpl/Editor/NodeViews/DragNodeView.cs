using TNodeCore.Attribute;
using TNodeCore.Editor.Serialization;
using TNodeCore.Models;
using TNodeGraphViewImpl.Editor.NodeViews;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Graphs;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.Editor.NodeViews{
    [ViewComponent]
    public class DragBaseNodeView:BaseNodeView<BlackboardDragNodeData>{
        public DragBaseNodeView() : base(){
            this.titleContainer.visible = false;
           this.titleContainer.RemoveFromHierarchy();
           this.OnDataChanged += OnDataChangedHandler;
        }

        private void OnDataChangedHandler(BlackboardDragNodeData obj){
            var port = this.Q<Port>();
            var label = port.Q<Label>();
            var blackboard = obj.BlackboardData;
            BlackboardDataWrapper blackboardWrapper = (BlackboardDataWrapper)blackboard;
            var serializedData = new SerializedObject(blackboardWrapper);
            var serializedProperty = serializedData.FindProperty("data").FindPropertyRelative(obj.blackDragData);
            PropertyField field = new PropertyField(serializedProperty,"");
            field.Bind(serializedData);
            label.parent.parent.style.flexDirection = FlexDirection.RowReverse;
            label.parent.parent.Add(field);
            label.parent.parent.style.alignItems = Align.Center;
            label.parent.parent.style.justifyContent = Justify.Center;
            label.parent.parent.style.paddingTop = 0;
            label.parent.parent.style.paddingBottom = 0;
            label.RemoveFromHierarchy();
            
        }
    }
}