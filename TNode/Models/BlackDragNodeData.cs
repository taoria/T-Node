using System.Runtime.InteropServices;
using Newtonsoft.Json;
using TNode.Attribute.Ports;

namespace TNode.Models{
    public class BlackDragNodeData<T>:NodeData{
        [JsonIgnore]
        private string _blackDragData;
        [JsonIgnore]
        private BlackboardData _blackboardData;

        [Output] public T value => _blackboardData.GetValue<T>(_blackDragData);
        public BlackDragNodeData(string blackDragData,BlackboardData blackboardData){
            _blackDragData = blackDragData;
            _blackboardData = blackboardData;
        }
        
    }
}