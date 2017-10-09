using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIToggleSettings : UIButtonSettings
    {
        public bool DefaultSelected { get; set; } = false;
    }

    public class UIToggle : UIComponent, IUIRenderer, IColored
    {
        public Renderer UIRenderer => _button.UIRenderer;
        public bool IsSelected => _isSelected;

        public event Func<UIToggle, bool> CanSelect;
        public event Func<UIToggle, bool> CanDeselect;

        public event Action<UIToggle> Selected;
        public event Action<UIToggle> Deselected;
        public event Action<UIToggle> Changed;

        private bool _isSelected;
        private UIButton _button;

        protected override void Init()
        {
            _isSelected = false;
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var toggleSettings = settings as UIToggleSettings;
            if (toggleSettings == null)
                throw new ArgumentException("UIToggle: The given settings is not UIToggleSettings");

            _button = this.Button((UIButtonSettings) settings);
            _button.Click += ButtonClick;
            _button.ButtonUp += (s, e) =>
            {
                if (_isSelected)
                    _button.StateableSprite.SetSelected();
            };

            if (toggleSettings.DefaultSelected)
                Select();

            base.ApplySettings(settings);
        }

        private void ButtonClick(UIButton sender)
        {
            Toggle();
        }

        public UIToggle Toggle()
        {
            if (_isSelected)
                Deselect();
            else
                Select();

            return this;
        }

        public UIToggle Select()
        {
            if (CanSelect != null && !CanSelect(this))
                return this;

            _isSelected = true;
            _button.StateableSprite.SetSelected();

            Selected?.Invoke(this);
            Changed?.Invoke(this);
            return this;
        }

        public UIToggle Deselect()
        {
            if (CanDeselect != null && !CanDeselect(this))
                return this;

            _isSelected = false;
            _button.StateableSprite.SetDefault();

            Deselected?.Invoke(this);
            Changed?.Invoke(this);
            return this;
        }

        public override UIRect GetRect()
        {
            return _button.GetRect();
        }

        public override float GetWidth()
        {
            return _button.GetWidth();
        }

        public override float GetHeight()
        {
            return _button.GetHeight();
        }

        public Color GetColor()
        {
            return _button.GetColor();
        }

        public UIObject SetColor(Color32 color)
        {
            _button.SetColor(color);
            return this;
        }

        public UIObject SetColor(UIColor color)
        {
            _button.SetColor(color);
            return this;
        }
    }
}
