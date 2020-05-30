namespace GemSwap
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public class KeyboardPlayer : Player<KeyboardState>
    {
        public KeyboardPlayer(Board board)
            : base(board)
        {
        }

        protected override KeyboardState GetCurrentState()
        {
            return Keyboard.GetState();
        }

        public override bool IsLeftPressed(KeyboardState state)
        {
            return state.IsKeyDown(Keys.A)
                && !(this.PreviousState?.IsKeyDown(Keys.A) ?? false);
        }

        public override bool IsRightPressed(KeyboardState state)
        {
            return state.IsKeyDown(Keys.D)
                && !(this.PreviousState?.IsKeyDown(Keys.D) ?? false);
        }

        public override bool IsDownPressed(KeyboardState state)
        {
            return state.IsKeyDown(Keys.S)
                && !(this.PreviousState?.IsKeyDown(Keys.S) ?? false);
        }

        public override bool IsUpPressed(KeyboardState state)
        {
            return state.IsKeyDown(Keys.W)
                && !(this.PreviousState?.IsKeyDown(Keys.W) ?? false);
        }

        public override bool IsSwapPressed(KeyboardState state)
        {
            return state.IsKeyDown(Keys.Space)
                && !(this.PreviousState?.IsKeyDown(Keys.Space) ?? false);
        }
    }
}
