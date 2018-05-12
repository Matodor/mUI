using mFramework;
using mFramework.UI;
using UnityEngine;
using UnityEngine.U2D;

namespace Example
{
    public class Game : MonoBehaviour
    {
        public static Game Instance { get; private set; }

        private static SpriteAtlas _spriteAtlas;

        public void Awake()
        {
            Instance = this;
            Application.targetFrameRate = 60;
            Application.runInBackground = true;

#if UNITY_EDITOR
            mCore.IsDebug = true;
#endif

            _spriteAtlas = Resources.Load<SpriteAtlas>("UIAtlas");

            mUI.Create(new UISettings
            {
                CameraSettings =
                {
                    CameraClearFlags = CameraClearFlags.SolidColor
                }
            });

            //mUI.UICamera.Camera.backgroundColor = Color.gray;
            mUI.LoadOSFont("Arial");
            mUI.BaseView.View<MainMenuView>();
        }

        public static Sprite GetSprite(string name)
        {
            return _spriteAtlas.GetSprite(name);
        }
    }
}
