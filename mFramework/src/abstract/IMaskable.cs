using UnityEngine;

namespace mFramework.UI
{
    public interface IMaskable
    {
        SpriteMask SpriteMask { get; }
        void RemoveMask();
        SpriteMask SetMask(Sprite mask);
    }
}