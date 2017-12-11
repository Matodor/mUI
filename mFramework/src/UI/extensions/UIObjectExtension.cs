using System;
using mFramework.UI;
using mFramework.UI.PageSlider;

namespace mFramework
{
    public static class UIExtension
    {
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
        
        public static UIScrollBar ScrollBar(this IUIObject obj, UIScrollBarSettings settings)
        {
            return obj.Component<UIScrollBar>(settings);
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
        #endregion

        #region Animations
        public static UIBezierQuadraticAnimation BezierQuadraticAnimation(this IUIObject obj,
            UIBezierQuadraticAnimationSettings settings)
        {
            return obj.Animation<UIBezierQuadraticAnimation>(settings);
        }

        public static UIColorAnimation ColorAnimation(this IUIColored obj, UIColorAnimationSettings settings)
        {
            return obj.Animation<UIColorAnimation>(settings);
        }
        
        public static UILinearAnimation LinearAnimation(this IUIObject obj, UILinearAnimationSettings settings)
        {
            return obj.Animation<UILinearAnimation>(settings);
        }
        
        public static UIRotateAnimation RotateAnimation(this IUIObject obj, UIRotateAnimationSettings settings)
        {
            return obj.Animation<UIRotateAnimation>(settings);
        }
        
        public static UIScaleAnimation ScaleAnimation(this IUIObject obj, UIScaleAnimationSettings settings)
        {
            return obj.Animation<UIScaleAnimation>(settings);
        }
        #endregion
    }
}