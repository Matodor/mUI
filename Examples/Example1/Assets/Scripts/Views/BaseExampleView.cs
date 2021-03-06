﻿using System;
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
                this.RelativeX(1f) - backButton.GetWidth() / 2,
                this.RelativeY(0f) + backButton.GetHeight() / 2
            );
            backButton.Click += Close;
        }

        private void Close(IUIButton sender)
        {
            MainMenuView.Instance.Show();
            Destroy();
        }
    }
}