using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIMeshSettings : UIComponentSettings
    {
        public Mesh Mesh = null;
        public Mesh SharedMesh = null;
        public float Width;
        public float Height;
    }

    public class UIMesh : UIComponent, IUIRenderer
    {
        public Renderer UIRenderer => _meshRenderer;
        public MeshFilter MeshFilter => _meshFilter;
        public MeshRenderer MeshRenderer => _meshRenderer;

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;

        private float _width;
        private float _height;

        protected override void Init()
        {
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshFilter = gameObject.AddComponent<MeshFilter>();

            SortingOrderChanged += s =>
            {
                UIRenderer.sortingOrder = SortingOrder();
            };

            base.Init();
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIMeshSettings meshSettings))
                throw new ArgumentException("UIMesh: The given settings is not UIMeshSettings");

            if (meshSettings.Mesh != null)
                _meshFilter.mesh = meshSettings.Mesh;
            if (meshSettings.SharedMesh != null)
                _meshFilter.sharedMesh = meshSettings.SharedMesh;

            _width = meshSettings.Width;
            _height = meshSettings.Height;
            _meshRenderer.sharedMaterial = UIStencilMaterials.GetOrCreate(ParentView.StencilId ?? 0).SpritesMaterial;

            base.ApplySettings(settings);
        }

        public UIMesh SetWidth(float width)
        {
            _width = width;
            return this;
        }

        public UIMesh SetHeight(float height)
        {
            _height = height;
            return this;
        }
        public override float GetHeight()
        {
            return _height * GlobalScale().y;
        }

        public override float GetWidth()
        {
            return _width * GlobalScale().x;
        }

        public UIMesh SetSharedMesh(Mesh mesh)
        {
            _meshFilter.sharedMesh = mesh;
            return this;
        }

        public UIMesh SetMesh(Mesh mesh)
        {
            _meshFilter.mesh = mesh;
            return this;
        }
    }
}