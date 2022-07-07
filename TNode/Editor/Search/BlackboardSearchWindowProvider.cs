using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

namespace TNode.Editor{
    public class BlackboardSearchWindowProvider:ISearchWindowProvider{
  
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context){
            throw new System.NotImplementedException();
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context){
            throw new System.NotImplementedException();
        }
    }
    
}