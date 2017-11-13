using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Assets.Scripts.Views
{
    public class TextBoxView : UIView
    {
        public UITextBox TextBox { get; private set; }

        protected override void CreateInterface(params object[] @params)
        {
            TextBox = this.TextBox(new UITextBoxSettings
            {
                BackgroundSettings =
                {
                    ButtonSpriteStates =
                    {
                        Default = Resources.Load<Sprite>("bg")
                    }
                },
                KeyboardSettings =
                {
                    
                },
                LabelSettings = 
                {
                    Text = "Test",
                    Size = 40,
                    Font = "Arial",
                    Color = UIColors.White,
                    TextAnchor = TextAnchor.MiddleCenter,
                    TextAlignment = TextAlignment.Center
                }
            });

            TextBox.Selected += s => s.Background.SetColor(new UIColor("#ffffff", 150));
            TextBox.Deselected += s => s.Background.SetColor(new UIColor("#ffffff", 255));
        }

        public override void OnTick()
        {
            var a = (Input.acceleration.x + 1) / 2;
            TextBox.Label.SetText($"a = {a}");
            TextBox.PositionX(RelativeX(a));
        }
    }
}