using System.Runtime.InteropServices;
using Newtonsoft.Json;
using TNode.Attribute.Ports;
using TNode.RuntimeCache;

namespace TNode.Models{
    public class BlackDragNodeData<T>:NodeData where T:BlackboardData{
        [JsonIgnore]
        private string _blackDragData;
        [JsonIgnore]
        private T _blackboardData;

        [Output] 
        public T Value => _blackboardData.GetValue<T>(_blackDragData);
        public BlackDragNodeData(string blackDragData,T blackboardData){
            _blackDragData = blackDragData;
            _blackboardData = blackboardData;
        }
        
    }
}