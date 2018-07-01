using System;
using UnityEngine;

namespace mFramework.UI
{
    /*public class TouchScreenKeyboardSettings
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
        public virtual UIButtonSettings BackgroundSettings { get; set; } = new UIButtonSettings();
        public virtual UILabelSettings LabelSettings { get; set; } = new UILabelSettings();
        public virtual TouchScreenKeyboardSettings KeyboardSettings { get; set; } = new TouchScreenKeyboardSettings();
    }

    public class UITextBox : UIComponent
    {
        public event UIEventHandler<UITextBox> Selected = delegate { };
        public event UIEventHandler<UITextBox> Deselected = delegate { };

        public UILabel Label { get; private set; }
        public UIButton Background { get; private set; }

        public override Vector2 CenterOffset => Background.CenterOffset;

        private TouchScreenKeyboardSettings _keyboardSettings;
        private TouchScreenKeyboard _keyboard;

        private bool _isSelected;

        protected override void AfterAwake()
        {
            _isSelected = false;
            base.AfterAwake();
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

        private void CheckForLeave(IUIButton sender, Vector2 worldPos)
        {
            if (sender.UiClickableOld.Area2D.InArea(worldPos))
                return;

            _isSelected = false;
            TextBoxDeselected();
        }

        private void OnClickTextBox(IUIButton sender)
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
    }*/
}