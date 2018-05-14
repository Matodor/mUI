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
                    Direction = FlexboxDirection.ROW_REVERSE,
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
        }

        private void CreateItemMenu(string text)
        {
            var button = _scrollView.Button(new UIButtonSettings
            {
                Sprite = Game.GetSprite("mp_bar_text"),
            });
        }
    }
}