using System.Collections.Generic;
using UnityEngine;

namespace mUIApp
{
    public interface ISpriteRepository : IEnumerable<Sprite>
    {
        int Count();
        Sprite this[string name] { get; }
        IEnumerator<string> Sprites { get; }
        ISpriteRepository Load(string spritePathWithName);
        ISpriteRepository LoadAll(string pathToSprites);
        ISpriteRepository Add(Sprite sprite);
        ISpriteRepository AddRange(IEnumerable<Sprite> sprites);
        ISpriteRepository Remove(string name);
        ISpriteRepository RemoveRange(params string[] names);
    }
}
