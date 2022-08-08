using System;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Models;

namespace TNode.GraphCreator.Runtime.Nodes{
  

    namespace TNode.GraphCreator.Runtime{
        [GraphUsage(typeof(MetaGraph))]
        [Serializable]
        public class GraphMetaNode:NodeData{
            [ShowInNode]
            public string createNodeName;
            
        }
    }
}