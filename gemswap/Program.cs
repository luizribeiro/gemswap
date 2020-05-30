namespace GemSwap
{
    using System;

    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using GemSwapGame game = new GemSwapGame();
            game.Run();
        }
    }
}
