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
        private IUIColored _animatedObj;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _animatedObj = UIObject as IUIColored;
            if (_animatedObj == null)
                throw new Exception("The animated object is not IColored");

            if (!(settings is UIColorAnimationSettings colorSettings))
                throw new ArgumentException("UIColorAnimation: The given settings is not UIColorAnimationSettings");

            if (colorSettings.FromColor.ColorType != colorSettings.ToColor.ColorType)
                throw new Exception("UIColorAnimationSettings.FromColor.Type != UIColorAnimationSettings.ToColor.Type");

            _fromColor = colorSettings.FromColor;
            _toColor = colorSettings.ToColor;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            var color = UIColor.Lerp(_fromColor, _toColor, EasingTime);
            _animatedObj.Color = (Color) color;
        }
    }
}
