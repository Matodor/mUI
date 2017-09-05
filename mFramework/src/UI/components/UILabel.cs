using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mFramework.UI
{
    public class UILabelSettings : UIComponentSettings
    {
        public string Text;
        public string Font = "Arial";
        public int Size;
        public VerticalAlign VerticalAlign = VerticalAlign.BASELINE;
        public TextAnchor TextAnchor = TextAnchor.LowerLeft;
        public UIColor Color = UIColors.White;
        public FontStyle FontStyle = FontStyle.Normal;
        public TextAlignment TextAlignment = TextAlignment.Left;
        public float LetterSpacing = 1;
        public float WordSpacing = 1;
        public float Harshness = 4f;
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
        public float? Harshness = null;
    }

    public class UILabel : UIComponent, IUIRenderer, IColored
    {
        public Renderer UIRenderer { get; }
        public string Text => _cachedText;
        public event UIEventHandler<UILabel> TextUpdated;
        
        private Font _cachedFont;
        private readonly MeshRenderer _meshRenderer;
        private readonly MeshFilter _meshFilter;
        //private readonly MaterialPropertyBlock _textPropertyBlock;

        private string _fontName;
        private string _cachedText;
        private int _fontSize = 50;
        private float _letterSpacing = 1;
        private float _wordSpacing = 1;
        private float _harshness = 4f;

        private TextAlignment _textAlignment;
        private TextAnchor _textAnchor;
        private VerticalAlign _verticalAlign;
        private FontStyle _fontStyle;
        private Color _color;
        private bool _needUpdate;

        private float _textWidth;
        private float _textHeight;

        private readonly Dictionary<int, TextFormatting> _textFormatting;

        protected UILabel(UIObject parent) : base(parent)
        {
            _textFormatting = new Dictionary<int, TextFormatting>();
            _meshRenderer = _gameObject.AddComponent<MeshRenderer>();
            _meshFilter = _gameObject.AddComponent<MeshFilter>();
            _meshFilter.mesh = new Mesh();
            _meshFilter.mesh.Clear();

            UIRenderer = _meshRenderer;

            SortingOrderChanged += s =>
            {
                _meshRenderer.sortingOrder = SortingOrder();
            };

            ActiveChanged += s =>
            {
                if (IsActive && _needUpdate)
                {
                    _needUpdate = false;
                    UpdateMeshText();
                }
            };
        }

        public UILabel UpdateSettings(UILabelSettings settings)
        {
            SetFontStyle(settings.FontStyle, false);
            SetVerticalAlign(settings.VerticalAlign, false);
            SetLetterSpacing(settings.LetterSpacing, false);
            SetFontSize(settings.Size, false);
            SetTextAlignment(settings.TextAlignment, false);
            SetText(settings.Text, false);
            SetFont(settings.Font, false);

            UpdateMeshText();
            return this;
        }

        public UILabel SetFontStyle(FontStyle fontStyle, bool updateMesh = true)
        {
            if (_fontStyle != fontStyle)
            {
                _fontStyle = fontStyle;
                if (updateMesh)
                    UpdateMeshText();
            }
            return this;
        }

        public UILabel SetVerticalAlign(VerticalAlign align, bool updateMesh = true)
        {
            if (_verticalAlign != align)
            {
                _verticalAlign = align;
                if (updateMesh)
                    UpdateMeshText();
            }
            return this;
        }

        public UILabel SetLetterSpacing(float spacing, bool updateMesh = true)
        {
            if (Math.Abs(_letterSpacing - spacing) > 0.001f)
            {
                _letterSpacing = spacing;
                if (updateMesh)
                    UpdateMeshText();
            }
            return this;
        }

        public UILabel SetFontSize(int size, bool updateMesh = true)
        {
            if (_fontSize != size)
            {
                _fontSize = size;
                if (updateMesh)
                    UpdateMeshText();
            }
            return this;
        }

        public UILabel SetTextAlignment(TextAlignment alignment, bool updateMesh = true)
        {
            if (_textAlignment != alignment)
            {
                _textAlignment = alignment;
                if (updateMesh)
                    UpdateMeshText();
            }
            return this;
        }

        public UILabel SetText(string text, bool updateMesh = true)
        {
            if (string.IsNullOrEmpty(text) || _cachedText == text)
                return this;

            _cachedText = text;
            if (updateMesh)
                UpdateMeshText();
            return this;
        }

        public UILabel SetFont(string fontName, bool updateMesh = true)
        {
            if (_fontName != fontName)
            {
                _fontName = fontName;
                _cachedFont = mUI.GetFont(_fontName);

                if (_cachedFont == null)
                    throw new Exception("Not fount font: " + _fontName);
              
                if (updateMesh)
                    UpdateMeshText();
            }
            return this;
        }

        private void FontRebuilt(Font font)
        {
            if (font == _cachedFont)
            {
                UpdateMeshText();
            }
        }

        protected override void ApplySettings(UIComponentSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            var labelSettings = settings as UILabelSettings;
            if (labelSettings == null)
                throw new ArgumentException("UILabel: The given settings is not UILabelSettings");

            Font.textureRebuilt += FontRebuilt;
            BeforeDestroy += s => Font.textureRebuilt -= FontRebuilt;

            _cachedText = labelSettings.Text;
            _fontSize = labelSettings.Size;
            _verticalAlign = labelSettings.VerticalAlign;
            _textAnchor = labelSettings.TextAnchor;
            _color = labelSettings.Color.Color32;
            _fontStyle = labelSettings.FontStyle;
            _textAlignment = labelSettings.TextAlignment;
            _letterSpacing = labelSettings.LetterSpacing;
            _wordSpacing = labelSettings.WordSpacing;
            _harshness = labelSettings.Harshness;

            base.ApplySettings(settings);
             
            SetFont(labelSettings.Font, false);
            UpdateMeshText();
            SetColor(_color);
        }

        public void TextFormatting(int index, TextFormatting formatting)
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
            if (formatting.Harshness == null)
                formatting.Harshness = _harshness;

            if (_textFormatting.ContainsKey(index))
                _textFormatting[index] = formatting;
            else
                _textFormatting.Add(index, formatting);

            UpdateMeshText();
        }

        private static void AlignVertices(List<Vector3> vertices, TextAlignment alignment, 
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
                    }
                    break;
                case TextAlignment.Left:
                    if (yOffset != 0.0f)
                    {
                        for (int i = startIndex; i <= endIndex; i++)
                        {
                            vertices[i] = new Vector3(
                                vertices[i].x,
                                vertices[i].y + yOffset,
                                vertices[i].z
                            );
                        }
                    }
                    break;
                
            }
        }

        private static int ParseFormatting(int i, string text, ref int formattingIndex)
        {
            // [=0]
            if (i + 3 < text.Length &&
                text[i] == '[' &&
                text[i + 1] == '=' &&
                text[i + 3] == ']' &&
                char.IsNumber(text[i + 2]))
            {
                int index;
                if (int.TryParse(text.Substring(i + 2, 1), out index))
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
                int index;
                if (int.TryParse(text.Substring(i + 2, 2), out index))
                    formattingIndex = index;
                return 5;
            }

            // [=/]
            if (i + 3 < text.Length &&
                text[i] == '[' &&
                text[i + 1] == '=' &&
                text[i + 2] == '/' &&
                text[i + 3] == ']')
            {
                formattingIndex = -1;
                return 4;
            }

            return 0;
        }

        internal void UpdateMeshText()
        {
            if (!IsActive)
            {
                _needUpdate = true;
                return;
            }

            if (string.IsNullOrEmpty(_cachedText))
                return;

            const float pixelsPerWorldUnit = 100f;
            var defaultLineHeightScale = _cachedFont.lineHeight / (float) _cachedFont.fontSize;
            
            _cachedFont.RequestCharactersInTexture(_cachedText, (int) (_fontSize * _harshness), _fontStyle);

            foreach (var textFormatting in _textFormatting)
            {
                _cachedFont.RequestCharactersInTexture(_cachedText,
                    (int) (textFormatting.Value.Size.GetValueOrDefault(_fontSize) *
                           textFormatting.Value.Harshness.GetValueOrDefault(_harshness)),
                    textFormatting.Value.FontStyle.GetValueOrDefault(_fontStyle));
            }

            var localScale = LocalScale();
            Scale(1, 1);

            var text = _cachedText.Replace('\t', ' ');
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
            var prevLineHeight = 0f;

            var formattingIndex = -1;
            var startLineIndex = 0;

            var topY = 0f;
            var bottomY = 0f;

            var lines = 1;

            for (int i = 0; i < text.Length; i++)
            {
                CharacterInfo characterInfo;
                var currentCharacter = text[i];

                if (_textFormatting.Count > 0)
                {
                    var skip = ParseFormatting(i, text, ref formattingIndex);
                    if (skip != 0)
                    {
                        i += skip;
                        if (i >= text.Length)
                            goto End;

                        i--;
                        continue;
                    }

                    if (formattingIndex != -1 && _textFormatting.ContainsKey(formattingIndex))
                    {
                        currentFormatting = _textFormatting[formattingIndex];
                        if (!_cachedFont.GetCharacterInfo(currentCharacter, out characterInfo,
                            (int) (currentFormatting.Size.GetValueOrDefault(_fontSize) *
                                   currentFormatting.Harshness.GetValueOrDefault(_harshness)),
                            currentFormatting.FontStyle.GetValueOrDefault(_fontStyle)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!_cachedFont.GetCharacterInfo(currentCharacter, out characterInfo,
                            (int) (_fontSize * _harshness), _fontStyle))
                            continue;
                    }
                }
                else
                {
                    if (!_cachedFont.GetCharacterInfo(currentCharacter, out characterInfo,
                        (int)(_fontSize * _harshness), _fontStyle))
                        continue;
                }

                var harshness = currentFormatting == null
                    ? _harshness
                    : currentFormatting.Harshness.GetValueOrDefault(_harshness);

                var offset = new Vector3(textXOffset, textYOffset);
                var minX = characterInfo.minX / harshness / pixelsPerWorldUnit;
                var maxX = characterInfo.maxX / harshness / pixelsPerWorldUnit;
                var minY = characterInfo.minY / harshness / pixelsPerWorldUnit;
                var maxY = characterInfo.maxY / harshness / pixelsPerWorldUnit;

                if (lineHeight < (characterInfo.size / harshness / pixelsPerWorldUnit) * defaultLineHeightScale)
                    lineHeight = (characterInfo.size / harshness / pixelsPerWorldUnit) * defaultLineHeightScale;

                if (currentCharacter == ' ')
                {
                    textXOffset += (characterInfo.advance / harshness / pixelsPerWorldUnit) *
                                   (currentFormatting == null
                                       ? _wordSpacing
                                       : currentFormatting.WordSpacing.GetValueOrDefault(_wordSpacing));
                }
                else
                {
                    textXOffset += (characterInfo.advance / harshness / pixelsPerWorldUnit) *
                                   (currentFormatting == null
                                       ? _letterSpacing
                                       : currentFormatting.LetterSpacing.GetValueOrDefault(_letterSpacing));
                }

                verticesList.Add(new Vector3(minX, minY) + offset);
                verticesList.Add(new Vector3(minX, maxY) + offset);
                verticesList.Add(new Vector3(maxX, maxY) + offset);
                verticesList.Add(new Vector3(maxX, minY) + offset);

                if (topY < verticesList[verticesList.Count - 2].y)
                    topY = verticesList[verticesList.Count - 2].y;
                if (bottomY > verticesList[verticesList.Count - 1].y)
                    bottomY = verticesList[verticesList.Count - 1].y;

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
                if (i + 1 >= text.Length || i + 1 < text.Length && text[i + 1] == '\n')
                {
                    var lineWidth = verticesList[verticesList.Count - 1].x - verticesList[startLineIndex].x;

                    if (_textWidth < lineWidth)
                        _textWidth = lineWidth;

                    AlignVertices(verticesList, _textAlignment, startLineIndex,
                        verticesList.Count - 1, lineWidth, lines > 1 ? 0 : 0);

                    if (i + 1 < text.Length && text[i + 1] == '\n')
                    {
                        prevLineHeight = lineHeight;
                        textXOffset = 0f;
                        textYOffset -= lineHeight;
                        lineHeight = 0f;
                        lines++;

                        if (i + 2 < text.Length)
                            startLineIndex = verticesList.Count;
                    }
                }
            }

            _textHeight = topY - bottomY;

            _meshFilter.mesh.Clear();
            _meshFilter.mesh.vertices = verticesList.ToArray();
            _meshFilter.mesh.triangles = trianglesList.ToArray();
            _meshFilter.mesh.colors32 = colorsList.ToArray();
            _meshFilter.mesh.normals = normalsList.ToArray();
            _meshFilter.mesh.uv = uvList.ToArray();

            if (_meshRenderer.sharedMaterial != _cachedFont.material)
                _meshRenderer.sharedMaterial = _cachedFont.material;

            Scale(localScale);
            TextUpdated?.Invoke(this);
        }

        public override float GetHeight()
        {
            return _textHeight * GlobalScale().y;
        }

        public override float GetWidth()
        {
            return _textWidth * GlobalScale().x;
        }

        public UIObject SetColor(Color32 color)
        {
            _color = color;

            var colors = new Color[_meshFilter.mesh.colors.Length];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = color;
            _meshFilter.mesh.colors = colors;

            if (_meshRenderer.sharedMaterial != _cachedFont.material)
                _meshRenderer.sharedMaterial = _cachedFont.material;

            return this;
        }

        public UIObject SetColor(UIColor color)
        {
            SetColor(color.Color32);
            return this;
        }
    }
}
