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
        public bool IsLocalPos;
    }

    public class UIBezierCubicAnimation : UIAnimation
    {
        private Vector2 _firstPoint;
        private Vector2 _secondPoint;
        private Vector2 _thirdPoint;
        private Vector2 _fourthPoint;
        private bool _isLocalPos;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIBezierCubicAnimationSettings bezierSettings))
                throw new ArgumentException("UIBezierCubicAnimation: The given settings is not UIBezierCubicAnimationSettings");

            _isLocalPos = bezierSettings.IsLocalPos;
            _firstPoint = bezierSettings.FirstPoint;
            _secondPoint = bezierSettings.SecondPoint;
            _thirdPoint = bezierSettings.ThirdPoint;
            _fourthPoint = bezierSettings.FourthPoint;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            if (_isLocalPos)
            {
                UIObject.LocalPosition = BezierHelper.Cubic(
                    EasingTime,
                    _firstPoint,
                    _secondPoint,
                    _thirdPoint,
                    _fourthPoint
                );
            }
            else
            {
                UIObject.Position = BezierHelper.Cubic(
                    EasingTime,
                    _firstPoint,
                    _secondPoint,
                    _thirdPoint,
                    _fourthPoint
                );
            }
        }
    }
}