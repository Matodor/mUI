using System;
using System.Collections.Generic;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UISettings
    {
        public UICameraSettings CameraSettings { get; } = new UICameraSettings();
    }

    public sealed class mUI
    {
        internal static mUI Instance => _instance;

        public static event Action<UIObject> UIObjectCreated;
        public static event Action<UIObject> UIObjectRemoved;

        public static UIView BaseView => _instance._baseView;
        public static UICamera UICamera => _instance._uiCamera;

        internal static Dictionary<string, Font> _fonts;
        private static mUI _instance;
        private readonly BaseView _baseView;
        private readonly UICamera _uiCamera;
        private readonly Dictionary<ulong, UIObject> _uiObjects;

        private mUI(UISettings settings)
        {
            _uiCamera = UICamera.Create(settings.CameraSettings);
            _uiCamera.GameObject.SetParentTransform(mEngine.Instance.gameObject);

            _instance = this;
            _fonts = new Dictionary<string, Font>();
            _uiObjects = new Dictionary<ulong, UIObject>();
            _baseView = UIView.Create<BaseView>(new UIViewSettings
            {
                Height = UICamera.PureHeight,
                Width = UICamera.PureWidth,
            }, null);
            
            mCore.ApplicationQuitEvent += OnApplicationQuitEvent;
            mCore.Log("[ui] created");
        }
         
        private void OnApplicationQuitEvent()
        {
            UnityEngine.Object.Destroy(_baseView);
        }

        public static mUI Create()
        {
            return Create(new UISettings());
        }

        public static UIObject GetClickableObject(MouseEvent e, Func<UIObject, bool> predicate)
        {
            foreach (var kvp in Instance._uiObjects)
            {
                var uiClickable = kvp.Value as IUIClickable;
                if (uiClickable == null)
                    continue;
                if (uiClickable.UIClickable.InArea(e) && predicate(kvp.Value))
                    return kvp.Value;
            }
            return null;
        }

        public static UIObject GetClickableObject(MouseEvent e)
        {
            foreach (var kvp in Instance._uiObjects)
            {
                var uiClickable = kvp.Value as IUIClickable;
                if (uiClickable == null)
                    continue;
                if (uiClickable.UIClickable.InArea(e))
                    return kvp.Value;
            }
            return null;
        }

        public static mUI Create(UISettings settings)
        {
            if (_instance != null)
                throw new Exception("UI already created");
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return new mUI(settings);
        }

        public static Font GetFont(string font)
        {
            if (!_fonts.ContainsKey(font))
                return null;
            return _fonts[font];
        }

        public static bool LoadOSFont(string fontName)
        {
            var font = Font.CreateDynamicFontFromOSFont(fontName, 10);
            if (font == null)
                return false;

            _fonts.Add(fontName, font);
            return true;
        }

        public static bool LoadFont(string path)
        {
            var font = Resources.Load<Font>(path);
            if (font == null || _fonts.ContainsKey(font.name))
                return false;
            
            _fonts.Add(font.name, font);
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

        internal bool RemoveUIObject(UIObject obj)
        {
            if (!_uiObjects.ContainsKey(obj.GUID))
                return false;

            _uiObjects.Remove(obj.GUID);
            UIObjectRemoved?.Invoke(obj);

            return true;
        }

        internal bool AddUIObject(UIObject obj)
        {
            if (_uiObjects.ContainsKey(obj.GUID))
                return false;

            _uiObjects.Add(obj.GUID, obj);
            UIObjectCreated?.Invoke(obj);

            return true;
        }

        internal void Tick()
        {
            BaseView.Tick();
        }

        internal void FixedTick()
        {
            BaseView.FixedTick();
        }

        internal void LateTick()
        {
            BaseView.LateTick();
        }
    }
}
