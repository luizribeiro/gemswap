namespace GemSwap
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public class GamePadPlayer : Player<GamePadState>
    {
        private readonly PlayerIndex playerIndex;

        public GamePadPlayer(
            Board board,
            PlayerIndex playerIndex
        )
            : base(board)
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
            return state.Buttons.A == ButtonState.Pressed
                && state.Buttons.A != this.PreviousState?.Buttons.A;
        }

        protected override GamePadState GetCurrentState()
        {
            return GamePad.GetState(this.playerIndex);
        }
    }
}
