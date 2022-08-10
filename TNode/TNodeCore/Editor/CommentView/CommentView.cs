using System;
using TNode.TNodeCore.Editor.Binding;
using TNode.TNodeCore.Editor.Models;
using TNodeCore.Editor.NodeGraphView;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TNode.TNodeCore.Editor.CommentView{
    public class CommentView:GraphElement,IModelBinding<Comment>{
        public Comment Data => _data;
        private Comment _data;
        public void Bind(Comment data){
            _data = data;
            OnChange();
        }

        public CommentView(){
            var txtField = new TextField();
            var btn = new Button(() => {
                var graphElement = (Node) this.GetFirstOfType<Node>();
                graphElement.Remove(this);
            });
            btn.text = "-";
            this.Add(btn);
            this.Add(txtField);
            txtField.RegisterValueChangedCallback(evt => {
                if (_data != null){
                    _data.CommentText = evt.newValue;
                }
            });
            
            
            
            capabilities |= Capabilities.Collapsible | Capabilities.Deletable|Capabilities.Selectable;
            
            styleSheets.Add(Resources.Load<StyleSheet>("CommentView"));
        }

        private void ClickComment(){
           
        }

        public void OnChange(){
            var str = this._data.CommentText;
            this.Q<TextField>().value = str;
        }
    }
}