using TNodeCore.Editor.Serialization;
using TNodeCore.Extensions;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Models;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNodeGraphViewImpl.Editor.NodeViews{
    [ViewComponent]
    public class DragBaseNodeView:BaseNodeView<BlackboardDragNode>{
        public DragBaseNodeView() : base(){
            this.titleContainer.visible = false;
           this.titleContainer.RemoveFromHierarchy();
           this.OnDataChanged += OnDataChangedHandler;
        }

        private void OnDataChangedHandler(BlackboardDragNode obj){
            var port = this.Q<Port>();
            var label = port.Q<Label>();
            var blackboard = obj.BlackboardData;
            BlackboardDataWrapper blackboardWrapper = (BlackboardDataWrapper)blackboard;
            var serializedData = new SerializedObject(blackboardWrapper);
            var arrayElement = obj.isListElement;
            SerializedProperty serializedProperty = null;
            if (arrayElement){
                var part = obj.BlackDragData.Split('.');
                serializedProperty = serializedData.FindProperty(BlackboardDataWrapper.DataPath)
                    .FindPropertyRelative(part[0])
                    .GetArrayElementAtIndex(int.Parse(part[1]));
            }
            else{
                 serializedProperty = serializedData.FindProperty(BlackboardDataWrapper.DataPath).FindPropertyRelative(obj.BlackDragData);
            }
            label.text = ObjectNames.NicifyVariableName(obj.BlackDragData);
            //Get serialized property's icon
            Texture2D icon = null;
            if (serializedProperty == null) return;
            if (serializedProperty.BoxedValue() is Object value){
                icon = AssetPreview.GetMiniThumbnail(value);
            }
            else{
                if (serializedProperty.BoxedValue() == null){
                    return;
                }
                icon = AssetPreview.GetMiniTypeThumbnail(serializedProperty.BoxedValue().GetType());
            }

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