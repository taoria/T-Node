using JetBrains.Annotations;
using TNodeCore.Runtime.Models;

namespace TNodeCore.Runtime.Attributes{
    [MeansImplicitUse]
    [BaseTypeRequired(typeof(NodeData))]
    
    public class NodeAttribute:System.Attribute{
        public NodeAttribute(GraphData graphData){
            
        }
    }
}