using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mFramework.UI
{
    public class UIToggleSettings : UIButtonSettings
    {
        public bool DefaultSelected { get; set; } = false;
    }

    public class UIToggle : UIComponent
    {
        public bool IsSelected { get { return _isSelected; } }
        public event Action<UIToggle> OnSelect, OnDeselect, OnChanged;

        private bool _isSelected;
        private UIButton _button;

        protected UIToggle(UIObject parent) : base(parent)
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

            _button = Component<UIButton>((UIButtonSettings) settings);
            _button.OnClick += ButtonClick;
            _button.OnMouseUp += button =>
            {
                if (_isSelected)
                    _button.StateableSprite.SetSelected();
            };

            if (toggleSettings.DefaultSelected)
                Select();

            base.ApplySettings(settings);
        }

        private void ButtonClick(UIButton button)
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
            _isSelected = true;
            _button.StateableSprite.SetSelected();

            OnSelect?.Invoke(this);
            OnChanged?.Invoke(this);
            return this;
        }

        public UIToggle Deselect()
        {
            _isSelected = false;
            _button.StateableSprite.SetDefault();

            OnDeselect?.Invoke(this);
            OnChanged?.Invoke(this);
            return this;
        }
    }
}
