using TNode.TNodeCore.Editor.Binding;
using TNode.TNodeCore.Editor.Models;
using UnityEditor.Experimental.GraphView;
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
            this.Add(txtField);
            txtField.RegisterValueChangedCallback(evt => {
                if (_data != null){
                    _data.CommentText = evt.newValue;
                }
            });
        }
        public void OnChange(){
            var str = this._data.CommentText;
            this.Q<TextField>().value = str;
        }
    }
}