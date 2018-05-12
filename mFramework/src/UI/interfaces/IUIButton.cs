using System;
using UnityEngine;

namespace mFramework.UI
{
    public interface IUIButton : IUIClickable
    {
        ClickCondition ClickCondition { get; set; }

        event UIEventHandler<IUIButton> OnClick;
        event Func<IUIButton, bool> CanClick;

        event UIEventHandler<IUIButton, Vector2> ButtonDown;
        event UIEventHandler<IUIButton, Vector2> ButtonUp;

        void Click();
    }
}