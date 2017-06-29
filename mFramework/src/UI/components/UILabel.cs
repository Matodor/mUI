using System;
using System.Linq;
using UnityEngine;

namespace mFramework.UI
{
    public class UILabelSettings : UIComponentSettings
    {
        public string Text { get; set; }
        public string Font { get; set; } = "Arial";
        public int Size { get; set; }
        public UILabel.VerticalAlign VerticalAlign { get; set; } = UILabel.VerticalAlign.BASELINE;
        public TextAnchor TextAnchor { get; set; } = TextAnchor.LowerLeft;
        public UIColor Color { get; set; } = UIColors.White;
        public FontStyle FontStyle { get; set; } = FontStyle.Normal;
        public TextAlignment TextAlignment { get; set; } = TextAlignment.Left;
    }
    
    public class UILabel : UIComponent, IUIRenderer, IColored
    {
        public enum VerticalAlign
        {
            BASELINE = 0,
            ASCENDERLINE = 1,
            DESCENDERLINE = 2
        }

        public Renderer UIRenderer { get; }
        public int FontSize { get { return _fontSize; } }
        public int LetterSpacing { get { return _letterSpacing; } }
        public string Text { get { return _cachedText; } }
        public Color TextColor { get { return _color; } }

        private Font _cachedFont;
        private readonly MeshRenderer _meshRenderer;
        private readonly MeshFilter _meshFilter;
        private readonly MaterialPropertyBlock _textPropertyBlock;

        private bool _forceUpdate;
        private string _fontName;
        private string _cachedText;
        private int _fontSize = 50;
        private int _letterSpacing = 1;

        private TextAlignment _textAlignment;
        private TextAnchor _textAnchor;
        private VerticalAlign _verticalAlign;
        private FontStyle _fontStyle;
        private Color _color;

        protected UILabel(UIObject parent) : base(parent)
        {
            _forceUpdate = false;
            _meshRenderer = _gameObject.AddComponent<MeshRenderer>();
            _meshFilter = _gameObject.AddComponent<MeshFilter>();
            _textPropertyBlock = new MaterialPropertyBlock();
            UIRenderer = _meshRenderer;

            OnSortingOrderChanged += o =>
            {
                mCore.Log("OnSortingOrderChanged: {0} | Order: {1} | {2}", ToString(), SortingOrder(), _sortingOrder);
                _meshRenderer.sortingOrder = SortingOrder();
            };
        }

        public UILabel SetFontStyle(FontStyle fontStyle)
        {
            if (_fontStyle != fontStyle)
            {
                _fontStyle = fontStyle;
                ForceUpdate();
            }
            return this;
        }

        public UILabel SetVerticalAlign(VerticalAlign align)
        {
            if (_verticalAlign != align)
            {
                _verticalAlign = align;
                ForceUpdate();
            }
            return this;
        }

        public UILabel SetTetterSpacing(int spacing)
        {
            if (_letterSpacing != spacing)
            {
                _letterSpacing = spacing;
                ForceUpdate();
            }
            return this;
        }

        public UILabel SetFontSize(int size)
        {
            if (_fontSize != size)
            {
                _fontSize = size;
                ForceUpdate();
            }
            return this;
        }

        public UILabel SetTextAlignment(TextAlignment alignment)
        {
            if (_textAlignment != alignment)
            {
                _textAlignment = alignment;
                ForceUpdate();
            }
            return this;
        }

        public UILabel SetText(string text)
        {
            if (string.IsNullOrEmpty(text) || _cachedText == text)
                return this;

            _cachedText = text;
            ForceUpdate();
            return this;
        }

        public UILabel SetFont(string fontName)
        {
            if (_fontName != fontName)
            {
                _fontName = fontName;
                _cachedFont = mUI.GetFont(_fontName);

                if (_cachedFont == null)
                    throw new Exception("Not fount font: " + _fontName);

                ForceUpdate();
            }
            return this;
        }

        private void FontRebuilt(Font font)
        {
            if (font == _cachedFont)
            {
                ForceUpdate();
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
            OnBeforeDestroy += o => Font.textureRebuilt -= FontRebuilt;

            _cachedText = labelSettings.Text;
            _fontSize = labelSettings.Size;
            _verticalAlign = labelSettings.VerticalAlign;
            _textAnchor = labelSettings.TextAnchor;
            _color = labelSettings.Color.Color32;
            _fontStyle = labelSettings.FontStyle;
            _textAlignment = labelSettings.TextAlignment;

            SetFont(labelSettings.Font);
            ForceUpdate();

            //mCore.Log("!!! _cachedFont = name: {0} | lineHeight: {1} | fontSize: {2}", _cachedFont.name, _cachedFont.lineHeight, _cachedFont.fontSize);
            base.ApplySettings(settings);
        }

        internal void ForceUpdate()
        {
            _forceUpdate = true;
        }
        
        internal void UpdateMeshText()
        {
            if (!_forceUpdate || string.IsNullOrEmpty(_cachedText))
                return;

            _forceUpdate = false;
            _cachedFont.RequestCharactersInTexture(_cachedText, _fontSize, _fontStyle);
            var localScale = LocalScale();
            Scale(1, 1);

            var cleanText = _cachedText.Replace("\t", "");
            var textLines = cleanText.Split('\n');
            var charCount = textLines.Sum(l => l.Length);

            var vertices = new Vector3[charCount * 4];
            var normals = new Vector3[charCount * 4];
            var uv = new Vector2[charCount * 4];
            var colors = new Color32[charCount * 4];
            var triangles = new int[charCount * 6];

            var textHeight = 0f;
            var textWidth = 0f;
            var charIndex = 0;
            var trianglesIndex = 0;

            const float magic = 200f;
            var lineHeight = ((_fontSize / _cachedFont.fontSize) * _cachedFont.lineHeight) / magic;

            foreach (var line in textLines)
            {
                var lineWidth = 0f;
                var lastLetterSpacing = 0f;
                var charsInLine = 0;

                foreach (var ch in line)
                {                   
                    CharacterInfo characterInfo;
                    if (!_cachedFont.GetCharacterInfo(ch, out characterInfo, _fontSize, _fontStyle))
                        continue;
                    var offsetPos = new Vector3(lineWidth, -textHeight);

                    /*mCore.Log("CH: {4} | maxX: {0} | minX: {1} | maxY: {2} | minY: {3} | advance: {5} | bearing: {6} |glyphHeight: {7} |glyphWidth: {8} | size: {9}", 
                        characterInfo.maxX,
                        characterInfo.minX,
                        characterInfo.maxY,
                        characterInfo.minY,
                        ch,
                        characterInfo.advance,
                        characterInfo.bearing,
                        characterInfo.glyphHeight,
                        characterInfo.glyphWidth,
                        characterInfo.size
                    );*/

                    var minX = characterInfo.minX / magic;
                    var maxX = characterInfo.maxX / magic;
                    var minY = characterInfo.minY / magic;
                    var maxY = characterInfo.maxY / magic;

                    switch (_verticalAlign)
                    {
                        case VerticalAlign.ASCENDERLINE:
                            maxY += Mathf.Abs(minY);
                            minY += Mathf.Abs(minY);
                            break;
                        case VerticalAlign.DESCENDERLINE:
                            minY -= Mathf.Abs(maxY);
                            maxY -= Mathf.Abs(maxY);
                            offsetPos = offsetPos + new Vector3(0, _cachedFont.lineHeight / magic);
                            break;
                    }

                    vertices[charIndex + 0] = new Vector3(minX, minY) + offsetPos;
                    vertices[charIndex + 1] = new Vector3(minX, maxY) + offsetPos;
                    vertices[charIndex + 2] = new Vector3(maxX, maxY) + offsetPos;
                    vertices[charIndex + 3] = new Vector3(maxX, minY) + offsetPos;

                    lastLetterSpacing = characterInfo.advance / magic;
                    lineWidth += lastLetterSpacing;

                    colors[charIndex + 0] = Color.white;
                    colors[charIndex + 1] = Color.white;
                    colors[charIndex + 2] = Color.white;
                    colors[charIndex + 3] = Color.white;

                    uv[charIndex + 0] = characterInfo.uvBottomLeft;
                    uv[charIndex + 1] = characterInfo.uvTopLeft;
                    uv[charIndex + 2] = characterInfo.uvTopRight;
                    uv[charIndex + 3] = characterInfo.uvBottomRight;

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

                    charIndex = charIndex + 4;
                    trianglesIndex = trianglesIndex + 6;
                    charsInLine++;
                }

                lineWidth -= lastLetterSpacing;
                textHeight += lineHeight;

                switch (_textAlignment)
                {
                    case TextAlignment.Center:
                        for (int i = charIndex - charsInLine * 4; i < charIndex; i++)
                        {
                            vertices[i].x = vertices[i].x - lineWidth / 2f;
                            vertices[i].y = vertices[i].y;
                            vertices[i].z = vertices[i].z;
                        }
                        break;
                    case TextAlignment.Right:
                        for (int i = charIndex - charsInLine * 4; i < charIndex; i++)
                        {
                            vertices[i].x = vertices[i].x - lineWidth;
                            vertices[i].y = vertices[i].y;
                            vertices[i].z = vertices[i].z;
                        }
                        break;
                }

                if (lineWidth > textWidth)
                    textWidth = lineWidth;
            }

            /*switch (_textAnchor)
            {
                case TextAnchor.UpperLeft:
                    for (int i = 0; i < vertices.Length; i++)
                        vertices[i] = new Vector3(vertices[i].x, vertices[i].y, vertices[i].z);
                    break;
                case TextAnchor.UpperCenter:
                    break;
                case TextAnchor.UpperRight:
                    break;
                case TextAnchor.MiddleLeft:
                    break;
                case TextAnchor.MiddleCenter:
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        vertices[i] = new Vector3(
                            vertices[i].x - textWidth / 2,
                            vertices[i].y + textHeight / 2 - lineHeight,
                            vertices[i].z
                        );
                    }
                    break;
                case TextAnchor.MiddleRight:
                    break;
                case TextAnchor.LowerLeft:
                    break;
                case TextAnchor.LowerCenter:
                    break;
                case TextAnchor.LowerRight:
                    break;
            }*/
            
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
            _meshRenderer.material = _cachedFont.material;
            _meshRenderer.SetPropertyBlock(_textPropertyBlock);
            Scale(localScale);
        }

        public UIObject SetColor(Color32 color)
        {
            _color = color;
            _textPropertyBlock.SetColor("_Color", _color);
            _meshRenderer.SetPropertyBlock(_textPropertyBlock);
            return this;
        }

        public UIObject SetColor(UIColor color)
        {
            return SetColor(color.Color32);
        }

        internal override void OnPostRender()
        {
            if (_forceUpdate)
                UpdateMeshText();

            base.OnPostRender();
        }
    }
}
