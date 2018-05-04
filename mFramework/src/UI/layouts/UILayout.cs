using System;

namespace mFramework.UI.Layouts
{
    public abstract class UILayout : UIView
    {
        protected override void AfterAwake()
        {
            ChildObjectAdded += OnChildObjectAdded;
            base.AfterAwake();
        }

        protected virtual void OnChildObjectAdded(IUIObject sender, IUIObject child)
        {
            
        }
    }
}