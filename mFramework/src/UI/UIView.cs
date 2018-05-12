using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIViewSettings
    {
        /// <summary>
        /// View height in wolrd units
        /// </summary>
        public virtual float? Height { get; set; } = null;

        /// <summary>
        /// View width in wolrd units
        /// </summary>
        public virtual float? Width { get; set; } = null;
        public virtual int? SortingOrder { get; set; } = null;
        public virtual ushort? StencilId { get; set; } = null;
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
        private ushort? _stencilId;

        public static bool IsViewType(Type viewType)
        {
            return typeof(UIView).IsAssignableFrom(viewType);
        }

        public virtual UIView View(Type viewType, UIViewSettings settings, params object[] @params)
        {
            if (!IsViewType(viewType))
                throw new Exception("The given viewType paramater is not UIView");

            var view = (UIView)new GameObject(viewType.Name).AddComponent(viewType);
            view.SetupView(settings, this, @params);
            return view;
        }

        public UIView View(Type viewType, params object[] @params)
        {
            return View(viewType, new UIViewSettings
            {
                Width = UnscaledWidth,
                Height = UnscaledHeight,
            }, @params);
        }
        
        public T View<T>(params object[] @params) where T : UIView
        {
            return (T) View(typeof(T), new UIViewSettings
            {
                Width = UnscaledWidth,
                Height = UnscaledHeight,
            }, @params);
        }

        public T View<T>(UIViewSettings settings, params object[] @params) where T : UIView
        {
            return (T) View(typeof(T), settings, @params);
        }

        internal static UIView Create(Type viewType, UIViewSettings settings, IView parent, params object[] @params)
        {
            if (!IsViewType(viewType))
                throw new Exception("The given viewType paramater is not UIView");

            var view = (UIView) new GameObject(viewType.Name).AddComponent(viewType);
            view.SetupView(settings, parent, @params);
            return view;
        }

        internal static T Create<T>(UIViewSettings settings, IView parent, params object[] @params) where T : UIView
        {
            return (T) Create(typeof(T), settings, parent, @params);
        }
        
        internal virtual void SetupView(UIViewSettings settings, IView parent, object[] @params)
        {
            SetupParent((UIObject) parent);
            ApplySettings(settings, parent);
            InitCompleted();
            CreateInterface(@params);

            if (_stencilId.HasValue)
            {
                UIStencilMaterials.CreateMesh(this);
            }
        }

        protected abstract void CreateInterface(object[] @params);

        protected virtual void ApplySettings(UIViewSettings settings, IView parent)
        {
            _stencilId = settings.StencilId;
            UnscaledHeight = settings.Height ?? parent.Height;
            UnscaledWidth = settings.Width ?? parent.Width;

            if (settings.SortingOrder.HasValue)
                SortingOrder = settings.SortingOrder.Value;

            if (_stencilId.HasValue && _stencilId.Value != 0)
            {
                var meshRenderer = GameObject.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = UIStencilMaterials.GetOrCreate(_stencilId.Value).CanvasMaterial;
                meshRenderer.sharedMaterial.SetTexture("_MainTex", Texture2D.blackTexture);
                meshRenderer.sortingOrder = SortingOrder;

                SortingOrderChanged += sender =>
                {
                    meshRenderer.sortingOrder = SortingOrder;
                };
            }
        }
    }
}
