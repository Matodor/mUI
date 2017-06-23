using System;

namespace mFramework.UI
{
    public class UIRotateAnimationSettings : UIAnimationSettings
    {
        public float FromAngle { get; set; } = 0;
        public float ToAngle { get; set; } = 0;
    }

    public class UIRotateAnimation : UIAnimation
    {
        private float _fromAngle;
        private float _endAngle;

        protected UIRotateAnimation(UIObject animatedObject) : base(animatedObject)
        {
        }

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var rotateSettings = settings as UIRotateAnimationSettings;
            if (rotateSettings == null)
                throw new ArgumentException("UILinearAnimation: The given settings is not UIRotateAnimationSettings");

            _fromAngle = rotateSettings.FromAngle;
            _endAngle = rotateSettings.ToAngle;
            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            var newAngle = BezierHelper.Linear(CurrentEasingTime, _fromAngle, _endAngle);
            _animatedObject.Rotate(newAngle);
            base.OnAnimate();
        }
    }
}
