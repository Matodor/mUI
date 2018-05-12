using System;

namespace mFramework.UI.Layouts
{
    public abstract class UILayoutSettings : UIViewSettings
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

        protected override void ApplySettings(UIViewSettings settings, IView parent)
        {
            base.ApplySettings(settings, parent);
        }
    }
}