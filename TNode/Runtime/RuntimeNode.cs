using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TNode.Models;

namespace TNode.Runtime{
    public abstract class RuntimeNode{
        public object NodeData;
        public List<NodeLink> NodeLinks;
        public void ProcessThisNode(){
            
        }

    }
    public class RuntimeNode<T>:RuntimeNode where T:NodeData{
        public new T NodeData{ get; set; }

        //Links related to runtime node,for fast access.only remember out links
        public List<NodeLink> NodeLinks;

        public void OnCreate(){
            RuntimeCache.RuntimeCache.Instance.RegisterRuntimeNode<T>();
        }
    }
}