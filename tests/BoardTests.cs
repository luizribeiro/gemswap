using NUnit.Framework;

namespace gemswap.tests
{
    public class BoardTests
    {
        [Test]
        public void TestBottomRowContainsGems()
        {
            Config config = new Config();
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
            Config config = new Config();
            Board board = new Board(config);
            for (int x = 0; x < config.BoardWidth; x++) {
                Assert.AreEqual(board.getCell(x, 0), Board.EMPTY);
            }
        }
    }
}
