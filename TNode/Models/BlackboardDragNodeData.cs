using System.Runtime.InteropServices;
using Newtonsoft.Json;
using TNode.Attribute;
using TNode.Attribute.Ports;
using TNode.RuntimeCache;

namespace TNode.Models{
    public class BlackboardDragNodeData<T>:NodeData{
        private string _blackDragData;
        [JsonIgnore]
        private BlackboardData _blackboardData;

        [Output("",PortNameHandling.MemberType)] 
        public T Value => _blackboardData.GetValue<T>(_blackDragData);
 
        public BlackboardDragNodeData(){
            
        }
        
    }
}