using System;

namespace gemswap
{
    public class Board
    {
        public const int EMPTY = -1;

        private int[,] board;
        private float offset;

        public Board()
        {
            this.board = new int[Constants.BOARD_WIDTH, Constants.BOARD_HEIGHT];

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
            for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
                for (int y = 0; y < Constants.BOARD_HEIGHT - 1; y++) {
                    this.board[x, y] = this.board[x, y + 1];
                }
            }

            Random random = new Random();
            for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
                this.board[x, Constants.BOARD_HEIGHT - 1] =
                    random.Next(0, Constants.NUM_GEMS);
            }
        }

        public int getCell(int x, int y) {
            return this.board[x, y];
        }

        public float getOffset() {
            return this.offset;
        }
    }
}
