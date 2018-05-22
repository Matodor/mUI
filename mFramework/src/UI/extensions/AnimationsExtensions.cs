using System.Collections.Generic;
using System.Linq;

namespace mFramework.UI
{
    public static partial class UIExtensions
    {
        // BezierQuadraticAnimation
        public static IEnumerable<UIBezierQuadraticAnimation> BezierQuadraticAnimation(
            this IEnumerable<IUIObject> objs, UIBezierQuadraticAnimationSettings settings)
        {
            return objs.Select(uiObject => uiObject.Animation<UIBezierQuadraticAnimation>(settings)).ToArray();
        }

        public static UIBezierQuadraticAnimation BezierQuadraticAnimation(this IUIObject obj,
            UIBezierQuadraticAnimationSettings settings)
        {
            return obj.Animation<UIBezierQuadraticAnimation>(settings);
        }

        // BezierCubicAnimation
        public static IEnumerable<UIBezierCubicAnimation> BezierCubicAnimation(
            this IEnumerable<IUIObject> objs, UIBezierCubicAnimationSettings settings)
        {
            return objs.Select(uiObject => uiObject.Animation<UIBezierCubicAnimation>(settings)).ToArray();
        }

        public static UIBezierCubicAnimation BezierCubicAnimation(this IUIObject obj,
            UIBezierCubicAnimationSettings settings)
        {
            return obj.Animation<UIBezierCubicAnimation>(settings);
        }

        // ColorAnimation
        public static IEnumerable<UIColorAnimation> ColorAnimation(
            this IEnumerable<IUIColored> objs, UIColorAnimationSettings settings)
        {
            return objs.Select(uiObject => uiObject.Animation<UIColorAnimation>(settings)).ToArray();
        }

        public static UIColorAnimation ColorAnimation(this IUIColored obj, UIColorAnimationSettings settings)
        {
            return obj.Animation<UIColorAnimation>(settings);
        }

        // LinearAnimation
        public static IEnumerable<UILinearAnimation> LinearAnimation(
            this IEnumerable<IUIObject> objs, UILinearAnimationSettings settings)
        {
            return objs.Select(uiObject => uiObject.Animation<UILinearAnimation>(settings)).ToArray();
        }

        public static UILinearAnimation LinearAnimation(this IUIObject obj, 
            UILinearAnimationSettings settings)
        {
            return obj.Animation<UILinearAnimation>(settings);
        }

        // RotateAnimation
        public static IEnumerable<UIRotateAnimation> RotateAnimation(
            this IEnumerable<IUIObject> objs, UIRotateAnimationSettings settings)
        {
            return objs.Select(uiObject => uiObject.Animation<UIRotateAnimation>(settings)).ToArray();
        }

        public static UIRotateAnimation RotateAnimation(this IUIObject obj, 
            UIRotateAnimationSettings settings)
        {
            return obj.Animation<UIRotateAnimation>(settings);
        }

        // ScaleAnimation
        public static IEnumerable<UIScaleAnimation> ScaleAnimation(
            this IEnumerable<IUIObject> objs, UIScaleAnimationSettings settings)
        {
            return objs.Select(uiObject => uiObject.Animation<UIScaleAnimation>(settings)).ToArray();
        }

        public static UIScaleAnimation ScaleAnimation(this IUIObject obj, 
            UIScaleAnimationSettings settings)
        {
            return obj.Animation<UIScaleAnimation>(settings);
        }
    }
}