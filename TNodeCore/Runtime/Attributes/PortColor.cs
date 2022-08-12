using System;
using UnityEngine;

namespace TNodeCore.Runtime.Attributes{
    /// <summary>
    /// this attribute only works on implemented types
    /// </summary>
    public class PortColorAttribute : Attribute{
        public Color Color;
        public PortColorAttribute(float r, float g, float b){
            Color = new Color(r, g, b);
        }
        public PortColorAttribute(int r, int g,int b){
            Color = new Color(r/255.0f, g/255.0f, b/255.0f);
        }
    }
}