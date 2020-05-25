using System;

namespace gemswap
{
    public class Board
    {
        public const int EMPTY = -1;

        private int[,] board;
        private int[] upcomingRow;
        private float offset;
        private int cursorX;
        private int cursorY;
        private int swapCursorX;
        private int swapCursorY;

        private Timer swapTimer;

        public Board()
        {
            this.board = new int[Constants.BOARD_WIDTH, Constants.BOARD_HEIGHT];

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
        }

        private void AddNewRow() {
            this.cursorY--;

            for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
                for (int y = 0; y < Constants.BOARD_HEIGHT - 1; y++) {
                    this.board[x, y] = this.board[x, y + 1];
                }
            }

            for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
                this.board[x, Constants.BOARD_HEIGHT - 1] = this.upcomingRow[x];
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

        private bool IsSwapping() {
            return this.swapTimer != null && this.swapTimer.IsActive();
        }

        public float? GetSwapProgress() {
            if (!this.IsSwapping()) {
                return null;
            }
            return this.swapTimer.Progress();
        }

        public int? GetSwapCursorX() {
            if (!this.IsSwapping()) {
                return null;
            }
            return this.swapCursorX;
        }

        public int? GetSwapCursorY() {
            if (!this.IsSwapping()) {
                return null;
            }
            return this.swapCursorY;
        }

        public void Swap() {
            if (this.IsSwapping()) {
                return;
            }

            int x = this.swapCursorX = cursorX;
            int y = this.swapCursorY = cursorY;

            this.swapTimer = new Timer(
                durationMilliseconds: Constants.SWAP_DURATION_MS,
                delayMilliseconds: 0.0f,
                callback: () => {
                    int temp = this.board[x, y];
                    this.board[x, y] = this.board[x + 1, y];
                    this.board[x + 1, y] = temp;
                }
            );

            TimerManager.AddTimer(this.swapTimer);
        }
    }
}
