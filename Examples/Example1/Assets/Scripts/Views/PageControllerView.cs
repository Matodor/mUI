using mFramework;
using mFramework.UI;
using UnityEngine;

public class PageControllerView : UIView
{
    private UIPageSlider _pageSlider;

    protected override void CreateInterface(params object[] @params)
    {
        _pageSlider = this.PageSlider(new UISliderSettings
        {
            Height = this.GetHeight(),
            Width = this.GetWidth(),
            SliderType = UIObjectOrientation.HORIZONTAL,
            DirectionOfAddingSlides = DirectionOfAddingSlides.BACKWARD
        });

        var sp = Resources.Load<Sprite>("bg");
        for (int i = 0; i < 10; i++)
        {
            _pageSlider.Sprite(new UISpriteSettings
            {
                Sprite = sp
            }).Label(new UILabelSettings
            {
                Text = $"{i}",
                Size = 40,
                Color = UIColors.White,
                Font = "Arial"
            }).SortingOrder(2);
        }
    }
}
