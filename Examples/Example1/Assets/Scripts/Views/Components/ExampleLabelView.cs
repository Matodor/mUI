using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Example
{
    public class ExampleLabelView : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            var label1 = this.Label(new UILabelSettings
            {
                Text = "test [=0]text[=/]\nte[=1]st[=/] text\nte[=0]st te[=/]xt\ntest text",
                TextAlignment = TextAlignment.Center,
                TextAnchor = TextAnchor.MiddleCenter,
                Size = 50,
                LetterSpacing = 2f,
                Color = UIColor.Black,
            });
            label1.TextFormatting(0, new TextFormatting
            {
                Color = UIColor.White.Color32,
                Size = 30,
                LetterSpacing = 1f,
                FontStyle = FontStyle.Italic
            });
            label1.TextFormatting(1, new TextFormatting
            {
                Color = new Color32(255, 12, 75, 255),
                FontStyle = FontStyle.Bold
            });
            base.CreateInterface(@params);
        }
    }
}