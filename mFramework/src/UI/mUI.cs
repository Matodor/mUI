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
        public static event Action<MouseEvent> OnMouseDown = delegate { };
        public static event Action<MouseEvent> OnMouseUp = delegate { };
        public static event Action<MouseEvent> OnMouseDrag = delegate { };

        public static event Action<UIObject> UIObjectCreated = delegate {};
        public static event Action<UIObject> UIObjectRemoved = delegate {};

        public static UIView BaseView { get; private set; }
        public static UICamera UICamera { get; private set; }

        private Dictionary<string, UIFont> _fonts;
        private static mUI _instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            if (_instance != null)
                return;

            _instance = mCore.Instance.gameObject.AddComponent<mUI>();
            _instance.hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(_instance);
        }

        private void Awake()
        {
            _fonts = new Dictionary<string, UIFont>();
            Font.textureRebuilt += UILabel.FontOnTextureRebuilt;
        }

        private void OnDestroy()
        {
            Font.textureRebuilt -= UILabel.FontOnTextureRebuilt;
            BaseView?.DestroyImpl(false);
            _instance = null;
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
            OnMouseDown(e);
            UIClickablesHandler.MouseDown(e);
        }

        internal static void MouseUp(MouseEvent e)
        {
            OnMouseUp(e);
            UIClickablesHandler.MouseUp(e);
        }

        internal static void MouseDrag(MouseEvent e)
        {
            OnMouseDrag(e);
            UIClickablesHandler.MouseDrag(e);
        }

        public static UIFont GetFont(string font)
        {
            if (!_instance._fonts.ContainsKey(font))
                return null;
            return _instance._fonts[font];
        }

        public static bool LoadOSFont(string fontName, float harshness = UILabel.DEFAULT_HARSHNESS)
        {
            var font = Font.CreateDynamicFontFromOSFont(fontName, 10);
            if (font == null)
                return false;

            _instance._fonts.Add(fontName, new UIFont(harshness, font));
            return true;
        }

        public static bool LoadFont(string path, float harshness = UILabel.DEFAULT_HARSHNESS)
        {
            var font = Resources.Load<Font>(path);
            if (font == null || _instance._fonts.ContainsKey(font.name))
                return false;

            _instance._fonts.Add(font.name, new UIFont(harshness, font));
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
            UIObjectRemoved.Invoke(obj);
        }

        internal static void ObjectCreated(UIObject obj)
        {
            UIObjectCreated.Invoke(obj);
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
