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

        public UIAnchor? Anchor { get; set; } = null;
    }

    public class UIBezierCubicAnimation : UIAnimation
    {
        public Vector2 FirstPoint;
        public Vector2 SecondPoint;
        public Vector2 ThirdPoint;
        public Vector2 FourthPoint;
        public Space RelativeTo = Space.World;
        public UIAnchor? Anchor;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (!(settings is UIBezierCubicAnimationSettings bezierSettings))
                throw new ArgumentException("UIBezierCubicAnimation: The given settings is not UIBezierCubicAnimationSettings");

            RelativeTo = bezierSettings.RelativeTo;
            FirstPoint = bezierSettings.FirstPoint;
            SecondPoint = bezierSettings.SecondPoint;
            ThirdPoint = bezierSettings.ThirdPoint;
            FourthPoint = bezierSettings.FourthPoint;
            Anchor = bezierSettings.Anchor;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            UIObject.Position(
                position: BezierHelper.Cubic(EasingTime, FirstPoint, SecondPoint, ThirdPoint, FourthPoint), 
                anchor: Anchor.GetValueOrDefault(UIObject.Anchor), 
                relativeTo: RelativeTo
            );
        }
    }
}