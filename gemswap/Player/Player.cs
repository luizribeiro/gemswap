namespace GemSwap.Player
{
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
}
