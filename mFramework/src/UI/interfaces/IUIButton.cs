using System;
using UnityEngine;

namespace mFramework.UI
{
    public delegate void UIButtonClickEvent(IUIButton sender);
    public delegate bool UIButtonAllowClick(IUIButton sender);

    public interface IUIButton : IUIClickable
    {
        ClickCondition ClickCondition { get; set; }

        event UIButtonClickEvent OnClick;
        event UIButtonAllowClick CanClick;

        void Click();
    }
}