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
        public float LetterSpacing { get; set; } = 1;
        public float WordSpacing { get; set; } = 1;
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
        public float LetterSpacing { get { return _letterSpacing; } }
        public string Text { get { return _cachedText; } }
        public Color TextColor { get { return _color; } }

        public event UIEventHandler<UILabel> TextUpdated;
        
        private Font _cachedFont;
        private readonly MeshRenderer _meshRenderer;
        private readonly MeshFilter _meshFilter;
        private readonly MaterialPropertyBlock _textPropertyBlock;

        private string _fontName;
        private string _cachedText;
        private int _fontSize = 50;
        private float _letterSpacing = 1;
        private float _wordSpacing = 1;

        private TextAlignment _textAlignment;
        private TextAnchor _textAnchor;
        private VerticalAlign _verticalAlign;
        private FontStyle _fontStyle;
        private Color _color;
        private bool _needUpdate;

        private float _textWidth;
        private float _textHeight;

        protected UILabel(UIObject parent) : base(parent)
        {
            _meshRenderer = _gameObject.AddComponent<MeshRenderer>();
            _meshFilter = _gameObject.AddComponent<MeshFilter>();
            _meshFilter.mesh = new Mesh();
            _textPropertyBlock = new MaterialPropertyBlock();
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

            base.ApplySettings(settings);
             
            SetFont(labelSettings.Font, false);
            UpdateMeshText();
            SetColor(_color);
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

            _cachedFont.RequestCharactersInTexture(_cachedText, _fontSize, _fontStyle);
            var localScale = LocalScale();
            Scale(1, 1);

            var cleanText = _cachedText.Replace("\t", "");
            var textLines = cleanText.Split('\n');

            var charCount = 0;
            for (int i = 0; i < textLines.Length; i++)
                charCount += textLines[i].Length;

            var vertices = new Vector3[charCount * 4];
            var normals = new Vector3[charCount * 4];
            var uv = new Vector2[charCount * 4];
            var colors = new Color32[charCount * 4];
            var triangles = new int[charCount * 6];

            var textHeight = 0f;
            var charIndex = 0;
            var trianglesIndex = 0;

            _textWidth = 0;
            const float magic = 220f;
            var lineHeight = ((_fontSize / _cachedFont.fontSize) * _cachedFont.lineHeight) / magic;
            var maxTop = 0f;
            var minBottom = 0f;

            for (int i = 0; i < textLines.Length; i++)
            {
                var lineWidth = 0f;
                var charsInLine = 0;

                var firstX = 0f;
                var lastX = 0f;

                for (int k = 0; k < textLines[i].Length; k++)
                {                   
                    CharacterInfo characterInfo;
                    if (!_cachedFont.GetCharacterInfo(textLines[i][k], out characterInfo, _fontSize, _fontStyle))
                        continue;

                    var offsetPos = new Vector3(lineWidth, -textHeight);
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

                    if (k == 0)
                        firstX = vertices[charIndex + 0].x;
                    if (k == textLines[i].Length - 1)
                        lastX = vertices[charIndex + 3].x;

                    if (maxTop < vertices[charIndex + 1].y)
                        maxTop = vertices[charIndex + 1].y;
                    if (minBottom > vertices[charIndex + 0].y)
                        minBottom = vertices[charIndex + 0].y;

                    if (textLines[i][k] == ' ')
                        lineWidth += (characterInfo.advance / magic) * _wordSpacing;
                    else
                        lineWidth += (characterInfo.advance / magic) * _letterSpacing;

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

                var pureWidth = Mathf.Abs(lastX - firstX);
                var firstOffset = -firstX;
                textHeight += lineHeight;

                switch (_textAlignment)
                {
                    case TextAlignment.Center:
                        for (int j = charIndex - charsInLine * 4; j < charIndex; j++)
                        {
                            vertices[j].x = vertices[j].x - pureWidth / 2f - firstOffset;
                            vertices[j].y = vertices[j].y;
                            vertices[j].z = vertices[j].z;
                        }
                        break;
                    case TextAlignment.Right:
                        for (int j = charIndex - charsInLine * 4; j < charIndex; j++)
                        {
                            vertices[j].x = vertices[j].x - pureWidth - firstOffset;
                            vertices[j].y = vertices[j].y;
                            vertices[j].z = vertices[j].z;
                        }
                        break;
                    case TextAlignment.Left:
                        for (int j = charIndex - charsInLine * 4; j < charIndex; j++)
                        {
                            vertices[j].x = vertices[j].x - firstOffset;
                            vertices[j].y = vertices[j].y;
                            vertices[j].z = vertices[j].z;
                        }
                        break;
                } 

                if (pureWidth > _textWidth)
                    _textWidth = pureWidth;
            }

            _textHeight = maxTop - minBottom;

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

            var mesh = new Mesh
            {
                vertices = vertices,
                colors32 = colors,
                normals = normals,
                uv = uv,
                triangles = triangles
            };

            _meshFilter.mesh = mesh;
            //_meshFilter.mesh.RecalculateNormals();
            //_meshFilter.mesh.RecalculateBounds();
            _meshRenderer.material = _cachedFont.material;

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
            _textPropertyBlock.SetColor("_Color", _color);
            _meshRenderer.SetPropertyBlock(_textPropertyBlock);
            return this;
        }

        public UIObject SetColor(UIColor color)
        {
            return SetColor(color.Color32);
        }
    }
}
