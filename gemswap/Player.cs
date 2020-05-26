using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace gemswap {
    public abstract class Player<T> where T : struct {
        private Board board;
        protected T? previousState;

        protected Player(Board board) {
            this.board = board;
        }

        protected abstract T GetCurrentState();

        public abstract bool IsLeftPressed(T state);

        public abstract bool IsRightPressed(T state);

        public abstract bool IsDownPressed(T state);

        public abstract bool IsUpPressed(T state);

        public abstract bool IsSwapPressed(T state);

        public void ProcessInput() {
            T currentState = GetCurrentState();

            if (IsLeftPressed(currentState)) {
                this.board.MoveCursor(dx: -1, dy: 0);
            }
            if (IsRightPressed(currentState)) {
                this.board.MoveCursor(dx: +1, dy: 0);
            }
            if (IsDownPressed(currentState)) {
                this.board.MoveCursor(dx: 0, dy: +1);
            }
            if (IsUpPressed(currentState)) {
                this.board.MoveCursor(dx: 0, dy: -1);
            }
            if (IsSwapPressed(currentState)) {
                this.board.Swap();
            }

            this.previousState = currentState;
        }
    }

    public class GamePadPlayer : Player<GamePadState> {
        PlayerIndex playerIndex;

        public GamePadPlayer(
            Board board,
            PlayerIndex playerIndex
        ) : base(board) {
            this.playerIndex = playerIndex;
        }

        protected override GamePadState GetCurrentState() {
            return GamePad.GetState(this.playerIndex);
        }

        public override bool IsLeftPressed(GamePadState state) {
            return state.DPad.Left == ButtonState.Pressed
                && state.DPad.Left != previousState?.DPad.Left;
        }

        public override bool IsRightPressed(GamePadState state) {
            return state.DPad.Right == ButtonState.Pressed
                && state.DPad.Right != previousState?.DPad.Right;
        }

        public override bool IsDownPressed(GamePadState state) {
            return state.DPad.Down == ButtonState.Pressed
                && state.DPad.Down != previousState?.DPad.Down;
        }

        public override bool IsUpPressed(GamePadState state) {
            return state.DPad.Up == ButtonState.Pressed
                && state.DPad.Up != previousState?.DPad.Up;
        }

        public override bool IsSwapPressed(GamePadState state) {
            return state.Buttons.A == ButtonState.Pressed
                && state.Buttons.A != previousState?.Buttons.A;
        }
    }
}
