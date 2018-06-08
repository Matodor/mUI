using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mFramework.UI
{
    public static partial class UIExtensions
    {
        public static UITurnAnimation[] TurnAnimation(
            this IEnumerable<IUIObject> objs, UITurnAnimationSettings settings)
        {
            return objs.Select(uiObject => uiObject.Animation<UITurnAnimation>(settings)).ToArray();
        }

        public static UITurnAnimation TurnAnimation(this IUIObject obj,
            UITurnAnimationSettings settings)
        {
            return obj.Animation<UITurnAnimation>(settings);
        }
    }

    public class UITurnAnimationSettings : UIAnimationSettings
    {
        public float TurnValue;
        public Vector3? RotateAround;
    }

    public class UITurnAnimation : UIAnimation
    {
        public float TurnValue;
        public Vector3? RotateAround;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (!(settings is UITurnAnimationSettings rotateSettings))
                throw new ArgumentException("UITurnAnimation: The given settings is not UITurnAnimationSettings");

            TurnValue = rotateSettings.TurnValue;
            RotateAround = rotateSettings.RotateAround;

            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            UIObject.TurnAround(
                point: RotateAround.GetValueOrDefault(UIObject.Position), 
                turnValue: TurnValue * DeltaEasingTime
            );
        }
    }
}