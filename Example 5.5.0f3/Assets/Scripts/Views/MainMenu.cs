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
            var sp = Component<UISprite>(new UISpriteSettings {Sprite = SpritesRepository.Get("GameGUI_35")});
            sp.Animation<UILinearAnimation>(new UILinearAnimationSettings
            {
                StartPos = sp.Position(),
                EndPos = new Vector2(sp.Position().x, RelativeY(0) + sp.GetHeight()/2),
                PlayType = UIAnimationPlayType.END_FLIP,
                Duration = 5
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
            button.Animation<UIRotateAnimation>(new UIRotateAnimationSettings
                {
                    FromAngle = 90,
                    ToAngle = 270,
                    EasingType = EasingType.easeInBack,
                    Duration = 2,
                    PlayType = UIAnimationPlayType.END_FLIP
                }).OnEndEvent +=
                a => a.EasingType = a.EasingType == EasingType.easeInBack
                    ? a.EasingType = EasingType.easeOutBack
                    : a.EasingType = EasingType.easeInBack;


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
                t.OnSelect += tgl => mCore.Log("UIRadioGroup: toggle");
                return t;
            };

            addToggle(radioGroup).Translate(-1, 0);
            addToggle(radioGroup).Translate(0, 0);
            addToggle(radioGroup).Translate(1, 0);
            radioGroup.Translate(0, -1);


            // UISlider horizontal
            var horizontalSlider = Component<UISlider>(new UISliderSettings
            {
                Width = mUI.MaxWidth() * 0.9f,
                Height = SpritesRepository.Get("GameGUI_40").WorldSize().y*3,
                Offset = SpritesRepository.Get("GameGUI_40").WorldSize().x / 4,
                DirectionOfAddingSlides = DirectionOfAddingSlides.FORWARD,
                SliderType = SliderType.HORIZONTAL
            });
            horizontalSlider.Translate(new Vector2
            {
                x = RelativeX(0.5f),
                y = RelativeY(1) - SpritesRepository.Get("GameGUI_40").WorldSize().y*2
            });
            
            for (int i = 0; i < 20; i++)
            {
                horizontalSlider.Component<UIButton>(new UIButtonSettings
                {
                    ButtonSpriteStates = new SpriteStates
                    {
                        Default = SpritesRepository.Get("GameGUI_40"),
                        Highlighted = SpritesRepository.Get("GameGUI_41")
                    }
                });
            }

            // UISlider horizontal
            var verticalSlider = Component<UISlider>(new UISliderSettings
            {
                Width = SpritesRepository.Get("GameGUI_40").WorldSize().y*3,
                Height = mUI.MaxHeight() - SpritesRepository.Get("GameGUI_40").WorldSize().y * 5,
                Offset = SpritesRepository.Get("GameGUI_40").WorldSize().x / 4,
                DirectionOfAddingSlides = DirectionOfAddingSlides.FORWARD,
                SliderType = SliderType.VERTICAL
            });
            verticalSlider.Translate(new Vector2
            {
                x = RelativeX(0) + SpritesRepository.Get("GameGUI_40").WorldSize().x * 1.5f,
                y = RelativeY(1) - verticalSlider.GetHeight() / 2 - SpritesRepository.Get("GameGUI_40").WorldSize().y * 3
            });

            for (int i = 0; i < 20; i++)
            {
                verticalSlider.Component<UIButton>(new UIButtonSettings
                {
                    ButtonSpriteStates = new SpriteStates
                    {
                        Default = SpritesRepository.Get("GameGUI_40"),
                        Highlighted = SpritesRepository.Get("GameGUI_41")
                    }
                }).OnClick += b => mCore.Log("GUID: {0}", b.GUID);
            }
        }
    }
}
