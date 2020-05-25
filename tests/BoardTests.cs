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

        private Board SetupBoard(Config config, int[,] board) {
            int width = board.Length;
            int height = board.GetLength(0);
            Assert.AreEqual(config.BoardHeight, height);
            Assert.AreEqual(config.BoardWidth, width);

            int[,] rotatedBoard = new int[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    rotatedBoard[x, y] = board[y, x];
                }
            }

            return new Board(config, rotatedBoard);
        }

        private void Update(Board board, float ellapsedMilliseconds) {
            TimerManager.Update(ellapsedMilliseconds);
            board.Update(ellapsedMilliseconds);
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

            Assert.AreEqual(1, board.getCell(0, 0));
            Assert.AreEqual(2, board.getCell(1, 0));

            board.Swap();
            this.Update(board, config.SwapDurationMs);

            Assert.AreEqual(2, board.getCell(0, 0));
            Assert.AreEqual(1, board.getCell(1, 0));
        }

        [Test]
        public void TestEliminatingGems()
        {
            Config config = this.SetupConfig(boardWidth: 3, boardHeight: 1);
            Board board = this.SetupBoard(config, new[,] {{1, 1, 1}});

            Assert.AreEqual(1, board.getCell(0, 0));
            Assert.AreEqual(1, board.getCell(1, 0));
            Assert.AreEqual(1, board.getCell(2, 0));

            // trigger the elimination
            this.Update(board, 0);
            // wait for it to finish
            this.Update(board, config.SwapDurationMs);

            Assert.AreEqual(-1, board.getCell(0, 0));
            Assert.AreEqual(-1, board.getCell(1, 0));
            Assert.AreEqual(-1, board.getCell(2, 0));
        }
    }
}
