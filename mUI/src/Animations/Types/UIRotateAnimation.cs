using mUIApp.Other;
using mUIApp.Views;
using UnityEngine;

namespace mUIApp.Animations
{
    public static class UIRotateAnimationHelper
    {
        public static UIRotateAnimation RotateAnimation<T>(this T obj, float fromAngle, float endAngle) where T : UIGameObject
        {
            return new UIRotateAnimation(fromAngle, endAngle, obj);
        }
    }

    public class UIRotateAnimation : mUIAnimation
    {
        private readonly float _fromAngle, _endAngle;

        public UIRotateAnimation(float fromAngle, float endAngle, UIGameObject uiGameObject) : base(uiGameObject)
        {
            _fromAngle = fromAngle;
            _endAngle = endAngle;
        }

        protected override void OnAnimation()
        {
            float newAngle = mUIBezierHelper.Linear(CurrentEasingTime, _fromAngle, _endAngle);
            UIGameObject.Transform.eulerAngles = new Vector3(
                UIGameObject.Transform.eulerAngles.x,
                UIGameObject.Transform.eulerAngles.y,
                newAngle
            );
        }

        protected override void OnEndAnimation()
        {
            
        }
    }
}
