using NUnit.Framework;

namespace GemSwap.Tests
{
    public class TimerManagerTests
    {
        [TearDown]
        public void TearDownTest() {
            TimerManager.ClearAll();
        }

        [Test]
        public void TestTimerSetup() {
            Assert.AreEqual(0, TimerManager.GetNumTimers());
            Timer t = TimerManager.AddTimer(durationMilliseconds: 1000.0f);
            Assert.IsTrue(t.IsActive());
            Assert.AreEqual(1, TimerManager.GetNumTimers());
        }

        [Test]
        public void TestTimerCallback() {
            bool hasCallbackBeenCalled = false;
            Timer t = TimerManager.AddTimer(
                durationMilliseconds: 1000.0f,
                onDoneCallback: () => { hasCallbackBeenCalled = true; }
            );

            Assert.IsFalse(hasCallbackBeenCalled);
            TimerManager.Update(500.0f);
            Assert.IsFalse(hasCallbackBeenCalled);
            TimerManager.Update(500.0f);
            Assert.IsTrue(hasCallbackBeenCalled);
        }

        [Test]
        public void TestTimerCallbackWithDelay() {
            bool hasCallbackBeenCalled = false;
            Timer t = TimerManager.AddTimer(
                durationMilliseconds: 1000.0f,
                delayMilliseconds: 500.0f,
                onDoneCallback: () => { hasCallbackBeenCalled = true; }
            );

            Assert.IsFalse(hasCallbackBeenCalled);
            TimerManager.Update(500.0f);
            Assert.IsFalse(hasCallbackBeenCalled);
            TimerManager.Update(500.0f);
            Assert.IsFalse(hasCallbackBeenCalled);
            TimerManager.Update(500.0f);
            Assert.IsTrue(hasCallbackBeenCalled);
        }

        [Test]
        public void TestProgress() {
            Timer t = TimerManager.AddTimer(
                durationMilliseconds: 1000.0f,
                delayMilliseconds: 1000.0f
            );

            Assert.AreEqual(0.0f, t.Progress());
            TimerManager.Update(500.0f);
            Assert.AreEqual(0.0f, t.Progress());
            TimerManager.Update(500.0f);
            Assert.AreEqual(0.0f, t.Progress());
            TimerManager.Update(500.0f);
            Assert.AreEqual(0.5f, t.Progress());
            TimerManager.Update(500.0f);
            Assert.AreEqual(1.00f, t.Progress());
        }

        [Test]
        public void TestCleanupOfInactiveTimers() {
            TimerManager.AddTimer(durationMilliseconds: 1000.0f);
            TimerManager.AddTimer(durationMilliseconds: 50.0f);
            Assert.AreEqual(2, TimerManager.GetNumTimers());

            TimerManager.Update(50.0f);
            Assert.AreEqual(1, TimerManager.GetNumTimers());

            TimerManager.Update(50.0f);
            Assert.AreEqual(1, TimerManager.GetNumTimers());

            TimerManager.Update(899.0f);
            Assert.AreEqual(1, TimerManager.GetNumTimers());

            TimerManager.Update(1.0f);
            Assert.AreEqual(0, TimerManager.GetNumTimers());
        }
    }
}
