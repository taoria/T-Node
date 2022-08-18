using TNode.TNodeCore.Editor.Models;
using TNodeCore.Runtime.Models;
using UnityEngine;

namespace TNodeCore.Editor.Models{
    public class Comment:EditorModel{
        [SerializeReference]
        public Model CommentedModel;
        public string CommentText;
    }
}