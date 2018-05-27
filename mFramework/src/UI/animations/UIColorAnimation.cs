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
        public UIColor FromColor;
        public UIColor ToColor;
        private IUIColored _animatedObj;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            _animatedObj = UIObject as IUIColored;
            if (_animatedObj == null)
                throw new Exception("The animated object is not IColored");

            if (!(settings is UIColorAnimationSettings colorSettings))
                throw new ArgumentException("UIColorAnimation: The given settings is not UIColorAnimationSettings");

            if (colorSettings.FromColor.ColorType != colorSettings.ToColor.ColorType)
                throw new Exception("UIColorAnimationSettings.FromColor.Type != UIColorAnimationSettings.ToColor.Type");

            FromColor = colorSettings.FromColor;
            ToColor = colorSettings.ToColor;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            _animatedObj.Color = (Color) UIColor.Lerp(FromColor, ToColor, EasingTime);
        }
    }
}
