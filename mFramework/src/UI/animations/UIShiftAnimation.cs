using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mFramework.UI
{
    public class UIShiftAnimationSettings : UIAnimationSettings
    {
        public Vector3 Shift;
        public Space RelativeTo = Space.World;
    }

    public static partial class UIExtensions
    {
        public static UIShiftAnimation[] ShiftAnimation(
            this IEnumerable<IUIObject> objs, UIShiftAnimationSettings settings)
        {
            return objs.Select(uiObject => uiObject
                .Animation<UIShiftAnimation>(settings))
                .ToArray();
        }

        public static UIShiftAnimation ShiftAnimation(this IUIObject obj,
            UIShiftAnimationSettings settings)
        {
            return obj.Animation<UIShiftAnimation>(settings);
        }
    }

    public class UIShiftAnimation : UIAnimation
    {
        public Vector3 Shift;
        public Space RelativeTo = Space.World;

        protected override void ApplySettings(UIAnimationSettings settings)
        {
            if (!(settings is UIShiftAnimationSettings translateSettings))
                throw new ArgumentException("TranslateAnimation: The given settings is not TranslateAnimationSettings");

            Shift = translateSettings.Shift;
            RelativeTo = translateSettings.RelativeTo;
            
            base.ApplySettings(settings);
        }

        protected override void OnAnimate()
        {
            var shift = Shift * DeltaEasingTime;
            UIObject.Translate(shift, RelativeTo);
        }
    }
}