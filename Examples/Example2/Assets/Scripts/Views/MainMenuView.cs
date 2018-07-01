using Example.Examples;
using mFramework;
using mFramework.UI;
using mFramework.UI.Layouts;
using UnityEngine;

namespace Example
{
    public class MainMenuView : UIView
    {
        private ScrollView _scrollView;

        protected override void CreateInterface(object[] @params)
        {
            //Debug.Log("center = " + Game.GetSprite("mp_bar_text").bounds.center.ToString("F4"));
            //Debug.Log("extents = " + Game.GetSprite("mp_bar_text").bounds.extents.ToString("F4"));
            //Debug.Log("size = " + Game.GetSprite("mp_bar_text").bounds.size.ToString("F4"));
            //return;

            _scrollView = this.ScrollView(new ScrollViewSettings
            {
                FlexboxSettings = new FlexboxLayoutSettings
                {
                    Direction = FlexboxDirection.COLUMN,
                    MarginBetween = 0.5f,
                },
            });

            CreateItemMenu<UIButtonExample>("UIButton");
            CreateItemMenu<UIAnchorExample>("UIAnchor");

            Debug.Log("ClosestPointOnLine = " + mMath.ClosestPointOnLine(new Vector2(0, 5), new Vector2(0, -5),  new Vector2(-5, -12)));
        }

        private void CreateItemMenu<T>(string text) where T : BaseExampleView
        {
            var button = _scrollView.Button(new UIButtonSettings
            {
                Sprite = Game.GetSprite("mp_bar_text"),
            });

            button.Label(new UILabelSettings
            {
                Text = text,
                Color = Color.black,
                Font = "Arial",
                TextStyle =
                {
                    FontStyle = FontStyle.Bold,
                    TextAlignment = TextAlignment.Center,
                    Size = 40
                }
            }).SortingOrder(1);

            button.OnClick += sender =>
            {
                Hide();
                ParentView.View<T>().BeforeDestroy += s =>
                {
                    Show();
                };
            };
        }
    }
}