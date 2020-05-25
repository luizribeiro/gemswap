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
        private Timer[,] timerBoard;
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
            this.timerBoard = new Timer[
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
                    this.timerBoard[x, y] = null;
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

        public float GetCellOffsetX(int x, int y) {
            return (this.timerBoard[x, y]?.Progress() ?? 0.0f)
                * this.boardDX[x, y] * Constants.GEM_WIDTH;
        }

        public float GetCellOffsetY(int x, int y) {
            return (this.timerBoard[x, y]?.Progress() ?? 0.0f)
                * this.boardDY[x, y] * Constants.GEM_HEIGHT;
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
                    this.board[x, y] = rightGem;
                    this.board[x + 1, y] = leftGem;

                    this.timerBoard[x, y] = null;
                    this.timerBoard[x + 1, y] = null;

                    this.isLocked[x, y] = false;
                    this.isLocked[x + 1, y] = false;
                }
            );

            this.timerBoard[x, y] = swapTimer;
            this.timerBoard[x + 1, y] = swapTimer;

            TimerManager.AddTimer(swapTimer);
        }
    }
}
