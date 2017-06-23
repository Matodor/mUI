﻿using mUIApp.Other;
using UnityEngine;

namespace mUIApp.Animations
{
    public static class UIScaleAnimationHelper
    {
        public static UIScaleAnimation ScaleAnimation<T>(this T obj, Vector2 startScale, Vector2 endScale) where T : UIObject
        {
            return new UIScaleAnimation(startScale, endScale, obj);
        }
    }

    public class UIScaleAnimation : mUIAnimation
    {
        private readonly Vector2 _startScale, _endScale;

        public UIScaleAnimation(Vector2 startScale, Vector2 endScale, UIObject uiGameObject) : base(uiGameObject)
        {
            _startScale = startScale;
            _endScale = endScale;
        }

        protected override void OnAnimation()
        {
            var newScale = mUIBezierHelper.Linear(CurrentEasingTime, _startScale, _endScale);
            UIObject.Transform.localScale = new Vector3(
                newScale.x, newScale.y, UIObject.Transform.localScale.z
            );
        }

        protected override void OnEndAnimation()
        {
            
        }
    }
}
