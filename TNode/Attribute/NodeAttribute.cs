using JetBrains.Annotations;
using TNode.Models;

namespace TNode.Attribute{
    [MeansImplicitUse]
    [BaseTypeRequired(typeof(NodeData))]
    
    public class NodeAttribute:System.Attribute{
        public NodeAttribute(GraphData graphData){
            
        }
    }
}