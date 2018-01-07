using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mFramework.UI
{
    public class UILabelSettings : UIComponentSettings
    {
        public string Text = string.Empty;
        public string Font = "Arial";
        public int Size = 40;
        public TextAnchor? TextAnchor = null;
        public UIColor Color = UIColor.White;
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

    public class UILabel : UIComponent, IUIColored
    {
        public const float DEFAULT_HARSHNESS = 2;

        public TextFormatting this[int index] => _textFormatting[index];
        public Renderer UIRenderer { get; private set; }

        public string Text { get; private set; }
        public int Size { get; private set; } = 50;
        public TextAlignment TextAlignment { get; private set; }
        public TextAnchor TextAnchor { get; private set; }
        public FontStyle FontStyle { get; private set; }
        public float LetterSpacing { get; private set; } = 1f;
        public float WordSpacing { get; private set; } = 1f;
        public float LinesSpacing { get; private set; } = 1f;

        public event UIEventHandler<UILabel> TextUpdated = delegate { };

        private UIFont _cachedFont;
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;

        private float? _maxWidth = null;
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
                    UpdateMesh();
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
            Text = labelSettings.Text;
            Size = labelSettings.Size;
            FontStyle = labelSettings.FontStyle;
            TextAlignment = labelSettings.TextAlignment;
            LetterSpacing = labelSettings.LetterSpacing;
            WordSpacing = labelSettings.WordSpacing;
            LinesSpacing = labelSettings.LinesSpacing;

            if (labelSettings.TextAnchor == null)
            {
                if (TextAlignment == TextAlignment.Left)
                    TextAnchor = TextAnchor.LowerLeft;
                else if (TextAlignment == TextAlignment.Center)
                    TextAnchor = TextAnchor.LowerCenter;
                else if (TextAlignment == TextAlignment.Right)
                    TextAnchor = TextAnchor.LowerRight;
            }
            else
            {
                TextAnchor = labelSettings.TextAnchor.Value;
            }

            _color = labelSettings.Color.Color32;
            _cachedFont = mUI.GetFont(labelSettings.Font);

            if (_cachedFont == null)
                throw new Exception("Not fount font: " + labelSettings.Font);

            UIRenderer.sharedMaterial = UIStencilMaterials.GetOrCreate(ParentView.StencilId ?? 0)
                .TextMaterials[labelSettings.Font];

            RequestCharactersInFont();
            UpdateMaterial(_cachedFont.Font);
            UpdateMesh(true);
            SetColor(_color);

            base.ApplySettings(settings);
        }

        private void FontRebuilt(Font font)
        {
            if (_cachedFont.Font == font)
            {
                RequestCharactersInFont();
                UpdateMaterial(font);
                UpdateMesh();
            }
        }

        private void RequestCharactersInFont()
        {
            var formattingIndex = -1;
            var text = Text
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
                        size = (int)(currentFormatting.Size.GetValueOrDefault(Size) * _cachedFont.Harshness);

                        if (!_cachedFont.Font.GetCharacterInfo(currentCharacter, out characterInfo,
                            size, currentFormatting.FontStyle.GetValueOrDefault(FontStyle)))
                        {
                            _cachedFont.Font.RequestCharactersInTexture(currentCharacter.ToString(),
                                size, currentFormatting.FontStyle.GetValueOrDefault(FontStyle));
                        }
                    }
                    else
                    {
                        size = (int)(Size * _cachedFont.Harshness);

                        if (!_cachedFont.Font.GetCharacterInfo(currentCharacter,
                            out characterInfo, size, FontStyle))
                        {
                            _cachedFont.Font.RequestCharactersInTexture(currentCharacter.ToString(), size, FontStyle);
                        }
                    }
                }
                else
                {
                    size = (int)(Size * _cachedFont.Harshness);

                    if (!_cachedFont.Font.GetCharacterInfo(currentCharacter,
                        out characterInfo, size, FontStyle))
                    {
                        _cachedFont.Font.RequestCharactersInTexture(currentCharacter.ToString(), size, FontStyle);
                    }
                }
            }
        }

        public UILabel SetFontStyle(FontStyle fontStyle, bool updateMesh = true)
        {
            if (FontStyle != fontStyle)
            {
                FontStyle = fontStyle;
                RequestCharactersInFont();

                if (updateMesh)
                    UpdateMesh();
            }
            return this;
        }

        public UILabel SetLinesSpacing(float spacing, bool updateMesh = true)
        {
            if (Math.Abs(LinesSpacing - spacing) > 0.001f)
            {
                LinesSpacing = spacing;
                if (updateMesh)
                    UpdateMesh();
            }
            return this;
        }

        public UILabel SetWordSpacing(float spacing, bool updateMesh = true)
        {
            if (Math.Abs(WordSpacing - spacing) > 0.001f)
            {
                WordSpacing = spacing;
                if (updateMesh)
                    UpdateMesh();
            }
            return this;
        }

        public UILabel SetLetterSpacing(float spacing, bool updateMesh = true)
        {
            if (Math.Abs(LetterSpacing - spacing) > 0.001f)
            {
                LetterSpacing = spacing;
                if (updateMesh)
                    UpdateMesh();
            }
            return this;
        }

        public UILabel SetFontSize(int size, bool updateMesh = true)
        {
            if (Size != size)
            {
                Size = size;
                RequestCharactersInFont();
                
                if (updateMesh)
                    UpdateMesh();
            }
            return this;
        }

        public UILabel SetTextAnchor(TextAnchor anchor, bool updateMesh = true)
        {
            if (TextAnchor != anchor)
            {
                TextAnchor = anchor;

                if (updateMesh)
                    UpdateMesh();
            }
            return this;
        }

        public UILabel SetTextAlignment(TextAlignment alignment, bool updateMesh = true)
        {
            if (TextAlignment != alignment)
            {
                TextAlignment = alignment;

                if (updateMesh)
                    UpdateMesh();
            }
            return this;
        }

        public UILabel SetText(string text, bool updateMesh = true)
        {
            if (text == null || Text == text)
                return this;

            Text = text;
            RequestCharactersInFont();

            if (updateMesh)
                UpdateMesh();
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
            if (_textFormatting.ContainsKey(index))
                _textFormatting[index] = formatting;
            else
                _textFormatting.Add(index, formatting);

            RequestCharactersInFont();
            UpdateMesh();
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

        public void UpdateMesh(bool ignoreActive = false)
        {
            if (!ignoreActive && !IsActive)
            {
                _needUpdate = true;
                return;
            }

            if (string.IsNullOrEmpty(Text))
                return;

            const int maxSize = 256 / 2;
            const float pixelsPerWorldUnit = 100f;
            
            Size = mMath.Clamp(Size, 1, maxSize);

            var localScale = Scale();
            Scale(1, 1);

            var text = Text
                .Replace("\t", "   ")
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

            //mCore.Log($"ascent={_cachedFont.Font.ascent} dynamic={_cachedFont.Font.dynamic} fontSize={_cachedFont.Font.fontSize} fontNames={_cachedFont.Font.fontNames.Aggregate((s1, s2) => $"{s1},{s2}")}");

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
                        size = (int) (currentFormatting.Size.GetValueOrDefault(Size) * _cachedFont.Harshness);

                        if (!_cachedFont.Font.GetCharacterInfo(currentCharacter, out characterInfo,
                            size, currentFormatting.FontStyle.GetValueOrDefault(FontStyle)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        currentFormatting = null;
                        size = (int) (Size * _cachedFont.Harshness);

                        if (!_cachedFont.Font.GetCharacterInfo(currentCharacter,
                            out characterInfo, size, FontStyle))
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    size = (int) (Size * _cachedFont.Harshness);

                    if (!_cachedFont.Font.GetCharacterInfo(currentCharacter, 
                        out characterInfo, size, FontStyle))
                        continue;
                }

                var minX = characterInfo.minX / pixelsPerWorldUnit / _cachedFont.Harshness;
                var maxX = characterInfo.maxX / pixelsPerWorldUnit / _cachedFont.Harshness;
                var minY = characterInfo.minY / pixelsPerWorldUnit / _cachedFont.Harshness;
                var maxY = characterInfo.maxY / pixelsPerWorldUnit / _cachedFont.Harshness;

                //minX += minX * -1;
                //maxX += minX * -1;

                if (_maxWidth.HasValue && textXOffset + maxX > _maxWidth)
                {
                    i--;
                    forceNewLine = true;
                    goto End;
                }

                //var w = characterInfo.glyphWidth / pixelsPerWorldUnit / _cachedFont.Harshness;
                //var h = characterInfo.glyphHeight / pixelsPerWorldUnit / _cachedFont.Harshness;
                var bearing = characterInfo.bearing / pixelsPerWorldUnit / _cachedFont.Harshness;
                var advance = characterInfo.advance / pixelsPerWorldUnit / _cachedFont.Harshness;

                //textXOffset -= bearing;
                
                verticesList.Add(new Vector3(textXOffset + minX, minY));
                verticesList.Add(new Vector3(textXOffset + minX, maxY));
                verticesList.Add(new Vector3(textXOffset + maxX, maxY));
                verticesList.Add(new Vector3(textXOffset + maxX, minY));

                // TODO подобрать LetterSpacing

                //mCore.Log("{0} minX={1} maxX={2} minY={3} maxY={4} advance={5} bearing={6} size={7} h={8} w={9}", 
                //  currentCharacter, characterInfo.minX, characterInfo.maxX, characterInfo.minY, characterInfo.maxY, 
                //  characterInfo.advance, characterInfo.bearing, characterInfo.size, characterInfo.glyphHeight, characterInfo.glyphWidth);

                if (lineHeight < characterInfo.size / pixelsPerWorldUnit / _cachedFont.Harshness)
                    lineHeight = characterInfo.size / pixelsPerWorldUnit / _cachedFont.Harshness;

                if (currentCharacter == ' ')
                {
                    textXOffset += advance *
                                   (currentFormatting == null
                                       ? WordSpacing
                                       : currentFormatting.WordSpacing.GetValueOrDefault(WordSpacing));
                }
                else
                {
                    textXOffset += advance *
                                   (currentFormatting == null
                                       ? LetterSpacing
                                       : currentFormatting.LetterSpacing.GetValueOrDefault(LetterSpacing));
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
                                            ? LinesSpacing
                                            : currentFormatting.LinesSpacing.GetValueOrDefault(LinesSpacing));
                        textYOffset -= lineHeight;
                    }
                    else
                    {
                        textYOffset -= pureLineHeight;
                    }

                    AlignVertices(
                        vertices: verticesList,
                        alignment: TextAlignment,
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

            var _anchorOffset = new Vector2(0, 0);

            switch (TextAnchor)
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

            _meshFilter.mesh.Clear();
            _meshFilter.mesh.vertices = verticesList.ToArray();
            _meshFilter.mesh.triangles = trianglesList.ToArray();
            _meshFilter.mesh.colors32 = colorsList.ToArray();
            _meshFilter.mesh.normals = normalsList.ToArray();
            _meshFilter.mesh.uv = uvList.ToArray();

            Scale(localScale);
            TextUpdated.Invoke(this);
        }

        public override UIRect GetRect()
        {
            var pos = Pos();
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

        public override float UnscaledHeight()
        {
            return _textHeight;
        }

        public override float UnscaledWidth()
        {
            return _textWidth;
        }

        public override float GetHeight()
        {
            return UnscaledHeight() * GlobalScale().y;
        }

        public override float GetWidth()
        {
            return UnscaledWidth() * GlobalScale().x;
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
                UpdateMesh();
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
