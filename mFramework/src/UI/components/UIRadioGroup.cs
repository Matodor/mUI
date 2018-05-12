using System;

namespace mFramework.UI
{
    public class UIRadioGroupSettings : UIComponentSettings
    {
        public virtual bool CanDeselectCurrent { get; set; }
    }

    public class UIRadioGroup : UIComponent
    {
        public event Action<UIRadioGroup> Selected = delegate { };
        public UIToggle CurrentSelected { get; private set; }

        private bool _canDeselectCurrent;

        protected override void AfterAwake()
        {
            ChildObjectAdded += CheckChildren;
            base.AfterAwake();
        }

        private void CheckChildren(IUIObject sender, IUIObject addedObj)
        {
            var toggle = addedObj as UIToggle;
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

        private void ToggleOnBeforeDestroy(IUIObject sender)
        {
            if ((UIToggle) sender == CurrentSelected)
                CurrentSelected = null;
        }

        private bool ToggleOnCanDeselect(UIToggle toggle)
        {
            if (_canDeselectCurrent)
            {
                return true;
            }

            if (CurrentSelected == toggle)
                return false;
            return true;
        }

        private void ToggleDeselected(UIToggle toggle)
        {
            if (CurrentSelected == toggle)
                CurrentSelected = null;
        }

        private void ToggleSelected(UIToggle toggle)
        {
            var prev = CurrentSelected;
            CurrentSelected = toggle;

            if (prev != null && prev != toggle)
                prev.Deselect();

            Selected.Invoke(this);
        }
    }
}
