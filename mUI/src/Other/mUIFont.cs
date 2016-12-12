using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mUIApp.Other
{
    public class mUIFont
    {
        public enum ECharCase
        {
            NONE,
            ANY_CASE,
            UPPERCASE,
            LOWERCASE,    
        }

        public class CharMesh
        {
            public readonly Vector2 Pivot;
            public readonly Vector2[] Uv;
            public readonly Vector3[] Vertices;
            public readonly Sprite Sprite;

            public float Width { get { return Vertices[3].x - Vertices[0].x; } }
            public float Height { get { return Vertices[1].y - Vertices[0].y; } }

            public CharMesh(Vector2 pivot, Vector2[] uv, Vector3[] vertices, Sprite sprite)
            {
                Sprite = sprite;
                Pivot = pivot;
                Uv = uv;
                Vertices = vertices;
            }
        }

        public Material Material { get { return _fontMaterial; } }
        public bool Loaded { get; }
        public float SpaceLength { get; set; }
        public float MaxCharHeight { get; }
        public float MaxCharWidth { get; }
        public float AvgCharWidth { get; }
        public float AvgCharHeight { get; }

        public ECharCase CharCase { get; set; }
        private readonly Material _fontMaterial;
        private readonly Dictionary<char, CharMesh> _charMeshes;

        public mUIFont(string name)
        {
            Loaded = true;

            var fontSprits = Resources.LoadAll<Sprite>("mUI/Fonts/" + name);
            if (fontSprits.Length > 0)
            {
                _fontMaterial = new Material(Shader.Find("Sprites/Default"));
                _fontMaterial.mainTexture = fontSprits[0].texture;
                try
                {
                    _charMeshes = fontSprits.Select(CreateCharMesh).ToDictionary(sprite => KeySelector(sprite.Sprite.name));
                }
                catch (Exception)
                {
                    mUI.Log("Can't load font: " + name);
                    Loaded = false;
                    throw;
                }
            }
            else
            {
                Loaded = false;
                mUI.Log("Can't load font: " + name);
            }

            if (Loaded)
            {
                mUI.Log("Font: " + name + " loaded");
               
                MaxCharHeight = _charMeshes.Max(c => c.Value.Height);
                MaxCharWidth = _charMeshes.Max(c => c.Value.Width);
                AvgCharWidth = _charMeshes.Average(c => c.Value.Width);
                AvgCharHeight = _charMeshes.Average(c => c.Value.Height);
                SpaceLength = AvgCharWidth/3;
                CharCase = ECharCase.NONE;
            }
        }

        public bool Contains(char @char)
        {
            @char = ParseChar(@char);
            return _charMeshes.ContainsKey(@char);
        }

        public CharMesh GetCharMesh(char @char)
        {
            @char = ParseChar(@char);
            if (_charMeshes.ContainsKey(@char))
                return _charMeshes[@char];
            return null;
        }

        private CharMesh CreateCharMesh(Sprite sprite)
        {
            var vertices = new Vector3[] {
                new Vector2(0, 0),
                new Vector2(0, sprite.bounds.size.y),
                new Vector2(sprite.bounds.size.x, sprite.bounds.size.y),
                new Vector2(sprite.bounds.size.x, 0),
            };

            Vector2 textureCoordsFix = new Vector2(
                1.0f / sprite.texture.width,
                1.0f / sprite.texture.height
            );

            Vector2 _00 = new Vector2(sprite.rect.x, sprite.rect.y);
            Vector2 _01 = new Vector2(sprite.rect.x, sprite.rect.yMax);
            Vector2 _11 = new Vector2(sprite.rect.xMax, sprite.rect.yMax);
            Vector2 _10 = new Vector2(sprite.rect.xMax, sprite.rect.y);

            _00.Scale(textureCoordsFix);
            _01.Scale(textureCoordsFix);
            _11.Scale(textureCoordsFix);
            _10.Scale(textureCoordsFix);

            var pivotX = -sprite.bounds.center.x / sprite.bounds.extents.x / 2 + 0.5f;
            var pivotY = -sprite.bounds.center.y / sprite.bounds.extents.y / 2 + 0.5f;

            var pivot = new Vector2(pivotX, pivotY);
            var uv = new [] {_00, _01, _11, _10};

            return new CharMesh(pivot, uv, vertices, sprite);
        }

        public static char KeySelector(string st)
        {
            int code;
            if (int.TryParse(st, out code) && code > 10)
                return (char)code;
            return !string.IsNullOrEmpty(st) ? st[0] : '\0';
        }

        private char ParseChar(char @char)
        {
            bool isUpper = char.IsUpper(@char);

            if (CharCase == ECharCase.ANY_CASE)
            {
                if (_charMeshes.ContainsKey(@char))
                    return @char;
                if (!isUpper & _charMeshes.ContainsKey(char.ToUpper(@char)))
                    return char.ToUpper(@char);
                if (isUpper & _charMeshes.ContainsKey(char.ToLower(@char)))
                    return char.ToLower(@char);
            }
            else if (CharCase == ECharCase.UPPERCASE)
            {
                if (!isUpper)
                    return char.ToUpper(@char);
            }
            else if (CharCase == ECharCase.LOWERCASE)
            {
                if (isUpper)
                    return char.ToLower(@char);
            }

            return @char;
        }
    }
}
