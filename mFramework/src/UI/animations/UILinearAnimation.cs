using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UILinearAnimationSettings : UIAnimationSettings
    {
        public Vector2 StartPos;
        public Vector2 EndPos;
        public Space RelativeTo = Space.World;
        public UIAnchor? Anchor { get; set; } = null;
    }

    public class UILinearAnimation : UIAnimation
    {
        public Vector3 StartPos;
        public Vector3 EndPos;
        public Space RelativeTo = Space.World;
        public UIAnchor? Anchor;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (!(settings is UILinearAnimationSettings linearSettings))
                throw new ArgumentException("UILinearAnimation: The given settings is not UILinearAnimationSettings");

            StartPos = linearSettings.StartPos;
            EndPos = linearSettings.EndPos;
            RelativeTo = linearSettings.RelativeTo;
            Anchor = linearSettings.Anchor;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            UIObject.Position(
                position: BezierHelper.Linear(EasingTime, StartPos, EndPos),
                anchor: Anchor.GetValueOrDefault(UIObject.Anchor),
                relativeTo: RelativeTo
            );
        }
    }
}
