using TNode.TNodeCore.Editor.Models;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNodeGraphViewImpl.Editor.Placemats{
    public class PlacematView:Placemat{
        public PlacematModel PlacematModel{
            get => _placematModel;
            set{
                _placematModel = value;
                UpdatePlacematData();
            }
        }
        private PlacematModel _placematModel;
        public int zOrder  {get;set;}
        public PlacematView(){
            var title = this.Q<TextField>();
            title.RegisterValueChangedCallback(evt => {
                    PlacematModel.title = title.value;
            });
        }
        public virtual void UpdatePlacematData(){
            SetPosition(_placematModel.positionInView);
            this.Color = new Color(43/255.0f, 72/255.0f, 101/255.0f);
        }
        public sealed override void SetPosition(Rect newPos){
            base.SetPosition(newPos);
            if (_placematModel == null){
                return;
            }
            _placematModel.positionInView = newPos;
        }
        public virtual void Collapse(){
            this.Collapsed = true;
        }
        
        
        
    }
}