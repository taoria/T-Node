﻿using System;
using JetBrains.Annotations;
using TNodeCore.Models;

namespace TNodeCore.Attribute{
    /// <summary>
    /// Use this attribute to claim the usage of a type derived IModel IModel
    /// it can be  applied to the same node multiple times.
    /// <example>
    /// [GraphUsage(DialogueGraph)]
    /// </example>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [BaseTypeRequired(typeof(IModel))]
    [UsedImplicitly]
    public class GraphUsageAttribute:System.Attribute{
        public readonly Type GraphDataType;
        public string Category;
        public GraphUsageAttribute(Type t,string category = "default"){
            //check if the type t is graph
            if(!typeof(GraphData).IsAssignableFrom(t)){
                throw new Exception("The type used on Graph Usage must be a graph");
            }
            GraphDataType = t;
            Category = category;
           
        }
    }


}