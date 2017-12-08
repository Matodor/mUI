using mFramework;
using mFramework.UI;

namespace Example
{
    public class ExampleSpriteView : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            this.Sprite(new UISpriteSettings {Sprite = Game.GetSprite("mp_gui_finish")});
            base.CreateInterface(@params);
        }
    }
}