using System;

namespace mFramework.UI
{
    public class UIContainerProps : UIComponentProps, ISizeable
    {
        public float SizeX { get; set; }
        public float SizeY { get; set; }
    }

    public class UIContainer : UIComponent
    {
        protected override void ApplyProps(UIComponentProps props)
        {
            if (!(props is UIContainerProps containerSettings))
                throw new ArgumentException("UIContainer: The given settings is not UIContainerSettings");

            SizeX = containerSettings.SizeX;
            SizeY = containerSettings.SizeY;

            base.ApplyProps(props);
        }

        public UIContainer SetWidth(float width)
        {
            SizeX = width;
            return this;
        }

        public UIContainer SetHeight(float height)
        {
            SizeY = height;
            return this;
        }
    }
}
