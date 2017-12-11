using mFramework;
using mFramework.UI;
using mFramework.UI.Layouts;

namespace Example
{
    public class ExampleHorizontalSliderView : BaseExampleView
    {
        protected override void CreateInterface(object[] @params)
        {
            var slider1 = this.HorizontalSlider(new UISliderSettings
            {
                Height = GetHeight() / 2.5f,
                Width = GetWidth(),
                Offset = 0.1f,
                ElementsDirection = LayoutElemsDirection.FORWARD,
            });
            slider1.PosY(this.RelativeY(1f) - slider1.GetHeight() / 2);

            for (int i = 0; i < 20; i++)
                slider1.Sprite(new UISpriteSettings { Sprite = Game.GetSprite("mp_chat_msg") }).SortingOrder(1);

            var slider2 = this.HorizontalSlider(new UISliderSettings
            {
                Height = GetHeight() / 2.5f,
                Width = GetWidth(),
                ElementsDirection = LayoutElemsDirection.BACKWARD,
                Padding = 0.5f,
                Offset = 0.2f,
                TimeToStop = 1f
            });
            slider2.PosY(this.RelativeY(0f) + slider2.GetHeight() / 2);

            for (int i = 0; i < 20; i++)
                slider2.Sprite(new UISpriteSettings { Sprite = Game.GetSprite("mp_chat_msg") }).SortingOrder(1);
            base.CreateInterface(@params);
        }
    }
}