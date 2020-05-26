using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace gemswap {
    public class Player {
        Board board;
        GamePadState? previousState;

        public Player(Board board) {
            this.board = board;
        }

        protected GamePadState GetCurrentState() {
            return GamePad.GetState(PlayerIndex.One);
        }

        public bool IsLeftPressed(GamePadState state) {
            return state.DPad.Left == ButtonState.Pressed
                && state.DPad.Left != previousState?.DPad.Left;
        }

        public bool IsRightPressed(GamePadState state) {
            return state.DPad.Right == ButtonState.Pressed
                && state.DPad.Right != previousState?.DPad.Right;
        }

        public bool IsDownPressed(GamePadState state) {
            return state.DPad.Down == ButtonState.Pressed
                && state.DPad.Down != previousState?.DPad.Down;
        }

        public bool IsUpPressed(GamePadState state) {
            return state.DPad.Up == ButtonState.Pressed
                && state.DPad.Up != previousState?.DPad.Up;
        }

        public bool IsSwapPressed(GamePadState state) {
            return state.Buttons.A == ButtonState.Pressed
                && state.Buttons.A != previousState?.Buttons.A;
        }

        public void ProcessInput() {
            GamePadState gamePadState = GetCurrentState();

            if (IsLeftPressed(gamePadState)) {
                this.board.MoveCursor(dx: -1, dy: 0);
            }
            if (IsRightPressed(gamePadState)) {
                this.board.MoveCursor(dx: +1, dy: 0);
            }
            if (IsDownPressed(gamePadState)) {
                this.board.MoveCursor(dx: 0, dy: +1);
            }
            if (IsUpPressed(gamePadState)) {
                this.board.MoveCursor(dx: 0, dy: -1);
            }
            if (IsSwapPressed(gamePadState)) {
                this.board.Swap();
            }

            this.previousState = gamePadState;
        }
    }
}
