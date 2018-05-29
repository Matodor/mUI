using System;

namespace mFramework.UI.Layouts
{
    public abstract class UILayoutProps : UIViewProps
    {
        
    }

    public abstract class UILayout : UIView
    {
        protected override void OnBeforeDestroy()
        {
            ChildAdded -= OnChildAdded;
            base.OnBeforeDestroy();
        }

        protected override void AfterAwake()
        {
            ChildAdded += OnChildAdded;
            base.AfterAwake();
        }

        protected abstract void OnChildAdded(IUIObject sender, IUIObject child);

        protected override void ApplyProps(UIViewProps props, IView parent)
        {
            base.ApplyProps(props, parent);
        }
    }
}