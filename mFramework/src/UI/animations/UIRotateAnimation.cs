using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIRotateAnimationSettings : UIAnimationSettings
    {
        public float FromAngle;
        public float ToAngle;
        public Space RelativeTo = Space.World;
    }

    public class UIRotateAnimation : UIAnimation
    {
        public float FromAngle;
        public float EndAngle;
        public Space RelativeTo = Space.World;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (!(settings is UIRotateAnimationSettings rotateSettings))
                throw new ArgumentException("UILinearAnimation: The given settings is not UIRotateAnimationSettings");

            FromAngle = rotateSettings.FromAngle;
            EndAngle = rotateSettings.ToAngle;
            RelativeTo = rotateSettings.RelativeTo;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            UIObject.Rotation(BezierHelper.Linear(EasingTime, FromAngle, EndAngle), 
                RelativeTo);
        }
    }
}
