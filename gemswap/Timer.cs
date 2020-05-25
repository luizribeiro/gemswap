using System;

namespace gemswap {
    public class Timer {
        bool isActive;

        float currentTime;
        float delayMilliseconds;
        float durationMilliseconds;

        Action? onDoneCallback;

        public Timer(
            float durationMilliseconds,
            float delayMilliseconds = 0.0f,
            Action? onDoneCallback = null
        ) {
            this.durationMilliseconds = durationMilliseconds;
            this.delayMilliseconds = delayMilliseconds;
            this.onDoneCallback = onDoneCallback;

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
            if (this.onDoneCallback != null) {
                this.onDoneCallback();
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

        public bool IsActive() {
            return this.isActive;
        }
    }
}
