﻿using System;
using TNodeCore.Runtime.Models;

namespace TNodeCore.Editor.Tools.NodeCreator{
    public static class NodeCreator{
        
        /// <summary>
        /// always use this to create a new node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T InstantiateNodeData<T>() where T:NodeData{
            var res = Activator.CreateInstance<T>();
            res.id = Guid.NewGuid().ToString();
            return res;
        }
        public static NodeData InstantiateNodeData(Type type){
            if (Activator.CreateInstance(type) is NodeData res){
                res.id = Guid.NewGuid().ToString();
                return res;
            }

            return null;
        }
    }
}