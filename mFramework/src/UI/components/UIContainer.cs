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
        private float _width;
        private float _height;

        protected override void Init()
        {
            base.Init();
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

        public override float UnscaledHeight()
        {
            return _height;
        }

        public override float UnscaledWidth()
        {
            return _width;
        }

        public override float GetWidth()
        {
            return _width * GlobalScale().x;
        }

        public override float GetHeight()
        {
            return _height * GlobalScale().y;
        }
    }
}
