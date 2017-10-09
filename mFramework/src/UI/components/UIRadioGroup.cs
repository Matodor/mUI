using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework.UI
{
    public class UIRadioGroupSettings : UIComponentSettings
    {
        public bool CanDeselectCurrent { get; set; }
    }

    public class UIRadioGroup : UIComponent
    {
        public event Action<UIRadioGroup> OnSelected;
        public UIToggle CurrentSelected => _currentSelected;

        private bool _canDeselectCurrent;
        private UIToggle _currentSelected;

        protected override void Init()
        {
            СhildObjectAdded += CheckChildren;
        }

        private void CheckChildren(UIObject sender, AddedСhildObjectEventArgs e)
        {
            var toggle = e.AddedObject as UIToggle;
            if (toggle == null)
                return;
            SetupToggle(toggle);
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var radioGroupSettings = settings as UIRadioGroupSettings;
            if (radioGroupSettings == null)
                throw new ArgumentException("UIRadioGroup: The given settings is not UIRadioGroupSettings");

            _canDeselectCurrent = radioGroupSettings.CanDeselectCurrent;

            base.ApplySettings(settings);
        }

        public UIToggle AddToggle(UIToggle toggle)
        {
            SetupToggle(toggle);
            return toggle;
        }

        public UIToggle CreateToggle(UIToggleSettings toggleSettings)
        {
            toggleSettings.DefaultSelected = false;
            var toggle = this.Toggle(toggleSettings);
            SetupToggle(toggle);
            return toggle;
        }

        private void SetupToggle(UIToggle toggle)
        {
            toggle.Selected += ToggleSelected;
            toggle.Deselected += ToggleDeselected;
        }

        private void ToggleDeselected(UIToggle toggle)
        {
            _currentSelected = null;
            OnSelected?.Invoke(this);
        }

        private void ToggleSelected(UIToggle toggle)
        {
            if (_currentSelected != null && _currentSelected != toggle)
            {
                if (!_canDeselectCurrent)
                    _currentSelected.Enable();
                _currentSelected.Deselect();
            }

            _currentSelected = toggle;
            if (!_canDeselectCurrent)
                _currentSelected.Disable();

            OnSelected?.Invoke(this);
        }
    }
}
