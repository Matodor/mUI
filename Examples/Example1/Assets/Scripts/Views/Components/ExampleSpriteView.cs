using mFramework;
using mFramework.UI;
using UnityEngine;

namespace Example
{
    public class ExampleSpriteView : BaseExampleView
    {
        private UISprite _sprite;
        private Vector2 _pos;

        protected override void CreateInterface(object[] @params)
        {
            _sprite = this.Sprite(new UISpriteSettings {Sprite = Game.GetSprite("mp_gui_finish")});
            Debug.Log($"size = {_sprite.Renderer.size}");
            Debug.Log($"pivot = {_sprite.Renderer.sprite.pivot}");
            Debug.Log($"bounds.center = {_sprite.Renderer.sprite.bounds.center}");
            Debug.Log($"bounds.size = {_sprite.Renderer.sprite.bounds.size}");

            _pos = _sprite.Pos() - new Vector2(_sprite.GetWidth() / 2, _sprite.GetHeight() / 2);
            base.CreateInterface(@params);
        }

        protected override void OnFixedTick()
        {
            _sprite.transform.RotateAround(_pos, Vector3.forward, Time.time % 1 * 90f);

            base.OnFixedTick();
        }
    }
}