namespace GemSwap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Board
    {
        public const int EMPTY = -1;

        private readonly Config config;
        private readonly int[,] board;
        private readonly int[,] boardDX;
        private readonly int[,] boardDY;
        private readonly bool[,] isLocked;
        private readonly Timer?[,] movimentTimer;
        private readonly Timer?[,] fadeOutTimer;
        private readonly Dictionary<BoardEvent, List<Action>> listeners;
        private readonly Random random;

        private int[] upcomingRow;
        private float offset;
        private int cursorX;
        private int cursorY;
        private int boardVerticalOffset;
        private Timer? eliminationTimer;

        public Board(Config config)
        {
            this.config = config;

            this.Score = 0;
            this.HasGameEnded = false;
            this.board = new int[config.BoardWidth, config.BoardHeight];
            this.boardDX = new int[config.BoardWidth, config.BoardHeight];
            this.boardDY = new int[config.BoardWidth, config.BoardHeight];
            this.isLocked = new bool[
                config.BoardWidth,
                config.BoardHeight
            ];
            this.movimentTimer = new Timer[
                config.BoardWidth,
                config.BoardHeight
            ];
            this.fadeOutTimer = new Timer[
                config.BoardWidth,
                config.BoardHeight
            ];

            this.listeners = new Dictionary<BoardEvent, List<Action>>();
            BoardEvent[] allEvents =
                (BoardEvent[])Enum.GetValues(typeof(BoardEvent));
            foreach (BoardEvent boardEvent in allEvents)
            {
                this.listeners[boardEvent] = new List<Action>();
            }

            this.cursorX = 0;
            this.cursorY = config.BoardHeight - 1;
            this.boardVerticalOffset = 0;

            this.random = new Random();
            for (int x = 0; x < config.BoardWidth; x++)
            {
                int height = this.random.Next(
                    config.BoardInitialMinHeight,
                    config.BoardInitialMaxHeight
                );
                for (int y = 0; y < config.BoardHeight; y++)
                {
                    this.board[x, y] = y > config.BoardHeight - height
                        ? this.random.Next(0, config.NumGems)
                        : Board.EMPTY;
                    this.isLocked[x, y] = false;
                    this.movimentTimer[x, y] = null;
                    this.fadeOutTimer[x, y] = null;
                    this.boardDX[x, y] = 0;
                    this.boardDY[x, y] = 0;
                }
            }

            this.upcomingRow = this.BuildUpcomingRow();
        }

        public Board(Config config, int[,] board, int initialScore)
            : this(config)
        {
            this.board = board;
            this.Score = initialScore;
        }

        public Board(
            Config config,
            int[,] board,
            int[] upcomingRow,
            int initialScore
        )
            : this(config, board, initialScore)
        {
            this.upcomingRow = upcomingRow;
        }

        public int Score { get; private set; }

        public bool HasGameEnded { get; private set; }

        public bool IsCloseToLosing
        {
            get
            {
                for (int y = 0; y < this.config.NumRowsLeftForAlarm; y++)
                {
                    for (int x = 0; x < this.config.BoardWidth; x++)
                    {
                        if (this.board[x, y] != Board.EMPTY)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public void Update(float ellapsedMilliseconds)
        {
            if (this.HasGameEnded)
            {
                return;
            }

            if (this.ShouldScroll())
            {
                this.ScrollBoardUp(ellapsedMilliseconds);
            }

            for (int x = 0; x < this.config.BoardWidth; x++)
            {
                for (int y = this.config.BoardHeight - 1; y >= 0; y--)
                {
                    if (this.isLocked[x, y])
                    {
                        continue;
                    }

                    if (this.board[x, y] == Board.EMPTY)
                    {
                        this.FallAllAbove(x, y);
                    }
                }
            }

            this.EliminateContiguous();
        }

        public int GetCell(int x, int y)
        {
            return this.board[x, y];
        }

        public int GetUpcomingCell(int x)
        {
            return this.upcomingRow[x];
        }

        public float GetOffset()
        {
            return this.offset;
        }

        public int GetCursorX()
        {
            return this.cursorX;
        }

        public int GetCursorY()
        {
            return this.cursorY;
        }

        public void MoveCursor(int dx, int dy)
        {
            if (this.HasGameEnded)
            {
                return;
            }

            this.cursorX += dx;
            this.cursorY += dy;

            if (this.cursorX < 0)
            {
                this.cursorX = 0;
            }

            if (this.cursorX > this.config.BoardWidth - 2)
            {
                this.cursorX = this.config.BoardWidth - 2;
            }

            if (this.cursorY < 0)
            {
                this.cursorY = 0;
            }

            if (this.cursorY > this.config.BoardHeight - 1)
            {
                this.cursorY = this.config.BoardHeight - 1;
            }
        }

        public float GetCellOffsetX(int x, int y)
        {
            return (this.movimentTimer[x, y]?.Progress() ?? 0.0f)
                * this.boardDX[x, y] * this.config.GemWidth;
        }

        public float GetCellOffsetY(int x, int y)
        {
            return (this.movimentTimer[x, y]?.Progress() ?? 0.0f)
                * this.boardDY[x, y] * this.config.GemHeight;
        }

        public int GetCellAlpha(int x, int y)
        {
            return Convert.ToInt32(
                (1.0f - (this.fadeOutTimer[x, y]?.Progress() ?? 0.0f)) * 255
            );
        }

        public void Swap()
        {
            int x = this.cursorX;
            int y = this.cursorY;

            if (this.isLocked[x, y] || this.isLocked[x + 1, y])
            {
                return;
            }

            if (this.HasGameEnded)
            {
                return;
            }

            this.isLocked[x, y] = true;
            this.isLocked[x + 1, y] = true;

            int leftGem = this.board[x, y];
            int rightGem = this.board[x + 1, y];

            this.boardDX[x, y] = +1;
            this.boardDY[x, y] = 0;
            this.boardDX[x + 1, y] = -1;
            this.boardDY[x + 1, y] = 0;

            Timer swapTimer = this.CreateCellTimer(
                x,
                y,
                durationMilliseconds: this.config.SwapDurationMs,
                onDoneCallback: (x, y) =>
                {
                    this.board[x, y] = rightGem;
                    this.board[x + 1, y] = leftGem;

                    this.movimentTimer[x, y] = null;
                    this.movimentTimer[x + 1, y] = null;

                    this.isLocked[x, y] = false;
                    this.isLocked[x + 1, y] = false;
                }
            );

            this.movimentTimer[x, y] = swapTimer;
            this.movimentTimer[x + 1, y] = swapTimer;

            this.Notify(BoardEvent.Swap);
        }

        public void When(BoardEvent boardEvent, Action callback)
        {
            this.listeners[boardEvent].Add(callback);
        }

        private void ScrollBoardUp(float ellapsedMilliseconds)
        {
            for (int x = 0; x < this.config.BoardWidth; x++)
            {
                for (int y = 0; y < this.config.BoardHeight; y++)
                {
                    if (this.fadeOutTimer[x, y] != null)
                    {
                        return;
                    }
                }
            }

            this.offset +=
                this.config.BoardSpeedPixelsPerMs * ellapsedMilliseconds;

            if (this.offset >= this.config.GemHeight)
            {
                this.offset = 0.0f;
                this.AddNewRow();
            }
        }

        private void EliminateContiguous()
        {
            List<Tuple<int, int>> toEliminate = new List<Tuple<int, int>>();

            for (int x = 0; x < this.config.BoardWidth; x++)
            {
                int count = 0;
                int currentGem = Board.EMPTY;
                List<Tuple<int, int>> currentCells =
                    new List<Tuple<int, int>>();

                for (int y = 0; y < this.config.BoardHeight; y++)
                {
                    if (
                        this.board[x, y] == Board.EMPTY || this.isLocked[x, y]
                    )
                    {
                        currentCells.Clear();
                        count = 0;
                        currentGem = Board.EMPTY;
                        continue;
                    }

                    if (this.board[x, y] != currentGem)
                    {
                        currentGem = this.board[x, y];
                        currentCells.Clear();
                        currentCells.Add(new Tuple<int, int>(x, y));
                        count = 1;
                    }
                    else
                    {
                        currentCells.Add(new Tuple<int, int>(x, y));
                        count++;
                    }

                    if (count >= 3)
                    {
                        toEliminate.AddRange(currentCells);
                    }
                }
            }

            for (int y = 0; y < this.config.BoardHeight; y++)
            {
                int count = 0;
                int currentGem = Board.EMPTY;
                List<Tuple<int, int>> currentCells =
                    new List<Tuple<int, int>>();

                for (int x = 0; x < this.config.BoardWidth; x++)
                {
                    if (
                        this.board[x, y] == Board.EMPTY || this.isLocked[x, y]
                    )
                    {
                        currentCells.Clear();
                        count = 0;
                        currentGem = Board.EMPTY;
                        continue;
                    }

                    if (this.board[x, y] != currentGem)
                    {
                        currentGem = this.board[x, y];
                        currentCells.Clear();
                        currentCells.Add(new Tuple<int, int>(x, y));
                        count = 1;
                    }
                    else
                    {
                        currentCells.Add(new Tuple<int, int>(x, y));
                        count++;
                    }

                    if (count >= 3)
                    {
                        toEliminate.AddRange(currentCells);
                    }
                }
            }

            if (toEliminate.Count > 0)
            {
                this.eliminationTimer = TimerManager.AddTimer(
                    durationMilliseconds: this.config.EliminationScrollCooldownMs,
                    onDoneCallback: () =>
                    {
                        this.eliminationTimer = null;
                    }
                );
            }

            foreach (Tuple<int, int> cell in toEliminate.Distinct().ToList())
            {
                this.EliminateCell(cell.Item1, cell.Item2);
            }

            int numGemsEliminated = toEliminate.Distinct().Count();
            this.Score += numGemsEliminated * (numGemsEliminated - 2);
        }

        private bool ShouldScroll()
        {
            return this.eliminationTimer == null;
        }

        private void EliminateCell(int x, int y)
        {
            this.isLocked[x, y] = true;
            Timer timer = this.CreateCellTimer(
                x,
                y,
                durationMilliseconds: this.config.EliminationDurationMs,
                onDoneCallback: (x, y) =>
                {
                    this.board[x, y] = Board.EMPTY;
                    this.isLocked[x, y] = false;
                    this.fadeOutTimer[x, y] = null;
                }
            );
            this.fadeOutTimer[x, y] = timer;

            this.Notify(BoardEvent.Elimination);
        }

        private void Notify(BoardEvent boardEvent)
        {
            foreach (Action callback in this.listeners[boardEvent])
            {
                callback.Invoke();
            }
        }

        private void FallAllAbove(int x, int origY)
        {
            for (int y = origY - 1; y >= 0; y--)
            {
                int gem = this.board[x, y];
                if (gem == Board.EMPTY)
                {
                    break;
                }

                // fall x, y into x, y + 1
                this.isLocked[x, y] = true;
                this.isLocked[x, y + 1] = true;
                this.boardDX[x, y] = 0;
                this.boardDY[x, y] = +1;

                Timer fallTimer = this.CreateCellTimer(
                    x,
                    y,
                    durationMilliseconds: this.config.FallDurationMs,
                    onDoneCallback: (x, y) =>
                    {
                        this.board[x, y] = Board.EMPTY;
                        this.board[x, y + 1] = gem;

                        this.movimentTimer[x, y] = null;

                        this.isLocked[x, y] = false;
                        this.isLocked[x, y + 1] = false;
                    }
                );

                this.movimentTimer[x, y] = fallTimer;
            }
        }

        private void AddNewRow()
        {
            this.cursorY--;
            this.boardVerticalOffset++;

            this.HasGameEnded = Enumerable.Range(0, this.board.GetLength(0))
                .Any(x => this.board[x, 1] != Board.EMPTY);

            for (int x = 0; x < this.config.BoardWidth; x++)
            {
                for (int y = 0; y < this.config.BoardHeight - 1; y++)
                {
                    this.board[x, y] = this.board[x, y + 1];
                    this.isLocked[x, y] = this.isLocked[x, y + 1];
                    this.movimentTimer[x, y] = this.movimentTimer[x, y + 1];
                    this.fadeOutTimer[x, y] = this.fadeOutTimer[x, y + 1];
                    this.boardDX[x, y] = this.boardDX[x, y + 1];
                    this.boardDY[x, y] = this.boardDY[x, y + 1];
                }
            }

            for (int x = 0; x < this.config.BoardWidth; x++)
            {
                int y = this.config.BoardHeight - 1;
                this.board[x, y] = this.upcomingRow[x];
                this.isLocked[x, y] = false;
                this.movimentTimer[x, y] = null;
                this.fadeOutTimer[x, y] = null;
                this.boardDX[x, y] = 0;
                this.boardDY[x, y] = 0;
            }

            this.upcomingRow = this.BuildUpcomingRow();
        }

        private int[] BuildUpcomingRow()
        {
            int[] upcomingRow = new int[this.config.BoardWidth];
            for (int x = 0; x < this.config.BoardWidth; x++)
            {
                upcomingRow[x] = this.random.Next(0, this.config.NumGems);
            }

            return upcomingRow;
        }

        private Timer CreateCellTimer(
            int x,
            int y,
            float durationMilliseconds,
            Action<int, int> onDoneCallback
        )
        {
            int xx = x, yy = y + this.boardVerticalOffset;
            return TimerManager.AddTimer(
                durationMilliseconds: durationMilliseconds,
                onDoneCallback: () => onDoneCallback(
                    xx,
                    yy - this.boardVerticalOffset
                )
            );
        }
    }
}
