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
        public IUIToggle CurrentSelected { get; private set; }

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

        private void SetupToggle(IUIToggle toggle)
        {
            toggle.BeforeDestroy += ToggleOnBeforeDestroy;
            toggle.Selected += ToggleSelected;
            toggle.Deselected += ToggleDeselected;
            toggle.CanDeselect += ToggleOnCanDeselect;
        }

        private void ToggleOnBeforeDestroy(IUIObject sender)
        {
            if (sender == CurrentSelected)
                CurrentSelected = null;
        }

        private bool ToggleOnCanDeselect(IUIToggle toggle)
        {
            if (_canDeselectCurrent)
                return true;
            return CurrentSelected != toggle;
        }

        private void ToggleDeselected(IUIToggle toggle)
        {
            if (CurrentSelected == toggle)
                CurrentSelected = null;
        }

        private void ToggleSelected(IUIToggle toggle)
        {
            var prev = CurrentSelected;
            CurrentSelected = toggle;

            if (prev != null && prev != toggle)
                prev.Deselect();

            Selected.Invoke(this);
        }
    }
}
