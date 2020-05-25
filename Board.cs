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
        private Timer[,] movimentTimer;
        private Timer[,] fadeOutTimer;
        private int[] upcomingRow;
        private float offset;
        private int cursorX;
        private int cursorY;

        public Board()
        {
            this.board = new int[Constants.BOARD_WIDTH, Constants.BOARD_HEIGHT];
            this.boardDX = new int[Constants.BOARD_WIDTH, Constants.BOARD_HEIGHT];
            this.boardDY = new int[Constants.BOARD_WIDTH, Constants.BOARD_HEIGHT];
            this.isLocked = new bool[
                Constants.BOARD_WIDTH,
                Constants.BOARD_HEIGHT
            ];
            this.movimentTimer = new Timer[
                Constants.BOARD_WIDTH,
                Constants.BOARD_HEIGHT
            ];
            this.fadeOutTimer = new Timer[
                Constants.BOARD_WIDTH,
                Constants.BOARD_HEIGHT
            ];

            this.cursorX = 0;
            this.cursorY = Constants.BOARD_HEIGHT - 1;

            Random random = new Random();
            for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
                int height = random.Next(
                    Constants.BOARD_INITIAL_MIN_HEIGHT,
                    Constants.BOARD_INITIAL_MAX_HEIGHT
                );
                for (int y = 0; y < Constants.BOARD_HEIGHT; y++) {
                    this.board[x, y] = y > Constants.BOARD_HEIGHT - height
                        ? random.Next(0, Constants.NUM_GEMS)
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

        public void Update(float ellapsedMilliseconds) {
            this.offset += (Constants.GEM_HEIGHT * ellapsedMilliseconds)
                / Constants.BOARD_SPEED_ROW_PER_MS;

            if (this.offset >= Constants.GEM_HEIGHT) {
                this.offset = 0.0f;
                this.AddNewRow();
            }

            EliminateContiguous();

            for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
                for (int y = Constants.BOARD_HEIGHT - 1; y >= 0; y--) {
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
            for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
                int count = 0;
                int currentGem = Board.EMPTY;
                for (int y = 0; y < Constants.BOARD_HEIGHT; y++) {
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
                    EliminateVertical(x, Constants.BOARD_HEIGHT - 1);
                }
            }

            for (int y = 0; y < Constants.BOARD_HEIGHT; y++) {
                int count = 0;
                int currentGem = Board.EMPTY;
                for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
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
                    EliminateHorizontal(Constants.BOARD_WIDTH - 1, y);
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
            for (int x = xx + 1; x < Constants.BOARD_WIDTH; x--) {
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
            for (int y = yy + 1; y < Constants.BOARD_HEIGHT; y--) {
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
            Timer timer = new Timer(
                durationMilliseconds: Constants.SWAP_DURATION_MS,
                delayMilliseconds: 0.0f,
                onDoneCallback: () => {
                    this.board[X, Y] = Board.EMPTY;
                    this.isLocked[X, Y] = false;
                    this.fadeOutTimer[x, y] = null;
                }
            );
            this.fadeOutTimer[x, y] = timer;
            TimerManager.AddTimer(timer);
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

                Timer fallTimer = new Timer(
                    durationMilliseconds: Constants.SWAP_DURATION_MS,
                    delayMilliseconds: 0.0f,
                    onDoneCallback: () => {
                        this.board[X, Y] = Board.EMPTY;
                        this.board[X, Y + 1] = gem;

                        this.movimentTimer[X, Y] = null;

                        this.isLocked[X, Y] = false;
                        this.isLocked[X, Y + 1] = false;
                    }
                );
                TimerManager.AddTimer(fallTimer);

                this.movimentTimer[x, y] = fallTimer;
            }
        }

        private void AddNewRow() {
            this.cursorY--;

            for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
                for (int y = 0; y < Constants.BOARD_HEIGHT - 1; y++) {
                    this.board[x, y] = this.board[x, y + 1];
                    this.isLocked[x, y] = this.isLocked[x, y + 1];
                    this.movimentTimer[x, y] = this.movimentTimer[x, y + 1];
                    this.fadeOutTimer[x, y] = this.fadeOutTimer[x, y + 1];
                    this.boardDX[x, y] = this.boardDX[x, y + 1];
                    this.boardDY[x, y] = this.boardDY[x, y + 1];
                }
            }

            for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
                int y = Constants.BOARD_HEIGHT - 1;
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
            int[] upcomingRow = new int[Constants.BOARD_WIDTH];
            Random random = new Random();
            for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
                upcomingRow[x] = random.Next(0, Constants.NUM_GEMS);
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
            if (this.cursorX > Constants.BOARD_WIDTH - 2) {
                this.cursorX = Constants.BOARD_WIDTH - 2;
            }

            if (this.cursorY < 0) {
                this.cursorY = 0;
            }
            if (this.cursorY > Constants.BOARD_HEIGHT - 1) {
                this.cursorY = Constants.BOARD_HEIGHT - 1;
            }
        }

        public float GetCellOffsetX(int x, int y) {
            return (this.movimentTimer[x, y]?.Progress() ?? 0.0f)
                * this.boardDX[x, y] * Constants.GEM_WIDTH;
        }

        public float GetCellOffsetY(int x, int y) {
            return (this.movimentTimer[x, y]?.Progress() ?? 0.0f)
                * this.boardDY[x, y] * Constants.GEM_HEIGHT;
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

            Timer swapTimer = new Timer(
                durationMilliseconds: Constants.SWAP_DURATION_MS,
                delayMilliseconds: 0.0f,
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

            TimerManager.AddTimer(swapTimer);
        }
    }
}
