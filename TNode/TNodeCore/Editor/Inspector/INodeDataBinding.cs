﻿namespace TNodeCore.Editor.Inspector{
    public interface INodeDataBinding<out T>:INodeDataBindingBase{



        void OnBindingDataUpdate();
    }
}