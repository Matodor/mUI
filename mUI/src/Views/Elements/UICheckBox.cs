using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mUIApp.Views.Elements
{
    public enum CheckBoxState
    {
        UNCHECKED,
        CHECKED
    }

    public static class UICheckboxHelper
    {
        public static UICheckBox CreateCheckBox(this BaseView view, Sprite uncheck, Sprite hoverSprite = null,
            Sprite @checked = null, string objName = "CheckBox")
        {
            return new UICheckBox(view, uncheck, hoverSprite, @checked).SetName(objName);
        }
    } 

    public sealed class UICheckBox : UIButton
    {
        private CheckBoxState _checkBoxState;
        private object _onCheckClickArgs;
        private object _onUnCheckClickArgs;
        private event Action<UIObject, object> _onCheckClick;
        private event Action<UIObject, object> _onUnCheckClick;
        private Sprite _uncheckedSprite;
        private Sprite _checkedSprite;

        public UICheckBox(BaseView view, Sprite uncheck, Sprite hoverSprite = null, Sprite @checked = null) :
            base(view, uncheck, hoverSprite)
        {
            _uncheckedSprite = uncheck;
            _checkedSprite = @checked;
            _checkBoxState = CheckBoxState.UNCHECKED;

            base.Click(ChangeState);
        }

        private void ChangeState(UIObject uiObject, object obj)
        {
            if (_checkBoxState == CheckBoxState.UNCHECKED)
            {
                SetChecked();
            }
            else if (_checkBoxState == CheckBoxState.CHECKED)
            {
                SetUnChecked();
            }
        }

        public UICheckBox UpdateSprite(Sprite sprite, CheckBoxState state)
        {
            if (sprite != null)
            {
                if (state == CheckBoxState.CHECKED)
                    _checkedSprite = sprite;
                else if (state == CheckBoxState.UNCHECKED)
                    _uncheckedSprite = sprite;
            }

            return this;
        }

        public UICheckBox SetChecked()
        {
            _onCheckClick?.Invoke(this, _onCheckClickArgs);
            _checkBoxState = CheckBoxState.CHECKED;
            UpdateSprite(_checkedSprite, UIButtonState.ACTIVE);
            return this;
        }

        public UICheckBox SetUnChecked()
        {
            _onUnCheckClick?.Invoke(this, _onUnCheckClickArgs);
            _checkBoxState = CheckBoxState.UNCHECKED;
            UpdateSprite(_uncheckedSprite, UIButtonState.ACTIVE);
            return this;
        }

        public UICheckBox UnCheck(Action<UIObject, object> onClick, object args = null)
        {
            _onUnCheckClick += onClick;
            _onUnCheckClickArgs = args;
            return this;
        }

        public UICheckBox Check(Action<UIObject, object> onClick, object args = null)
        {
            _onCheckClick += onClick;
            _onCheckClickArgs = args;
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
