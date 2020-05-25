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
    }
}
