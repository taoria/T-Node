using System.Collections.Generic;
using TNodeCore.Runtime.Models;
using UnityEngine;

namespace TNodeCore.Runtime{
    public static class NodeLogger{
        public static Dictionary<string, INodeLoggerImpl> Loggers = new  Dictionary<string, INodeLoggerImpl>();

        public static void Log(this NodeData t,string message){
            if (!Loggers.ContainsKey(t.id)) return;
            var nodeLoggerImpl = Loggers[t.id];
            nodeLoggerImpl.Log(message);
            Debug.Log(message);
            
        }
    }

    public interface INodeLoggerImpl{
        public void Log(string message);
    }
}