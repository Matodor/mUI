using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIViewProps : UIObjectProps
    {
        public virtual ushort? StencilId { get; set; } = null;
     
        /// <summary>
        /// Element bound width
        /// </summary>
        public virtual float? SizeX { get; set; }

        /// <summary>
        /// Element bound height
        /// </summary>
        public virtual float? SizeY { get; set; }
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

        public virtual UIView View(Type viewType, UIViewProps props, params object[] @params)
        {
            if (!IsViewType(viewType))
                throw new Exception("The given viewType paramater is not UIView");

            var view = (UIView)new GameObject(viewType.Name).AddComponent(viewType);
            view.SetupView(props, this, @params);
            return view;
        }

        public UIView View(Type viewType, params object[] @params)
        {
            return View(viewType, new UIViewProps
            {
                SizeX = SizeX,
                SizeY = SizeY,
            }, @params);
        }
        
        public T View<T>(params object[] @params) where T : UIView
        {
            return (T) View(typeof(T), new UIViewProps
            {
                SizeX = SizeX,
                SizeY = SizeY,
            }, @params);
        }

        public T View<T>(UIViewProps props, params object[] @params) where T : UIView
        {
            return (T) View(typeof(T), props, @params);
        }

        internal static UIView Create(Type viewType, UIViewProps props, IView parent, params object[] @params)
        {
            if (!IsViewType(viewType))
                throw new Exception("The given viewType paramater is not UIView");

            var view = (UIView) new GameObject(viewType.Name).AddComponent(viewType);
            view.SetupView(props, parent, @params);
            return view;
        }

        internal static T Create<T>(UIViewProps props, IView parent, params object[] @params) where T : UIView
        {
            return (T) Create(typeof(T), props, parent, @params);
        }
        
        internal virtual void SetupView(UIViewProps props, IView parent, object[] @params)
        {
            if (props == null)
                throw new Exception("UIView: UIViewProps can not be null");

            SetupParent((UIObject) parent);
            ApplyProps(props, parent);
            InitCompleted(true);

            if (_stencilId.HasValue && _stencilId.Value != 0)
            {
                // TODO при изменении padding изменение canvas
                var meshRenderer = GameObject.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = UIStencilMaterials.GetOrCreate(_stencilId.Value).CanvasMaterial;
                meshRenderer.sharedMaterial.SetTexture("_MainTex", Texture2D.blackTexture);
                meshRenderer.sortingOrder = SortingOrder;

                SortingOrderChanged += sender =>
                {
                    meshRenderer.sortingOrder = SortingOrder;
                };

                UIStencilMaterials.CreateMesh(this);
            }

            mUI.ObjectCreated(this);
            CreateInterface(@params);
        }

        protected abstract void CreateInterface(object[] @params);

        protected virtual void ApplyProps(UIViewProps props, IView parent)
        {
            base.ApplyProps(props);

            _stencilId = props.StencilId;
            SizeY = props.SizeY ?? parent.SizeY;
            SizeX = props.SizeX ?? parent.SizeX;
        }
    }
}
