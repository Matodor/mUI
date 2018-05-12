using System;

namespace mFramework.UI
{
    public class UIContainerSettings : UIComponentSettings
    {
        public float Height;
        public float Width;
    }

    public class UIContainer : UIComponent
    {
        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIContainerSettings containerSettings))
                throw new ArgumentException("UIContainer: The given settings is not UIContainerSettings");

            UnscaledWidth = containerSettings.Width;
            UnscaledHeight = containerSettings.Height;

            base.ApplySettings(settings);
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
