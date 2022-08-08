


using TNodeCore.Editor.Models;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.TNodeGraphViewImpl.Editor.Placemats{
    public class PlacematView:Placemat{
        public PlacematData PlacematData{
            get => _placematData;
            set{
                _placematData = value;
                UpdatePlacematData();
            }
        }
        private PlacematData _placematData;
        public int zOrder  {get;set;}
        public PlacematView(){
            var title = this.Q<TextField>();
            title.RegisterValueChangedCallback(evt => {
                    PlacematData.title = title.value;
            });
        }
        public virtual void UpdatePlacematData(){
            SetPosition(_placematData.positionInView);
            this.Color = new Color(43/255.0f, 72/255.0f, 101/255.0f);
        }
        public sealed override void SetPosition(Rect newPos){
            base.SetPosition(newPos);
            if (_placematData == null){
                return;
            }
            _placematData.positionInView = newPos;
        }
        public virtual void Collapse(){
            this.Collapsed = true;
        }
        
        
        
    }
}