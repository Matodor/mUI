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

    public class StateableSpriteStateChangedEventArgs : EventArgs
    {
        public Sprite StateSprite { get; }

        public StateableSpriteStateChangedEventArgs(Sprite stateSprite)
        {
            StateSprite = stateSprite;
        }
    }

    public class StateableSprite
    {
        public enum State
        {
            DEFAULT = 0,
            HIGHLIGHTED = 1,
            SELECTED = 2
        }

        public event EventHandler<StateableSpriteStateChangedEventArgs> StateChanged = delegate { };
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
            StateChanged.Invoke(this, new StateableSpriteStateChangedEventArgs(_spriteStates.Default));
        }

        public void SetHighlighted()
        {
            _state = State.HIGHLIGHTED;
            StateChanged.Invoke(this, new StateableSpriteStateChangedEventArgs(_spriteStates.Highlighted));
        }

        public void SetSelected()
        {
            _state = State.SELECTED;
            StateChanged.Invoke(this, new StateableSpriteStateChangedEventArgs(_spriteStates.Selected));
        }
    }
}
