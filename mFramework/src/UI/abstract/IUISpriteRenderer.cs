using UnityEngine;

namespace mFramework.UI
{
    public interface IUISpriteRenderer : IUIRenderer
    {
        SpriteRenderer Renderer { get; }
        IUISpriteRenderer SpriteMask { get; }

        void Flip(bool flipX, bool flipY);
        void SetSprite(Sprite sprite);

        void RemoveMask();
        IUISpriteRenderer SetMask(Sprite mask, bool useAlphaClip = true, bool insideMask = true);
    }
}