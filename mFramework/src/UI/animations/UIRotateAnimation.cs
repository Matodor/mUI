using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIRotateAnimationSettings : UIAnimationSettings
    {
        public float FromAngle;
        public float ToAngle;
    }

    public class UIRotateAnimation : UIAnimation
    {
        private float _fromAngle;
        private float _endAngle;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIRotateAnimationSettings rotateSettings))
                throw new ArgumentException("UILinearAnimation: The given settings is not UIRotateAnimationSettings");

            _fromAngle = rotateSettings.FromAngle;
            _endAngle = rotateSettings.ToAngle;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            UIObject.Rotation = BezierHelper.Linear(EasingTime, _fromAngle, _endAngle);
        }
    }
}
