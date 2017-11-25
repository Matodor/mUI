using System;
using mFramework.UI;

namespace mFramework
{
    public static class UIObjectExtension
    {
        // views
        public static UIView ChildView(this IView view, Type viewType, params object[] @params)
        {
            return ChildView(view, viewType, new UIViewSettings
            {
                Width = ((UIObject)view).GetWidth(),
                Height = ((UIObject)view).GetHeight()
            }, @params);
        }

        public static UIView ChildView(this IView view, Type viewType, UIViewSettings settings, params object[] @params)
        {
            return UIView.Create(viewType, settings, (UIObject)view, @params);
        }

        public static T ChildView<T>(this IView view, params object[] @params) where T : UIView, new()
        {
            return ChildView<T>(view, new UIViewSettings
            {
                Width = ((UIObject)view).GetWidth(),
                Height = ((UIObject)view).GetHeight()
            }, @params);
        }

        public static T ChildView<T>(this IView view, UIViewSettings settings, params object[] @params) where T : UIView, new()
        {
            return UIView.Create<T>(settings, (UIObject) view, @params);
        }

        // components
        public static UITextBox TextBox(this UIObject uiObject, UITextBoxSettings settings)
        {
            return uiObject.Component<UITextBox>(settings);
        }

        public static UIPageSlider PageSlider(this UIObject uiObject, UISliderSettings settings)
        {
            return uiObject.Component<UIPageSlider>(settings);
        }

        public static UIMesh Mesh(this UIObject uiObject, UIMeshSettings settings)
        {
            return uiObject.Component<UIMesh>(settings);
        }

        public static UIButton Button(this UIObject uiObject, UIButtonSettings settings)
        {
            return uiObject.Component<UIButton>(settings);
        }
        
        public static UILabel Label(this UIObject uiObject, UILabelSettings settings)
        {
            return uiObject.Component<UILabel>(settings);
        }
        
        public static UIRadioGroup RadioGroup(this UIObject uiObject, UIRadioGroupSettings settings)
        {
            return uiObject.Component<UIRadioGroup>(settings);
        }
        
        public static UIScrollBar ScrollBar(this UIObject uiObject, UIScrollBarSettings settings)
        {
            return uiObject.Component<UIScrollBar>(settings);
        }
        
        public static UISlider Slider(this UIObject uiObject, UISliderSettings settings)
        {
            return uiObject.Component<UISlider>(settings);
        }
        
        public static UISprite Sprite(this UIObject uiObject, UISpriteSettings settings)
        {
            return uiObject.Component<UISprite>(settings);
        }
        
        public static UIToggle Toggle(this UIObject uiObject, UIToggleSettings settings)
        {
            return uiObject.Component<UIToggle>(settings);
        }

        // animations
        public static UIBezierQuadraticAnimation BezierQuadraticAnimation(this UIObject uiObject,
            UIBezierQuadraticAnimationSettings settings)
        {
            return uiObject.Animation<UIBezierQuadraticAnimation>(settings);
        }

        public static UIColorAnimation ColorAnimation(this IColored uiObject, UIColorAnimationSettings settings)
        {
            return ((UIObject) uiObject).Animation<UIColorAnimation>(settings);
        }
        
        public static UILinearAnimation LinearAnimation(this UIObject uiObject, UILinearAnimationSettings settings)
        {
            return uiObject.Animation<UILinearAnimation>(settings);
        }
        
        public static UIRotateAnimation RotateAnimation(this UIObject uiObject, UIRotateAnimationSettings settings)
        {
            return uiObject.Animation<UIRotateAnimation>(settings);
        }
        
        public static UIScaleAnimation ScaleAnimation(this UIObject uiObject, UIScaleAnimationSettings settings)
        {
            return uiObject.Animation<UIScaleAnimation>(settings);
        }
    }
}