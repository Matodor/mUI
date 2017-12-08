using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Example
{
    public class ExampleRadioGroupView : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            var group = this.RadioGroup(new UIRadioGroupSettings
            {
                CanDeselectCurrent = false
            });
            group.Selected += sender =>
            {
                Debug.Log("Radio group change");
            }; 

            CreateToggle(group).Translate(0f, -1f);
            CreateToggle(group).Translate(0f, +1f);

            base.CreateInterface(@params);
        }

        private UIToggle CreateToggle(UIRadioGroup group)
        {
            var toggle = group.Toggle(new UIToggleSettings
            {
                ButtonSpriteStates =
                {
                    Default = Game.GetSprite("notif_agree"),
                    Selected = Game.GetSprite("notif_cancel_closed"),
                    Highlighted = Game.GetSprite("notif_agree_closed")
                },
                
            });
            return toggle;
        }
    }
}