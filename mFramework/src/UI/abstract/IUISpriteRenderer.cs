using UnityEngine;

namespace mFramework.UI
{
    public interface IUISpriteRenderer
    {
        SpriteRenderer Renderer { get; }
        UISprite SpriteMask { get; }

        void Flip(bool flipX, bool flipY);
        void SetSprite(Sprite sprite);

        void RemoveMask();
        UISprite SetMask(Sprite mask, bool useAlphaClip = true, bool insideMask = true);

        float GetHeight();
        float GetWidth();

        UIRect GetRect();
    }
}