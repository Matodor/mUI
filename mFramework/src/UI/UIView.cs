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
        public ushort? StencilId => _stencilId ?? InternalParentView.StencilId;
        public UIView ParentView => (UIView) Parent;

        private float _height;
        private float _width;
        private ushort? _stencilId;

        internal static UIView Create(Type viewType, UIViewSettings settings, IView parent, params object[] @params)
        {
            if (!typeof(UIView).IsAssignableFrom(viewType))
                throw new Exception("The given viewType paramater is not UIView");

            var view = (UIView) new GameObject(viewType.Name).AddComponent(viewType);
            view.SetupView(settings, (UIObject) parent, @params);
            return view;
        }

        internal static T Create<T>(UIViewSettings settings, IView parent, params object[] @params) where T : UIView
        {
            var view = new GameObject(typeof(T).Name).AddComponent<T>();
            view.SetupView(settings, (UIObject) parent, @params);
            return view;
        }
        
        private void SetupView(UIViewSettings settings, UIObject parent, object[] @params)
        {
            SetupParent(parent);
            SetupSettings(settings, parent);
            InitCompleted();
            CreateInterface(@params);

            if (_stencilId.HasValue)
            {
                UIStencilMaterials.CreateMesh(this);
            }
        }

        protected abstract void CreateInterface(object[] @params);

        protected virtual void SetupSettings(UIViewSettings settings, IUIObject parent)
        {
            _stencilId = settings.StencilId;
            _height = settings.Height ?? parent.GetHeight();
            _width = settings.Width ?? parent.GetWidth();

            if (settings.DefaultPos.HasValue)
                Position(settings.DefaultPos.Value);

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

        public float RelativeX(float x)
        {
            return Position().x - GetWidth() / 2 + GetWidth() * mMath.Clamp(x, 0, 1);
        }

        public float RelativeY(float y)
        {
            return Position().y - GetHeight() / 2 + GetHeight() * mMath.Clamp(y, 0, 1);
        }

        public Vector2 RelativePos(float x, float y)
        {
            return new Vector2
            {
                x = RelativeX(x),
                y = RelativeY(y)
            };
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
