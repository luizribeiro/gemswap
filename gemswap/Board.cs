using System;
using System.Collections.Generic;
using System.Linq;

namespace gemswap
{
    public class Board
    {
        public const int EMPTY = -1;

        private int[,] board;
        private int[,] boardDX;
        private int[,] boardDY;
        private bool[,] isLocked;
        private Timer?[,] movimentTimer;
        private Timer?[,] fadeOutTimer;
        private int[] upcomingRow;
        private float offset;
        private int cursorX;
        private int cursorY;
        private int boardVerticalOffset;
        private Config config;

        public Board(Config config)
        {
            this.config = config;

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

            this.cursorX = 0;
            this.cursorY = config.BoardHeight - 1;
            this.boardVerticalOffset = 0;

            Random random = new Random();
            for (int x = 0; x < config.BoardWidth; x++) {
                int height = random.Next(
                    config.BoardInitialMinHeight,
                    config.BoardInitialMaxHeight
                );
                for (int y = 0; y < config.BoardHeight; y++) {
                    this.board[x, y] = y > config.BoardHeight - height
                        ? random.Next(0, config.NumGems)
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

        public Board(Config config, int[,] board) : this(config) {
            this.board = board;
        }

        public Board(
            Config config,
            int[,] board,
            int[] upcomingRow
        ) : this(config, board) {
            this.upcomingRow = upcomingRow;
        }

        public void Update(float ellapsedMilliseconds) {
            this.offset += (config.GemHeight * ellapsedMilliseconds)
                / config.BoardSpeedRowPerMs;

            if (this.offset >= config.GemHeight) {
                this.offset = 0.0f;
                this.AddNewRow();
            }

            EliminateContiguous();

            for (int x = 0; x < config.BoardWidth; x++) {
                for (int y = config.BoardHeight - 1; y >= 0; y--) {
                    if (this.isLocked[x, y]) {
                        continue;
                    }
                    if (this.board[x, y] == Board.EMPTY) {
                        FallAllAbove(x, y);
                        break;
                    }
                }
            }
        }

        private void EliminateContiguous() {
            List<Tuple<int, int>> toEliminate = new List<Tuple<int, int>>();

            for (int x = 0; x < config.BoardWidth; x++) {
                int count = 0;
                int currentGem = Board.EMPTY;
                List<Tuple<int, int>> currentCells =
                    new List<Tuple<int, int>>();

                for (int y = 0; y < config.BoardHeight; y++) {
                    if (
                        this.board[x, y] == Board.EMPTY || this.isLocked[x, y]
                    ) {
                        currentCells.Clear();
                        count = 0;
                        currentGem = Board.EMPTY;
                        continue;
                    }

                    if (this.board[x, y] != currentGem) {
                        currentGem = this.board[x, y];
                        currentCells.Clear();
                        currentCells.Add(new Tuple<int, int>(x, y));
                        count = 1;
                    } else {
                        currentCells.Add(new Tuple<int, int>(x, y));
                        count++;
                    }

                    if (count >= 3) {
                        toEliminate.AddRange(currentCells);
                    }
                }
            }

            for (int y = 0; y < config.BoardHeight; y++) {
                int count = 0;
                int currentGem = Board.EMPTY;
                List<Tuple<int, int>> currentCells =
                    new List<Tuple<int, int>>();

                for (int x = 0; x < config.BoardWidth; x++) {
                    if (
                        this.board[x, y] == Board.EMPTY || this.isLocked[x, y]
                    ) {
                        currentCells.Clear();
                        count = 0;
                        currentGem = Board.EMPTY;
                        continue;
                    }

                    if (this.board[x, y] != currentGem) {
                        currentGem = this.board[x, y];
                        currentCells.Clear();
                        currentCells.Add(new Tuple<int, int>(x, y));
                        count = 1;
                    } else {
                        currentCells.Add(new Tuple<int, int>(x, y));
                        count++;
                    }

                    if (count >= 3) {
                        toEliminate.AddRange(currentCells);
                    }
                }
            }

            foreach (Tuple<int, int> cell in toEliminate.Distinct().ToList()) {
                EliminateCell(cell.Item1, cell.Item2);
            }
        }

        private void EliminateCell(int x, int y) {
            this.isLocked[x, y] = true;
            Timer timer = this.CreateCellTimer(
                x,
                y,
                durationMilliseconds: config.EliminationDurationMs,
                onDoneCallback: (x, y) => {
                    this.board[x, y] = Board.EMPTY;
                    this.isLocked[x, y] = false;
                    this.fadeOutTimer[x, y] = null;
                }
            );
            this.fadeOutTimer[x, y] = timer;
        }

        private void FallAllAbove(int x, int origY) {
            for (int y = origY - 1; y >= 0; y--) {
                int gem = this.board[x, y];
                if (gem == Board.EMPTY) {
                    continue;
                }

                // fall x, y into x, y + 1
                this.isLocked[x, y] = true;
                this.isLocked[x, y + 1] = true;
                this.boardDX[x, y] =  0;
                this.boardDY[x, y] = +1;

                Timer fallTimer = this.CreateCellTimer(
                    x,
                    y,
                    durationMilliseconds: config.FallDurationMs,
                    onDoneCallback: (x, y) => {
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

        private void AddNewRow() {
            this.cursorY--;
            this.boardVerticalOffset++;

            for (int x = 0; x < config.BoardWidth; x++) {
                for (int y = 0; y < config.BoardHeight - 1; y++) {
                    this.board[x, y] = this.board[x, y + 1];
                    this.isLocked[x, y] = this.isLocked[x, y + 1];
                    this.movimentTimer[x, y] = this.movimentTimer[x, y + 1];
                    this.fadeOutTimer[x, y] = this.fadeOutTimer[x, y + 1];
                    this.boardDX[x, y] = this.boardDX[x, y + 1];
                    this.boardDY[x, y] = this.boardDY[x, y + 1];
                }
            }

            for (int x = 0; x < config.BoardWidth; x++) {
                int y = config.BoardHeight - 1;
                this.board[x, y] = this.upcomingRow[x];
                this.isLocked[x, y] = false;
                this.movimentTimer[x, y] = null;
                this.fadeOutTimer[x, y] = null;
                this.boardDX[x, y] = 0;
                this.boardDY[x, y] = 0;
            }

            this.upcomingRow = BuildUpcomingRow();
        }

        private int[] BuildUpcomingRow() {
            int[] upcomingRow = new int[config.BoardWidth];
            Random random = new Random();
            for (int x = 0; x < config.BoardWidth; x++) {
                upcomingRow[x] = random.Next(0, config.NumGems);
            }
            return upcomingRow;
        }

        public int getCell(int x, int y) {
            return this.board[x, y];
        }

        public int getUpcomingCell(int x) {
            return this.upcomingRow[x];
        }

        public float getOffset() {
            return this.offset;
        }

        public int getCursorX() {
            return this.cursorX;
        }

        public int getCursorY() {
            return this.cursorY;
        }

        public void MoveCursor(int dx, int dy) {
            this.cursorX += dx;
            this.cursorY += dy;

            if (this.cursorX < 0) {
                this.cursorX = 0;
            }
            if (this.cursorX > config.BoardWidth - 2) {
                this.cursorX = config.BoardWidth - 2;
            }

            if (this.cursorY < 0) {
                this.cursorY = 0;
            }
            if (this.cursorY > config.BoardHeight - 1) {
                this.cursorY = config.BoardHeight - 1;
            }
        }

        public float GetCellOffsetX(int x, int y) {
            return (this.movimentTimer[x, y]?.Progress() ?? 0.0f)
                * this.boardDX[x, y] * config.GemWidth;
        }

        public float GetCellOffsetY(int x, int y) {
            return (this.movimentTimer[x, y]?.Progress() ?? 0.0f)
                * this.boardDY[x, y] * config.GemHeight;
        }

        public int GetCellAlpha(int x, int y) {
            return Convert.ToInt32(
                (1.0f - (this.fadeOutTimer[x, y]?.Progress() ?? 0.0f))  * 255
            );
        }

        public void Swap() {
            int x = this.cursorX;
            int y = this.cursorY;

            if (this.isLocked[x, y] || this.isLocked[x + 1, y]) {
                return;
            }

            this.isLocked[x, y] = true;
            this.isLocked[x + 1, y] = true;

            int leftGem = this.board[x, y];
            int rightGem = this.board[x + 1, y];

            this.boardDX[x, y] = +1;
            this.boardDY[x, y] =  0;
            this.boardDX[x + 1, y] = -1;
            this.boardDY[x + 1, y] =  0;

            Timer swapTimer = this.CreateCellTimer(
                x,
                y,
                durationMilliseconds: config.SwapDurationMs,
                onDoneCallback: (x, y) => {
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
        }

        private Timer CreateCellTimer(
            int x,
            int y,
            float durationMilliseconds,
            Action<int, int> onDoneCallback
        ) {
            int X = x, Y = y + this.boardVerticalOffset;
            return TimerManager.AddTimer(
                durationMilliseconds: durationMilliseconds,
                onDoneCallback: () => onDoneCallback(
                    X,
                    Y - this.boardVerticalOffset
                )
            );
        }
    }
}
