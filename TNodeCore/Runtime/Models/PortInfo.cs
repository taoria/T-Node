using System;

namespace TNodeCore.Runtime.Models{
    [Serializable]
    public class PortInfo{
        /// <summary>
        /// Port entry name is port's name ,not the portName of the port
        /// </summary>
        public string portEntryName;
        public string nodeDataId;
    }
}