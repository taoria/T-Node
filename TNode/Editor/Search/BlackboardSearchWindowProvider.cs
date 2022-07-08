using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace TNode.Editor{
    public class BlackboardSearchWindowProvider:ISearchWindowProvider{
        private Type _graphType;
        private GraphView _graphView;
        private EditorWindow _editor;
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context){
            return null;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context){
            return false;
        }
    }
    
}