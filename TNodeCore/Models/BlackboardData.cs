﻿using System;

namespace TNodeCore.Models{
    /// <summary>
    /// Black board data can store scene data
    /// </summary>
    [Serializable]
    
    public class BlackboardData:IModel,ICloneable{
        public object Clone(){
            return this.MemberwiseClone();
        }
    }
}