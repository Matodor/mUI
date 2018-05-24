using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIBezierQuadraticAnimationSettings : UIAnimationSettings
    {
        public Vector2 FirstPoint;
        public Vector2 SecondPoint;
        public Vector2 ThirdPoint;
        public Space RelativeTo = Space.World;
    }

    public class UIBezierQuadraticAnimation : UIAnimation
    {
        private Vector2 _firstPoint;
        private Vector2 _secondPoint;
        private Vector2 _thirdPoint;
        private Space _relativeTo = Space.World;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIBezierQuadraticAnimationSettings bezierSettings))
                throw new ArgumentException("UIBezierQuadraticAnimation: The given settings is not UIBezierQuadraticAnimationSettings");

            _relativeTo = bezierSettings.RelativeTo;
            _firstPoint = bezierSettings.FirstPoint;
            _secondPoint = bezierSettings.SecondPoint;
            _thirdPoint = bezierSettings.ThirdPoint;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            UIObject.Position(BezierHelper.Quadratic(EasingTime,
                _firstPoint, _secondPoint, _thirdPoint), _relativeTo);
        }
    }
}