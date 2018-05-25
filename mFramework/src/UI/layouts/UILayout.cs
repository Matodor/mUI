using System;

namespace mFramework.UI.Layouts
{
    public abstract class UILayoutProps : UIViewProps
    {
        
    }

    public abstract class UILayout : UIView
    {
        protected override void AfterAwake()
        {
            ChildObjectAdded += OnChildObjectAdded;
            base.AfterAwake();
        }

        protected abstract void OnChildObjectAdded(IUIObject sender, IUIObject child);

        protected override void ApplyProps(UIViewProps props, IView parent)
        {
            base.ApplyProps(props, parent);
        }
    }
}