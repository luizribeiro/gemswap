using System;

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

        public void Update(float ellapsedMilliseconds) {
            this.offset += (config.GemHeight * ellapsedMilliseconds)
                / config.BoardSpeedRowPermMs;

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
            for (int x = 0; x < config.BoardWidth; x++) {
                int count = 0;
                int currentGem = Board.EMPTY;
                for (int y = 0; y < config.BoardHeight; y++) {
                    if (
                        this.board[x, y] == Board.EMPTY || this.isLocked[x, y]
                    ) {
                        count = 0;
                        currentGem = Board.EMPTY;
                        continue;
                    }

                    if (this.board[x, y] == currentGem) {
                        count++;
                    } else {
                        if (count >= 3) {
                            EliminateVertical(x, y - 1);
                        }
                        count = 1;
                        currentGem = this.board[x, y];
                    }
                }
                if (count >= 3) {
                    EliminateVertical(x, config.BoardHeight - 1);
                }
            }

            for (int y = 0; y < config.BoardHeight; y++) {
                int count = 0;
                int currentGem = Board.EMPTY;
                for (int x = 0; x < config.BoardWidth; x++) {
                    if (
                        this.board[x, y] == Board.EMPTY || this.isLocked[x, y]
                    ) {
                        count = 0;
                        currentGem = Board.EMPTY;
                        continue;
                    }

                    if (this.board[x, y] == currentGem) {
                        count++;
                    } else {
                        if (count >= 3) {
                            EliminateHorizontal(x - 1, y);
                        }
                        count = 1;
                        currentGem = this.board[x, y];
                    }
                }
                if (count >= 3) {
                    EliminateHorizontal(config.BoardWidth - 1, y);
                }
            }
        }

        private void EliminateHorizontal(int xx, int y) {
            for (int x = xx - 1; x >= 0; x--) {
                if (this.board[x, y] != this.board[xx, y]) {
                    break;
                }
                EliminateCell(x, y);
            }
            for (int x = xx + 1; x < config.BoardWidth; x--) {
                if (this.board[x, y] != this.board[xx, y]) {
                    break;
                }
                EliminateCell(x, y);
            }
            EliminateCell(xx, y);
        }

        private void EliminateVertical(int x, int yy) {
            for (int y = yy - 1; y >= 0; y--) {
                if (this.board[x, y] != this.board[x, yy]) {
                    break;
                }
                EliminateCell(x, y);
            }
            for (int y = yy + 1; y < config.BoardHeight; y--) {
                if (this.board[x, y] != this.board[x, yy]) {
                    break;
                }
                EliminateCell(x, y);
            }
            EliminateCell(x, yy);
        }

        private void EliminateCell(int x, int y) {
            int X = x, Y = y;
            this.isLocked[X, Y] = true;
            Timer timer = TimerManager.AddTimer(
                durationMilliseconds: config.SwapDurationMs,
                onDoneCallback: () => {
                    this.board[X, Y] = Board.EMPTY;
                    this.isLocked[X, Y] = false;
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

                int X = x, Y = y;

                // fall x, y into x, y + 1
                this.isLocked[x, y] = true;
                this.isLocked[x, y + 1] = true;
                this.boardDX[x, y] =  0;
                this.boardDY[x, y] = +1;

                Timer fallTimer = TimerManager.AddTimer(
                    durationMilliseconds: config.SwapDurationMs,
                    onDoneCallback: () => {
                        this.board[X, Y] = Board.EMPTY;
                        this.board[X, Y + 1] = gem;

                        this.movimentTimer[X, Y] = null;

                        this.isLocked[X, Y] = false;
                        this.isLocked[X, Y + 1] = false;
                    }
                );

                this.movimentTimer[x, y] = fallTimer;
            }
        }

        private void AddNewRow() {
            this.cursorY--;

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

            Timer swapTimer = TimerManager.AddTimer(
                durationMilliseconds: config.SwapDurationMs,
                onDoneCallback: () => {
                    // FIXME: there is a bug here where x, y may be outdated
                    // (in case a new row is added to the board before setting
                    // the timer and executing the callback)
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
    }
}
