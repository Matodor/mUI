using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mUIApp.Other;
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

    public class UILabel : UIObject
    {
        public override float Width { get; }
        public override float Height { get; }
        public TextAlignment TextAlignment { get; set; }

        private mUIFont _cachedFont;
        private string _cachedText;
        private float _textWidth;
        private float _textHeight;
        private float _charSpacingScale;

        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;
        private MaterialPropertyBlock _textPropertyBlock;
        
        public UILabel(BaseView view, string text, string fontName) : base(view, false)
        {
            TextAlignment = TextAlignment.Left;
            Renderer = GameObject.AddComponent<MeshRenderer>();

            _charSpacingScale = 1;
            _cachedFont = mUI.GetFont(fontName);
            _cachedText = text;
            _meshRenderer = (MeshRenderer) Renderer;
            _meshRenderer.sortingOrder = view.SortingOrder;
            _meshFilter = GameObject.AddComponent<MeshFilter>();
            _textPropertyBlock = new MaterialPropertyBlock();
             
            UpdateMeshText();
        }
        
        private void UpdateMeshText()
        {
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
                var lastWidth = 0f;

                mUI.Log("Line: " + line);
                foreach (char ch in line)
                {
                    mUI.Log("Char: " + ch);

                    if (ch == ' ')
                    {
                        lineWidth += _cachedFont.SpaceLength + _cachedFont.AvgCharWidth*_charSpacingScale*0.1f;
                        continue;
                    }

                    if (!_cachedFont.Contains(ch))
                        continue;

                    var charMesh = _cachedFont.GetCharMesh(ch);
                    var charWidth = charMesh.Width;
                    var charHeight = charMesh.Height;

                    var offsetPos = new Vector3(
                        lineWidth,
                        sumHeight - charHeight * charMesh.Pivot.y, 0);

                    vertices[charIndex + 0] = charMesh.Vertices[0] + offsetPos;
                    vertices[charIndex + 1] = charMesh.Vertices[1] + offsetPos;
                    vertices[charIndex + 2] = charMesh.Vertices[2] + offsetPos;
                    vertices[charIndex + 3] = charMesh.Vertices[3] + offsetPos;

                    colors[charIndex + 0] = Color.white;
                    colors[charIndex + 1] = Color.white;
                    colors[charIndex + 2] = Color.white;
                    colors[charIndex + 3] = Color.white;

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

                    lastWidth = charWidth + _cachedFont.AvgCharWidth*_charSpacingScale*0.3f;
                    lineWidth += lastWidth;
                    charIndex = charIndex + 4;
                    trianglesIndex = trianglesIndex + 6;
                    charsInLine++;
                }

                lineWidth -= lastWidth;

                switch (TextAlignment)
                {
                    case TextAlignment.Center:
                        for (int i = charIndex - charsInLine * 4; i < charIndex; i++)
                            vertices[i] = new Vector3(vertices[i].x - lineWidth / 2f, vertices[i].y, vertices[i].z);
                        break;
                    case TextAlignment.Right:
                        for (int i = charIndex - charsInLine * 4; i < charIndex; i++)
                            vertices[i] = new Vector3(vertices[i].x - lineWidth, vertices[i].y, vertices[i].z);
                        break;
                }

                sumHeight -= _cachedFont.MaxCharHeight;
                if (_textWidth < lineWidth)
                    _textWidth = lineWidth;
            }

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
        }
    }
}