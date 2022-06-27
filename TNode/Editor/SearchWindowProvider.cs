using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TNode.Editor{
    public class SearchWindowProvider:ScriptableObject,ISearchWindowProvider{
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context){
            var list = new List<SearchTreeEntry>();
            list.Add(new SearchTreeGroupEntry(new GUIContent("Add New Node"), 0));
            list.Add(new SearchTreeGroupEntry(new GUIContent("Add Placemat"), 0));
            return list;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context){
            return false;
        }
    }
}