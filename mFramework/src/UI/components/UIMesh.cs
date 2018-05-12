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

    public class UIMesh : UIComponent, IUIColored, IUIRenderer<MeshRenderer>, IUIRenderer
    {
        public virtual Color Color
        {
            get => _color ?? Color.black;
            set
            {
                _color = value;
                SetColor(value);
            }
        }

        public virtual float Opacity
        {
            get => _color?.a ?? 1;
            set
            {
                if (_color == null)
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
                else
                {
                    var color = _color.Value;
                    color.a = value;
                    SetColor(color);
                }
            }
        }

        public MeshFilter MeshFilter { get; private set; }
        public MeshRenderer UIRenderer { get; private set; }
        Renderer IUIRenderer.UIRenderer => UIRenderer;

        private Color? _color;

        protected override void AfterAwake()
        {
            UIRenderer = GameObject.AddComponent<MeshRenderer>();
            MeshFilter = GameObject.AddComponent<MeshFilter>();

            SortingOrderChanged += s =>
            {
                UIRenderer.sortingOrder = SortingOrder;
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
            UnscaledWidth = meshSettings.Width;
            UnscaledHeight = meshSettings.Height;

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
            
            UIRenderer.sharedMaterial = UIStencilMaterials.GetOrCreate(ParentView.StencilId ?? 0).SpritesMaterial;

            base.ApplySettings(settings);
        }

        public UIMesh SetWidth(float width)
        {
            UnscaledWidth = width;
            return this;
        }

        public UIMesh SetHeight(float height)
        {
            UnscaledHeight = height;
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
            _color = color;

            var colors = new Color[MeshFilter.mesh.colors.Length];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = color;

            MeshFilter.mesh.colors = colors;
        }
    }
}