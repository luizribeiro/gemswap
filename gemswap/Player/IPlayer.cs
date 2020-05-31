namespace GemSwap.Player
{
    public interface IPlayer
    {
        public Board Board { get; set; }

        public void ProcessInput();
    }
}
