using System;
using mUIApp.Other;
using mUIApp.Views;
using mUIApp.Views.Elements;
using UnityEngine;

namespace mUIApp.Animations
{
    public static class UIColorAnimationHelper
    {
        public static UIColorAnimation ColorAnimation<T>(this T obj, mUIColor from, mUIColor to) where T : UIObject
        {
            return new UIColorAnimation(from, to, obj);
        }
    }

    public class UIColorAnimation : mUIAnimation
    {
        private readonly mUIColor _fromColor, _toColor;
        private readonly UIObject _uiObject;
        private readonly Action<mUIColor> _onSetColor;
        private readonly mUIColor _cachedColor;

        public UIColorAnimation(mUIColor from, mUIColor to, UIObject uiObject) : base(uiObject)
        {
            _cachedColor = new mUIColor(255, 255, 255, 255, from.Type);
            _uiObject = uiObject;
            _fromColor = from;
            _toColor = to;

            if (_uiObject.GetType() == typeof (UILabel))
                _onSetColor = SetLabelColor;
            else if (_uiObject.Renderer is SpriteRenderer)
                _onSetColor = SetRendererColor;
        }

        private void SetLabelColor(mUIColor color)
        {
            ((UILabel) _uiObject).Color(color);
        }

        private void SetRendererColor(mUIColor color)
        {
            ((SpriteRenderer) _uiObject.Renderer).color = color.Color32;
        }

        protected override void OnAnimation()
        {
            _cachedColor.n1 = mUIBezierHelper.Linear(CurrentEasingTime, _fromColor.n1, _toColor.n1);
            _cachedColor.n2 = mUIBezierHelper.Linear(CurrentEasingTime, _fromColor.n2, _toColor.n2);
            _cachedColor.n3 = mUIBezierHelper.Linear(CurrentEasingTime, _fromColor.n3, _toColor.n3);
            _cachedColor.Aplha = mUIBezierHelper.Linear(CurrentEasingTime, _fromColor.Aplha, _toColor.Aplha);
            _onSetColor(_cachedColor);
        }

        protected override void OnEndAnimation()
        {

        }
    }
}
