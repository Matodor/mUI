using mFramework.UI;
using UnityEngine;

namespace mFramework
{
    public static partial class UIExtensions
    {
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
            return obj.Component<UISprite>(new UISpriteSettings
            {
                Sprite = sprite
            });
        }

        public static UISprite Sprite(this IUIObject obj, UISpriteSettings settings)
        {
            return obj.Component<UISprite>(settings);
        }

        public static UIToggle Toggle(this IUIObject obj, UIToggleSettings settings)
        {
            return obj.Component<UIToggle>(settings);
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

        public static UIJoystick Joystick(this IUIObject obj, UIJoystickSettings settings)
        {
            return obj.Component<UIJoystick>(settings);
        }
    }
}