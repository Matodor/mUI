using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIBezierQuadraticAnimationSettings : UIAnimationSettings
    {
        public Vector2 FirstPoint;
        public Vector2 SecondPoint;
        public Vector2 ThirdPoint;
        public bool IsLocalPos;
    }

    public class UIBezierQuadraticAnimation : UIAnimation
    {
        private Vector2 _firstPoint;
        private Vector2 _secondPoint;
        private Vector2 _thirdPoint;
        private bool _isLocalPos;

        protected UIBezierQuadraticAnimation(UIObject animatedObject) : base(animatedObject)
        {
        }

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIBezierQuadraticAnimationSettings bezierSettings))
                throw new ArgumentException("UIBezierQuadraticAnimation: The given settings is not UIBezierQuadraticAnimationSettings");

            _isLocalPos = bezierSettings.IsLocalPos;
            _firstPoint = bezierSettings.FirstPoint;
            _secondPoint = bezierSettings.SecondPoint;
            _thirdPoint = bezierSettings.ThirdPoint;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            if (_isLocalPos)
            {
                AnimatedObject.LocalPosition = BezierHelper.Quadratic(
                    CurrentEasingTime,
                    _firstPoint,
                    _secondPoint,
                    _thirdPoint
                );
            }
            else
            {
                AnimatedObject.Position = BezierHelper.Quadratic(
                    CurrentEasingTime,
                    _firstPoint,
                    _secondPoint,
                    _thirdPoint
                );
            }
        }
    }
}