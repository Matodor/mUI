using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Example
{
    public class ExampleToggleView : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            var toggle = this.Toggle(new UIToggleSettings
            {
                ButtonSpriteStates =
                {
                    Default = Game.GetSprite("notif_agree"),
                    Selected = Game.GetSprite("notif_cancel_closed"),
                    Highlighted = Game.GetSprite("notif_agree_closed")
                }
            });
            toggle.Selected += _ => Debug.Log("Toggle selected");
            toggle.Deselected += _ => Debug.Log("Toggle deselected");

            base.CreateInterface(@params);
        }
    }
}