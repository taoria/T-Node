﻿using System;
using JetBrains.Annotations;
using TNode.Models;
using Unity.VisualScripting;

namespace TNode.Attribute{
    /// <summary>
    /// Use this attribute to claim the usage of a type of node on the derived NodeData class.
    /// it can be  applied to the same node multiple times.
    /// <example>
    /// [GraphUsage(DialogueGraph)]
    /// </example>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [BaseTypeRequired(typeof(NodeData))]
    public class GraphUsageAttribute:System.Attribute{
        public Type GraphDataType;
        public GraphUsageAttribute(Type t){
            //check if the type t is graph
            if(!typeof(GraphData).IsAssignableFrom(t)){
                throw new Exception("The type must be a graph");
            }
            GraphDataType = t;
        }
    }
}