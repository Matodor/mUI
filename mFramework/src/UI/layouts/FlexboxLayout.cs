using System;

namespace mFramework.UI.Layouts
{
    public class FlexboxLayoutSettings : UILayoutSettings
    {
        public virtual FlexboxDirection Direction { get; set; } = FlexboxDirection.COLUMN;
        public virtual float MarginBetween { get; set; } = 0f;
    }

    public enum FlexboxDirection
    {
        ROW = 0,
        ROW_REVERSE,
        COLUMN,
        COLUMN_REVERSE
    }

    public class FlexboxLayout : UILayout
    {
        public FlexboxDirection Direction { get; private set; }

        protected override void AfterAwake()
        {
            base.AfterAwake();
        }

        protected override void CreateInterface(object[] @params)
        {
        }

        protected override void OnChildObjectAdded(IUIObject sender, IUIObject child)
        {
            if (UnscaledWidth < child.UnscaledWidth)
                UnscaledWidth = child.UnscaledWidth;
        }

        protected override void ApplySettings(UIViewSettings settings, IView parent)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is FlexboxLayoutSettings layoutSettings))
                throw new ArgumentException("FlexboxLayout: The given settings is not FlexboxLayoutSettings");

            base.ApplySettings(settings, parent);

            Direction = layoutSettings.Direction;
            UnscaledWidth = 0f;
            UnscaledHeight = 0f;
        }
    }
}