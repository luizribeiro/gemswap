namespace gemswap
{
    public class Config
    {
        public virtual int BoardWidth { get => 6; }
        public virtual int BoardHeight { get => 14; }

        public virtual int BoardInitialMinHeight { get => 2; }
        public virtual int BoardInitialMaxHeight { get => 8; }

        public virtual float BoardSpeedRowPerMs { get => 4000.0f; }

        public virtual float AnimationGemFadeInMs { get => 1000.0f; }
        public virtual float SwapDurationMs { get => 100.0f; }
        public virtual float EliminationDurationMs { get => 200.0f; }
        public virtual float FallDurationMs { get => 50.0f; }

        public virtual int GemWidth { get => 64; }
        public virtual int GemHeight { get => 64; }
        public virtual int NumGems { get => 7; }

        public virtual int CursorOffsetPx { get => 8; }
    }
}
