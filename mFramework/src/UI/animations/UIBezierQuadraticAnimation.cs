using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIBezierQuadraticAnimationSettings : UIAnimationSettings
    {
        public Vector2 FirstPoint;
        public Vector2 SecondPoint;
        public Vector2 ThirdPoint;
    }

    public class UIBezierQuadraticAnimation : UIAnimation
    {
        private Vector2 _firstPoint;
        private Vector2 _secondPoint;
        private Vector2 _thirdPoint;

        protected UIBezierQuadraticAnimation(UIObject animatedObject) : base(animatedObject)
        {
        }

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIBezierQuadraticAnimationSettings bezierSettings))
                throw new ArgumentException("UIScaleAnimation: The given settings is not UIScaleAnimationSettings");

            _firstPoint = bezierSettings.FirstPoint;
            _secondPoint = bezierSettings.SecondPoint;
            _thirdPoint = bezierSettings.ThirdPoint;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            var newPos = BezierHelper.Quadratic(CurrentEasingTime, _firstPoint, _secondPoint, _thirdPoint);
            AnimatedObject.Position(newPos);
        }
    }
}