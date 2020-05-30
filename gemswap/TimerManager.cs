namespace GemSwap
{
    using System;
    using System.Collections.Generic;

    public class TimerManager
    {
        private static readonly List<Timer> Timers = new List<Timer>();

        public static void AddTimer(Timer timer)
        {
            TimerManager.Timers.Add(timer);
        }

        public static Timer AddTimer(
            float durationMilliseconds,
            float delayMilliseconds = 0.0f,
            Action? onDoneCallback = null
        )
        {
            Timer timer = new Timer(
                durationMilliseconds: durationMilliseconds,
                delayMilliseconds: delayMilliseconds,
                onDoneCallback: onDoneCallback
            );
            TimerManager.AddTimer(timer);
            return timer;
        }

        public static void Update(float ellapsedMilliseconds)
        {
            foreach (Timer timer in TimerManager.Timers)
            {
                timer.Update(ellapsedMilliseconds);
            }

            TimerManager.Timers.RemoveAll(t => !t.IsActive());
        }

        public static int GetNumTimers()
        {
            return TimerManager.Timers.Count;
        }

        public static void ClearAll()
        {
            TimerManager.Timers.Clear();
        }
    }
}
