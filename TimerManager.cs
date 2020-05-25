using System.Collections.Generic;

namespace gemswap {
    public class TimerManager {
        private static List<Timer> timers = new List<Timer>();

        public static void AddTimer(Timer timer) {
            TimerManager.timers.Add(timer);
        }

        public static void Update(float ellapsedMilliseconds) {
            foreach (Timer timer in TimerManager.timers) {
                timer.Update(ellapsedMilliseconds);
            }
        }
    }
}
