using UnityEngine;

namespace mFramework.UI
{
    public static partial class UIExtensions
    {
        public static UIContainer Container(this IUIObject obj, UIContainerProps props)
        {
            return obj.Component<UIContainer>(props);
        }

        public static UIRectAreaButton RectAreaButton(this IUIObject obj, UIRectAreaButtonProps props)
        {
            return obj.Component<UIRectAreaButton>(props);
        }

        /*public static UITextBox TextBox(this IUIObject obj, UITextBoxSettings settings)
        {
            return obj.Component<UITextBox>(settings);
        }*/

        public static UIMesh Mesh(this IUIObject obj, UIMeshProps props)
        {
            return obj.Component<UIMesh>(props);
        }

        public static UIButton Button(this IUIObject obj, UIButtonProps props)
        {
            return obj.Component<UIButton>(props);
        }

        public static UILabel Label(this IUIObject obj, UILabelProps props)
        {
            return obj.Component<UILabel>(props);
        }

        public static UIRadioGroup RadioGroup(this IUIObject obj, UIRadioGroupProps props)
        {
            return obj.Component<UIRadioGroup>(props);
        }

        /*public static UIVerticalSlider VerticalSlider(this IUIObject obj, UISliderSettings settings)
        {
            return obj.Component<UIVerticalSlider>(settings);
        }

        public static UIHorizontalSlider HorizontalSlider(this IUIObject obj, UISliderSettings settings)
        {
            return obj.Component<UIHorizontalSlider>(settings);
        }*/

        public static UISprite Sprite(this IUIObject obj, Sprite sprite)
        {
            return obj.Component<UISprite>(new UISpriteProps
            {
                Sprite = sprite
            });
        }

        public static UISprite Sprite(this IUIObject obj, UISpriteProps props)
        {
            return obj.Component<UISprite>(props);
        }

        public static UIToggle Toggle(this IUIObject obj, UIToggleProps props)
        {
            return obj.Component<UIToggle>(props);
        }

        /*public static UIHorizontalPageSlider HorizontalPageSlider(this IUIObject obj, UIPageSliderSettings settings)
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
        }*/

        /*public static UIJoystick Joystick(this IUIObject obj, UIJoystickSettings settings)
        {
            return obj.Component<UIJoystick>(settings);
        }*/
    }
}