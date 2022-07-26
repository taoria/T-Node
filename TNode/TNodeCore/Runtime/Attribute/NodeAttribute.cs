using JetBrains.Annotations;
using TNodeCore.Models;

namespace TNodeCore.Attribute{
    [MeansImplicitUse]
    [BaseTypeRequired(typeof(NodeData))]
    
    public class NodeAttribute:System.Attribute{
        public NodeAttribute(GraphData graphData){
            
        }
    }
}