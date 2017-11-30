using System;
using UnityEngine;

namespace mFramework.UI
{
    public class TouchScreenKeyboardSettings
    {
        public TouchScreenKeyboardType KeyboardType = TouchScreenKeyboardType.Default;
        public bool Autocorrection = false;
        public bool Multiline = false;
        public bool Secure = false;
        public bool Alert = false;
        public string Placeholder = string.Empty;
    }

    public class UITextBoxSettings : UIComponentSettings
    {
        public readonly UIButtonSettings BackgroundSettings = new UIButtonSettings();
        public readonly UILabelSettings LabelSettings = new UILabelSettings();
        public readonly TouchScreenKeyboardSettings KeyboardSettings = new TouchScreenKeyboardSettings();
    }

    public class UITextBox : UIComponent, IUIClickable
    {
        public event UIEventHandler<UITextBox> Selected = delegate { };
        public event UIEventHandler<UITextBox> Deselected = delegate { };

        public UIClickable UIClickable => Background.UIClickable;
        public UILabel Label { get; private set; }
        public UIButton Background { get; private set; }

        private TouchScreenKeyboardSettings _keyboardSettings;
        private TouchScreenKeyboard _keyboard;

        private bool _isSelected;

        protected override void Init()
        {
            _isSelected = false;
            base.Init();
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UITextBoxSettings textBoxSettings))
                throw new ArgumentException("UITextBox: The given settings is not UITextBoxSettings");

            _keyboardSettings = textBoxSettings.KeyboardSettings;

            base.ApplySettings(settings);

            Background = this.Button(textBoxSettings.BackgroundSettings);
            Background.Click += OnClickTextBox;
            Background.ButtonUp += CheckForLeave;
            Label = Background.Label(textBoxSettings.LabelSettings);
            Label.SortingOrder(1);
        }

        private void CheckForLeave(UIButton sender, ButtonEventArgs e)
        {
            if (sender.UIClickable.Area2D.InArea(e.ClickWorldPos))
                return;

            _isSelected = false;
            TextBoxDeselected();
        }

        private void OnClickTextBox(UIButton sender)
        {
            _isSelected = !_isSelected;

            if (_isSelected)
                TextBoxSelected();
            else
                TextBoxDeselected();
        }

        private void TextBoxSelected()
        {
            if (TouchScreenKeyboard.isSupported)
            {
                _keyboard = TouchScreenKeyboard.Open(
                    Label.Text,
                    _keyboardSettings.KeyboardType,
                    _keyboardSettings.Autocorrection,
                    _keyboardSettings.Multiline,
                    _keyboardSettings.Secure,
                    _keyboardSettings.Alert,
                    _keyboardSettings.Placeholder
                );
            }

            Selected.Invoke(this);
        }

        protected override void OnFixedTick()
        {
            if (_keyboard == null)
                return;

            if (_keyboard.done)
            {
                Label.SetText(_keyboard.text);
                _isSelected = false;
                TextBoxDeselected();
            }

            if (_keyboard.wasCanceled)
            {
                _isSelected = false;
                TextBoxDeselected();
            }
            base.OnFixedTick();
        }

        private void TextBoxDeselected()
        {
            Deselected.Invoke(this);
            _keyboard = null;
        }

        public override float GetWidth()
        {
            return Background.GetWidth();
        }

        public override float GetHeight()
        {
            return Background.GetHeight();
        }

        public override UIRect GetRect()
        {
            return Background.GetRect();
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
    }
}