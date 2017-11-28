using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UISettings
    {
        public UICameraSettings CameraSettings { get; } = new UICameraSettings();
    }

    public sealed class mUI
    {
        internal static mUI Instance { get; private set; }

        public static event Action<UIObject> UIObjectCreated = delegate { };
        public static event Action<UIObject> UIObjectRemoved = delegate { };

        public static UIView BaseView => Instance._baseView;
        public static UICamera UICamera => Instance._uiCamera;

        private readonly Dictionary<string, UIFont> _fonts;
        private readonly BaseView _baseView;
        private readonly UICamera _uiCamera;
        private readonly Dictionary<ulong, UIObject> _uiObjects;

        private mUI(UISettings settings)
        {
            if (Instance != null)
                throw new Exception("mUI already created");

            _uiCamera = UICamera.Create(settings.CameraSettings);
            _uiCamera.GameObject.SetParentTransform(mEngine.Instance.gameObject);

            Instance = this;
            _fonts = new Dictionary<string, UIFont>();
            _uiObjects = new Dictionary<ulong, UIObject>();
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
         
        private void OnApplicationQuitEvent()
        {
            _baseView.DestroyWithoutChecks();
        }

        public static mUI Create()
        {
            return Create(new UISettings());
        }

        public static mUI Create(UISettings settings)
        {
            if (Instance != null)
                throw new Exception("UI already created");
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            return new mUI(settings);
        }

        public static UIObject GetClickableObject(MouseEvent e, Func<UIObject, bool> predicate)
        {
            bool FindPredicate(UIObject o)
            {
                var clickable = o as IUIClickable;
                if (clickable != null && clickable.UIClickable.InArea(e) && predicate(o))
                    return o;
                return o.Childs.Find(FindPredicate);
            }

            return BaseView.Childs.Find(FindPredicate);
        }

        public static UIObject GetClickableObject(MouseEvent e)
        {
            bool FindPredicate(UIObject o)
            {
                var clickable = o as IUIClickable;
                if (clickable != null && clickable.UIClickable.InArea(e))
                    return o;
                return o.Childs.Find(FindPredicate);
            }

            return BaseView.Childs.Find(FindPredicate);
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
            UIObjectRemoved.Invoke(obj);

            return true;
        }

        internal bool AddUIObject(UIObject obj)
        {
            if (_uiObjects.ContainsKey(obj.GUID))
                return false;

            _uiObjects.Add(obj.GUID, obj);
            UIObjectCreated.Invoke(obj);

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
