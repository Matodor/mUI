using System;

namespace mFramework.UI
{
    public delegate void UIEventHandler<in TSender>(TSender sender) where TSender : UIObject;
    public delegate void UIEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e) where TSender : UIObject
        where TEventArgs : EventArgs;
}
