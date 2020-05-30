namespace GemSwap.Tests
{
    using NUnit.Framework;

    public class TimerManagerTests
    {
        [TearDown]
        public void TearDownTest()
        {
            TimerManager.ClearAll();
        }

        [Test]
        public void TestTimerSetup()
        {
            Assert.That(TimerManager.GetNumTimers(), Is.EqualTo(0));
            Timer t = TimerManager.AddTimer(durationMilliseconds: 1000.0f);
            Assert.That(t.IsActive(), Is.True);
            Assert.That(TimerManager.GetNumTimers(), Is.EqualTo(1));
        }

        [Test]
        public void TestTimerCallback()
        {
            bool hasCallbackBeenCalled = false;
            Timer t = TimerManager.AddTimer(
                durationMilliseconds: 1000.0f,
                onDoneCallback: () => { hasCallbackBeenCalled = true; }
            );

            Assert.That(hasCallbackBeenCalled, Is.False);
            TimerManager.Update(500.0f);
            Assert.That(hasCallbackBeenCalled, Is.False);
            TimerManager.Update(500.0f);
            Assert.That(hasCallbackBeenCalled, Is.True);
        }

        [Test]
        public void TestTimerCallbackWithDelay()
        {
            bool hasCallbackBeenCalled = false;
            Timer t = TimerManager.AddTimer(
                durationMilliseconds: 1000.0f,
                delayMilliseconds: 500.0f,
                onDoneCallback: () => { hasCallbackBeenCalled = true; }
            );

            Assert.That(hasCallbackBeenCalled, Is.False);
            TimerManager.Update(500.0f);
            Assert.That(hasCallbackBeenCalled, Is.False);
            TimerManager.Update(500.0f);
            Assert.That(hasCallbackBeenCalled, Is.False);
            TimerManager.Update(500.0f);
            Assert.That(hasCallbackBeenCalled, Is.True);
        }

        [Test]
        public void TestProgress()
        {
            Timer t = TimerManager.AddTimer(
                durationMilliseconds: 1000.0f,
                delayMilliseconds: 1000.0f
            );

            Assert.That(t.Progress(), Is.EqualTo(0.0f));
            TimerManager.Update(500.0f);
            Assert.That(t.Progress(), Is.EqualTo(0.0f));
            TimerManager.Update(500.0f);
            Assert.That(t.Progress(), Is.EqualTo(0.0f));
            TimerManager.Update(500.0f);
            Assert.That(t.Progress(), Is.EqualTo(0.5f));
            TimerManager.Update(500.0f);
            Assert.That(t.Progress(), Is.EqualTo(1.0f));
        }

        [Test]
        public void TestCleanupOfInactiveTimers()
        {
            Assert.That(TimerManager.GetNumTimers(), Is.EqualTo(0));
            TimerManager.AddTimer(durationMilliseconds: 1000.0f);
            Assert.That(TimerManager.GetNumTimers(), Is.EqualTo(1));
            TimerManager.AddTimer(durationMilliseconds: 50.0f);
            Assert.That(TimerManager.GetNumTimers(), Is.EqualTo(2));

            TimerManager.Update(50.0f);
            Assert.That(TimerManager.GetNumTimers(), Is.EqualTo(1));

            TimerManager.Update(50.0f);
            Assert.That(TimerManager.GetNumTimers(), Is.EqualTo(1));

            TimerManager.Update(899.0f);
            Assert.That(TimerManager.GetNumTimers(), Is.EqualTo(1));

            TimerManager.Update(1.0f);
            Assert.That(TimerManager.GetNumTimers(), Is.EqualTo(0));
        }
    }
}
