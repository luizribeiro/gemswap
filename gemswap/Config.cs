namespace gemswap
{
    public class Config
    {
        public int BoardWidth { get => 6; }
        public int BoardHeight { get => 14; }

        public int BoardInitialMinHeight { get => 2; }
        public int BoardInitialMaxHeight { get => 8; }

        public float BoardSpeedRowPermMs { get => 10000.0f; }

        public float AnimationGemFadeInMs { get => 1000.0f; }
        public float SwapDurationMs { get => 100.0f; }

        public int GemWidth { get => 64; }
        public int GemHeight { get => 64; }
        public int NumGems { get => 7; }

        public int CursorOffsetPx { get => 8; }
    }
}
