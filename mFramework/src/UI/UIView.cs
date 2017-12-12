using System;
using UnityEngine;

namespace mFramework.UI
{
    public sealed class UIViewSettings
    {
        public float? Height = null;
        public float? Width = null;
        public int? SortingOrder = null;
        public Vector2? DefaultPos = null;
        public ushort? StencilId = null;
    }

    /*public static class NewView<T> where T : UIView
    {
        public static readonly Func<T> Instance;

        static NewView()
        {
            var constructor = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                new Type[0], null);
            var e = Expression.New(constructor);
            Instance = Expression.Lambda<Func<T>>(e).Compile();
        }
    }*/

    public abstract class UIView : UIObject, IView
    {
        public ushort? StencilId => _stencilId ?? ParentView.StencilId;

        private float _height;
        private float _width;
        private ushort? _stencilId;

        internal static UIView Create(Type viewType, UIViewSettings settings, IView parent, params object[] @params)
        {
            if (!typeof(UIView).IsAssignableFrom(viewType))
                throw new Exception("The given viewType paramater is not UIView");

            var view = (UIView) new GameObject(viewType.Name).AddComponent(viewType);
            view.SetupView(settings, parent, @params);
            return view;
        }

        internal static T Create<T>(UIViewSettings settings, IView parent, params object[] @params) where T : UIView
        {
            var view = new GameObject(typeof(T).Name).AddComponent<T>();
            view.SetupView(settings, parent, @params);
            return view;
        }
        
        private void SetupView(UIViewSettings settings, IView parent, object[] @params)
        {
            SetupParent((UIObject) parent);
            SetupSettings(settings, parent);
            InitCompleted();
            CreateInterface(@params);

            if (_stencilId.HasValue)
            {
                UIStencilMaterials.CreateMesh(this);
            }
        }

        protected abstract void CreateInterface(object[] @params);

        protected virtual void SetupSettings(UIViewSettings settings, IView parent)
        {
            _stencilId = settings.StencilId;
            _height = settings.Height ?? parent.GetHeight();
            _width = settings.Width ?? parent.GetWidth();

            if (settings.DefaultPos.HasValue)
                Pos(settings.DefaultPos.Value);

            if (settings.SortingOrder.HasValue)
                SortingOrder(settings.SortingOrder.Value);

            if (_stencilId.HasValue && _stencilId.Value != 0)
            {
                var meshRenderer = gameObject.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = UIStencilMaterials.GetOrCreate(_stencilId.Value).CanvasMaterial;
                meshRenderer.sharedMaterial.SetTexture("_MainTex", Texture2D.blackTexture);
                meshRenderer.sortingOrder = SortingOrder();

                SortingOrderChanged += sender =>
                {
                    meshRenderer.sortingOrder = SortingOrder();
                };
            }
        }

        public override float UnscaledHeight()
        {
            return _height;
        }

        public override float UnscaledWidth()
        {
            return _width;
        }

        public override float GetHeight()
        {
            return _height * GlobalScale().y;
        }

        public override float GetWidth()
        {
            return _width * GlobalScale().x;
        }
    }
}
