﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace mFramework.UI
{
    public class UILabelSettings : UIComponentSettings
    {
        public string Text = string.Empty;
        public string Font = "Arial";
        public int Size = 40;
        public TextAnchor? TextAnchor = null;
        public UIColor Color = UIColors.White;
        public FontStyle FontStyle = FontStyle.Normal;
        public TextAlignment TextAlignment = TextAlignment.Left;
        public float LetterSpacing = 1f;
        public float WordSpacing = 1f;
        public float LinesSpacing = 1f;
        public float? MaxWidth = null;
    }

    public enum VerticalAlign
    {
        BASELINE = 0,
        ASCENDERLINE = 1,
        DESCENDERLINE = 2
    }

    public class TextFormatting
    {
        public int? Size = null;
        public Color32? Color = null; 
        public float? LetterSpacing = null;
        public float? WordSpacing = null;
        public FontStyle? FontStyle = null;
        public float? LinesSpacing = null;
    }

    public class UILabel : UIComponent, IUIColored, IUIRenderer
    {
        public const float DEFAULT_HARSHNESS = 2;

        public Renderer UIRenderer { get; private set; }
        public string Text => _cachedText;
        public int Size => _fontSize;
        public event UIEventHandler<UILabel> TextUpdated = delegate { };

        private UIFont _cachedFont;
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;

        private string _cachedText;

        private int _fontSize = 50;
        private float _letterSpacing = 1;
        private float _wordSpacing = 1;
        private float _linesSpacing = 1f;

        private float? _maxWidth = null;
        private TextAlignment _textAlignment;
        private TextAnchor? _textAnchor;
        private VerticalAlign _verticalAlign;
        private FontStyle _fontStyle;
        private Color _color;

        private bool _needUpdate;

        private float _textWidth;
        private float _textHeight;

        private float _left;
        private float _right;
        private float _top;
        private float _bottom;

        private Dictionary<int, TextFormatting> _textFormatting;

        protected override void Init()
        {
            _textFormatting = new Dictionary<int, TextFormatting>();

            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshFilter.mesh = new Mesh();
            _meshFilter.mesh.Clear();

            UIRenderer = _meshRenderer;

            SortingOrderChanged += s =>
            {
                UIRenderer.sortingOrder = SortingOrder();
            };

            ActiveChanged += s =>
            {
                if (IsActive && _needUpdate)
                {
                    _needUpdate = false;
                    CreateMesh();
                }
            };

            base.Init();
        }

        /*static UILabel()
        {
            Font.textureRebuilt += font =>
            {
                mCore.Log($"Font rebuilt: {font.name}");
            };
        }*/

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!(settings is UILabelSettings labelSettings))
                throw new ArgumentException("UILabel: The given settings is not UILabelSettings");

            Font.textureRebuilt += FontRebuilt;
            BeforeDestroy += s => Font.textureRebuilt -= FontRebuilt;
            
            _maxWidth = labelSettings.MaxWidth;
            _cachedText = labelSettings.Text;
            _fontSize = labelSettings.Size;
            _textAnchor = labelSettings.TextAnchor;
            _color = labelSettings.Color.Color32;
            _fontStyle = labelSettings.FontStyle;
            _textAlignment = labelSettings.TextAlignment;
            _letterSpacing = labelSettings.LetterSpacing;
            _wordSpacing = labelSettings.WordSpacing;
            _linesSpacing = labelSettings.LinesSpacing;

            if (_textAnchor == null)
            {
                if (_textAlignment == TextAlignment.Left)
                    _textAnchor = TextAnchor.LowerLeft;
                else if (_textAlignment == TextAlignment.Center)
                    _textAnchor = TextAnchor.LowerCenter;
                else if (_textAlignment == TextAlignment.Right)
                    _textAnchor = TextAnchor.LowerRight;
            }

            _cachedFont = mUI.GetFont(labelSettings.Font);

            if (_cachedFont == null)
                throw new Exception("Not fount font: " + labelSettings.Font);

            UIRenderer.sharedMaterial = UIStencilMaterials.GetOrCreate(ParentView.StencilId ?? 0)
                .TextMaterials[labelSettings.Font];

            RequestCharactersInFont();
            UpdateMaterial(_cachedFont.Font);
            CreateMesh(true);
            SetColor(_color);

            base.ApplySettings(settings);
        }

        private void FontRebuilt(Font font)
        {
            if (_cachedFont.Font == font)
            {
                RequestCharactersInFont();
                UpdateMaterial(font);
                CreateMesh();
            }
        }

        private void RequestCharactersInFont()
        {
            var formattingIndex = -1;
            var text = _cachedText
                .Replace('\t', ' ')
                .TrimStart('\n');
            
            for (int i = 0; i < text.Length; i++)
            {
                CharacterInfo characterInfo;
                var currentCharacter = text[i];
                var size = 0;

                if (_textFormatting.Count > 0)
                {
                    var skip = ParseFormatting(i, text, ref formattingIndex);
                    if (skip != 0)
                    {
                        i += skip;
                        i--;
                        continue;
                    }

                    if (formattingIndex != -1 && _textFormatting.ContainsKey(formattingIndex))
                    {
                        var currentFormatting = _textFormatting[formattingIndex];
                        size = (int)(currentFormatting.Size.GetValueOrDefault(_fontSize) * _cachedFont.Harshness);

                        if (!_cachedFont.Font.GetCharacterInfo(currentCharacter, out characterInfo,
                            size, currentFormatting.FontStyle.GetValueOrDefault(_fontStyle)))
                        {
                            _cachedFont.Font.RequestCharactersInTexture(currentCharacter.ToString(),
                                size, currentFormatting.FontStyle.GetValueOrDefault(_fontStyle));
                        }
                    }
                    else
                    {
                        size = (int)(_fontSize * _cachedFont.Harshness);

                        if (!_cachedFont.Font.GetCharacterInfo(currentCharacter,
                            out characterInfo, size, _fontStyle))
                        {
                            _cachedFont.Font.RequestCharactersInTexture(currentCharacter.ToString(), size, _fontStyle);
                        }
                    }
                }
                else
                {
                    size = (int)(_fontSize * _cachedFont.Harshness);

                    if (!_cachedFont.Font.GetCharacterInfo(currentCharacter,
                        out characterInfo, size, _fontStyle))
                    {
                        _cachedFont.Font.RequestCharactersInTexture(currentCharacter.ToString(), size, _fontStyle);
                    }
                }
            }
        }

        public UILabel SetFontStyle(FontStyle fontStyle, bool updateMesh = true)
        {
            if (_fontStyle != fontStyle)
            {
                _fontStyle = fontStyle;
                RequestCharactersInFont();

                if (updateMesh)
                    CreateMesh();
            }
            return this;
        }

        public UILabel SetVerticalAlign(VerticalAlign align, bool updateMesh = true)
        {
            if (_verticalAlign != align)
            {
                _verticalAlign = align;
                if (updateMesh)
                    CreateMesh();
            }
            return this;
        }

        public UILabel SetLetterSpacing(float spacing, bool updateMesh = true)
        {
            if (Math.Abs(_letterSpacing - spacing) > 0.001f)
            {
                _letterSpacing = spacing;
                if (updateMesh)
                    CreateMesh();
            }
            return this;
        }

        public UILabel SetFontSize(int size, bool updateMesh = true)
        {
            if (_fontSize != size)
            {
                _fontSize = size;
                RequestCharactersInFont();
                
                if (updateMesh)
                    CreateMesh();
            }
            return this;
        }

        public UILabel SetTextAlignment(TextAlignment alignment, bool updateMesh = true)
        {
            if (_textAlignment != alignment)
            {
                _textAlignment = alignment;

                if (updateMesh)
                    CreateMesh();
            }
            return this;
        }

        public UILabel SetText(string text, bool updateMesh = true)
        {
            if (text == null || _cachedText == text)
                return this;

            _cachedText = text;
            RequestCharactersInFont();

            if (updateMesh)
                CreateMesh();
            return this;
        }

        private void UpdateMaterial(Font font)
        {
            _meshRenderer.sharedMaterial.SetTexture("_MainTex", font.material.mainTexture);
            _meshRenderer.sharedMaterial.SetTextureOffset("_MainTex", font.material.mainTextureOffset);
            _meshRenderer.sharedMaterial.SetTextureScale("_MainTex", font.material.mainTextureScale);
        }

        public UILabel TextFormatting(int index, TextFormatting formatting)
        {
            if (formatting.Size == null)
                formatting.Size = _fontSize;
            if (formatting.Color == null)
                formatting.Color = _color;
            if (formatting.LetterSpacing == null)
                formatting.LetterSpacing = _letterSpacing;
            if (formatting.WordSpacing == null)
                formatting.WordSpacing = _wordSpacing;
            if (formatting.FontStyle == null)
                formatting.FontStyle = _fontStyle;
            if (formatting.LinesSpacing == null)
                formatting.LinesSpacing = _linesSpacing;

            if (_textFormatting.ContainsKey(index))
                _textFormatting[index] = formatting;
            else
                _textFormatting.Add(index, formatting);

            RequestCharactersInFont();
            CreateMesh();
            return this;
        }

        private void CheckBoundingBox(float vertX, float vertY)
        {
            if (_left > vertX)
                _left = vertX;

            if (_right < vertX)
                _right = vertX;

            if (_top < vertY)
                _top = vertY;

            if (_bottom > vertY)
                _bottom = vertY;
        }

        private void AlignVertices(IList<Vector3> vertices, TextAlignment alignment, 
            int startIndex, int endIndex, float lineWidth, float yOffset = 0.0f)
        {
            switch (alignment)
            {
                case TextAlignment.Center:
                    for (int i = startIndex; i <= endIndex; i++)
                    {
                        vertices[i] = new Vector3(
                            vertices[i].x - lineWidth / 2f,
                            vertices[i].y + yOffset,
                            vertices[i].z
                        );

                        CheckBoundingBox(vertices[i].x, vertices[i].y);
                    }
                    break;
                case TextAlignment.Right:
                    for (int i = startIndex; i <= endIndex; i++)
                    {
                        vertices[i] = new Vector3(
                            vertices[i].x - lineWidth,
                            vertices[i].y + yOffset,
                            vertices[i].z
                        );
                        CheckBoundingBox(vertices[i].x, vertices[i].y);
                    }
                    break;
                case TextAlignment.Left:
                    for (int i = startIndex; i <= endIndex; i++)
                    {
                        vertices[i] = new Vector3(
                            vertices[i].x,
                            vertices[i].y + yOffset,
                            vertices[i].z
                        );
                        CheckBoundingBox(vertices[i].x, vertices[i].y);
                    }
                    break;
            }
        }

        private static int ParseFormatting(int i, string text, ref int formattingIndex)
        {
            // [=0]
            if (i + 3 < text.Length &&
                text[i + 0] == '[' &&
                text[i + 1] == '=' &&
                text[i + 3] == ']' &&
                char.IsNumber(text[i + 2]))
            {
                if (int.TryParse(text.Substring(i + 2, 1), out var index))
                    formattingIndex = index;
                return 4;
            }

            // [=10]
            if (i + 4 < text.Length &&
                text[i] == '[' &&
                text[i + 1] == '=' &&
                text[i + 4] == ']' &&
                char.IsNumber(text[i + 2]) &&
                char.IsNumber(text[i + 3]))
            {
                if (int.TryParse(text.Substring(i + 2, 2), out var index))
                    formattingIndex = index;
                return 5;
            }

            // [=/]
            if (i + 3 < text.Length &&
                text[i + 0] == '[' &&
                text[i + 1] == '=' &&
                text[i + 2] == '/' &&
                text[i + 3] == ']')
            {
                formattingIndex = -1;
                return 4;
            }

            return 0;
        }

        internal void CreateMesh(bool ignoreActive = false)
        {
            if (!ignoreActive && !IsActive)
            {
                _needUpdate = true;
                return;
            }

            if (string.IsNullOrEmpty(_cachedText))
                return;

            const int maxSize = 256 / 2;
            const float pixelsPerWorldUnit = 100f;
            
            _fontSize = mMath.Clamp(_fontSize, 1, maxSize);

            var localScale = LocalScale();
            Scale(1, 1);

            var text = _cachedText
                .Replace('\t', ' ')
                .TrimStart('\n');

            _left = 0;
            _bottom = 0;
            _right = 0;
            _top = 0;

            var verticesList = new List<Vector3>(text.Length * 4);
            var normalsList = new List<Vector3>(text.Length * 4);
            var uvList = new List<Vector2>(text.Length * 4);
            var colorsList = new List<Color32>(text.Length * 4);
            var trianglesList = new List<int>(text.Length * 6);

            _textHeight = 0f;
            _textWidth = 0f;

            var currentFormatting = (TextFormatting) null;
            var textXOffset = 0f;
            var textYOffset = 0f;
            var lineHeight = 0f;

            var formattingIndex = -1;
            var startLineIndex = 0;
            var lines = 1;
            var pureLineHeight = 0f;

            for (int i = 0; i < text.Length; i++)
            {
                var forceNewLine = false;
                CharacterInfo characterInfo;
                var currentCharacter = text[i];
                var size = 0;

                if (_textFormatting.Count > 0)
                {
                    var skip = ParseFormatting(i, text, ref formattingIndex);
                    if (skip != 0)
                    {
                        i += skip;
                        if (i >= text.Length || text[i] == '\n')
                        {
                            i--;
                            goto End;
                        }

                        i--;
                        continue;
                    }

                    if (formattingIndex != -1 && _textFormatting.ContainsKey(formattingIndex))
                    {
                        currentFormatting = _textFormatting[formattingIndex];
                        size = (int) (currentFormatting.Size.GetValueOrDefault(_fontSize) * _cachedFont.Harshness);

                        if (!_cachedFont.Font.GetCharacterInfo(currentCharacter, out characterInfo,
                            size, currentFormatting.FontStyle.GetValueOrDefault(_fontStyle)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        currentFormatting = null;
                        size = (int) (_fontSize * _cachedFont.Harshness);

                        if (!_cachedFont.Font.GetCharacterInfo(currentCharacter,
                            out characterInfo, size, _fontStyle))
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    size = (int) (_fontSize * _cachedFont.Harshness);

                    if (!_cachedFont.Font.GetCharacterInfo(currentCharacter, 
                        out characterInfo, size, _fontStyle))
                        continue;
                }

                var minX = characterInfo.minX / pixelsPerWorldUnit / _cachedFont.Harshness;
                var maxX = characterInfo.maxX / pixelsPerWorldUnit / _cachedFont.Harshness;
                var minY = characterInfo.minY / pixelsPerWorldUnit / _cachedFont.Harshness;
                var maxY = characterInfo.maxY / pixelsPerWorldUnit / _cachedFont.Harshness;

                minX += minX * -1;
                maxX += minX * -1;

                if (_maxWidth.HasValue && textXOffset + maxX > _maxWidth)
                {
                    i--;
                    forceNewLine = true;
                    goto End;
                }

                verticesList.Add(new Vector3(textXOffset + minX, minY));
                verticesList.Add(new Vector3(textXOffset + minX, maxY));
                verticesList.Add(new Vector3(textXOffset + maxX, maxY));
                verticesList.Add(new Vector3(textXOffset + maxX, minY));

                //mCore.Log("{0} minX={1} maxX={2} minY={3} maxY={4} advance={5} bearing={6} size={7} h={8} w={9}", 
                //  currentCharacter, characterInfo.minX, characterInfo.maxX, characterInfo.minY, characterInfo.maxY, 
                //  characterInfo.advance, characterInfo.bearing, characterInfo.size, characterInfo.glyphHeight, characterInfo.glyphWidth);

                if (lineHeight < characterInfo.size / pixelsPerWorldUnit / _cachedFont.Harshness)
                    lineHeight = characterInfo.size / pixelsPerWorldUnit / _cachedFont.Harshness;

                if (currentCharacter == ' ')
                {
                    textXOffset += characterInfo.advance / pixelsPerWorldUnit / _cachedFont.Harshness *
                                   (currentFormatting == null
                                       ? _wordSpacing
                                       : currentFormatting.WordSpacing.GetValueOrDefault(_wordSpacing));
                }
                else
                {
                    textXOffset += characterInfo.advance / pixelsPerWorldUnit / _cachedFont.Harshness *
                                   (currentFormatting == null
                                       ? _letterSpacing
                                       : currentFormatting.LetterSpacing.GetValueOrDefault(_letterSpacing));
                }

                if (pureLineHeight < verticesList[verticesList.Count - 2].y - verticesList[verticesList.Count - 1].y)
                    pureLineHeight = verticesList[verticesList.Count - 2].y - verticesList[verticesList.Count - 1].y;

                var color = currentFormatting == null 
                    ? (Color32) _color
                    : currentFormatting.Color.GetValueOrDefault(Color.white);

                colorsList.Add(color);
                colorsList.Add(color);
                colorsList.Add(color);
                colorsList.Add(color);

                uvList.Add(characterInfo.uvBottomLeft);
                uvList.Add(characterInfo.uvTopLeft);
                uvList.Add(characterInfo.uvTopRight);
                uvList.Add(characterInfo.uvBottomRight);

                normalsList.Add(Vector3.back);
                normalsList.Add(Vector3.back);
                normalsList.Add(Vector3.back);
                normalsList.Add(Vector3.back);

                trianglesList.Add(verticesList.Count - 4); // 0
                trianglesList.Add(verticesList.Count - 3); // 1 
                trianglesList.Add(verticesList.Count - 2); // 2
                trianglesList.Add(verticesList.Count - 2); // 2
                trianglesList.Add(verticesList.Count - 1); // 3
                trianglesList.Add(verticesList.Count - 4); // 0

                // new line
            End:
                if (forceNewLine || i + 1 >= text.Length || i + 1 < text.Length && text[i + 1] == '\n')
                {
                    var lineWidth = verticesList[verticesList.Count - 1].x - verticesList[startLineIndex].x;
                    if (lines > 1)
                    {
                        lineHeight = lineHeight * (currentFormatting == null
                                            ? _linesSpacing
                                            : currentFormatting.LinesSpacing.GetValueOrDefault(_linesSpacing));
                        textYOffset -= lineHeight;
                    }
                    else
                    {
                        textYOffset -= pureLineHeight;
                    }

                    AlignVertices(
                        vertices: verticesList,
                        alignment: _textAlignment,
                        startIndex: startLineIndex,
                        endIndex: verticesList.Count - 1,
                        lineWidth: lineWidth,
                        yOffset: textYOffset
                    );

                    textXOffset = 0f;
                    lineHeight = 0f;
                    lines++;
                    startLineIndex = verticesList.Count;
                }
            }

            _textHeight = _top - _bottom;
            _textWidth = _right - _left;

            if (_textAnchor != null)
            {
                var _anchorOffset = new Vector2(0, 0);

                switch (_textAnchor)
                {
                    case TextAnchor.UpperLeft:
                        _anchorOffset = new Vector2(_textWidth / 2, -_textHeight / 2);
                        break;
                    case TextAnchor.UpperCenter:
                        _anchorOffset = new Vector2(0, -_textHeight / 2);
                        break;
                    case TextAnchor.UpperRight:
                        _anchorOffset = new Vector2(-_textWidth / 2, -_textHeight / 2);
                        break;

                    case TextAnchor.MiddleLeft:
                        _anchorOffset = new Vector2(_textWidth / 2, 0);
                        break;
                    case TextAnchor.MiddleCenter:
                        _anchorOffset = new Vector2(0, 0);
                        break;
                    case TextAnchor.MiddleRight:
                        _anchorOffset = new Vector2(-_textWidth / 2, 0);
                        break;

                    case TextAnchor.LowerLeft:
                        _anchorOffset = new Vector2(_textWidth / 2, _textHeight / 2);
                        break;
                    case TextAnchor.LowerCenter:
                        _anchorOffset = new Vector2(0, _textHeight / 2);
                        break;
                    case TextAnchor.LowerRight:
                        _anchorOffset = new Vector2(-_textWidth / 2, _textHeight / 2);
                        break;
                }
                
                //mCore.Log("_left = {0}", _left);
                //mCore.Log("_right = {0}", _right);

                var xDiff = -_left - _textWidth / 2 + _anchorOffset.x;
                var yDiff = _textHeight / 2 + _anchorOffset.y;

                for (int i = 0; i < verticesList.Count; i++)
                {
                    verticesList[i] = new Vector3(
                        verticesList[i].x + xDiff,
                        verticesList[i].y + yDiff,
                        verticesList[i].z
                    );
                    CheckBoundingBox(verticesList[i].x, verticesList[i].y);
                }
            }

            _meshFilter.mesh.Clear();
            _meshFilter.mesh.vertices = verticesList.ToArray();
            _meshFilter.mesh.triangles = trianglesList.ToArray();
            _meshFilter.mesh.colors32 = colorsList.ToArray();
            _meshFilter.mesh.normals = normalsList.ToArray();
            _meshFilter.mesh.uv = uvList.ToArray();

            //_meshRenderer.sharedMaterial.SetTexture("_MainTex", _cachedFont.Font.material.mainTexture);
            //_meshRenderer.sharedMaterial.SetTextureOffset("_MainTex", _cachedFont.Font.material.mainTextureOffset);
            //_meshRenderer.sharedMaterial.SetTextureScale("_MainTex", _cachedFont.Font.material.mainTextureScale);

            Scale(localScale);
            TextUpdated.Invoke(this);
        }

        public override UIRect GetRect()
        {
            var pos = Position();
            var scale = GlobalScale();

            return new UIRect
            {
                Position = pos,
                Bottom = pos.y + _bottom * scale.y,
                Top = pos.y + _top * scale.y,
                Left = pos.x + _left * scale.x,
                Right = pos.x + _right * scale.x,
            };
        }

        public override float GetHeight()
        {
            return _textHeight * GlobalScale().y;
        }

        public override float GetWidth()
        {
            return _textWidth * GlobalScale().x;
        }

        public Color GetColor()
        {
            return _color;
        }

        public IUIColored SetColor(Color32 color)
        {
            if (_color == color)
                return this;
            _color = color;

            if (_textFormatting.Count > 0)
            {
                CreateMesh();
            }
            else
            {
                var colors = new Color[_meshFilter.mesh.colors.Length];
                for (int i = 0; i < colors.Length; i++)
                    colors[i] = color;
                _meshFilter.mesh.colors = colors;
            }
            return this;
        }

        public IUIColored SetColor(UIColor color)
        {
            return SetColor(color.Color32);
        }
    }
}
