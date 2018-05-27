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
        public Vector2 FirstPoint;
        public Vector2 SecondPoint;
        public Vector2 ThirdPoint;
        public Vector2 FourthPoint;
        public Space RelativeTo = Space.World;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (!(settings is UIBezierCubicAnimationSettings bezierSettings))
                throw new ArgumentException("UIBezierCubicAnimation: The given settings is not UIBezierCubicAnimationSettings");

            RelativeTo = bezierSettings.RelativeTo;
            FirstPoint = bezierSettings.FirstPoint;
            SecondPoint = bezierSettings.SecondPoint;
            ThirdPoint = bezierSettings.ThirdPoint;
            FourthPoint = bezierSettings.FourthPoint;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            UIObject.Position(BezierHelper.Cubic(EasingTime, 
                FirstPoint, SecondPoint, ThirdPoint, FourthPoint), RelativeTo);
        }
    }
}