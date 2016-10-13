using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mUIApp
{
    public class mUIDefaultSpriteRepository : ISpriteRepository
    {
        private readonly Dictionary<string, Sprite> _sprites;

        public mUIDefaultSpriteRepository()
        {
            _sprites = new Dictionary<string, Sprite>();
        }

        public IEnumerator<Sprite> GetEnumerator()
        {
            return _sprites.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count()
        {
            return _sprites.Count;
        }

        public Sprite this[string name]
        {
            get
            {
                if (_sprites.ContainsKey(name))
                    return _sprites[name];
                return null;
            }
        }

        public IEnumerator<string> Sprites
        {
            get { return _sprites.Keys.GetEnumerator(); } 
        }


        public ISpriteRepository Load(string spritePathWithName)
        {
            var sp = Resources.Load<Sprite>(spritePathWithName);
            Add(sp);
            return this;
        }

        public ISpriteRepository LoadAll(string pathToSprites = "")
        {
            var loadedSprites = Resources.LoadAll<Sprite>(pathToSprites);
            foreach (var loadedSprite in loadedSprites)
                Add(loadedSprite);
            return this;
        }

        public ISpriteRepository Add(Sprite sprite)
        {
            if (sprite == null || _sprites.ContainsKey(sprite.name))
                return this;
            _sprites.Add(sprite.name, sprite);
            return this;
        }

        public ISpriteRepository AddRange(IEnumerable<Sprite> sprites)
        {
            foreach (var sprite in sprites)
                Add(sprite);
            return this;
        }

        public ISpriteRepository Remove(string name)
        {
            if (_sprites.ContainsKey(name))
                _sprites.Remove(name);
            return this;
        }

        public ISpriteRepository RemoveRange(params string[] names)
        {
            foreach (var name in names)
                Remove(name);
            return this;
        }
    }
}
