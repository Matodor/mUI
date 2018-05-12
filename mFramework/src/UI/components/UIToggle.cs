using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIToggleSettings : UIButtonSettings
    {
        public virtual bool DefaultSelected { get; set; } = false;
    }

    public class UIToggle : UIButton
    {
        public bool IsSelected { get; private set; }

        public event Func<UIToggle, bool> CanSelect = delegate { return true; };
        public event Func<UIToggle, bool> CanDeselect = delegate { return true; };

        public event UIEventHandler<UIToggle> Selected = delegate { };
        public event UIEventHandler<UIToggle> Deselected = delegate { };
        public event UIEventHandler<UIToggle> Changed = delegate { };

        protected override void AfterAwake()
        {
            IsSelected = false;
            base.AfterAwake();
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIToggleSettings toggleSettings))
                throw new ArgumentException("UIToggle: The given settings is not UIToggleSettings");

            if (toggleSettings.DefaultSelected)
                Select();

            OnClick += OnToggleClick;
            ButtonUp += OnButtonUp;
            base.ApplySettings(settings);
        }

        private void OnButtonUp(IUIButton sender, Vector2 vector2)
        {
            if (IsSelected)
                StateableSprite.SetSelected();
            else
                StateableSprite.SetDefault();
        }

        public UIToggle Toggle()
        {
            if (IsSelected)
                Deselect();
            else
                Select();

            return this;
        }

        public UIToggle Select()
        {
            if (!CanSelect(this))
                return this;

            IsSelected = true;
            StateableSprite.SetSelected();

            Selected.Invoke(this);
            Changed.Invoke(this);
            return this;
        }

        public UIToggle Deselect()
        {
            if (!CanDeselect(this))
                return this;

            IsSelected = false;
            StateableSprite.SetDefault();

            Deselected.Invoke(this);
            Changed.Invoke(this);
            return this;
        }

        private void OnToggleClick(IUIButton sender)
        {
            Toggle();
        }
    }
}
