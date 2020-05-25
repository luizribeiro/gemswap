using System;

namespace gemswap {
    public class Timer {
        bool isActive;

        float currentTime;
        float delayMilliseconds;
        float durationMilliseconds;

        Action callback;

        public Timer(
            float durationMilliseconds,
            float delayMilliseconds = 0.0f,
            Action callback = null
        ) {
            this.durationMilliseconds = durationMilliseconds;
            this.delayMilliseconds = delayMilliseconds;
            this.callback = callback;

            this.isActive = true;
            this.currentTime = 0.0f;
        }

        public void Update(float ellapsedMilliseconds) {
            if (!this.isActive) {
                return;
            }

            this.currentTime += ellapsedMilliseconds;
            if (this.currentTime < this.durationMilliseconds) {
                return;
            }

            this.isActive = false;
            if (this.callback != null) {
                this.callback();
            }
        }

        public float Progress() {
            if (this.currentTime < this.delayMilliseconds) {
                return 0.0f;
            }

            return Math.Min(
                (this.currentTime - this.delayMilliseconds)
                    / this.durationMilliseconds,
                1.0f
            );
        }
    }
}
