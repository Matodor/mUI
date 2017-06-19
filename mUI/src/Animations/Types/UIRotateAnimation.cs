using mUIApp.Other;
using UnityEngine;

namespace mUIApp.Animations
{
    public static class UIRotateAnimationHelper
    {
        public static UIRotateAnimation RotateAnimation<T>(this T obj, float fromAngle, float endAngle) where T : UIObject
        {
            return new UIRotateAnimation(fromAngle, endAngle, obj);
        }
    }

    public class UIRotateAnimation : mUIAnimation
    {
        private readonly float _fromAngle, _endAngle;

        public UIRotateAnimation(float fromAngle, float endAngle, UIObject uiGameObject) : base(uiGameObject)
        {
            _fromAngle = fromAngle;
            _endAngle = endAngle;
        }

        protected override void OnAnimation()
        {
            float newAngle = mUIBezierHelper.Linear(CurrentEasingTime, _fromAngle, _endAngle);
            UIObject.Transform.eulerAngles = new Vector3(
                UIObject.Transform.eulerAngles.x,
                UIObject.Transform.eulerAngles.y,
                newAngle
            );
        }

        protected override void OnEndAnimation()
        {
            
        }
    }
}
