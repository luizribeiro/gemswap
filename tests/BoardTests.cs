using NUnit.Framework;

namespace gemswap.tests
{
    public class BoardTests
    {
        [Test]
        public void TestBottomRowContainsGems()
        {
            Board board = new Board();
            for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
                Assert.AreNotEqual(
                    board.getCell(x, Constants.BOARD_HEIGHT - 1),
                    Board.EMPTY
                );
            }
        }

        [Test]
        public void TestTopRowIsEmpty()
        {
            Board board = new Board();
            for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
                Assert.AreEqual(board.getCell(x, 0), Board.EMPTY);
            }
        }
    }
}
