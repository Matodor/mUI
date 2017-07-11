using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIColorAnimationSettings : UIAnimationSettings
    {
        public UIColor FromColor { get; set; }
        public UIColor ToColor { get; set; }
    }

    public class UIColorAnimation : UIAnimation
    {
        private UIColor _fromColor;
        private UIColor _toColor;
        private float n1, n2, n3, Alpha;
        private IColored _animatedObj;

        protected UIColorAnimation(UIObject animatedObject) : base(animatedObject)
        {
        }

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _animatedObj = AnimatedObject as IColored;
            if (_animatedObj == null)
                throw new Exception("The animated object is not IColored");

            var colorSettings = settings as UIColorAnimationSettings;
            if (colorSettings == null)
                throw new ArgumentException("UIColorAnimation: The given settings is not UIColorAnimationSettings");

            if (colorSettings.FromColor.Type != colorSettings.ToColor.Type)
                throw new Exception("UIColorAnimationSettings.FromColor.Type != UIColorAnimationSettings.ToColor.Type");

            _fromColor = colorSettings.FromColor;
            _toColor = colorSettings.ToColor;

            n1 = _fromColor.n1;
            n2 = _fromColor.n2;
            n3 = _fromColor.n3;
            Alpha = _fromColor.Alpha;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            n1 = BezierHelper.Linear(CurrentEasingTime, _fromColor.n1, _toColor.n1);
            n2 = BezierHelper.Linear(CurrentEasingTime, _fromColor.n2, _toColor.n2);
            n3 = BezierHelper.Linear(CurrentEasingTime, _fromColor.n3, _toColor.n3);
            Alpha = BezierHelper.Linear(CurrentEasingTime, _fromColor.Alpha, _toColor.Alpha);

            if (_fromColor.Type == UIColorType.RGBA)
                _animatedObj.SetColor(new Color32((byte) n1, (byte) n2, (byte) n3, (byte) Alpha));
            else
                _animatedObj.SetColor(UIColor.HSVToRGB(n1, n2, n3, Alpha));

            base.OnAnimate();
        }
    }
}
