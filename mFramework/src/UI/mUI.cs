using System;
using System.Collections.Generic;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UISettings
    {
        public readonly UICameraSettings CameraSettings = new UICameraSettings();
    }

    public sealed class mUI
    {
        public static event Action<UIObject> UIObjectCreated = delegate {};
        public static event Action<UIObject> UIObjectRemoved = delegate {};

        public static UIView BaseView => _baseView;
        public static UICamera UICamera => _uiCamera;

        private static Dictionary<string, UIFont> _fonts;
        private static BaseView _baseView;
        private static UICamera _uiCamera;
        private static Dictionary<ulong, UIObject> _uiObjects;
        private static mUI _instance;
        private static bool _isCreated = false;

        static mUI()
        {
            if (_instance == null)
                _instance = new mUI();
        }

        ~mUI()
        {
            mCore.Log("~mUI");
        }

        private mUI()
        {
            _fonts = new Dictionary<string, UIFont>();
            _uiObjects = new Dictionary<ulong, UIObject>();
        }

        public static void Create()
        {
            Create(new UISettings());
        }

        public static void Create(UISettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            _uiCamera = UICamera.Create(settings.CameraSettings);
            _uiCamera.Transform.SetParentTransform(mCore.Behaviour.transform);
            _isCreated = true;
            _baseView = UIView.Create<BaseView>(new UIViewSettings
            {
                Height = UICamera.PureHeight,
                Width = UICamera.PureWidth,
                SortingOrder = 0,
                StencilId = 0,
                DefaultPos = Vector2.zero
            }, null);
            
            mCore.ApplicationQuitEvent += OnApplicationQuitEvent;
            mCore.Log("[mFramework][UI] created");
        }
         
        private static void OnApplicationQuitEvent()
        {
            _baseView.DestroyWithoutChecks();
            _instance = null;
        }
        
        public static IUIClickable GetClickableObject(MouseEvent e, Func<IUIObject, bool> predicate)
        {
            var worldPos = UICamera.ScreenToWorldPoint(e.MouseScreenPos);
            BaseView.DeepFind(o => 
                o.IsActive && 
                o is IUIClickable clickable &&
                clickable.UIClickable.Area2D.InArea(worldPos) && predicate(o),
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
                clickable.UIClickable.Area2D.InArea(worldPos),
                out var result
            );
            return result as IUIClickable;
        }

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
            return BaseView.GetWidth();
        }

        public static float MaxHeight()
        {
            return BaseView.GetHeight();
        }

        internal static bool RemoveUIObject(UIObject obj)
        {
            if (!_uiObjects.ContainsKey(obj.GUID))
                return false;

            _uiObjects.Remove(obj.GUID);
            UIObjectRemoved.Invoke(obj);

            return true;
        }

        internal static bool AddUIObject(UIObject obj)
        {
            if (_uiObjects.ContainsKey(obj.GUID))
                return false;

            _uiObjects.Add(obj.GUID, obj);
            UIObjectCreated.Invoke(obj);

            return true;
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
