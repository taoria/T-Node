using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.Models;

namespace Samples.Nodes{
    [GraphUsage(typeof(HelloGraph),"Math")]
    public class AddNode:NodeData{
        [Input]
        public float A{ get; set; }
        [Input]
        public float B{ get; set; }

        [Output] public float C => A + B;
    }
}