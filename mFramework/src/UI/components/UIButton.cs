using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UIButtonSettings : UIComponentSettings
    {
        public AreaType ButtonAreaType { get; set; }
    }

    public sealed class UIButton : UIComponent, IUIClickable
    {
        private UIButton(UIObject parent) : base(parent)
        {
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (!(settings is UIButtonSettings))
                throw new ArgumentException("UIButton: The given settings is not UIButtonSettings");

            base.ApplySettings(settings);
        }

        public void MouseDown(Vector2 worldPos)
        {
            throw new NotImplementedException();
        }

        public void MouseUp(Vector2 worldPos)
        {
            throw new NotImplementedException();
        }

        public void MouseDrag(Vector2 worldPos)
        {
            throw new NotImplementedException();
        }
    }
}
