using System;
using System.Collections.Generic;
using System.IO;
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
        public UIColorOldd ColorOldd = UIColorOldd.White;
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

        public override float UnscaledHeight => _textHeight * GlobalScale.y;
        public override float UnscaledWidth => _textWidth * GlobalScale.x;
        public override Vector2 CenterOffset => Vector2.zero;

        private UIFont _cachedFont;
        private MeshRenderer _meshRenderer;
        private MeshFilter _meshFilter;

        private float? _maxWidth = null;
        private Color _color;

        private bool _needUpdate;

        private float _textWidth;
        private float _textHeight;

        private Dictionary<int, TextFormatting> _textFormatting;

        protected override void AfterAwake()
        {
            _textFormatting = new Dictionary<int, TextFormatting>();

            _meshRenderer = GameObject.AddComponent<MeshRenderer>();
            _meshFilter = GameObject.AddComponent<MeshFilter>();

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

            base.AfterAwake();
        }

        public static void FontOnTextureRebuilt(Font font)
        {
            if (!Application.isEditor)
                return;

            var layers = UIStencilMaterials.Layers();
            for (int i = 0; i < layers.Length; i++)
            {
                if (layers[i] == null)
                    continue;

                foreach (var pair in layers[i].TextMaterials.Materials)
                {
                    if (pair.Key == font.name)
                    {
                        pair.Value.SetTexture("_MainTex", font.material.mainTexture);
                        pair.Value.SetTextureOffset("_MainTex", font.material.mainTextureOffset);
                        pair.Value.SetTextureScale("_MainTex", font.material.mainTextureScale);
                    }

                    pair.Value.SetVector("_TextureSampleAdd", new Vector4(1f, 1f, 1f, 0f));
                }
            }
            

            //mCore.Log($"Font rebuilt: {font.name}");
        }

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

            _color = labelSettings.ColorOldd.Color32;
            _cachedFont = mUI.GetFont(labelSettings.Font);

            if (_cachedFont == null)
                throw new Exception("Not fount font: " + labelSettings.Font);

            UIRenderer.sharedMaterial = UIStencilMaterials.GetOrCreate(ParentView.StencilId ?? 0)
                .TextMaterials[labelSettings.Font];

            RequestCharactersInFont();
            UpdateMesh(true);
            SetColor(_color);

            base.ApplySettings(settings);
        }

        private void FontRebuilt(Font font)
        {
            if (_cachedFont.Font == font)
            {
                //UpdateMaterial(font);
                _meshRenderer.sharedMaterial.SetVector("_TextureSampleAdd", new Vector4(1f, 1f, 1f, 0f));
                RequestCharactersInFont();
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

        /*private void UpdateMaterial(Font font)
        {
            _meshRenderer.sharedMaterial.SetTexture("_MainTex", font.material.mainTexture);
            _meshRenderer.sharedMaterial.SetTextureOffset("_MainTex", font.material.mainTextureOffset);
            _meshRenderer.sharedMaterial.SetTextureScale("_MainTex", font.material.mainTextureScale);
        }*/

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

        private struct LineInfo
        {
            public int StartIndex;
            public int EndIndex;
            public float Width;
            public float Height;
            public float PureHeight;
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

            var localScale = Scale;
            Scale = Vector2.one;

            var text = Text
                .Replace("\t", "   ")
                .TrimStart('\n');

            var verticesList = new List<Vector3>(text.Length * 4);
            var normalsList = new List<Vector3>(text.Length * 4);
            var uvList = new List<Vector2>(text.Length * 4);
            var colorsList = new List<Color32>(text.Length * 4);
            var trianglesList = new List<int>(text.Length * 6);

            _textHeight = 0f;
            _textWidth = 0f;

            var currentFormatting = (TextFormatting) null;
            var textXOffset = 0f;

            var formattingIndex = -1;
            var startLineIndex = 0;
            var lines = 1;
            var pureLineHeight = 0f;

            var lastLineHeight = 0f;
            var lastLineWidth = 0f;
            var linesInfo = new List<LineInfo>();
            
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
                var w = characterInfo.glyphWidth / pixelsPerWorldUnit / _cachedFont.Harshness;

                if (_maxWidth.HasValue && lastLineWidth + w > _maxWidth)
                {
                    i--;
                    forceNewLine = true;
                    goto End;
                }

                //var h = characterInfo.glyphHeight / devider;
                var bearing = characterInfo.bearing / pixelsPerWorldUnit / _cachedFont.Harshness;
                var advance = characterInfo.advance / pixelsPerWorldUnit / _cachedFont.Harshness;

                var leftBottom = new Vector3(textXOffset + minX, minY);
                var leftTop = new Vector3(textXOffset + minX, maxY);
                var rightTop = new Vector3(textXOffset + maxX, maxY);
                var rightBottom = new Vector3(textXOffset + maxX, minY);

                verticesList.Add(leftBottom);
                verticesList.Add(leftTop);
                verticesList.Add(rightTop);
                verticesList.Add(rightBottom);

                lastLineWidth = rightBottom.x - verticesList[startLineIndex].x;

                if (lastLineHeight < characterInfo.size / pixelsPerWorldUnit / _cachedFont.Harshness)
                    lastLineHeight = characterInfo.size / pixelsPerWorldUnit / _cachedFont.Harshness;

                if (pureLineHeight < rightTop.y - rightBottom.y)
                    pureLineHeight = rightTop.y - rightBottom.y;

                //mCore.Log("{0} minX={1} maxX={2} minY={3} maxY={4} advance={5} bearing={6} size={7} h={8} w={9}", 
                //  currentCharacter, characterInfo.minX, characterInfo.maxX, characterInfo.minY, characterInfo.maxY, 
                //  characterInfo.advance, characterInfo.bearing, characterInfo.size, characterInfo.glyphHeight, characterInfo.glyphWidth);

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
                    if (lines > 1)
                    {
                        lastLineHeight = lastLineHeight * (currentFormatting == null
                                             ? LinesSpacing
                                             : currentFormatting.LinesSpacing.GetValueOrDefault(LinesSpacing));
                        _textHeight += lastLineHeight;
                    }
                    else
                    {
                        _textHeight += pureLineHeight;
                    }

                    linesInfo.Add(new LineInfo
                    {
                        StartIndex = startLineIndex,
                        EndIndex = verticesList.Count - 1,
                        Width = lastLineWidth,
                        Height = lastLineHeight,
                        PureHeight = pureLineHeight
                    });

                    if (_textWidth < lastLineWidth)
                        _textWidth = lastLineWidth;
                    
                    textXOffset = 0f;
                    lastLineWidth = 0f;
                    lastLineHeight = 0f;
                    startLineIndex = verticesList.Count;
                    pureLineHeight = 0f;
                    lines++;
                }
            }

            var _anchorOffset = new Vector2(0, 0);

            switch (TextAnchor)
            {
                case TextAnchor.UpperLeft:
                    _anchorOffset = new Vector2(0f, 0f);
                    break;
                case TextAnchor.UpperCenter:
                    _anchorOffset = new Vector2(-_textWidth / 2f, 0f);
                    break;
                case TextAnchor.UpperRight:
                    _anchorOffset = new Vector2(-_textWidth, 0f);
                    break;

                case TextAnchor.MiddleLeft:
                    _anchorOffset = new Vector2(0f, _textHeight / 2f);
                    break;
                case TextAnchor.MiddleCenter:
                    _anchorOffset = new Vector2(-_textWidth / 2f, _textHeight / 2f);
                    break;
                case TextAnchor.MiddleRight:
                    _anchorOffset = new Vector2(-_textWidth, _textHeight / 2f);
                    break;

                case TextAnchor.LowerLeft:
                    _anchorOffset = new Vector2(0f, _textHeight);
                    break;
                case TextAnchor.LowerCenter:
                    _anchorOffset = new Vector2(-_textWidth / 2f, _textHeight);
                    break;
                case TextAnchor.LowerRight:
                    _anchorOffset = new Vector2(-_textWidth, _textHeight);
                    break;
            }       

            var yOffset = 0f;

            for (var lineIndex = 0; lineIndex < linesInfo.Count; lineIndex++)
            {
                var xOffset = -verticesList[linesInfo[lineIndex].StartIndex].x;
                yOffset -= lineIndex == 0 
                    ? linesInfo[lineIndex].PureHeight 
                    : linesInfo[lineIndex].Height;

                switch (TextAlignment)
                {
                    case TextAlignment.Center:
                        xOffset += _textWidth / 2 - linesInfo[lineIndex].Width / 2f;
                        break;

                    case TextAlignment.Right:
                        xOffset += _textWidth - linesInfo[lineIndex].Width;
                        break;

                    case TextAlignment.Left:
                        break;
                }
                
                for (var vI = linesInfo[lineIndex].StartIndex; vI <= linesInfo[lineIndex].EndIndex; vI++)
                {
                    verticesList[vI] = new Vector3(
                        verticesList[vI].x + xOffset + _anchorOffset.x,
                        verticesList[vI].y + yOffset + _anchorOffset.y,
                        verticesList[vI].z
                    );
                }
            }

            _meshFilter.mesh.Clear();
            _meshFilter.mesh.vertices = verticesList.ToArray();
            _meshFilter.mesh.triangles = trianglesList.ToArray();
            _meshFilter.mesh.colors32 = colorsList.ToArray();
            _meshFilter.mesh.normals = normalsList.ToArray();
            _meshFilter.mesh.uv = uvList.ToArray();

            Scale = localScale;
            TextUpdated.Invoke(this);
        }

        /*public override UIRect GetRect()
        {
            var pos = Pos();
            var scale = GlobalScale();

            var hDiv2 = _textHeight / 2f;
            var wDiv2 = _textWidth / 2f;
            var xOffset = 0f;
            var yOffset = 0f;

            switch (TextAnchor)
            {
                case TextAnchor.UpperLeft:
                    xOffset += wDiv2;
                    yOffset -= hDiv2;
                    break;
                case TextAnchor.UpperCenter:
                    yOffset -= hDiv2;
                    break;
                case TextAnchor.UpperRight:
                    xOffset -= wDiv2;
                    yOffset -= hDiv2;
                    break;

                case TextAnchor.MiddleLeft:
                    xOffset += wDiv2;
                    break;
                case TextAnchor.MiddleCenter:
                    break;
                case TextAnchor.MiddleRight:
                    xOffset -= wDiv2;
                    break;

                case TextAnchor.LowerLeft:
                    xOffset += wDiv2;
                    yOffset += hDiv2;
                    break;
                case TextAnchor.LowerCenter:
                    yOffset += hDiv2;
                    break;
                case TextAnchor.LowerRight:
                    xOffset -= wDiv2;
                    yOffset += hDiv2;
                    break;
            }

            return new UIRect
            {
                Position = pos,
                Bottom = pos.y + yOffset * scale.y - hDiv2 * scale.y,
                Top = pos.y + yOffset * scale.y + hDiv2 * scale.y,
                Left = pos.x + xOffset * scale.x - wDiv2 * scale.x,
                Right = pos.x + xOffset * scale.x + wDiv2 * scale.x,
            };
        }*/

        public Color GetColor()
        {
            return _color;
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
