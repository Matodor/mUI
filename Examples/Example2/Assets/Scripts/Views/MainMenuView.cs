using mFramework;
using mFramework.UI;

namespace Example
{
    public class MainMenuView : UIView
    {
        protected override void CreateInterface(object[] @params)
        {
            var button = this.Button(new UIButtonSettings
            {
                Sprite = Game.GetSprite("mp_bar_text")
            });

            button.Anchor = UIAnchor.LowerLeft;
        }
    }
}