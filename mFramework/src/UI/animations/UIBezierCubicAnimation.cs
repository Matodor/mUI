using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIBezierCubicAnimationSettings : UIAnimationSettings
    {
        public Vector2 FirstPoint;
        public Vector2 SecondPoint;
        public Vector2 ThirdPoint;
        public Vector2 FourthPoint;
    }

    public class UIBezierCubicAnimation : UIAnimation
    {
        private Vector2 _firstPoint;
        private Vector2 _secondPoint;
        private Vector2 _thirdPoint;
        private Vector2 _fourthPoint;
        
        protected UIBezierCubicAnimation(UIObject animatedObject) : base(animatedObject)
        {
        }

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIBezierCubicAnimationSettings bezierSettings))
                throw new ArgumentException("UIBezierCubicAnimation: The given settings is not UIBezierCubicAnimationSettings");

            _firstPoint = bezierSettings.FirstPoint;
            _secondPoint = bezierSettings.SecondPoint;
            _thirdPoint = bezierSettings.ThirdPoint;
            _fourthPoint = bezierSettings.FourthPoint;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            AnimatedObject.Pos(BezierHelper.Cubic(
                CurrentEasingTime, 
                _firstPoint, 
                _secondPoint, 
                _thirdPoint,
                _fourthPoint
            ));
        }
    }
}