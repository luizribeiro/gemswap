namespace GemSwap.Player
{
    public abstract class Player<T> : IPlayer
        where T : struct
    {
        private Board? board;

        public Board Board
        {
            get => this.board!;
            set => this.board = value;
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
                this.Board.MoveCursor(dx: -1, dy: 0);
            }

            if (this.IsRightPressed(currentState))
            {
                this.Board.MoveCursor(dx: +1, dy: 0);
            }

            if (this.IsDownPressed(currentState))
            {
                this.Board.MoveCursor(dx: 0, dy: +1);
            }

            if (this.IsUpPressed(currentState))
            {
                this.Board.MoveCursor(dx: 0, dy: -1);
            }

            if (this.IsSwapPressed(currentState))
            {
                this.Board.Swap();
            }

            this.PreviousState = currentState;
        }

        protected abstract T GetCurrentState();
    }
}
