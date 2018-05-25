using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIToggleProps : UIButtonProps
    {
        public virtual bool DefaultSelected { get; set; } = false;
    }
    
    public class UIToggle : UIButton, IUIToggle
    {
        public bool IsSelected { get; protected set; }

        public event UIToggleAllowChangeState CanSelect = delegate { return true; };
        public event UIToggleAllowChangeState CanDeselect = delegate { return true; };

        public event UIToggleStateChangedEvent Selected = delegate { };
        public event UIToggleStateChangedEvent Deselected = delegate { };
        public event UIToggleStateChangedEvent Changed = delegate { };

        protected override void AfterAwake()
        {
            OnClick += OnToggleClick;
            MouseUp += OnUIMouseUp;

            IsSelected = false;
            base.AfterAwake();
        }

        protected override void ApplyProps(UIComponentProps props)
        {
            if (!(props is UIToggleProps toggleSettings))
                throw new ArgumentException("UIToggle: The given settings is not UIToggleSettings");

            if (toggleSettings.DefaultSelected)
                Select();
            
            base.ApplyProps(props);
        }

        private void OnUIMouseUp(IUIClickable sender, ref Vector2 vector2)
        {
            if (IsSelected)
                StateableSprite.SetSelected();
            else
                StateableSprite.SetDefault();
        }

        public IUIToggle Toggle()
        {
            if (IsSelected)
                Deselect();
            else
                Select();

            return this;
        }

        public IUIToggle Select()
        {
            if (!CanSelect(this))
                return this;

            IsSelected = true;
            StateableSprite.SetSelected();

            Selected(this);
            Changed(this);
            return this;
        }

        public IUIToggle Deselect()
        {
            if (!CanDeselect(this))
                return this;

            IsSelected = false;
            StateableSprite.SetDefault();

            Deselected(this);
            Changed(this);
            return this;
        }

        private void OnToggleClick(IUIButton sender)
        {
            Toggle();
        }
    }
}
