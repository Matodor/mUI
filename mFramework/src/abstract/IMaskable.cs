using UnityEngine;

namespace mFramework.UI
{
    public interface IMaskable
    {
        UISprite SpriteMask { get; }
        void RemoveMask();
        UISprite SetMask(Sprite mask, bool useAlphaClip = true, bool insideMask = true);
    }
}