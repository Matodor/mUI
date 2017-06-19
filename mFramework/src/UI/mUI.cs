using System;
using System.Collections.Generic;

namespace mFramework.UI
{
    public sealed class UISettings
    {
        public UICameraSettings CameraSettings { get; } = new UICameraSettings();
    }

    public sealed class mUI
    {
        internal static mUI Instance { get { return _instance; } }
        public static UIView BaseView { get { return _instance._baseView; } }
        public static UICamera UICamera { get { return _instance._uiCamera; } }

        private static mUI _instance;
        private readonly UIView _baseView;
        private readonly UICamera _uiCamera;
        private readonly Dictionary<ulong, UIObject> _uiObjects;

        private mUI(UISettings settings)
        {
            _uiCamera = UICamera.Create(settings.CameraSettings);
            _uiCamera.GameObject.SetParent(mEngine.Instance.gameObject);

            _instance = this;
            _uiObjects = new Dictionary<ulong, UIObject>();
            _baseView = UIView.Create<BaseView>(new UIViewSettings
            {
                Height = UICamera.PureHeight,
                Width = UICamera.PureWidth,
                Name = "BaseView"
            }, null);
            
            mCore.Log("[ui] created");
        }

        public static mUI Create()
        {
            return Create(new UISettings());
        }

        public static mUI Create(UISettings settings)
        {
            if (settings == null)
                throw new NullReferenceException("UISettings is null");
            if (_instance != null)
                throw new Exception("UI already created");
            return new mUI(settings);
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
            return true;
        }

        internal bool AddUIObject(UIObject obj)
        {
            if (_uiObjects.ContainsKey(obj.GUID))
                return false;

            _uiObjects.Add(obj.GUID, obj);
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
