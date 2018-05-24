using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UILinearAnimationSettings : UIAnimationSettings
    {
        public Vector2 StartPos;
        public Vector2 EndPos;
        public Space RelativeTo = Space.World;
    }

    public class UILinearAnimation : UIAnimation
    {
        private Vector3 _startPos;
        private Vector3 _endPos;
        private Space _relativeTo = Space.World;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UILinearAnimationSettings linearSettings))
                throw new ArgumentException("UILinearAnimation: The given settings is not UILinearAnimationSettings");

            _startPos = linearSettings.StartPos;
            _endPos = linearSettings.EndPos;
            _relativeTo = linearSettings.RelativeTo;
            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            UIObject.Position(BezierHelper.Linear(EasingTime, _startPos, _endPos),
                _relativeTo);
        }
    }
}
