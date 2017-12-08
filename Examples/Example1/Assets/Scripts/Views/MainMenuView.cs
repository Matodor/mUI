﻿using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Example
{
    public class MainMenuView : UIView
    {
        public static MainMenuView Instance { get; private set; }

        private UIBaseSlider _slider;

        protected override void Init()
        {
            Instance = this;
            base.Init();
        }

        protected override void CreateInterface(object[] @params)
        {
            _slider = this.VerticalSlider(new UISliderSettings
            {
                Height = GetHeight() * 0.8f,
                Offset = 0.2f,
            });

            CreateMenuItem<ExampleSpriteView>("Sprite");
            CreateMenuItem<ExampleButtonView>("Button");
            CreateMenuItem<ExampleToggleView>("Toggle");
            CreateMenuItem<ExampleRadioGroupView>("RadioGroup");
            CreateMenuItem<ExampleLabelView>("Label");

            CreateMenuItem<EasingAnimationsView>("Easing types");
        }

        private void CreateMenuItem<TView>(string text) where TView : UIView
        {
            var button = _slider.Button(new UIButtonSettings
            {
                ButtonSpriteStates = { Default = Game.GetSprite("mp_bar_text") }
            });
            button.SortingOrder(1);
            button.Click += _ =>
            {
                Hide();
                mUI.BaseView.ChildView<TView>(new UIViewSettings
                {
                    SortingOrder = 1
                });
            };

            button.Label(new UILabelSettings
            {
                Text = text,
                Size = 32,
                TextAlignment = TextAlignment.Center,
                TextAnchor = TextAnchor.MiddleCenter,
                Color = new UIColor("#4F4F4F")
            }).SortingOrder(1);
        }
    }
}