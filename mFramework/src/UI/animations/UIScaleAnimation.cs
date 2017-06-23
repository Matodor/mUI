﻿using System;
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

        protected UIScaleAnimation(UIObject animatedObject) : base(animatedObject)
        {
        }

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var scaleSettings = settings as UIScaleAnimationSettings;
            if (scaleSettings == null)
                throw new ArgumentException("UIScaleAnimation: The given settings is not UIScaleAnimationSettings");

            _startScale = scaleSettings.StartScale;
            _endScale = scaleSettings.EndScale;
            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            var newScale = BezierHelper.Linear(CurrentEasingTime, _startScale, _endScale);
            _animatedObject.Scale(newScale.x, newScale.y);
            base.OnAnimate();
        }
    }
}