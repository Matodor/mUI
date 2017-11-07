using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class TextBoxView : UIView
    {
        protected override void CreateInterface(params object[] @params)
        {
            this.TextBox(new UITextBoxSettings
            {
                Background = this.Sprite(new UISpriteSettings
                {
                    Sprite = Resources.Load<Sprite>("bg")
                }),
                LabelSettings = new UILabelSettings
                {
                    Text = "Test",
                    Size = 40,
                    Font = "Arial",
                    Color = UIColors.White
                }
            });
        }
    }
}