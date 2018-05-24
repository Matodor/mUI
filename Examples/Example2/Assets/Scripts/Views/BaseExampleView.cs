using mFramework;
using mFramework.UI;

namespace Example
{
    public abstract class BaseExampleView : UIView
    {
        protected override void CreateInterface(object[] @params)
        {
            var button = this.Button(new UIButtonSettings
            {
                SpriteStates = new SpriteStates
                {
                    Default = Game.GetSprite("notif_cancel"),
                    Highlighted = Game.GetSprite("notif_cancel_closed")
                },
            });

            button.Position(this.Position(UIAnchor.LowerRight), UIAnchor.LowerRight);

            button.OnClick += s => Destroy();
        }
    }
}