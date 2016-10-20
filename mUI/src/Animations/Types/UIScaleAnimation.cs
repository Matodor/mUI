using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Other;
using mUIApp.Views;
using UnityEngine;

namespace mUIApp.Animations
{
    public static class UIScaleAnimationHelper
    {
        public static UIScaleAnimation ScaleAnimation<T>(this T obj, Vector2 startScale, Vector2 endScale) where T : UIGameObject
        {
            return new UIScaleAnimation(startScale, endScale, obj);
        }
    }

    public class UIScaleAnimation : mUIAnimation
    {
        private readonly Vector2 _startScale, _endScale;

        public UIScaleAnimation(Vector2 startScale, Vector2 endScale, UIGameObject uiGameObject) : base(uiGameObject)
        {
            _startScale = startScale;
            _endScale = endScale;
        }

        protected override void OnAnimation()
        {
            var newScale = mUIBezierHelper.Linear(CurrentEasingTime, _startScale, _endScale);
            UIGameObject.Transform.localScale = new Vector3(
                newScale.x, newScale.y, UIGameObject.Transform.localScale.z
            );
        }

        protected override void OnEndAnimation()
        {
            
        }
    }
}
