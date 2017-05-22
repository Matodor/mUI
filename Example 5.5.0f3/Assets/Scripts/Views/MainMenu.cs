using System;
using System.Linq;
using mFramework;
using mFramework.UI;
using UnityEngine;

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
            

            // UISprite
            Component<UISprite>(new UISpriteSettings
            {
                Sprite = SpritesRepository.Get("GameGUI_35")
            });
            

            // UIButton
            var button = Component<UIButton>(new UIButtonSettings
            {
                ButtonSpriteStates = new SpriteStates
                {
                    Default = SpritesRepository.Get("GameGUI_28"),
                    Highlighted = SpritesRepository.Get("GameGUI_29"),
                }
            });
            button.OnClick += sender => mCore.Log("UIButton: click");
            button.Translate(0, -3);


            // UIToggle
            var toggle = Component<UIToggle>(new UIToggleSettings
            {
                ButtonSpriteStates = new SpriteStates
                {
                    Default = SpritesRepository.Get("GameGUI_6"),
                    Highlighted = SpritesRepository.Get("GameGUI_1"),
                    Selected = SpritesRepository.Get("GameGUI_1"),
                },
                DefaultSelected = true
            });
            toggle.OnChanged += sender => mCore.Log("UIToogle: {0}", sender.IsSelected);
            toggle.Translate(0, 3);


            // UIRadioGroup
            var radioGroup = Component<UIRadioGroup>(new UIRadioGroupSettings
            {
                CanDeselectCurrent = true
            });

            Func<UIRadioGroup, UIToggle> addToggle = group =>
            {
                var t = group.AddToggle(new UIToggleSettings
                {
                    ButtonSpriteStates = new SpriteStates
                    {
                        Default = SpritesRepository.Get("GameGUI_31"),
                        Highlighted = SpritesRepository.Get("GameGUI_32"),
                        Selected = SpritesRepository.Get("GameGUI_32"),
                    }
                });
                t.OnSelect += tgl => mCore.Log("UIRadioGroup: toggle {0}", tgl.GUID);
                return t;
            };

            addToggle(radioGroup).Translate(-1, 0);
            addToggle(radioGroup).Translate(0, 0);
            addToggle(radioGroup).Translate(1, 0);
            radioGroup.Translate(0, -1);


            // UISlider
            var slider = Component<UISlider>(new UISliderSettings
            {
                Width = mUI.MaxWidth() * 0.9f,
                Height = 1f,
                Offset = 0.3f,
                OrientationSettings = new UISliderHorizontalSettings
                {
                    SliderDirection = UISliderHorizontalSettings.Direction.RIGHT_TO_LEFT
                }
            });
            slider.Translate(0, 1.5f);
            
            for (int i = 0; i < 10; i++)
            {
                slider.Component<UIButton>(new UIButtonSettings
                {
                    ButtonSpriteStates = new SpriteStates
                    {
                        Default = SpritesRepository.Get("GameGUI_4"),
                        Highlighted = SpritesRepository.Get("GameGUI_5")
                    }
                });
            }
        }
    }
}
