using System;
using mFramework.UI.Layouts;
using UnityEngine;

namespace mFramework.UI
{
    public class ScrollViewSettings : UIViewSettings
    {
        public virtual FlexboxLayoutSettings FlexboxSettings { get; set; }
    }

    public class ScrollView : UIView, IUIDragable
    {
        public IAreaChecker AreaChecker { get; set; }

        private const float MAX_PATH_TO_CLICK = 0.03f;
        private const float MIN_DIFF_TO_MOVE = 0.0001f;

        private FlexboxLayout _flexboxLayout;

        protected override void ApplySettings(UIViewSettings settings, IView parent)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is ScrollViewSettings viewSettings))
                throw new ArgumentException("ScrollView: The given settings is not ScrollViewSettings");

            _flexboxLayout = Create<FlexboxLayout>(viewSettings.FlexboxSettings, this);

            AreaChecker = RectangleAreaChecker.Default;
            UIClickablesHandler.AddDragable(this);

            base.ApplySettings(settings, parent);
        }

        public void MouseDown(Vector2 worldPos)
        {
        }

        public void MouseUp(Vector2 worldPos)
        {
        }

        public void MouseDrag(Vector2 worldPos)
        {
        }

        public override UIView View(Type viewType, UIViewSettings settings, params object[] @params)
        {
            if (!IsViewType(viewType))
                throw new Exception("The given viewType paramater is not UIView");

            var view = (UIView) new GameObject(viewType.Name).AddComponent(viewType);
            view.SetupView(settings, _flexboxLayout, @params);
            return view;
        }

        public override T Component<T>(UIComponentSettings settings)
        {
            return _flexboxLayout.Component<T>(settings);
        }

        protected override void CreateInterface(object[] @params)
        {
        }
    }
}