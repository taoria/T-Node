using System.Collections.Generic;
using TNode.Models;

namespace TNode.Runtime{
    public class RuntimeNode{
        public NodeData NodeData { get; set; }
        public List<NodeLink> NodeLinks;
    }
}