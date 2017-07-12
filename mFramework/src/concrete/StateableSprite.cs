using System;
using UnityEngine;

namespace mFramework
{
    public struct SpriteStates
    {
        public Sprite Default { get; set; }
        public Sprite Highlighted { get; set; }
        public Sprite Selected { get; set; }
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

        public event EventHandler<StateableSpriteStateChangedEventArgs> StateChanged;
        public State CurrentState { get { return _state; } }
        
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
            StateChanged?.Invoke(this, new StateableSpriteStateChangedEventArgs(_spriteStates.Default));
        }

        public void SetHighlighted()
        {
            _state = State.HIGHLIGHTED;
            StateChanged?.Invoke(this, new StateableSpriteStateChangedEventArgs(_spriteStates.Highlighted));
        }

        public void SetSelected()
        {
            _state = State.SELECTED;
            StateChanged?.Invoke(this, new StateableSpriteStateChangedEventArgs(_spriteStates.Selected));
        }
    }
}
