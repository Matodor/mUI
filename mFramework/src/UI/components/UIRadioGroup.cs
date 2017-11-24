﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework.UI
{
    public class UIRadioGroupSettings : UIComponentSettings
    {
        public bool CanDeselectCurrent;
    }

    public class UIRadioGroup : UIComponent
    {
        public event Action<UIRadioGroup> Selected = delegate { };
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

            if (!(settings is UIRadioGroupSettings radioGroupSettings))
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
            toggle.BeforeDestroy += ToggleOnBeforeDestroy;
            toggle.Selected += ToggleSelected;
            toggle.Deselected += ToggleDeselected;
            toggle.CanDeselect += ToggleOnCanDeselect;
        }

        private void ToggleOnBeforeDestroy(UIObject sender)
        {
            if ((UIToggle) sender == _currentSelected)
                _currentSelected = null;
        }

        private bool ToggleOnCanDeselect(UIToggle toggle)
        {
            if (_canDeselectCurrent)
            {
                return true;
            }

            if (_currentSelected == toggle)
                return false;
            return true;
        }

        private void ToggleDeselected(UIToggle toggle)
        {
            if (_currentSelected == toggle)
                _currentSelected = null;
        }

        private void ToggleSelected(UIToggle toggle)
        {
            var prev = _currentSelected;
            _currentSelected = toggle;

            if (prev != null && prev != toggle)
                prev.Deselect();

            Selected.Invoke(this);
        }
    }
}
