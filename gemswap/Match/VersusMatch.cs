namespace GemSwap.Match
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GemSwap.Player;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Media;

    public class VersusMatch : Match
    {
        private readonly List<Board> boards;
        private readonly List<BoardRenderer> boardRenderers;
        private readonly List<IPlayer> players;

        private Texture2D? background;

        private SoundEffect? swapSoundEffect;
        private SoundEffect? eliminationSoundEffect;
        private SoundEffectInstance? alarmSoundEffect;

        public VersusMatch(
            GraphicsDevice graphicsDevice,
            List<IPlayer> players
        )
            : base(graphicsDevice)
        {
            this.boardRenderers = new List<BoardRenderer>();
            this.boards = new List<Board>();
            this.players = players;

            int playerIndex = 0;
            int numPlayers = players.Count;
            foreach (IPlayer player in players)
            {
                Board board = new Board(this.Config);
                player.Board = board;

                this.boards.Add(board);
                this.boardRenderers.Add(new BoardRenderer(
                    this.Config,
                    this.GraphicsDevice,
                    position: new Vector2(
                        (this.ScreenWidth - this.Config.BoardWidthInPixels * numPlayers)
                            / (numPlayers + 1.0f) * (playerIndex + 1)
                            + playerIndex * this.Config.BoardWidthInPixels,
                        (this.ScreenHeight - this.Config.BoardHeightInPixels) / 2.0f
                    )
                ));

                playerIndex++;
            }
        }

        public new void LoadContent(ContentManager contentManager)
        {
            this.background = contentManager.Load<Texture2D>("background");
            this.swapSoundEffect = contentManager.Load<SoundEffect>("swap");
            this.eliminationSoundEffect =
                contentManager.Load<SoundEffect>("eliminate");

            SoundEffect alarmSoundEffect =
                contentManager.Load<SoundEffect>("alarm");
            this.alarmSoundEffect = alarmSoundEffect.CreateInstance();
            this.alarmSoundEffect.IsLooped = true;
            this.alarmSoundEffect.Play();

            Song music = this.LoadRandomMusic(contentManager);
            MediaPlayer.Volume = 1.0f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(music);

            foreach (Board board in this.boards)
            {
                board.When(
                    BoardEvent.Elimination,
                    () => this.eliminationSoundEffect.Play()
                );
                board.When(
                    BoardEvent.Swap,
                    () => this.swapSoundEffect.Play()
                );
            }

            for (int i = 0; i < this.boardRenderers.Count; i++)
            {
                this.boardRenderers[i].LoadContent(contentManager);
            }

            base.LoadContent(contentManager);
        }

        public override void Update(float ellapsedMilliseconds)
        {
            TimerManager.Update(ellapsedMilliseconds);

            for (int i = 0; i < this.boards.Count; i++)
            {
                this.boards[i].Update(ellapsedMilliseconds);
            }

            for (int i = 0; i < this.players.Count; i++)
            {
                this.players[i].ProcessInput();
            }

            if (this.boards.Any(b => !b.HasGameEnded && b.IsCloseToLosing))
            {
                this.alarmSoundEffect!.Play();
            }
            else
            {
                this.alarmSoundEffect!.Stop();
            }
        }

        public override void Draw()
        {
            this.GraphicsDevice.Clear(new Color(0, 0, 0));

            this.SpriteBatch!.Begin(samplerState: SamplerState.PointClamp);
            this.SpriteBatch.Draw(
                this.background,
                new Rectangle(0, 0, this.ScreenWidth, this.ScreenHeight),
                Color.White
            );
            this.SpriteBatch.End();

            for (int i = 0; i < this.boardRenderers.Count; i++)
            {
                this.boardRenderers[i].Draw(this.boards[i]);
            }
        }

        private Song LoadRandomMusic(ContentManager contentManager)
        {
            string[] songs = new string[] {
                "Music/DealWithIt",
                "Music/EnjoyIt",
                "Music/Jewels",
                "Music/Puzzling",
            };
            string pickedSong = songs[new Random().Next(songs.Length)];
            return contentManager.Load<Song>(pickedSong);
        }
    }
}
