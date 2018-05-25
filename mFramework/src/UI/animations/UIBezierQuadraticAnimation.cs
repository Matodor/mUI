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
        public Vector2 FirstPoint;
        public Vector2 SecondPoint;
        public Vector2 ThirdPoint;
        public Space RelativeTo = Space.World;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIBezierQuadraticAnimationSettings bezierSettings))
                throw new ArgumentException("UIBezierQuadraticAnimation: The given settings is not UIBezierQuadraticAnimationSettings");

            RelativeTo = bezierSettings.RelativeTo;
            FirstPoint = bezierSettings.FirstPoint;
            SecondPoint = bezierSettings.SecondPoint;
            ThirdPoint = bezierSettings.ThirdPoint;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            UIObject.Position(BezierHelper.Quadratic(EasingTime,
                FirstPoint, SecondPoint, ThirdPoint), RelativeTo);
        }
    }
}