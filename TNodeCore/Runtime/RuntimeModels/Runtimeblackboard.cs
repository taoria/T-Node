using TNodeCore.Runtime.Models;

namespace TNodeCore.Runtime{
    public class RuntimeBlackboard<T> where T:BlackboardData{
      
        public T Data { get; set; }
        
    }
}