﻿using System;
using System.Collections;
using System.Collections.Generic;
using TNodeCore.Runtime.Attributes;
using UnityEngine;

namespace TNodeCore.Runtime.Models{
    /// <summary>
    /// this class is used to store the data of a node
    /// inherit it to implement your own node
    /// when declare a port for this node,you can use attribute [PortTypeName] on a field to claim a port.a port will not be inspected by default inspector.
    /// fields that are not marked with [PortTypeName] will be inspected by default inspector.
    /// 
    /// </summary>
    [Serializable]
    public class NodeData:Model{
   
        public NodeData() : base(){
            //Object Registration
           
        }

        public string nodeName;
        public bool entryPoint;

        public virtual void Process(){
            
        }

  
        
        public virtual IEnumerator AfterProcess(){
            yield return null;
        }

        
#if UNITY_EDITOR
        [HideInInspector] public bool isTest;
        public virtual void OnTest(){
            
        }
 #endif

        
    }


}