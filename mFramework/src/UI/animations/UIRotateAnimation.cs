using System;

namespace mFramework.UI
{
    public class UIRotateAnimationSettings : UIAnimationSettings
    {
        public float FromAngle;
        public float ToAngle;
        public bool IsLocal;
    }

    public class UIRotateAnimation : UIAnimation
    {
        private float _fromAngle;
        private float _endAngle;
        private bool _isLocal;

        protected UIRotateAnimation(UIObject animatedObject) : base(animatedObject)
        {
        }

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIRotateAnimationSettings rotateSettings))
                throw new ArgumentException("UILinearAnimation: The given settings is not UIRotateAnimationSettings");

            _fromAngle = rotateSettings.FromAngle;
            _endAngle = rotateSettings.ToAngle;
            _isLocal = rotateSettings.IsLocal;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            var newAngle = BezierHelper.Linear(CurrentEasingTime, _fromAngle, _endAngle);
            //if (_isLocal)
            //    AnimatedObject.LocalRotate(newAngle);
            //else
            //    AnimatedObject.Rotate(newAngle);
        }
    }
}
