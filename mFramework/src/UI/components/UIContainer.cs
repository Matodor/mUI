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
        public override float UnscaledWidth => _width;
        public override float UnscaledHeight => _height;

        private float _width;
        private float _height;

        protected override void AfterAwake()
        {
            base.AfterAwake();
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIContainerSettings containerSettings))
                throw new ArgumentException("UIContainer: The given settings is not UIContainerSettings");

            _width = containerSettings.Width;
            _height = containerSettings.Height;

            base.ApplySettings(settings);
        }

        public UIContainer SetWidth(float width)
        {
            _width = width;
            return this;
        }

        public UIContainer SetHeight(float height)
        {
            _height = height;
            return this;
        }
    }
}
