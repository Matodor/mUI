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

    public sealed class mUI
    {
        public static event Action<MouseEvent> OnMouseDown = delegate { };
        public static event Action<MouseEvent> OnMouseUp = delegate { };
        public static event Action<MouseEvent> OnMouseDrag = delegate { };

        public static event Action<UIObject> UIObjectCreated = delegate {};
        public static event Action<UIObject> UIObjectRemoved = delegate {};

        public static UIView BaseView { get; private set; }
        public static UICamera UICamera { get; private set; }

        private static Dictionary<string, UIFont> _fonts;
        private static mUI _instance;
        private static bool _isCreated;

        static mUI()
        {
            if (_instance == null)
            {
                _instance = new mUI();
            }
        }

        ~mUI()
        {
            mCore.Log("~mUI");
        }

        private mUI()
        {
            _fonts = new Dictionary<string, UIFont>();
        }

        public static void Create()
        {
            Create(new UISettings());
        }

        public static void Create(UISettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _isCreated = true;
            UICamera = UICamera.Create(settings.CameraSettings);
            UICamera.Transform.ParentTransform(mCore.Behaviour.transform);
            BaseView = UIView.Create<BaseView>(new UIViewProps
            {
                UnscaledHeight = settings.BaseViewProps.UnscaledHeight ?? UICamera.UnscaledHeight,
                UnscaledWidth = settings.BaseViewProps.UnscaledWidth ?? UICamera.UnscaledWidth,
                SortingOrder = settings.BaseViewProps.SortingOrder,
                StencilId = settings.BaseViewProps.StencilId ?? 0,
                Padding = settings.BaseViewProps.Padding ?? new UIPadding(),
                Anchor = settings.BaseViewProps.Anchor ?? UIAnchor.MiddleCenter
            }, null);

            Font.textureRebuilt += UILabel.FontOnTextureRebuilt;
            mCore.ApplicationQuitEvent += OnApplicationQuitEvent;
            mCore.Log("[mFramework][UI] created");
        }
         
        private static void OnApplicationQuitEvent()
        {
            Font.textureRebuilt -= UILabel.FontOnTextureRebuilt;
            BaseView.DestroyImpl(false);
            _instance = null;
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

        /*public static IUIClickable GetClickableObject(MouseEvent e, Func<IUIObject, bool> predicate)
        {
            var worldPos = UICamera.ScreenToWorldPoint(e.MouseScreenPos);
            BaseView.DeepFind(o => 
                o.IsActive && 
                o is IUIClickable clickable &&
                clickable.UiClickableOld.Area2D.InArea(worldPos) && predicate(o),
                out var result
            );
            return result as IUIClickable;
        }

        public static IUIClickable GetClickableObject(MouseEvent e)
        {
            var worldPos = UICamera.ScreenToWorldPoint(e.MouseScreenPos);
            BaseView.DeepFind(o => 
                o.IsActive &&
                o is IUIClickable clickable &&
                clickable.UiClickableOld.Area2D.InArea(worldPos),
                out var result
            );
            return result as IUIClickable;
        }*/

        public static UIFont GetFont(string font)
        {
            if (!_fonts.ContainsKey(font))
                return null;
            return _fonts[font];
        }

        public static bool LoadOSFont(string fontName, float harshness = UILabel.DEFAULT_HARSHNESS)
        {
            var font = Font.CreateDynamicFontFromOSFont(fontName, 10);
            if (font == null)
                return false;

            _fonts.Add(fontName, new UIFont(harshness, font));
            return true;
        }

        public static bool LoadFont(string path, float harshness = UILabel.DEFAULT_HARSHNESS)
        {
            var font = Resources.Load<Font>(path);
            if (font == null || _fonts.ContainsKey(font.name))
                return false;

            _fonts.Add(font.name, new UIFont(harshness, font));
            mCore.Log("Loaded font: {0} ", font.name);
            return true;
        }

        public static float MaxWidth()
        {
            return BaseView.Width;
        }

        public static float MaxHeight()
        {
            return BaseView.Height;
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

        internal static void AddUIObject(UIObject obj)
        {
            UIObjectCreated.Invoke(obj);
        }

        internal static void Tick()
        {
            if (!_isCreated)
                return;
            BaseView.Tick();
        }

        internal static void FixedTick()
        {
            if (!_isCreated)
                return;
            BaseView.FixedTick();
        }

        internal static void LateTick()
        {
            if (!_isCreated) 
                return;
            BaseView.LateTick();
        }
    }
}
