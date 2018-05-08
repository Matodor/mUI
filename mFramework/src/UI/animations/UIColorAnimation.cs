using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIColorAnimationSettings : UIAnimationSettings
    {
        public UIColorOldd FromColorOldd { get; set; }
        public UIColorOldd ToColorOldd { get; set; }
    }

    public class UIColorAnimation : UIAnimation
    {
        private UIColorOldd _fromColorOldd;
        private UIColorOldd _toColorOldd;
        private float n1, n2, n3, Alpha;
        private IUIColored _animatedObj;

        protected UIColorAnimation(UIObject animatedObject) : base(animatedObject)
        {
            
        }

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _animatedObj = AnimatedObject as IUIColored;
            if (_animatedObj == null)
                throw new Exception("The animated object is not IColored");

            if (!(settings is UIColorAnimationSettings colorSettings))
                throw new ArgumentException("UIColorAnimation: The given settings is not UIColorAnimationSettings");

            if (colorSettings.FromColorOldd.OldType != colorSettings.ToColorOldd.OldType)
                throw new Exception("UIColorAnimationSettings.FromColor.Type != UIColorAnimationSettings.ToColor.Type");

            _fromColorOldd = colorSettings.FromColorOldd;
            _toColorOldd = colorSettings.ToColorOldd;

            n1 = _fromColorOldd.n1;
            n2 = _fromColorOldd.n2;
            n3 = _fromColorOldd.n3;
            Alpha = _fromColorOldd.Alpha;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            n1 = BezierHelper.Linear(CurrentEasingTime, _fromColorOldd.n1, _toColorOldd.n1);
            n2 = BezierHelper.Linear(CurrentEasingTime, _fromColorOldd.n2, _toColorOldd.n2);
            n3 = BezierHelper.Linear(CurrentEasingTime, _fromColorOldd.n3, _toColorOldd.n3);
            Alpha = BezierHelper.Linear(CurrentEasingTime, _fromColorOldd.Alpha, _toColorOldd.Alpha);

            if (_fromColorOldd.OldType == UIColorOldType.RGBA)
                _animatedObj.SetColor(new Color32((byte) n1, (byte) n2, (byte) n3, (byte) Alpha));
            else
                _animatedObj.SetColor(UIColorOldd.HSVToRGB(n1, n2, n3, Alpha));
        }
    }
}
