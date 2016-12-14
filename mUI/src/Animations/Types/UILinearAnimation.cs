using mUIApp.Other;
using mUIApp.Views;
using UnityEngine;

namespace mUIApp.Animations
{
    public static class UILinearAnimationHelper
    {
        public static UILinearAnimation LinearAnimation<T>(this T obj, Vector2 startPos, Vector2 endPos) where T : UIObject
        {
            return new UILinearAnimation(startPos, endPos, obj);
        }
    }

    public class UILinearAnimation : mUIAnimation
    {
        private readonly Vector2 _startPos, _endPos;

        public UILinearAnimation(Vector2 startPos, Vector2 endPos, UIObject uiGameObject) : base(uiGameObject)
        {
            _startPos = startPos;
            _endPos = endPos;
        }

        protected override void OnAnimation()
        {
            var newPos = mUIBezierHelper.Linear(CurrentEasingTime, _startPos, _endPos);
            UIObject.Transform.position = new Vector3(
                newPos.x, newPos.y, UIObject.Transform.position.z
            );
        }

        protected override void OnEndAnimation()
        {

        }
    }
}
