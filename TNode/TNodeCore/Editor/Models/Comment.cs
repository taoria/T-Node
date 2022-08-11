using TNodeCore.Runtime.Models;
using UnityEngine;

namespace TNode.TNodeCore.Editor.Models{
    public class Comment:EditorModel{
        [SerializeReference]
        public Model CommentedModel;
        public string CommentText;
    }
}