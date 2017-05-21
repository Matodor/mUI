using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UISettings
    {
        public UICameraSettings CameraSettings { get; } = new UICameraSettings();
    }

    public sealed class mUI : ITicking
    {
        public static mUI Instance { get; private set; }
        public static UIView BaseView { get; private set; }

        public static UICamera UICamera { get; private set; }

        private mUI(UISettings settings)
        {
            UICamera = UICamera.Create(settings.CameraSettings);
            UICamera.GameObject.SetParent(mEngine.Instance.gameObject);

            Instance = this;
            BaseView = UIView.Create<BaseView>(new UIViewSettings
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
            if (Instance != null)
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

        public void Tick()
        {
            BaseView.Tick();
        }

        public void FixedTick()
        {
            BaseView.FixedTick();
        }

        public void LateTick()
        {
            BaseView.LateTick();
        }
    }
}
