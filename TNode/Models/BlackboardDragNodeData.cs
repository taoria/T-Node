using System.Runtime.InteropServices;
using Newtonsoft.Json;
using TNode.Attribute.Ports;
using TNode.RuntimeCache;

namespace TNode.Models{
    public class BlackboardDragNodeData<T>:NodeData{
        [JsonIgnore]
        private string _blackDragData;
        [JsonIgnore]
        private BlackboardData _blackboardData;

        [Output] 
        public T Value => _blackboardData.GetValue<T>(_blackDragData);
 
        public BlackboardDragNodeData(){
            
        }
        
    }
}