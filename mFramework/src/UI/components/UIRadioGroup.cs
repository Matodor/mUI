using System;

namespace mFramework.UI
{
    public class UIRadioGroupProps : UIComponentProps
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

        protected override void ApplyProps(UIComponentProps props)
        {
            if (!(props is UIRadioGroupProps radioGroupSettings))
                throw new ArgumentException("UIRadioGroup: The given settings is not UIRadioGroupSettings");

            _canDeselectCurrent = radioGroupSettings.CanDeselectCurrent;

            base.ApplyProps(props);
        }

        public UIToggle AddToggle(UIToggle toggle)
        {
            SetupToggle(toggle);
            return toggle;
        }

        public UIToggle CreateToggle(UIToggleProps toggleProps)
        {
            toggleProps.DefaultSelected = false;
            var toggle = this.Toggle(toggleProps);
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
