using System;
using System.Collections.Generic;
using System.Linq;
using mFramework.UI;

namespace mFramework
{
    public static class UIExtension
    {
        public static IEnumerable<T> DeepChilds<T>(this IUIObject obj) where T : IUIObject
        {
            foreach (var child in obj.Childs)
            {
                foreach (var c in DeepChilds<T>(child))
                    yield return c;

                if (child is T returnValue)
                    yield return returnValue;
            }
        }

        public static bool DeepFind(this IUIObject obj, Predicate<IUIObject> predicate, out IUIObject value)
        {
            if (predicate(obj))
            {
                value = obj;
                return true;
            }

            var tmp = obj.Childs.LastItem;
            while (tmp != null)
            {
                if (tmp.Value.DeepFind(predicate, out value))
                {
                    return true;
                }
                tmp = tmp.Prev;
            }

            value = null;
            return false;
        }

        #region Components
        public static UIContainer Container(this IUIObject obj, UIContainerSettings settings)
        {
            return obj.Component<UIContainer>(settings);
        }

        public static UIRectAreaButton RectAreaButton(this IUIObject obj, UIRectAreaButtonSettings settings)
        {
            return obj.Component<UIRectAreaButton>(settings);
        }

        public static UITextBox TextBox(this IUIObject obj, UITextBoxSettings settings)
        {
            return obj.Component<UITextBox>(settings);
        }

        public static UIMesh Mesh(this IUIObject obj, UIMeshSettings settings)
        {
            return obj.Component<UIMesh>(settings);
        }

        public static UIButton Button(this IUIObject obj, UIButtonSettings settings)
        {
            return obj.Component<UIButton>(settings);
        }
        
        public static UILabel Label(this IUIObject obj, UILabelSettings settings)
        {
            return obj.Component<UILabel>(settings);
        }
        
        public static UIRadioGroup RadioGroup(this IUIObject obj, UIRadioGroupSettings settings)
        {
            return obj.Component<UIRadioGroup>(settings);
        }
        
        public static UIVerticalSlider VerticalSlider(this IUIObject obj, UISliderSettings settings)
        {
            return obj.Component<UIVerticalSlider>(settings);
        }

        public static UIHorizontalSlider HorizontalSlider(this IUIObject obj, UISliderSettings settings)
        {
            return obj.Component<UIHorizontalSlider>(settings);
        }

        public static UISprite Sprite(this IUIObject obj, UISpriteSettings settings)
        {
            return obj.Component<UISprite>(settings);
        }
        
        public static UIToggle Toggle(this IUIObject obj, UIToggleSettings settings)
        {
            return obj.Component<UIToggle>(settings);
        }

        public static UIHorizontalPageSlider HorizontalPageSlider(this IUIObject obj, UIPageSliderSettings settings)
        {
            return obj.Component<UIHorizontalPageSlider>(settings);
        }

        public static UIVerticalPageSlider VerticalPageSlider(this IUIObject obj, UIPageSliderSettings settings)
        {
            return obj.Component<UIVerticalPageSlider>(settings);
        }

        public static UIHorizontalScrollBar HorizontalScrollBar(this IUIObject obj, UIScrollBarSettings settings)
        {
            return obj.Component<UIHorizontalScrollBar>(settings);
        }

        public static UIVerticalScrollBar VerticalScrollBar(this IUIObject obj, UIScrollBarSettings settings)
        {
            return obj.Component<UIVerticalScrollBar>(settings);
        }

        public static UIJoystick Joystick(this IUIObject obj, UIJoystickSettings settings)
        {
            return obj.Component<UIJoystick>(settings);
        }

        #endregion

        #region Animations

        // BezierQuadraticAnimation
        public static IEnumerable<UIBezierQuadraticAnimation> BezierQuadraticAnimation(this IEnumerable<IUIObject> objs,
            UIBezierQuadraticAnimationSettings settings)
        {
            return objs.Select(uiObject => uiObject.Animation<UIBezierQuadraticAnimation>(settings)).ToArray();
        }

        public static UIBezierQuadraticAnimation BezierQuadraticAnimation(this IUIObject obj,
            UIBezierQuadraticAnimationSettings settings)
        {
            return obj.Animation<UIBezierQuadraticAnimation>(settings);
        }

        // BezierCubicAnimation
        public static IEnumerable<UIBezierCubicAnimation> BezierCubicAnimation(this IEnumerable<IUIObject> objs,
            UIBezierCubicAnimationSettings settings)
        {
            return objs.Select(uiObject => uiObject.Animation<UIBezierCubicAnimation>(settings)).ToArray();
        }

        public static UIBezierCubicAnimation BezierCubicAnimation(this IUIObject obj,
            UIBezierCubicAnimationSettings settings)
        {
            return obj.Animation<UIBezierCubicAnimation>(settings);
        }

        // ColorAnimation
        public static IEnumerable<UIColorAnimation> ColorAnimation(this IEnumerable<IUIColored> objs, 
            UIColorAnimationSettings settings)
        {
            return objs.Select(uiObject => uiObject.Animation<UIColorAnimation>(settings)).ToArray();
        }

        public static UIColorAnimation ColorAnimation(this IUIColored obj, UIColorAnimationSettings settings)
        {
            return obj.Animation<UIColorAnimation>(settings);
        }

        // LinearAnimation
        public static IEnumerable<UILinearAnimation> LinearAnimation(this IEnumerable<IUIObject> objs, 
            UILinearAnimationSettings settings)
        {
            return objs.Select(uiObject => uiObject.Animation<UILinearAnimation>(settings)).ToArray();
        }

        public static UILinearAnimation LinearAnimation(this IUIObject obj, UILinearAnimationSettings settings)
        {
            return obj.Animation<UILinearAnimation>(settings);
        }

        // RotateAnimation
        public static IEnumerable<UIRotateAnimation> RotateAnimation(this IEnumerable<IUIObject> objs, 
            UIRotateAnimationSettings settings)
        {
            return objs.Select(uiObject => uiObject.Animation<UIRotateAnimation>(settings)).ToArray();
        }

        public static UIRotateAnimation RotateAnimation(this IUIObject obj, UIRotateAnimationSettings settings)
        {
            return obj.Animation<UIRotateAnimation>(settings);
        }

        // ScaleAnimation
        public static IEnumerable<UIScaleAnimation> ScaleAnimation(this IEnumerable<IUIObject> objs, 
            UIScaleAnimationSettings settings)
        {
            return objs.Select(uiObject => uiObject.Animation<UIScaleAnimation>(settings)).ToArray();
        }

        public static UIScaleAnimation ScaleAnimation(this IUIObject obj, UIScaleAnimationSettings settings)
        {
            return obj.Animation<UIScaleAnimation>(settings);
        }
        #endregion
    }
}