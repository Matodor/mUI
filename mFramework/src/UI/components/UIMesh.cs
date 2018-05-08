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

    public class UIMesh : UIComponent, IMeshRenderer, IUIColored
    {
        public Renderer UIRenderer => MeshRenderer;
        public MeshFilter MeshFilter { get; private set; }
        public MeshRenderer MeshRenderer { get; private set; }

        public override float UnscaledHeight => _height * GlobalScale.y;
        public override float UnscaledWidth => _width * GlobalScale.x;
        public override Vector2 CenterOffset => Vector2.zero;

        private float _width;
        private float _height;
        private Color? _color;

        protected override void AfterAwake()
        {
            MeshRenderer = GameObject.AddComponent<MeshRenderer>();
            MeshFilter = GameObject.AddComponent<MeshFilter>();

            SortingOrderChanged += s =>
            {
                UIRenderer.sortingOrder = SortingOrder();
            };

            base.AfterAwake();
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UIMeshSettings meshSettings))
                throw new ArgumentException("UIMesh: The given settings is not UIMeshSettings");

            _color = null;
            _width = meshSettings.Width;
            _height = meshSettings.Height;

            if (meshSettings.Mesh != null)
            {
                MeshFilter.mesh = meshSettings.Mesh;
                _color = MeshFilter.mesh.colors[0];
            }

            if (meshSettings.SharedMesh != null)
            {
                MeshFilter.sharedMesh = meshSettings.SharedMesh;
                _color = MeshFilter.mesh.colors[0];
            }
            
            MeshRenderer.sharedMaterial = UIStencilMaterials.GetOrCreate(ParentView.StencilId ?? 0).SpritesMaterial;

            base.ApplySettings(settings);
        }

        public IMeshRenderer SetWidth(float width)
        {
            _width = width;
            return this;
        }

        public IMeshRenderer SetHeight(float height)
        {
            _height = height;
            return this;
        }

        public IMeshRenderer SetSharedMesh(Mesh mesh)
        {
            MeshFilter.sharedMesh = mesh;
            return this;
        }

        public IMeshRenderer SetMesh(Mesh mesh)
        {
            MeshFilter.mesh = mesh;
            return this;
        }

        public Color GetColor()
        {
            return _color ?? new Color(0, 0, 0, 0);
        }

        public float GetOpacity()
        {
            return GetColor().a * 255f;
        }

        public IUIColored SetColor(Color32 color)
        {
            if (_color == color)
                return this;
            _color = color;

            var colors = new Color[MeshFilter.mesh.colors.Length];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = color;
            MeshFilter.mesh.colors = colors;
            return this;
        }

        public IUIColored SetColor(UIColorOldd colorOldd)
        {
            return SetColor(colorOldd.Color32);
        }

        public IUIColored SetOpacity(float opacity)
        {
            var c = GetColor();
            c.a = opacity / 255f;
            SetColor(c);
            return this;
        }
    }
}