namespace GemSwap
{
    public class Config
    {
        public virtual int BoardWidth => 6;

        public virtual int BoardHeight => 12;

        public virtual int NumRowsLeftForAlarm => 3;

        public virtual int BoardInitialMinHeight => 2;

        public virtual int BoardInitialMaxHeight => 8;

        public virtual float BoardSpeedRowPerMs => 4000.0f;

        public virtual float AnimationGemFadeInMs => 1000.0f;

        public virtual float SwapDurationMs => 100.0f;

        public virtual float EliminationDurationMs => 200.0f;

        public virtual float EliminationScrollCooldownMs => 1000.0f;

        public virtual float FallDurationMs => 50.0f;

        public virtual int GemWidth => 64;

        public virtual int GemHeight => 64;

        public virtual int NumGems => 7;

        public virtual int CursorOffsetPx => 8;

        public int BoardWidthInPixels => this.BoardWidth * this.GemWidth;

        public int BoardHeightInPixels => this.BoardHeight * this.GemHeight;

        public float BoardSpeedPixelsPerMs
            => this.GemHeight / this.BoardSpeedRowPerMs;
    }
}
