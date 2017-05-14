using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mFramework.UI
{
    public sealed class UIButtonSettings : UIComponentSettings
    {
        
    }

    public sealed class UIButton : UIComponent
    {
        private MouseEventListener _eventListener;

        private UIButton(UIObject parent) : base(parent)
        {
            _eventListener = MouseEventListener.Create();
            _eventListener.OnMouseDown += @event => mCore.Log(@event.ToString());
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            base.ApplySettings(settings);
        }
    }
}
