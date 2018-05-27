using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIScaleAnimationSettings : UIAnimationSettings
    {
        public Vector2 StartScale { get; set; }
        public Vector2 EndScale { get; set; }
        public UIAnchor? Anchor { get; set; } = null;
    }

    public class UIScaleAnimation : UIAnimation
    {
        public Vector2 StartScale;
        public Vector2 EndScale;
        public UIAnchor? Anchor;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (!(settings is UIScaleAnimationSettings scaleSettings))
                throw new ArgumentException("UIScaleAnimation: The given settings is not UIScaleAnimationSettings");

            StartScale = scaleSettings.StartScale;
            EndScale = scaleSettings.EndScale;
            Anchor = scaleSettings.Anchor;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            UIObject.Scale(
                BezierHelper.Linear(EasingTime, StartScale, EndScale),
                Anchor.GetValueOrDefault(UIObject.Anchor)
            );
        }
    }
}
