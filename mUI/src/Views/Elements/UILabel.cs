#define UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using mUIApp.Other;
using UnityEditor;
using UnityEngine;

namespace mUIApp.Views.Elements
{
    public static class UILabelHelper
    {
        public static UILabel CreateLabel(this BaseView view, string text)
        {
            return CreateLabel(view, text, mUI.DefaultFont, 45).SetName(text);
        }

        private static UILabel CreateLabel(BaseView view, string text, string fontName, float textSize)
        {
            return new UILabel(view, text, fontName);
        }

        public static UILabel TextAlignment(this UILabel obj, TextAlignment alignment)
        {
            obj.TextAlignment = alignment;
            return obj;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UILabelBehaviour))]
    class UILabelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            UILabelBehaviour behaviour = (UILabelBehaviour) target;
            behaviour.CachedColor = EditorGUILayout.ColorField("Label color", behaviour.CachedColor);

            if (GUILayout.Button("Print color"))
            {
                mUI.Log("Color: {0}", behaviour.CachedColor);
            }

            if (GUILayout.Button("Update"))
            {
                behaviour.Label.Color(behaviour.CachedColor);
            }
        }
    }

    class UILabelBehaviour : MonoBehaviour
    {
        public Color32 CachedColor { get; set; }
        public UILabel Label { get; set; }
    }
#endif

    public class UILabel : UIObject
    {
        public override float Width { get { return _textWidth; } }
        public override float Height { get { return _textHeight; } }
        public string LabelText { get { return _cachedText; } }

        public TextAlignment TextAlignment
        {
            get
            {
                return _textAlignment;
            }

            set
            {
                _textAlignment = value;
                UpdateAlignment();
            }
        }

        private mUIFont _cachedFont;
        private string _cachedText;
        private float _textWidth;
        private float _textHeight;
        private float _letterSpacingScale;
        private TextAlignment _textAlignment;

        private readonly MeshRenderer _meshRenderer;
        private readonly MeshFilter _meshFilter;
        private readonly MaterialPropertyBlock _textPropertyBlock;

#if UNITY_EDITOR
        private readonly UILabelBehaviour _labelBehaviour;
#endif

        public UILabel(BaseView view, string text, string fontName) : base(view, false)
        {
            _textAlignment = TextAlignment.Left;
            Renderer = GameObject.AddComponent<MeshRenderer>();

            _letterSpacingScale = 1;
            _cachedFont = mUI.GetFont(fontName);
            _cachedText = text;
            _meshRenderer = (MeshRenderer) Renderer;
            _meshRenderer.sortingOrder = view.SortingOrder;
            _meshFilter = GameObject.AddComponent<MeshFilter>();
            _textPropertyBlock = new MaterialPropertyBlock();
             
            UpdateMeshText();

#if UNITY_EDITOR
            _labelBehaviour = GameObject.AddComponent<UILabelBehaviour>();
            _labelBehaviour.Label = this;
            _labelBehaviour.CachedColor = mUIColors.White.Color32;
            var editor = (UILabelEditor)Editor.CreateEditor(GameObject, typeof(UILabelEditor));
#endif
        }

        public UILabel Color(Color32 newColor)
        {
            /*Color32[] colors = new Color32[_meshFilter.mesh.colors32.Length];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = newColor;
            _meshFilter.mesh.colors32 = colors;*/

#if UNITY_EDITOR
            _labelBehaviour.CachedColor = newColor;
#endif

            _textPropertyBlock.SetColor("_Color", newColor);
            _meshRenderer.SetPropertyBlock(_textPropertyBlock);
            return this;
        }

        public UILabel Color(mUIColor newColor)
        {
            return Color(newColor.Color32);
        }

        public UILabel Size(int size = 50)
        {
            Transform.localScale = new Vector3(
                Transform.localScale.x * (size / 50f),
                Transform.localScale.y * (size / 50f),
                Transform.localScale.z 
            );
            return this;
        }

        public UILabel Text(string text)
        {
            if (string.IsNullOrEmpty(text))
                return this;

            _cachedText = text;
            UpdateMeshText();
            return this;
        }

        private void UpdateAlignment()
        {
            UpdateMeshText();
        }

        private float LetterSpacing()
        {
            return _cachedFont.AvgCharWidth*0.3f*_letterSpacingScale;
        }

        private void UpdateMeshText()
        {
            var localScale = Transform.localScale;
            GameObject.name = _cachedText;
            Transform.localScale = new Vector3(1, 1, 1);

            var textLines = _cachedText.Split('\n');
            var charCount = _cachedText.Replace("\n", "").Replace("\t", "").Count(_cachedFont.Contains);

            var vertices = new Vector3[charCount * 4];
            var normals = new Vector3[charCount * 4];
            var uv = new Vector2[charCount * 4];
            var colors = new Color32[charCount * 4];
            var triangles = new int[charCount * 6]; 

            var sumHeight = 0f;
            var charIndex = 0;
            var trianglesIndex = 0;

            foreach (string line in textLines)
            {
                var lineWidth = 0f;
                var lineHeight = 0f;
                var charsInLine = 0;
                var lastCharOffset = 0f;

                foreach (char ch in line)
                {
                    if (ch == ' ')
                    {
                        lastCharOffset = _cachedFont.SpaceLength + LetterSpacing();
                        lineWidth += lastCharOffset;
                        continue;
                    }

                    if (!_cachedFont.Contains(ch))
                        continue;

                    var charMesh = _cachedFont.GetCharMesh(ch);
                    var charWidth = charMesh.Width;
                    var charHeight = charMesh.Height;
                    var offsetPos = new Vector3(lineWidth, sumHeight - charHeight * charMesh.Pivot.y, 0);

                    vertices[charIndex + 0] = charMesh.Vertices[0] + offsetPos;
                    vertices[charIndex + 1] = charMesh.Vertices[1] + offsetPos;
                    vertices[charIndex + 2] = charMesh.Vertices[2] + offsetPos;
                    vertices[charIndex + 3] = charMesh.Vertices[3] + offsetPos;

                    colors[charIndex + 0] = UnityEngine.Color.white;
                    colors[charIndex + 1] = UnityEngine.Color.white;
                    colors[charIndex + 2] = UnityEngine.Color.white;
                    colors[charIndex + 3] = UnityEngine.Color.white;

                    uv[charIndex + 0] = charMesh.Uv[0];
                    uv[charIndex + 1] = charMesh.Uv[1];
                    uv[charIndex + 2] = charMesh.Uv[2];
                    uv[charIndex + 3] = charMesh.Uv[3];

                    normals[charIndex + 0] = Vector3.back;
                    normals[charIndex + 1] = Vector3.back;
                    normals[charIndex + 2] = Vector3.back;
                    normals[charIndex + 3] = Vector3.back;

                    triangles[trianglesIndex + 0] = charIndex + 0;
                    triangles[trianglesIndex + 1] = charIndex + 1;
                    triangles[trianglesIndex + 2] = charIndex + 2;
                    triangles[trianglesIndex + 3] = charIndex + 2;
                    triangles[trianglesIndex + 4] = charIndex + 3;
                    triangles[trianglesIndex + 5] = charIndex + 0;

                    if (charHeight > lineHeight)
                        lineHeight = charHeight;

                    lastCharOffset = charWidth + LetterSpacing();
                    lineWidth += lastCharOffset;
                    charIndex = charIndex + 4;
                    trianglesIndex = trianglesIndex + 6;
                    charsInLine++;
                }

                lineWidth -= lastCharOffset;

                switch (_textAlignment)
                {
                    case TextAlignment.Center:
                        for (int i = charIndex - charsInLine*4; i < charIndex; i++)
                        {
                            vertices[i].x = vertices[i].x - lineWidth/2f;
                            vertices[i].y = vertices[i].y;
                            vertices[i].z = vertices[i].z;
                        }
                        break;
                    case TextAlignment.Right:
                        for (int i = charIndex - charsInLine*4; i < charIndex; i++)
                        {
                            vertices[i].x = vertices[i].x - lineWidth;
                            vertices[i].y = vertices[i].y;
                            vertices[i].z = vertices[i].z;
                        }
                        break;
                }

                sumHeight -= _cachedFont.MaxCharHeight;
                if (_textWidth < lineWidth)
                    _textWidth = lineWidth;
            }

            _textHeight = sumHeight + _cachedFont.MaxCharHeight;

            var textMesh = new Mesh
            {
                vertices = vertices,
                colors32 = colors,
                normals = normals,
                uv = uv,
                triangles = triangles
            };
            textMesh.RecalculateBounds();

            _meshFilter.mesh = textMesh;
            _meshRenderer.material = _cachedFont.Material;
            _meshRenderer.SetPropertyBlock(_textPropertyBlock);
            Transform.localScale = localScale;
        }
    }
}