using System;
using mFramework.UI;

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
        public static UIContainer Container(this IUIObject obj)
        {
            return obj.Component<UIContainer>(null);
        }

        public static UIRectAreaButton RectAreaButton(this IUIObject obj, UIRectAreaButtonSettings settings)
        {
            return obj.Component<UIRectAreaButton>(settings);
        }

        public static UITextBox TextBox(this IUIObject obj, UITextBoxSettings settings)
        {
            return obj.Component<UITextBox>(settings);
        }

        public static UIPageSlider PageSlider(this IUIObject obj, UISliderSettings settings)
        {
            return obj.Component<UIPageSlider>(settings);
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
        
        public static UISlider Slider(this IUIObject obj, UISliderSettings settings)
        {
            return obj.Component<UISlider>(settings);
        }
        
        public static UISprite Sprite(this IUIObject obj, UISpriteSettings settings)
        {
            return obj.Component<UISprite>(settings);
        }
        
        public static UIToggle Toggle(this IUIObject obj, UIToggleSettings settings)
        {
            return obj.Component<UIToggle>(settings);
        }

        // animations
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
    }
}