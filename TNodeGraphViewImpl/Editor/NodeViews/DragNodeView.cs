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
            
            //
            // field.Bind(serializedData);
            // var p = label.parent.parent.parent;
            // p.Add(field);
            // field.SendToBack();
            // field.SetEnabled(false);
            // p.style.alignItems = Align.Center;
            // p.style.justifyContent = Justify.Center;
            // p.style.paddingTop = 0;
            // p.style.paddingBottom = 0;
            label.text = obj.blackDragData;
            //Get serialized property's icon
            var icon = AssetPreview.GetMiniThumbnail(serializedProperty.objectReferenceValue);
         
            label.parent.Add(new Image(){
                image = icon
            });
            //more round style for entire element
            style.borderBottomLeftRadius = style.borderBottomRightRadius =
                style.borderTopLeftRadius = style.borderTopRightRadius = 25;
            styleSheets.Add(Resources.Load<StyleSheet>("DragNodeStyle"));
            this.name = "DragNodeView";
        }
    }
}