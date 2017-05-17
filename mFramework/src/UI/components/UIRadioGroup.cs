using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mFramework.UI
{
    public class UIRadioGroupSettings : UIComponentSettings
    {
        
    }

    public class UIRadioGroup : UIComponent
    {
        private UIToggle _currentActive;
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
            
            base.ApplySettings(settings);
        }

        public UIToggle AddToggle(UIToggleSettings toggleSettings)
        {
            toggleSettings.DefaultSelected = false;
            
            var toggle = Component<UIToggle>(toggleSettings);
            
            _toggles.Add(toggle);
            return toggle;
        }
    }
}
