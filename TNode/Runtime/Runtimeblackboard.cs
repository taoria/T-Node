using System.Collections.Generic;
using TNode.Models;

namespace TNode.Runtime{
    public class RuntimeBlackboard<T> where T:BlackboardData{
      
        public T Data { get; set; }
        
    }
}