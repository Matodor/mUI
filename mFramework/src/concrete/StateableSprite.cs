using System;
using UnityEngine;

namespace mFramework
{
    public class SpriteStates
    {
        public Sprite Default;
        public Sprite Highlighted;
        public Sprite Selected;
    }

    public class StateableSprite
    {
        public enum State
        {
            DEFAULT = 0,
            HIGHLIGHTED = 1,
            SELECTED = 2
        }

        public event Action<StateableSprite, Sprite> StateChanged = delegate { };
        public State CurrentState => _state;

        private readonly SpriteStates _spriteStates;
        private State _state;

        private StateableSprite(SpriteStates spriteStates)
        {
            _spriteStates = spriteStates;
            _state = State.DEFAULT;
        }

        public static StateableSprite Create(SpriteStates spriteStates)
        {
            return new StateableSprite(spriteStates);
        }

        public void SetDefault()
        {
            _state = State.DEFAULT;
            StateChanged.Invoke(this, _spriteStates.Default);
        }

        public void SetHighlighted()
        {
            _state = State.HIGHLIGHTED;
            StateChanged.Invoke(this, _spriteStates.Highlighted);
        }

        public void SetSelected()
        {
            _state = State.SELECTED;
            StateChanged.Invoke(this, _spriteStates.Selected);
        }
    }
}
