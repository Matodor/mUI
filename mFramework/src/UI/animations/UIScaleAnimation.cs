using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIScaleAnimationSettings : UIAnimationSettings
    {
        public Vector2 StartScale { get; set; }
        public Vector2 EndScale { get; set; }
    }

    public class UIScaleAnimation : UIAnimation
    {
        private Vector2 _startScale;
        private Vector2 _endScale;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIScaleAnimationSettings scaleSettings))
                throw new ArgumentException("UIScaleAnimation: The given settings is not UIScaleAnimationSettings");

            _startScale = scaleSettings.StartScale;
            _endScale = scaleSettings.EndScale;
            
            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            UIObject.Scale = BezierHelper.Linear(EasingTime, _startScale, _endScale);
        }
    }
}
