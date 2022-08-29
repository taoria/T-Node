using TNodeCore.Runtime;
using TNodeCore.Runtime.Attributes;
using TNodeCore.Runtime.Attributes.Ports;
using TNodeCore.Runtime.Models;

namespace Samples.Nodes{
    [GraphUsage(typeof(HelloGraph),"Math")]
    public class AddNode:NodeData{
        [Input] public float A{ get; set; } = default;
        [Input]
        public float B{ get; set; }= default;

        [Output] public float C => A + B;
        public override void Process(){

        }
    }
}