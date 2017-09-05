using System;
using mFramework.UI;

namespace mFramework
{
    public static class UIObjectExtension
    {
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

        public static UIColorAnimation ColorAnimation(this UIObject uiObject, UIColorAnimationSettings settings)
        {
            return uiObject.Animation<UIColorAnimation>(settings);
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