using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UITextBoxSettings : UIComponentSettings
    {
        public UIObject Background;
        public UILabelSettings LabelSettings;
    }

    public class UITextBox : UIComponent, IUIClickable
    {
        public UIClickable UIClickable => _clickableHandler;
        public UILabel Label => _boxLabel;
        public UIObject Background => _background;

        private UIObject _background;
        private UILabel _boxLabel;
        private UIClickable _clickableHandler;

        private bool _isPressed;

        protected override void Init()
        {
            base.Init();
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UITextBoxSettings textBoxSettings))
                throw new ArgumentException("UITextBox: The given settings is not UITextBoxSettings");

            _clickableHandler = UIClickable.Create(this, AreaType.RECTANGLE);
            _clickableHandler.Area2D.Update += area2d =>
            {
                if (!(area2d is RectangleArea2D rect2d))
                    return;

                rect2d.Height = _background.GetHeight();
                rect2d.Width = _background.GetWidth();
            };

            _background = textBoxSettings.Background;
            _background.ChangeParent(this);

            _boxLabel = _background.Label(textBoxSettings.LabelSettings);
            _boxLabel.SortingOrder(1);

            base.ApplySettings(settings);
        }

        public void MouseDown(Vector2 worldPos)
        {
            _isPressed = true;
        }

        public void MouseUp(Vector2 worldPos)
        {
            _isPressed = false;
        }

        public void MouseDrag(Vector2 worldPos)
        {
        }
    }
}