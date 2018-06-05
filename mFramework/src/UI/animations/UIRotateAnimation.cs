using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIRotateAnimationSettings : UIAnimationSettings
    {
        public float FromAngle;
        public float ToAngle;
        public Space RelativeTo = Space.World;
        public Vector3? RotateAround;
    }

    public class UIRotateAnimation : UIAnimation
    {
        public float FromAngle;
        public float ToAngle;
        public Space RelativeTo = Space.World;
        public Vector3? RotateAround;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (!(settings is UIRotateAnimationSettings rotateSettings))
                throw new ArgumentException("UILinearAnimation: The given settings is not UIRotateAnimationSettings");

            FromAngle = rotateSettings.FromAngle;
            ToAngle = rotateSettings.ToAngle;
            RelativeTo = rotateSettings.RelativeTo;
            RotateAround = rotateSettings.RotateAround;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            var rotation = BezierHelper.Linear(EasingTime, FromAngle, ToAngle);

            if (RotateAround != null)
            {
                UIObject.RotateAround(RotateAround.Value, rotation, RelativeTo);
            }
            else
            {
                UIObject.Rotation(rotation, RelativeTo);
            }
        }
    }
}
