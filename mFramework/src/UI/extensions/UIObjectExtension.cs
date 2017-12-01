using System;
using mFramework.UI;

namespace mFramework
{
    public static class UIExtension
    {
        // views
        public static UIView ChildView(this IView view, Type viewType, params object[] @params)
        {
            return ChildView(view, viewType, new UIViewSettings
            {
                Width = view.GetWidth(),
                Height = view.GetHeight()
            }, @params);
        }

        public static UIView ChildView(this IView view, Type viewType, UIViewSettings settings, params object[] @params)
        {
            return UIView.Create(viewType, settings, view, @params);
        }

        public static T ChildView<T>(this IView view, params object[] @params) where T : UIView, new()
        {
            return ChildView<T>(view, new UIViewSettings
            {
                Width = view.GetWidth(),
                Height = view.GetHeight()
            }, @params);
        }

        public static T ChildView<T>(this IView view, UIViewSettings settings, params object[] @params) where T : UIView, new()
        {
            return UIView.Create<T>(settings, view, @params);
        }

        // components
        public static UIContainer Container(this IUIObject IUIObject)
        {
            return IUIObject.Component<UIContainer>(null);
        }

        public static UIRectAreaButton RectAreaButton(this IUIObject IUIObject, UIRectAreaButtonSettings settings)
        {
            return IUIObject.Component<UIRectAreaButton>(settings);
        }

        public static UITextBox TextBox(this IUIObject IUIObject, UITextBoxSettings settings)
        {
            return IUIObject.Component<UITextBox>(settings);
        }

        public static UIPageSlider PageSlider(this IUIObject IUIObject, UISliderSettings settings)
        {
            return IUIObject.Component<UIPageSlider>(settings);
        }

        public static UIMesh Mesh(this IUIObject IUIObject, UIMeshSettings settings)
        {
            return IUIObject.Component<UIMesh>(settings);
        }

        public static UIButton Button(this IUIObject IUIObject, UIButtonSettings settings)
        {
            return IUIObject.Component<UIButton>(settings);
        }
        
        public static UILabel Label(this IUIObject IUIObject, UILabelSettings settings)
        {
            return IUIObject.Component<UILabel>(settings);
        }
        
        public static UIRadioGroup RadioGroup(this IUIObject IUIObject, UIRadioGroupSettings settings)
        {
            return IUIObject.Component<UIRadioGroup>(settings);
        }
        
        public static UIScrollBar ScrollBar(this IUIObject IUIObject, UIScrollBarSettings settings)
        {
            return IUIObject.Component<UIScrollBar>(settings);
        }
        
        public static UISlider Slider(this IUIObject IUIObject, UISliderSettings settings)
        {
            return IUIObject.Component<UISlider>(settings);
        }
        
        public static UISprite Sprite(this IUIObject IUIObject, UISpriteSettings settings)
        {
            return IUIObject.Component<UISprite>(settings);
        }
        
        public static UIToggle Toggle(this IUIObject IUIObject, UIToggleSettings settings)
        {
            return IUIObject.Component<UIToggle>(settings);
        }

        // animations
        public static UIBezierQuadraticAnimation BezierQuadraticAnimation(this IUIObject IUIObject,
            UIBezierQuadraticAnimationSettings settings)
        {
            return IUIObject.Animation<UIBezierQuadraticAnimation>(settings);
        }

        public static UIColorAnimation ColorAnimation(this IUIColored IUIObject, UIColorAnimationSettings settings)
        {
            return IUIObject.Animation<UIColorAnimation>(settings);
        }
        
        public static UILinearAnimation LinearAnimation(this IUIObject IUIObject, UILinearAnimationSettings settings)
        {
            return IUIObject.Animation<UILinearAnimation>(settings);
        }
        
        public static UIRotateAnimation RotateAnimation(this IUIObject IUIObject, UIRotateAnimationSettings settings)
        {
            return IUIObject.Animation<UIRotateAnimation>(settings);
        }
        
        public static UIScaleAnimation ScaleAnimation(this IUIObject IUIObject, UIScaleAnimationSettings settings)
        {
            return IUIObject.Animation<UIScaleAnimation>(settings);
        }
    }
}