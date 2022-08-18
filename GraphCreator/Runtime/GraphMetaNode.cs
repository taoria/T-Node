using System;
using System.Collections.Generic;
using TNodeCore.Runtime.Models;

namespace TNodeCore.GraphCreator.Runtime{
    

    public class GraphMetaNode : NodeData{
        public string NodeName;
        public string NodeInheritanceType;
        public List<Tuple<Type,string>> NodeFiled;

    }
}