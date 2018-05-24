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
        public Space RelativeTo = Space.World;
    }

    public class UIBezierCubicAnimation : UIAnimation
    {
        private Vector2 _firstPoint;
        private Vector2 _secondPoint;
        private Vector2 _thirdPoint;
        private Vector2 _fourthPoint;
        private Space _relativeTo = Space.World;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIBezierCubicAnimationSettings bezierSettings))
                throw new ArgumentException("UIBezierCubicAnimation: The given settings is not UIBezierCubicAnimationSettings");

            _relativeTo = bezierSettings.RelativeTo;
            _firstPoint = bezierSettings.FirstPoint;
            _secondPoint = bezierSettings.SecondPoint;
            _thirdPoint = bezierSettings.ThirdPoint;
            _fourthPoint = bezierSettings.FourthPoint;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            UIObject.Position(BezierHelper.Cubic(EasingTime, 
                _firstPoint, _secondPoint, _thirdPoint, _fourthPoint), _relativeTo);
        }
    }
}