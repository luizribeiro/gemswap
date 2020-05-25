using Moq;
using NUnit.Framework;

namespace gemswap.tests
{
    public class BoardTests
    {
        private Config SetupConfig(
            int boardWidth = 0,
            int boardHeight = 0
        ) {
            Mock<Config> mock = new Mock<Config> { CallBase = true };
            if (boardWidth != 0) {
                mock.Setup(c => c.BoardWidth).Returns(boardWidth);
            }
            if (boardHeight != 0) {
                mock.Setup(c => c.BoardHeight).Returns(boardHeight);
            }
            return mock.Object;
        }

        private Board SetupBoard(
            Config config,
            int[,] board,
            int[]? upcomingRow = null
        ) {
            int width = board.GetLength(1);
            int height = board.GetLength(0);
            Assert.AreEqual(config.BoardHeight, height);
            Assert.AreEqual(config.BoardWidth, width);

            int[,] rotatedBoard = new int[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    rotatedBoard[x, y] = board[y, x] - 1;
                }
            }

            if (upcomingRow == null) {
                return new Board(config, rotatedBoard);
            }

            int[] newUpcomingRow = new int[width];
            for (int x = 0; x < width; x++) {
                newUpcomingRow[x] = upcomingRow[x] - 1;
            }

            return new Board(config, rotatedBoard, newUpcomingRow);
        }

        private void Update(Board board, float ellapsedMilliseconds) {
            TimerManager.Update(ellapsedMilliseconds);
            board.Update(ellapsedMilliseconds);
        }

        private void AssertBoard(int[,] expectedBoard, Board board) {
            int width = expectedBoard.GetLength(1);
            int height = expectedBoard.GetLength(0);
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    Assert.AreEqual(expectedBoard[y, x] - 1, board.getCell(x, y));
                }
            }
        }

        [Test]
        public void TestBottomRowContainsGems()
        {
            Config config = this.SetupConfig();
            Board board = new Board(config);
            for (int x = 0; x < config.BoardWidth; x++) {
                Assert.AreNotEqual(
                    board.getCell(x, config.BoardHeight - 1),
                    Board.EMPTY
                );
            }
        }

        [Test]
        public void TestTopRowIsEmpty()
        {
            Config config = this.SetupConfig();
            Board board = new Board(config);
            for (int x = 0; x < config.BoardWidth; x++) {
                Assert.AreEqual(board.getCell(x, 0), Board.EMPTY);
            }
        }

        [Test]
        public void TestSwapping()
        {
            Config config = this.SetupConfig(boardWidth: 2, boardHeight: 1);
            Board board = this.SetupBoard(config, new[,] {{1, 2}});

            AssertBoard(new int[,] {{ 1, 2 }}, board);

            board.Swap();
            this.Update(board, config.SwapDurationMs);

            AssertBoard(new int[,] {{ 2, 1 }}, board);
        }

        static object[] TestCasesForEliminatingGems =
        {
            new object[] {
                new int[,] {{ 1,  1,  1}},
                new int[,] {{ 0,  0,  0}},
            },
            new object[] {
                new int[,] {{ 1,  2,  3}},
                new int[,] {{ 1,  2,  3}},
            },
            new object[] {
                new int[,] {
                    { 1 },
                    { 1 },
                    { 1 },
                },
                new int[,] {
                    { 0 },
                    { 0 },
                    { 0 },
                },
            },
            new object[] {
                new int[,] {
                    { 1, 0, 0 },
                    { 1, 0, 0 },
                    { 1, 1, 1 },
                },
                new int[,] {
                    { 0, 0, 0 },
                    { 0, 0, 0 },
                    { 0, 0, 0 },
                },
            },
        };

        [Test, TestCaseSource("TestCasesForEliminatingGems")]
        public void TestEliminatingGems(int[,] initialBoard, int[,] finalBoard)
        {
            Assert.AreEqual(initialBoard.GetLength(1), finalBoard.GetLength(1));
            Assert.AreEqual(initialBoard.GetLength(0), finalBoard.GetLength(0));

            Config config = this.SetupConfig(
                boardWidth: initialBoard.GetLength(1),
                boardHeight: initialBoard.GetLength(0)
            );
            Board board = this.SetupBoard(config, initialBoard);

            AssertBoard(initialBoard, board);

            // trigger the elimination
            this.Update(board, 0);
            // wait for it to finish
            this.Update(board, config.EliminationDurationMs);

            AssertBoard(finalBoard, board);
        }

        [Test]
        public void TestFallingSingleGem() {
            Config config = this.SetupConfig(boardWidth: 1, boardHeight: 2);
            Board board = this.SetupBoard(config, new[,] {{1}, {0}});

            // trigger the fall
            this.Update(board, 0);

            // check on it midway through
            this.Update(board, config.FallDurationMs / 2.0f);
            AssertBoard(new[,] {{1}, {0}}, board);
            Assert.AreEqual(
                config.GemHeight / 2.0f,
                board.GetCellOffsetY(0, 0)
            );

            // wait for it to finish
            this.Update(board, config.FallDurationMs / 2.0f);
            AssertBoard(new[,] {{0}, {1}}, board);
        }

        [Test]
        public void TestScrollingUp() {
            Config config = this.SetupConfig(boardWidth: 2, boardHeight: 2);
            Board board = this.SetupBoard(
                config,
                board: new[,] {
                    {0, 0},
                    {1, 2},
                },
                upcomingRow: new[] {3, 4}
            );
            Assert.AreEqual(board.getCursorX(), 0);
            Assert.AreEqual(board.getCursorY(), 1);

            this.Update(board, config.BoardSpeedRowPerMs);
            AssertBoard(new[,] {
                {1, 2},
                {3, 4},
            }, board);
            Assert.AreEqual(board.getCursorX(), 0);
            Assert.AreEqual(board.getCursorY(), 0);
        }

        [Test]
        public void TestSwapImmediatelyBeforeAppendingUpcomingRow() {
            Config config = this.SetupConfig(boardWidth: 2, boardHeight: 2);
            Board board = this.SetupBoard(
                config,
                board: new[,] {
                    {0, 0},
                    {1, 2},
                },
                upcomingRow: new[] {3, 4}
            );
            Assert.AreEqual(board.getCursorX(), 0);
            Assert.AreEqual(board.getCursorY(), 1);

            this.Update(board, config.BoardSpeedRowPerMs - 1);
            board.Swap();
            AssertBoard(new[,] {
                {0, 0},
                {1, 2},
            }, board);
            Assert.AreEqual(board.getCursorX(), 0);
            Assert.AreEqual(board.getCursorY(), 1);

            this.Update(board, 1);
            AssertBoard(new[,] {
                {1, 2},
                {3, 4},
            }, board);
            Assert.AreEqual(board.getCursorX(), 0);
            Assert.AreEqual(board.getCursorY(), 0);

            this.Update(board, config.SwapDurationMs);
            AssertBoard(new[,] {
                {2, 1},
                {3, 4},
            }, board);
            Assert.AreEqual(board.getCursorX(), 0);
            Assert.AreEqual(board.getCursorY(), 0);
        }

        [Test]
        public void TestNoEliminationWhenFallingThrough() {
            Config config = this.SetupConfig(boardWidth: 3, boardHeight: 3);
            Board board = this.SetupBoard(
                config,
                board: new[,] {
                    {2, 1, 2},
                    {1, 0, 1},
                    {2, 0, 2},
                }
            );

            // trigger fall
            this.Update(board, 0);

            // make piece fall to the next position
            this.Update(board, config.FallDurationMs);
            AssertBoard(new[,] {
                {2, 0, 2},
                {1, 1, 1},
                {2, 0, 2},
            }, board);

            // fall again
            this.Update(board, 0);
            // wait for elimination since this reproduces the bug
            this.Update(board, config.EliminationDurationMs);
            AssertBoard(new[,] {
                {2, 0, 2},
                {1, 0, 1},
                {2, 1, 2},
            }, board);
        }

        [Test]
        public void TestFallingAnimation() {
            Config config = this.SetupConfig(boardWidth: 1, boardHeight: 4);
            Board board = this.SetupBoard(
                config,
                board: new[,] {
                    {2},
                    {1},
                    {1},
                    {1},
                }
            );

            this.Update(board, 0);
            this.Update(board, config.EliminationDurationMs);
            AssertBoard(new[,] {
                {2},
                {0},
                {0},
                {0},
            }, board);

            // half-way through the fall
            this.Update(board, 0);
            this.Update(board, config.FallDurationMs / 2.0f);
            AssertBoard(new[,] {
                {2},
                {0},
                {0},
                {0},
            }, board);
            Assert.AreEqual(1, board.getCell(0, 0));
            Assert.AreEqual(
                config.GemHeight * 0.5f,
                board.GetCellOffsetY(0, 0)
            );

            // once done falling
            this.Update(board, 0);
            this.Update(board, config.FallDurationMs / 2.0f);
            AssertBoard(new[,] {
                {0},
                {2},
                {0},
                {0},
            }, board);
            Assert.AreEqual(1, board.getCell(0, 1));
            Assert.AreEqual(0.0f, board.GetCellOffsetY(0, 1));
        }
    }
}
