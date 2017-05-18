using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mFramework.UI
{
    public class UIRadioGroupSettings : UIComponentSettings
    {
        public bool CanDeselectCurrent { get; set; }
    }

    public class UIRadioGroup : UIComponent
    {
        public event Action<UIRadioGroup> OnSelected;
        public UIToggle CurrentSelected { get { return _currentSelected; } }

        private bool _canDeselectCurrent;
        private UIToggle _currentSelected;
        private List<UIToggle> _toggles;

        private UIRadioGroup(UIObject parent) : base(parent)
        {
            _toggles = new List<UIToggle>();
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

        public UIToggle AddToggle(UIToggleSettings toggleSettings)
        {
            toggleSettings.DefaultSelected = false;
            
            var toggle = Component<UIToggle>(toggleSettings);
            toggle.OnSelect += ToggleSelected;
            toggle.OnDeselect += ToggleDeselected;
            _toggles.Add(toggle);
            return toggle;
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
                    _currentSelected.Active();
                _currentSelected.Deselect();
            }

            _currentSelected = toggle;
            if (!_canDeselectCurrent)
                _currentSelected.Inactive();

            OnSelected?.Invoke(this);
        }
    }
}
