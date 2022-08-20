using System.Linq;
using TNodeCore.Runtime.Models;

namespace TNodeCore.Runtime.Extensions{
    public static  class NodeDataExtensions{
        public static  string[] GetDependentNodesId(this RuntimeNode runtimeNode){
            return runtimeNode.InputLinks.Select(x => x.outPort.nodeDataId).ToArray();
        }
    }
}