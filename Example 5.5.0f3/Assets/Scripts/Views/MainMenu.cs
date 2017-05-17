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
            
            Component<UISprite>(new UISpriteSettings
            {
                Sprite = SpritesRepository.Get("GameGUI_35")
            });
            
            var button = Component<UIButton>(new UIButtonSettings
            {
                ButtonSpriteStates = new SpriteStates
                {
                    Default = SpritesRepository.Get("GameGUI_28"),
                    Highlighted = SpritesRepository.Get("GameGUI_29"),
                }
            });
            button.OnClick += sender => mCore.Log("UIButton: click");
            button.Translate(new Vector2(0, -3));

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
            toggle.Translate(new Vector2(0, 3));
        }
    }
}
