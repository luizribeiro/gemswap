using NUnit.Framework;

namespace gemswap.tests
{
    public class BoardTests
    {
        Board? b;

        [SetUp]
        public void Setup()
        {
            this.b = new Board();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}
