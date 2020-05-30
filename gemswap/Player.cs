namespace GemSwap
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;

    public interface IPlayer
    {
        public void ProcessInput();
    }

    public abstract class Player<T> : IPlayer
        where T : struct
    {
        private readonly Board board;

        protected Player(Board board)
        {
            this.board = board;
        }

        protected T? PreviousState { get; set; }

        public abstract bool IsLeftPressed(T state);

        public abstract bool IsRightPressed(T state);

        public abstract bool IsDownPressed(T state);

        public abstract bool IsUpPressed(T state);

        public abstract bool IsSwapPressed(T state);

        public void ProcessInput()
        {
            T currentState = this.GetCurrentState();

            if (this.IsLeftPressed(currentState))
            {
                this.board.MoveCursor(dx: -1, dy: 0);
            }

            if (this.IsRightPressed(currentState))
            {
                this.board.MoveCursor(dx: +1, dy: 0);
            }

            if (this.IsDownPressed(currentState))
            {
                this.board.MoveCursor(dx: 0, dy: +1);
            }

            if (this.IsUpPressed(currentState))
            {
                this.board.MoveCursor(dx: 0, dy: -1);
            }

            if (this.IsSwapPressed(currentState))
            {
                this.board.Swap();
            }

            this.PreviousState = currentState;
        }

        protected abstract T GetCurrentState();
    }

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
