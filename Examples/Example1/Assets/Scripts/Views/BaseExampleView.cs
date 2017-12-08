using System;
using mFramework;
using mFramework.UI;

namespace Example
{
    public abstract class BaseExampleView : UIView
    {
        protected override void CreateInterface(object[] @params)
        {
            var backButton = this.Button(new UIButtonSettings
            {
                ButtonSpriteStates =
                {
                    Default = Game.GetSprite("notif_cancel"),
                    Highlighted = Game.GetSprite("notif_cancel_closed")
                }
            });
            backButton.Pos(
                RelativeX(1f) - backButton.GetWidth() / 2,
                RelativeY(0f) + backButton.GetHeight() / 2
            );
            backButton.Click += Close;
        }

        private void Close(UIButton sender)
        {
            MainMenuView.Instance.Show();
            Destroy();
        }
    }
}