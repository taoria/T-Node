using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace TNode.BaseViews{
    public class SimpleGraphSubWindow:GraphElement{
        private readonly Dragger _dragger = new Dragger();

        private void ConstructWindowBasicSetting(){
            RegisterCallback<WheelEvent>(evt => { evt.StopPropagation(); });
            focusable = false;
            capabilities |= Capabilities.Movable | Capabilities.Resizable;
            this.AddManipulator(_dragger);
        }
        private void BuildWindow(VisualTreeAsset visualTreeAsset){
            if(visualTreeAsset != null){
                visualTreeAsset.CloneTree(this);
            }
        }
  
        public SimpleGraphSubWindow(string defaultUxml=null){
            ConstructWindowBasicSetting();
            if (defaultUxml != null){
                var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(defaultUxml);
                BuildWindow(uxml);
            }
        }

        public SimpleGraphSubWindow(VisualTreeAsset visualTreeAsset){
            ConstructWindowBasicSetting();
            BuildWindow(visualTreeAsset);
        }
        
    }
}