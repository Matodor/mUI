using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace mFramework.UI
{
    public class UILabelProps : UIComponentProps
    {
        public string Text = string.Empty;
        public string Font = "Arial";
        public Color Color = Color.white;

        public TextStyle TextStyle = new TextStyle
        {
            FontStyle = FontStyle.Normal,
            LetterSpacing = 1f,
            LinesSpacing = 1f,
            WordSpacing = 1f,
            Size = 40,
            TextAlignment = TextAlignment.Left,
            MaxWidth = null
        };
    }

    /*
    public enum VerticalAlign
    {
        BASELINE = 0,
        ASCENDERLINE = 1,
        DESCENDERLINE = 2
    }
    */

    public class TextFormatting
    {
        public int? Size = null;
        public Color? Color = null; 
        public float? LetterSpacing = null;
        public float? WordSpacing = null;
        public FontStyle? FontStyle = null;
    }

    public struct TextStyle
    {
        public int Size;
        public TextAlignment TextAlignment;
        public FontStyle FontStyle;
        public float LetterSpacing;
        public float WordSpacing;
        public float LinesSpacing;
        public float? MaxWidth;
    }

    public class UILabel : UIComponent, IUIColored, IUIRenderer<MeshRenderer>, IUIRenderer
    {
        public const float DEFAULT_HARSHNESS = 2;

        public Color Color
        {
            get => _textColor;
            set => SetColor(value);
        }

        public float Opacity
        {
            get => _textColor.a;
            set
            {
                var color = _textColor;
                color.a = value;
                SetColor(color);
            }
        }

        public TextStyle TextStyle
        {
            get => _textStyle;
            set
            {
                _needUpdate = true;
                _textStyle = value;
                RequestCharactersInFont();
            }
        }

        public MeshRenderer UIRenderer { get; private set; }
        Renderer IUIRenderer.UIRenderer => UIRenderer;

        public TextFormatting this[int index]
        {
            get => _textFormatting[index];
            set
            {
                if (_textFormatting.ContainsKey(index))
                    _textFormatting[index] = value;
                else
                    _textFormatting.Add(index, value);

                _needUpdate = true;
                RequestCharactersInFont();
            }
        }

        public event UIEventHandler<UILabel> TextUpdated = delegate { };

        /// <summary>
        /// Text with formatting
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                _needUpdate = true;
                _text = value;
                RequestCharactersInFont();
            }
        }

        private string _text;
        private bool _needUpdate;
        private float? _maxWidth = null;
        private TextStyle _textStyle;
        private Color _textColor;
        private UIFont _cachedFont;
        private MeshFilter _meshFilter; private Dictionary<int, TextFormatting> _textFormatting;

        protected override void OnBeforeDestroy()
        {
            Font.textureRebuilt -= FontRebuilt;
            SortingOrderChanged -= OnSortingOrderChanged;
            TextUpdated = null;
            base.OnBeforeDestroy();
        }

        protected override void AfterAwake()
        {
            UIRenderer = GameObject.AddComponent<MeshRenderer>();
            _meshFilter = GameObject.AddComponent<MeshFilter>();
            _textFormatting = new Dictionary<int, TextFormatting>();

            Font.textureRebuilt += FontRebuilt;
            SortingOrderChanged += OnSortingOrderChanged;

            base.AfterAwake();
        }

        private void OnSortingOrderChanged(IUIObject sender)
        {
            UIRenderer.sortingOrder = SortingOrder;
        }

        private void SetColor(Color color)
        {
            _textColor = color;

            if (_textFormatting.Count > 0)
            {
                _needUpdate = true;
            }
            else
            {
                var colors = new Color[_meshFilter.mesh.colors.Length];
                for (var i = 0; i < colors.Length; i++)
                    colors[i] = color;
                _meshFilter.mesh.colors = colors;
            }
        }

        public static void FontOnTextureRebuilt(Font font)
        {
            if (!Application.isEditor)
                return;

            var layers = UIStencilMaterials.Layers();
            for (var i = 0; i < layers.Length; i++)
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

        protected override void ApplyProps(UIComponentProps props)
        {
            if (!(props is UILabelProps labelSettings))
                throw new ArgumentException("UILabel: The given settings is not UILabelSettings");

            _textColor = labelSettings.Color;
            _text = labelSettings.Text;
            _textStyle = labelSettings.TextStyle;
            _cachedFont = mUI.GetFont(labelSettings.Font);

            if (_cachedFont == null)
                throw new Exception("Not fount font: " + labelSettings.Font);

            UIRenderer.sharedMaterial = UIStencilMaterials.GetOrCreate(ParentView.StencilId ?? 0)
                .TextMaterials[labelSettings.Font];

            _needUpdate = true;
            RequestCharactersInFont();
            UpdateMesh();
            base.ApplyProps(props);
        }

        protected override void OnTick()
        {
            if (IsShowing && _needUpdate)
                UpdateMesh();
            base.OnTick();
        }

        private void FontRebuilt(Font font)
        {
            if (_cachedFont.Font == font)
            {
                //UpdateMaterial(font);
                UIRenderer.sharedMaterial.SetVector("_TextureSampleAdd", new Vector4(1f, 1f, 1f, 0f));
                RequestCharactersInFont();
                _needUpdate = true;
            }
        }

        private void RequestCharactersInFont()
        {
            var formattingIndex = -1;
            var text = Text
                .Replace('\t', ' ')
                .TrimStart('\n');

            var style = _textStyle;
            style.Size = mMath.Clamp(style.Size, 1, MAX_SIZE);

            for (var i = 0; i < text.Length; i++)
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
                        size = (int) (currentFormatting.Size.GetValueOrDefault(style.Size) 
                            * _cachedFont.Harshness);

                        if (!_cachedFont.Font.GetCharacterInfo(currentCharacter, out characterInfo,
                            size, currentFormatting.FontStyle.GetValueOrDefault(style.FontStyle)))
                        {
                            _cachedFont.Font.RequestCharactersInTexture(currentCharacter.ToString(),
                                size, currentFormatting.FontStyle.GetValueOrDefault(style.FontStyle));
                        }
                    }
                    else
                    {
                        size = (int) (style.Size * _cachedFont.Harshness);

                        if (!_cachedFont.Font.GetCharacterInfo(currentCharacter,
                            out characterInfo, size, style.FontStyle))
                        {
                            _cachedFont.Font.RequestCharactersInTexture(currentCharacter.ToString(), 
                                size, style.FontStyle);
                        }
                    }
                }
                else
                {
                    size = (int)(style.Size * _cachedFont.Harshness);

                    if (!_cachedFont.Font.GetCharacterInfo(currentCharacter,
                        out characterInfo, size, style.FontStyle))
                    {
                        _cachedFont.Font.RequestCharactersInTexture(currentCharacter.ToString(), 
                            size, style.FontStyle);
                    }
                }
            }
        }

        /*private void UpdateMaterial(Font font)
        {
            _meshRenderer.sharedMaterial.SetTexture("_MainTex", font.material.mainTexture);
            _meshRenderer.sharedMaterial.SetTextureOffset("_MainTex", font.material.mainTextureOffset);
            _meshRenderer.sharedMaterial.SetTextureScale("_MainTex", font.material.mainTextureScale);
        }*/

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

            public float Height;
            public float MaxY;
            public float MinY;
        }

        private const int MAX_SIZE = 256 / 2;

        private void UpdateMesh()
        {
            if (!IsActive)
                return;

            //if (string.IsNullOrEmpty(Text))
            //    return;

            const float pixelsPerWorldUnit = 100f;

            var pos = Position;
            var style = _textStyle;
            style.Size = mMath.Clamp(style.Size, 1, MAX_SIZE);

            //var localScale = Scale;
            //Scale = Vector2.one;

            var text = Text
                .Replace("\t", "   ")
                .TrimStart('\n');

            var verticesList = new List<Vector3>(text.Length * 4);
            var normalsList = new List<Vector3>(text.Length * 4);
            var uvList = new List<Vector2>(text.Length * 4);
            var colorsList = new List<Color32>(text.Length * 4);
            var trianglesList = new List<int>(text.Length * 6);

            UnscaledHeight = 0f;
            UnscaledWidth = 0f;

            var currentFormatting = (TextFormatting) null;
            var textXOffset = 0f;

            var formattingIndex = -1;
            var startLineIndex = 0;

            var lastLineHeight = 0f;
            var lastLineWidth = 0f;
            var linesInfo = new List<LineInfo>();

            var lineMaxY = 0f;
            var lineMinY = 0f;

            //mCore.Log($"ascent={_cachedFont.Font.ascent} dynamic={_cachedFont.Font.dynamic} fontSize={_cachedFont.Font.fontSize} fontNames={_cachedFont.Font.fontNames.Aggregate((s1, s2) => $"{s1},{s2}")}");

            for (var i = 0; i < text.Length; i++)
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
                        size = (int) (currentFormatting.Size.GetValueOrDefault(style.Size) * _cachedFont.Harshness);

                        if (!_cachedFont.Font.GetCharacterInfo(currentCharacter, out characterInfo,
                            size, currentFormatting.FontStyle.GetValueOrDefault(style.FontStyle)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        currentFormatting = null;
                        size = (int) (style.Size * _cachedFont.Harshness);

                        if (!_cachedFont.Font.GetCharacterInfo(currentCharacter,
                            out characterInfo, size, style.FontStyle))
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    size = (int) (style.Size * _cachedFont.Harshness);

                    if (!_cachedFont.Font.GetCharacterInfo(currentCharacter, 
                        out characterInfo, size, style.FontStyle))
                        continue;
                }

                var minX = characterInfo.minX / pixelsPerWorldUnit / _cachedFont.Harshness;
                var maxX = characterInfo.maxX / pixelsPerWorldUnit / _cachedFont.Harshness;
                var minY = characterInfo.minY / pixelsPerWorldUnit / _cachedFont.Harshness;
                var maxY = characterInfo.maxY / pixelsPerWorldUnit / _cachedFont.Harshness;
                var w = characterInfo.glyphWidth / pixelsPerWorldUnit / _cachedFont.Harshness;

                if (lineMaxY < maxY)
                    lineMaxY = maxY;

                if (lineMinY > minY)
                    lineMinY = minY;

                //Debug.Log($"minX={minX} minY={minY}");

                if (_maxWidth.HasValue && lastLineWidth + w > _maxWidth)
                {
                    i--;
                    forceNewLine = true;
                    goto End;
                }

                //var h = characterInfo.glyphHeight / devider;
                //var bearing = characterInfo.bearing / pixelsPerWorldUnit / _cachedFont.Harshness;
                var advance = characterInfo.advance / pixelsPerWorldUnit / _cachedFont.Harshness;

                var leftBottom = new Vector3(textXOffset, minY);
                var leftTop = new Vector3(textXOffset, maxY);
                var rightTop = new Vector3(textXOffset + (maxX - minX), maxY);
                var rightBottom = new Vector3(textXOffset + (maxX - minX), minY);

                verticesList.Add(leftBottom);
                verticesList.Add(leftTop);
                verticesList.Add(rightTop);
                verticesList.Add(rightBottom);

                lastLineWidth = rightBottom.x - verticesList[startLineIndex].x;

                if (lastLineHeight < characterInfo.size / pixelsPerWorldUnit / _cachedFont.Harshness)
                    lastLineHeight = characterInfo.size / pixelsPerWorldUnit / _cachedFont.Harshness;

                //mCore.Log("{0} minX={1} maxX={2} minY={3} maxY={4} advance={5} bearing={6} size={7} h={8} w={9}", 
                //  currentCharacter, characterInfo.minX, characterInfo.maxX, characterInfo.minY, characterInfo.maxY, 
                //  characterInfo.advance, characterInfo.bearing, characterInfo.size, characterInfo.glyphHeight, characterInfo.glyphWidth);

                if (currentCharacter == ' ')
                {
                    textXOffset += advance *
                                   (currentFormatting == null
                                       ? style.WordSpacing
                                       : currentFormatting.WordSpacing.GetValueOrDefault(style.WordSpacing));
                }
                else
                {
                    textXOffset += advance *
                                   (currentFormatting == null
                                       ? style.LetterSpacing
                                       : currentFormatting.LetterSpacing.GetValueOrDefault(style.LetterSpacing));
                }

                var color = currentFormatting == null 
                    ? _textColor
                    : currentFormatting.Color.GetValueOrDefault(_textColor);

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
                    lastLineHeight = lastLineHeight * style.LinesSpacing;

                    if (linesInfo.Count == 0)
                    {
                        UnscaledHeight = lineMaxY - lineMinY;
                    }
                    else
                    {
                        UnscaledHeight += lastLineHeight;
                    }

                    linesInfo.Add(new LineInfo
                    {
                        StartIndex = startLineIndex,
                        EndIndex = verticesList.Count - 1,
                        Height = lastLineHeight,
                        MaxY = lineMaxY,
                        MinY = lineMinY
                    });

                    if (UnscaledWidth < lastLineWidth)
                        UnscaledWidth = lastLineWidth;
                    
                    textXOffset = 0f;
                    lastLineWidth = 0f;
                    lastLineHeight = 0f;
                    startLineIndex = verticesList.Count;

                    lineMaxY = 0f;
                    lineMinY = 0f;
                }
            }

            if (linesInfo.Count > 1)
                UnscaledHeight += linesInfo[0].MinY - linesInfo[linesInfo.Count - 1].MinY;

            var yOffset = 0f;
            for (var lineIndex = 0; lineIndex < linesInfo.Count; lineIndex++)
            {
                var lineWidth =
                    verticesList[linesInfo[lineIndex].EndIndex].x -
                    verticesList[linesInfo[lineIndex].StartIndex].x;

                if (lineIndex == 0)
                    yOffset = -linesInfo[lineIndex].MaxY;
                else
                    yOffset -= linesInfo[lineIndex].Height;

                var xOffset = -UnscaledWidth / 2f;
                switch (style.TextAlignment)
                {
                    case TextAlignment.Center:
                        xOffset = -lineWidth / 2f;
                        break;

                    case TextAlignment.Right:
                        xOffset = -lineWidth + UnscaledWidth / 2f;
                        break;
                }
                
                for (var vI = linesInfo[lineIndex].StartIndex; vI <= linesInfo[lineIndex].EndIndex; vI++)
                {
                    verticesList[vI] = new Vector3(
                        verticesList[vI].x + xOffset,
                        verticesList[vI].y + yOffset + UnscaledHeight / 2,
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
            _needUpdate = false;

            Position = pos;
            TextUpdated(this);
        }
    }
}
