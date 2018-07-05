using System;
using System.Collections.Generic;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UISettings
    {
        public readonly UICameraSettings CameraSettings = new UICameraSettings();
        public readonly UIViewProps BaseViewProps = new UIViewProps()
        {
            Anchor = UIAnchor.MiddleCenter,
            StencilId = 0,
        };
    }

    public sealed class mUI : MonoBehaviour
    {
        public static mUI Instance { get; private set; }

        public static event Action<MouseEvent> OnMouseDown;
        public static event Action<MouseEvent> OnMouseUp;
        public static event Action<MouseEvent> OnMouseDrag;

        public static event Action<UIObject> UIObjectCreated;
        public static event Action<UIObject> UIObjectRemoved;

        public static UIView BaseView { get; private set; }
        public static UICamera UICamera { get; private set; }

        private Dictionary<string, UIFont> _fonts;
        private bool _destroyed;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            if (Instance != null)
                return;

            var instance = mCore.Instance.gameObject.AddComponent<mUI>();
            instance.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(instance);
        }

        private void Awake()
        {
            if (Instance != null)
                throw new Exception("mUI instance already created");

            Instance = this;
            _fonts = new Dictionary<string, UIFont>();
            Font.textureRebuilt += UILabel.FontOnTextureRebuilt;
            mCore.ApplicationQuit += mCoreOnApplicationQuit;
        }

        private void mCoreOnApplicationQuit()
        {
            OnDestroy();
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_destroyed)
                return;

            Debug.Log("[mUI] OnDestroy");
            _destroyed = true;
            Font.textureRebuilt -= UILabel.FontOnTextureRebuilt;
            BaseView?.DestroyImpl(false);
            Instance = null;
            
            OnMouseDown = null;
            OnMouseUp = null;
            OnMouseDrag = null;

            UIObjectCreated = null;
            UIObjectRemoved = null;
        }

        public static void Create()
        {
            Create(new UISettings());
        }

        public static void Create(UISettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (BaseView != null)
                throw new Exception("mUI already created");

            UICamera = UICamera.Create(settings.CameraSettings);
            BaseView = UIView.Create<BaseView>(new UIViewProps
            {
                SizeY = settings.BaseViewProps.SizeY ?? UICamera.SizeY,
                SizeX = settings.BaseViewProps.SizeX ?? UICamera.SizeX,
                SortingOrder = settings.BaseViewProps.SortingOrder,
                StencilId = settings.BaseViewProps.StencilId ?? 0,
                Padding = settings.BaseViewProps.Padding ?? new UIPadding(),
                Anchor = settings.BaseViewProps.Anchor ?? UIAnchor.MiddleCenter
            }, null);

            Debug.Log("[mFramework][UI] created");
        }

        internal static void MouseDown(MouseEvent e)
        {
            OnMouseDown?.Invoke(e);
            UIClickablesHandler.MouseDown(e);
        }

        internal static void MouseUp(MouseEvent e)
        {
            OnMouseUp?.Invoke(e);
            UIClickablesHandler.MouseUp(e);
        }

        internal static void MouseDrag(MouseEvent e)
        {
            OnMouseDrag?.Invoke(e);
            UIClickablesHandler.MouseDrag(e);
        }

        public static UIFont GetFont(string font)
        {
            if (!Instance._fonts.ContainsKey(font))
                return null;
            return Instance._fonts[font];
        }

        public static bool LoadOSFont(string fontName, float harshness = UILabel.DEFAULT_HARSHNESS)
        {
            var font = Font.CreateDynamicFontFromOSFont(fontName, 10);
            if (font == null)
                return false;

            Instance._fonts.Add(fontName, new UIFont(harshness, font));
            return true;
        }

        public static bool LoadFont(string path, float harshness = UILabel.DEFAULT_HARSHNESS)
        {
            var font = Resources.Load<Font>(path);
            if (font == null || Instance._fonts.ContainsKey(font.name))
                return false;

            Instance._fonts.Add(font.name, new UIFont(harshness, font));
            Debug.Log($"Loaded font: {font.name}");
            return true;
        }

        public static float MaxWidth()
        {
            return BaseView?.Width ?? 0f;
        }

        public static float MaxHeight()
        {
            return BaseView?.Height ?? 0f;
        }

        public static void OnClickables()
        {
            UIClickablesHandler.EnableClickables();
        }

        public static void OffClickables()
        {
            UIClickablesHandler.DisableClickables();    
        }

        internal static void RemoveUIObject(UIObject obj)
        {
            UIObjectRemoved?.Invoke(obj);
        }

        internal static void ObjectCreated(UIObject obj)
        {
            UIObjectCreated?.Invoke(obj);
        }

        private void Update()
        {
            BaseView?.Tick();
        }

        private void FixedUpdate()
        {
            BaseView?.FixedTick();
        }

        private void LateUpdate()
        {
            BaseView?.LateTick();
        }
    }
}
