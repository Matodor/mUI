using System;

namespace mFramework.UI
{
    public class UIContainerSettings : UIComponentSettings
    {
        public float UnscaledHeight;
        public float UnscaledWidth;
    }

    public class UIContainer : UIComponent
    {
        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIContainerSettings containerSettings))
                throw new ArgumentException("UIContainer: The given settings is not UIContainerSettings");

            UnscaledWidth = containerSettings.UnscaledWidth;
            UnscaledHeight = containerSettings.UnscaledHeight;

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
