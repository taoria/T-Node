using Newtonsoft.Json;
using TNode.Attribute.Ports;

namespace TNode.Models{
    public class BlackDragNodeData<T>:NodeData{
        [JsonIgnore]
        private string _blackDragData;
        [JsonIgnore]
        private BlackboardData _blackboardData;
        
    }
}