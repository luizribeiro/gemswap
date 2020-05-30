namespace GemSwap
{
    using System;

    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            using GemSwap game = new GemSwap();
            game.Run();
        }
    }
}
