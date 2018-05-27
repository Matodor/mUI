using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIMeshProps : UIComponentProps
    {
        public Mesh Mesh = null;
        public Mesh SharedMesh = null;
        public float UnscaledWidth;
        public float UnscaledHeight;
    }

    public class UIMesh : UIComponent, IUIColored, IUIRenderer<MeshRenderer>, IUIRenderer
    {
        public virtual Color Color
        {
            get => throw new Exception("UIMesh not support getter color");
            set => SetColor(value);
        }

        public virtual float Opacity
        {
            get => throw new Exception("UIMesh not support getter opacity");
            set
            {
                var colors = new Color[MeshFilter.mesh.colors.Length];
                for (int i = 0; i < colors.Length; i++)
                {
                    var color = MeshFilter.mesh.colors[i];
                    color.a = value;
                    colors[i] = color;
                }

                MeshFilter.mesh.colors = colors;
            }
        }

        public MeshFilter MeshFilter { get; private set; }
        public MeshRenderer UIRenderer { get; private set; }
        Renderer IUIRenderer.UIRenderer => UIRenderer;

        protected override void OnBeforeDestroy()
        {
            SortingOrderChanged -= OnSortingOrderChanged;
            base.OnBeforeDestroy();
        }

        protected override void AfterAwake()
        {
            UIRenderer = GameObject.AddComponent<MeshRenderer>();
            MeshFilter = GameObject.AddComponent<MeshFilter>();

            SortingOrderChanged += OnSortingOrderChanged;
            base.AfterAwake();
        }

        private void OnSortingOrderChanged(IUIObject sender)
        {
            UIRenderer.sortingOrder = SortingOrder;
        }

        protected override void ApplyProps(UIComponentProps props)
        {
            if (!(props is UIMeshProps meshSettings))
                throw new ArgumentException("UIMesh: The given settings is not UIMeshSettings");

            UnscaledWidth = meshSettings.UnscaledWidth;
            UnscaledHeight = meshSettings.UnscaledHeight;

            if (meshSettings.Mesh != null)
            {
                MeshFilter.mesh = meshSettings.Mesh;
            }

            if (meshSettings.SharedMesh != null)
            {
                MeshFilter.sharedMesh = meshSettings.SharedMesh;
            }
            
            UIRenderer.sharedMaterial = UIStencilMaterials.GetOrCreate(
                ParentView.StencilId ?? 0).SpritesMaterial;

            base.ApplyProps(props);
        }

        public UIMesh SetWidth(float unscaledWidth)
        {
            UnscaledWidth = unscaledWidth;
            return this;
        }

        public UIMesh SetHeight(float unscaledHeight)
        {
            UnscaledHeight = unscaledHeight;
            return this;
        }

        public UIMesh SetSharedMesh(Mesh mesh)
        {
            MeshFilter.sharedMesh = mesh;
            return this;
        }

        public UIMesh SetMesh(Mesh mesh)
        {
            MeshFilter.mesh = mesh;
            return this;
        }

        private void SetColor(Color color)
        {
            var colors = new Color[MeshFilter.mesh.colors.Length];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = color;
            MeshFilter.mesh.colors = colors;
        }
    }
}