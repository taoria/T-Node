using System.Linq;
using TNode.Editor;
using TNode.Editor.EditorPersistence;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace TNodeGraphViewImpl.Editor.NodeGraphView{
    public class SimpleGraphSubWindow:GraphElement,IGraphViewPersistence{
        private readonly Dragger _dragger = new Dragger();

        protected void ConstructWindowBasicSetting(){
            
            RegisterCallback<WheelEvent>(evt => { evt.StopPropagation(); });
            focusable = false;
            capabilities |= Capabilities.Movable | Capabilities.Resizable;
            this.AddManipulator(_dragger);
        }
        protected void BuildWindow(VisualTreeAsset visualTreeAsset){
            if(visualTreeAsset != null){
                visualTreeAsset.CloneTree(this);
            }
        }
  
        public SimpleGraphSubWindow(string defaultUxml=null){
            style.position = new StyleEnum<Position>(Position.Absolute);
            ConstructWindowBasicSetting();
            if (defaultUxml != null){
                var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(defaultUxml);
                BuildWindow(uxml);
            }
        }
        public SimpleGraphSubWindow(VisualTreeAsset visualTreeAsset){
            style.position = new StyleEnum<Position>(Position.Absolute);
            ConstructWindowBasicSetting();
            BuildWindow(visualTreeAsset);
        }

        public string GetPersistenceId(){
            throw new System.NotImplementedException();
        }

        public void ResetPos(GraphEditorData editorData){
            var res = editorData.graphElementsData.FirstOrDefault(x => x.guid == this.GetPersistenceId());
        }

        public void SavePos(GraphEditorData editorData){
          
        }

        public void OnRemoveFromGraph(GraphEditorData editorData){
            
        }
    }


}