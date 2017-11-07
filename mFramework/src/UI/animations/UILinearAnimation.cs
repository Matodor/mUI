using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UILinearAnimationSettings : UIAnimationSettings
    {
        public Vector2 StartPos { get; set; }
        public Vector2 EndPos { get; set; }
    }

    public class UILinearAnimation : UIAnimation
    {
        private Vector2 _startPos;
        private Vector2 _endPos;

        protected UILinearAnimation(UIObject animatedObject) : base(animatedObject)
        {

        }

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UILinearAnimationSettings linearSettings))
                throw new ArgumentException("UILinearAnimation: The given settings is not UILinearAnimationSettings");

            _startPos = linearSettings.StartPos;
            _endPos = linearSettings.EndPos;
            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            var newPos = BezierHelper.Linear(CurrentEasingTime, _startPos, _endPos);
            AnimatedObject.Position(newPos);
        }
    }
}
