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
        public Vector3 StartPos;
        public Vector3 EndPos;
        public Space RelativeTo = Space.World;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UILinearAnimationSettings linearSettings))
                throw new ArgumentException("UILinearAnimation: The given settings is not UILinearAnimationSettings");

            StartPos = linearSettings.StartPos;
            EndPos = linearSettings.EndPos;
            RelativeTo = linearSettings.RelativeTo;
            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            UIObject.Position(BezierHelper.Linear(EasingTime, StartPos, EndPos),
                RelativeTo);
        }
    }
}
