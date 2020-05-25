using System;
using System.Collections.Generic;

namespace gemswap {
    public class TimerManager {
        private static List<Timer> timers = new List<Timer>();

        public static void AddTimer(Timer timer) {
            TimerManager.timers.Add(timer);
        }

        public static Timer AddTimer(
            float durationMilliseconds,
            float delayMilliseconds = 0.0f,
            Action? onDoneCallback = null
        ) {
            Timer timer = new Timer(
                durationMilliseconds: durationMilliseconds,
                delayMilliseconds: delayMilliseconds,
                onDoneCallback: onDoneCallback
            );
            TimerManager.AddTimer(timer);
            return timer;
        }

        public static void Update(float ellapsedMilliseconds) {
            foreach (Timer timer in TimerManager.timers) {
                timer.Update(ellapsedMilliseconds);
            }
        }
    }
}
