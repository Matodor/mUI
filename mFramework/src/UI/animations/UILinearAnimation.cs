using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UILinearAnimationSettings : UIAnimationSettings
    {
        public Vector2 StartPos;
        public Vector2 EndPos;
        public bool IsLocal = false;
    }

    public class UILinearAnimation : UIAnimation
    {
        private Vector3 _startPos;
        private Vector3 _endPos;
        private bool _isLocal;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UILinearAnimationSettings linearSettings))
                throw new ArgumentException("UILinearAnimation: The given settings is not UILinearAnimationSettings");

            _startPos = linearSettings.StartPos;
            _endPos = linearSettings.EndPos;
            _isLocal = linearSettings.IsLocal;
            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            var newPos = BezierHelper.Linear(EasingTime, _startPos, _endPos);
            if (_isLocal)
                UIObject.LocalPosition = newPos;
            else
                UIObject.Position = newPos;
        }
    }
}
