using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace mFramework.UI
{
    public class UIStencilMaterials
    {
        public class TextMaterials
        {
            public IReadOnlyDictionary<string, Material> Materials => _materials;

            public Material this[string fontName]
            {
                get
                {
                    if (_materials.ContainsKey(fontName))
                        return _materials[fontName];

                    var uiFont = mUI.GetFont(fontName);
                    var material = CreateTextMaterial(_layer.StencilId, _layer.CompFunc, _layer.WriteMask, _layer.ReadMask);
                    material.SetTexture("_MainTex", uiFont.Font.material.mainTexture);
                    material.SetTextureOffset("_MainTex", uiFont.Font.material.mainTextureOffset);
                    material.SetTextureScale("_MainTex", uiFont.Font.material.mainTextureScale);
                    material.SetVector("_TextureSampleAdd", new Vector4(1f, 1f, 1f, 0f));

                    _materials.Add(fontName, material);
                    return material;
                }
            }

            private readonly Layer _layer;
            private readonly Dictionary<string, Material> _materials;

            public TextMaterials(Layer layer)
            {
                _layer = layer;
                _materials = new Dictionary<string, Material>();
            }
        }

        public class Layer
        {
            public readonly int StencilId;
            public readonly CompareFunction CompFunc;
            public readonly StencilOp OpFunc;
            public readonly int WriteMask;
            public readonly int ReadMask;

            public readonly Material CanvasMaterial;
            public readonly Material SpritesMaterial;
            public readonly TextMaterials TextMaterials;

            public Layer(int stencilId, CompareFunction compFunc, StencilOp opFunc,
                int writeMask = 255, int readMask = 255)
            {
                StencilId = stencilId;

                CompFunc = compFunc;
                OpFunc = opFunc;
                WriteMask = writeMask;
                ReadMask = readMask;

                TextMaterials = new TextMaterials(this);
                CanvasMaterial = CreateCanvasMaterial(stencilId, opFunc, writeMask, readMask);
                SpritesMaterial = CreateSpriteMaterial(stencilId, compFunc, writeMask, readMask);
            }
        }

        public const int MAX_LAYERS = byte.MaxValue;

        private static readonly UIStencilMaterials _instance;
        private readonly Layer[] _layers;

        static UIStencilMaterials()
        {
            if (_instance == null)
                _instance = new UIStencilMaterials();
        }

        private UIStencilMaterials()
        {
            _layers = new Layer[MAX_LAYERS];
            //_layers[0] = Create(0, CompareFunction.Always, StencilOp.Keep, 255, 255);
            //_layers[1] = Create(1, CompareFunction.Equal, StencilOp.Replace, 255, 255);
        }

        public static Layer[] Layers()
        {
            return _instance._layers.Where(l => l != null).ToArray();
        }

        public static Material CreateSpriteMaterial(int stencilId, CompareFunction compFunc, int writeMask = 255, int readMask = 255)
        {
            var spritesMaterial = new Material(Shader.Find("UI/Default"));
            spritesMaterial.SetFloat("_Stencil", stencilId);
            spritesMaterial.SetFloat("_StencilComp", (float)compFunc);
            spritesMaterial.SetFloat("_StencilWriteMask", writeMask);
            spritesMaterial.SetFloat("_StencilReadMask", readMask);
            spritesMaterial.color = Color.white;

            return spritesMaterial;
        }

        public static Material CreateCanvasMaterial(int stencilId, StencilOp opFunc, int writeMask = 255, int readMask = 255)
        {
            var canvasMaterial = new Material(Shader.Find("UI/Default"));
            canvasMaterial.SetFloat("_Stencil", stencilId);
            canvasMaterial.SetFloat("_StencilOp", (float)opFunc);
            canvasMaterial.SetFloat("_StencilComp", (float)CompareFunction.Always);
            canvasMaterial.SetFloat("_StencilWriteMask", writeMask);
            canvasMaterial.SetFloat("_StencilReadMask", readMask);
            canvasMaterial.color = Color.white;

            return canvasMaterial;
        }

        public static Material CreateTextMaterial(int stencilId, CompareFunction compFunc, int writeMask = 255, int readMask = 255)
        {
            var textMaterial = new Material(Shader.Find("UI/Default Font"));
            textMaterial.SetColor("_TextureSampleAdd", new Color32(255, 255, 255, 0));
            textMaterial.SetFloat("_Stencil", stencilId);
            textMaterial.SetFloat("_StencilComp", (float)compFunc);
            textMaterial.SetFloat("_StencilWriteMask", writeMask);
            textMaterial.SetFloat("_StencilReadMask", readMask);
            textMaterial.color = Color.white;

            return textMaterial;
        }

        public static Layer Create(int stencilId, CompareFunction compFunc, StencilOp opFunc, 
            int writeMask = 255, int readMask = 255)
        {
            return new Layer(stencilId, compFunc, opFunc, writeMask, readMask);
        }

        public static Layer GetOrCreate(int stencilId)
        {
            if (_instance._layers[stencilId] != null)
                return _instance._layers[stencilId];
            return _instance._layers[stencilId] = Create(stencilId, CompareFunction.Equal, StencilOp.Replace, stencilId, stencilId);
        }

        internal static void CreateMesh(UIObject obj)
        {
            var meshFilter = obj.GameObject.AddComponent<MeshFilter>();
            //meshFilter.mesh = new Mesh();
            //meshFilter.mesh.Clear();
            
            var vertices = new Vector3[4];
            var normals = new Vector3[4];
            var uv = new Vector2[4];
            var colors = new Color32[4];
            var triangles = new int[2 * 3];

            var width = obj.Width;
            var height = obj.Height;

            vertices[0] = new Vector3(-width / 2, -height / 2);
            vertices[1] = new Vector3(-width / 2, +height / 2);
            vertices[2] = new Vector3(+width / 2, +height / 2);
            vertices[3] = new Vector3(+width / 2, -height / 2);

            for (int i = 0; i < 4; i++)
            {
                colors[i] = Color.white;
                uv[i] = new Vector2(0f, 0f);
                normals[i] = new Vector3(0f, 0f, -1f);
            }

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;

            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;

            var mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.colors32 = colors;
            mesh.normals = normals;
            mesh.uv = uv;

            meshFilter.mesh = mesh;
        }
    }
}