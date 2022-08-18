using System;
using TNode.GraphCreator.Runtime;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Models;

namespace TNodeCore.GraphCreator.Runtime.Nodes{
  

    namespace TNode.GraphCreator.Runtime{
        [GraphUsage(typeof(MetaGraph))]
        [Serializable]
        public class GraphMetaNode:NodeData{
            [ShowInNode]
            public string createNodeName;
            
        }
    }
}