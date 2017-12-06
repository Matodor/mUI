using System;
using UnityEngine;

namespace mFramework.UI
{
    public class UIMeshSettings : UIComponentSettings
    {
        public Mesh Mesh = null;
        public Mesh SharedMesh = null;
    }

    public class UIMesh : UIComponent, IUIRenderer
    {
        public Renderer UIRenderer => _meshRenderer;
        public MeshFilter MeshFilter => _meshFilter;
        public MeshRenderer MeshRenderer => _meshRenderer;

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;

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

            _meshRenderer.sharedMaterial = UIStencilMaterials.GetOrCreate(ParentView.StencilId ?? 0).SpritesMaterial;
            
            if (meshSettings.Mesh != null)
                _meshFilter.mesh = meshSettings.Mesh;
            if (meshSettings.SharedMesh != null)
                _meshFilter.sharedMesh = meshSettings.SharedMesh;

            base.ApplySettings(settings);
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