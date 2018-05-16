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
            _scrollView = this.ScrollView(new ScrollViewSettings
            {
                FlexboxSettings = new FlexboxLayoutSettings
                {
                    Direction = FlexboxDirection.COLUMN,
                    MarginBetween = 0.5f,
                },
            });
            
            CreateItemMenu("Button");
            CreateItemMenu("Button");
            CreateItemMenu("Button");
            CreateItemMenu("Button");
            CreateItemMenu("Button");
            CreateItemMenu("Button");
            CreateItemMenu("Button");
            CreateItemMenu("Button");
            CreateItemMenu("Button");
            CreateItemMenu("Button");
            CreateItemMenu("Button");
            CreateItemMenu("Button");
            CreateItemMenu("Button");
            CreateItemMenu("Button");
            CreateItemMenu("Button");
            CreateItemMenu("Button");

            Debug.Log(mMath.ClosestPointOnLine(new Vector2(0, 5), new Vector2(0, -5),  new Vector2(-5, -12)));
        }

        private void CreateItemMenu(string text)
        {
            var button = _scrollView.Button(new UIButtonSettings
            {
                Sprite = Game.GetSprite("mp_bar_text"),
            }).Anchor(UIAnchor.MiddleCenter);

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

            button.OnClick += sender => Debug.Log(sender.GUID);
        }
    }
}