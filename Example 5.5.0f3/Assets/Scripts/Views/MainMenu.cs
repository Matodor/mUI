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

            this.UISprite("GameGUI_28");
        }
    }
}
