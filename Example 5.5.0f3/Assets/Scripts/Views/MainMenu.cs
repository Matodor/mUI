using System.Linq;
using mFramework;
using mFramework.UI;

namespace Assets.Scripts.Views
{
    public class MainMenu : UIView
    {
        protected override void CreateInterface(params object[] @params)
        {
            foreach (var o in @params)
            {
                mCore.Log(o.ToString());
            }

            Component<UISprite>(new UISpriteSettings
            {
                Sprite = SpritesRepository.Get("GameGUI_28")
            });

            Component<UIButton>(new UIButtonSettings());
        }
    }
}
