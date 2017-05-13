using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace mFramework
{
    public static class SpritesRepository
    {
        private static readonly Dictionary<string, Sprite> _sprites;

        static SpritesRepository()
        {
            _sprites = new Dictionary<string, Sprite>();
        }
        
        public static int Count()
        {
            return _sprites.Count;
        }

        public static Sprite Get(string name)
        {
            if (_sprites.ContainsKey(name))
                return _sprites[name];
            return null;
        }

        public static void Load(string spritePathWithName)
        {
            var sp = Resources.Load<Sprite>(spritePathWithName);
            Add(sp);
        }

        public static void LoadAll(string pathToSprites = "")
        {
            var loadedSprites = Resources.LoadAll<Sprite>(pathToSprites);
            foreach (var loadedSprite in loadedSprites)
                Add(loadedSprite);
        }

        public static void Add(Sprite sprite)
        {
            if (sprite == null || _sprites.ContainsKey(sprite.name))
                throw new NullReferenceException("SpriteRepository: the given sprite was null");
            _sprites.Add(sprite.name, sprite);
        }

        public static void AddRange(IEnumerable<Sprite> sprites)
        {
            foreach (var sprite in sprites)
                Add(sprite);
        }

        public static void Remove(string name)
        {
            if (_sprites.ContainsKey(name))
                _sprites.Remove(name);
        }

        public static void RemoveRange(params string[] names)
        {
            foreach (var name in names)
                Remove(name);
        }
    }
}
