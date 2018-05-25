using System;

namespace mFramework.UI
{
    public class UIContainerProps : UIComponentProps
    {
        public float UnscaledHeight;
        public float UnscaledWidth;
    }

    public class UIContainer : UIComponent
    {
        protected override void ApplyProps(UIComponentProps props)
        {
            if (!(props is UIContainerProps containerSettings))
                throw new ArgumentException("UIContainer: The given settings is not UIContainerSettings");

            UnscaledWidth = containerSettings.UnscaledWidth;
            UnscaledHeight = containerSettings.UnscaledHeight;

            base.ApplyProps(props);
        }

        public UIContainer SetWidth(float width)
        {
            UnscaledWidth = width;
            return this;
        }

        public UIContainer SetHeight(float height)
        {
            UnscaledHeight = height;
            return this;
        }
    }
}
