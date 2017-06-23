using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UILabelSettings : UIComponentSettings
    {
        public string Text { get; set; }
    }

    public class UILabel : UIComponent, IUIRenderer, IColored
    {
        public Renderer UIRenderer { get; }

        private TextMesh _textMesh;

        protected UILabel(UIObject parent) : base(parent)
        {
            
        }

        public UILabel FontSize(int size)
        {
            return this;
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var labelSettings = settings as UILabelSettings;
            if (labelSettings == null)
                throw new ArgumentException("UILabel: The given settings is not UILabelSettings");

            _textMesh = _gameObject.AddComponent<TextMesh>();
            _textMesh.text = labelSettings.Text;
            _textMesh.fontSize = (Screen.height / 100) * 10;
            _textMesh.characterSize = 10f / _textMesh.fontSize;

            base.ApplySettings(settings);
        }

        public UIObject SetColor(Color32 color)
        {
            return this;
        }

        public UIObject SetColor(UIColor color)
        {
            return this;
        }
    }
}
