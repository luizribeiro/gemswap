namespace GemSwap.Player
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public class GamePadPlayer : Player<GamePadState>
    {
        private readonly PlayerIndex playerIndex;

        public GamePadPlayer(
            PlayerIndex playerIndex
        )
        {
            this.playerIndex = playerIndex;
        }

        public override bool IsLeftPressed(GamePadState state)
        {
            return state.DPad.Left == ButtonState.Pressed
                && state.DPad.Left != this.PreviousState?.DPad.Left;
        }

        public override bool IsRightPressed(GamePadState state)
        {
            return state.DPad.Right == ButtonState.Pressed
                && state.DPad.Right != this.PreviousState?.DPad.Right;
        }

        public override bool IsDownPressed(GamePadState state)
        {
            return state.DPad.Down == ButtonState.Pressed
                && state.DPad.Down != this.PreviousState?.DPad.Down;
        }

        public override bool IsUpPressed(GamePadState state)
        {
            return state.DPad.Up == ButtonState.Pressed
                && state.DPad.Up != this.PreviousState?.DPad.Up;
        }

        public override bool IsSwapPressed(GamePadState state)
        {
            return this.WasButtonPressed(state, b => b?.A)
                || this.WasButtonPressed(state, b => b?.B)
                || this.WasButtonPressed(state, b => b?.X)
                || this.WasButtonPressed(state, b => b?.Y);
        }

        protected override GamePadState GetCurrentState()
        {
            return GamePad.GetState(this.playerIndex);
        }

        private bool WasButtonPressed(
            GamePadState state,
            Func<GamePadButtons?, ButtonState?> getButtonState
        )
        {
            return getButtonState.Invoke(state.Buttons) == ButtonState.Pressed
                && getButtonState.Invoke(state.Buttons)
                    != getButtonState.Invoke(this.PreviousState?.Buttons);
        }
    }
}
