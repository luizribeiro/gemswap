namespace GemSwap
{
    using System;

    public class Timer
    {
        private readonly float delayMilliseconds;
        private readonly float durationMilliseconds;
        private readonly Action? onDoneCallback;

        private bool isActive;
        private float currentTime;

        public Timer(
            float durationMilliseconds,
            float delayMilliseconds = 0.0f,
            Action? onDoneCallback = null
        )
        {
            this.durationMilliseconds = durationMilliseconds;
            this.delayMilliseconds = delayMilliseconds;
            this.onDoneCallback = onDoneCallback;

            this.isActive = true;
            this.currentTime = 0.0f;
        }

        public void Update(float ellapsedMilliseconds)
        {
            if (!this.isActive)
            {
                return;
            }

            this.currentTime += ellapsedMilliseconds;
            if (this.Progress() < 1.0f)
            {
                return;
            }

            this.isActive = false;
            this.onDoneCallback?.Invoke();
        }

        public float Progress()
        {
            if (this.currentTime < this.delayMilliseconds)
            {
                return 0.0f;
            }

            return Math.Min(
                (this.currentTime - this.delayMilliseconds)
                    / this.durationMilliseconds,
                1.0f
            );
        }

        public bool IsActive()
        {
            return this.isActive;
        }
    }
}
