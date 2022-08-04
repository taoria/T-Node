using System;
using TNode.GraphCreator.Editor;
using TNodeCore.Runtime.Attributes;

namespace TNode.GraphCreator.Runtime.Nodes{
    using TNodeCore.Runtime.Models;

    namespace TNode.GraphCreator.Runtime{
        [GraphUsage(typeof(MetaGraph))]
        [Serializable]
        public class GraphMetaNode:NodeData{
            [ShowInNode]
            public string createNodeName;
            
        }
    }
}