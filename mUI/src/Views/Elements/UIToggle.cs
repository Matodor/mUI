using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp.Views.Elements
{
    public enum ToggleState
    {
        DISABLED,
        ENABLED
    }

    public static partial class UIElementsHelper
    {
        public static UIToggle CreateToggle(this UIObject obj, Sprite disabled, Sprite hoverSprite = null,
            Sprite enabled = null, string objName = "Toggle")
        {
            return new UIToggle(obj, disabled, hoverSprite, enabled).SetName(objName);
        }
    } 

    public sealed class UIToggle : UIButton
    {
        public ToggleState State { get { return _toggleState; } }
        public event Action<UIToggle, ToggleState> OnChange;

        private ToggleState _toggleState;
        private object _onEnableClickArgs;
        private object _onDisableClickArgs;
        private event Action<UIObject, object> _onEnableClick;
        private event Action<UIObject, object> _onDisableClick;
        private Sprite _disabledSprite;
        private Sprite _enabledSprite;

        public UIToggle(UIObject obj, Sprite disabled, Sprite hoverSprite = null, Sprite enabled = null) :
            base(obj, disabled, hoverSprite)
        {
            _disabledSprite = disabled;
            _enabledSprite = enabled;
            _toggleState = ToggleState.DISABLED;

            base.Click(ChangeState);
        }

        private void ChangeState(UIObject sender, object obj)
        {
            if (_toggleState == ToggleState.DISABLED)
            {
                SetEnabled();
            }
            else if (_toggleState == ToggleState.ENABLED)
            {
                SetDisabled();
            }
        }

        public UIToggle UpdateSprite(Sprite sprite, ToggleState state)
        {
            if (sprite != null)
            {
                if (state == ToggleState.ENABLED)
                {
                    _enabledSprite = sprite;
                    if (_toggleState == ToggleState.ENABLED)
                        UpdateSprite(_enabledSprite, UIButtonState.ACTIVE);
                }
                else if (state == ToggleState.DISABLED)
                {
                    _disabledSprite = sprite;
                    if (_toggleState == ToggleState.ENABLED)
                        UpdateSprite(_disabledSprite, UIButtonState.ACTIVE);
                }
            }

            return this;
        }

        public UIToggle SetEnabled()
        {
            _onEnableClick?.Invoke(this, _onEnableClickArgs);
            _toggleState = ToggleState.ENABLED;
            UpdateSprite(_enabledSprite, UIButtonState.ACTIVE);
            OnChange?.Invoke(this, _toggleState);
            return this;
        }

        public UIToggle SetDisabled()
        {
            _onDisableClick?.Invoke(this, _onDisableClickArgs);
            _toggleState = ToggleState.DISABLED;
            UpdateSprite(_disabledSprite, UIButtonState.ACTIVE);
            OnChange?.Invoke(this, _toggleState);
            return this;
        }

        public UIToggle Disable(Action<UIObject, object> onClick, object args = null)
        {
            _onDisableClick += onClick;
            _onDisableClickArgs = args;
            return this;
        }

        public UIToggle Enable(Action<UIObject, object> onClick, object args = null)
        {
            _onEnableClick += onClick;
            _onEnableClickArgs = args;
            return this;
        }

        public override UIButton ClickCondition(UIClickCondition condition)
        {
            return this;
        }

        public override UIButton Click(Action<UIObject, object> onClick, object args = null)
        {
            return this;
        }
    }
}
