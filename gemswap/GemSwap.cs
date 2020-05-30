namespace GemSwap
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class GemSwap : Game
    {
        private const int ScreenWidth = 1920;
        private const int ScreenHeight = 1080;

        private readonly GraphicsDeviceManager graphics;
        private readonly List<Board> boards;
        private readonly List<BoardRenderer> boardRenderers;
        private readonly List<IPlayer> players;

        private SpriteBatch? spriteBatch;
        private Texture2D? background;

        public GemSwap()
        {
            this.graphics = new GraphicsDeviceManager(this)
            {
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8,
            };
            this.graphics.PreferredBackBufferWidth = GemSwap.ScreenWidth;
            this.graphics.PreferredBackBufferHeight = GemSwap.ScreenHeight;
            this.graphics.ApplyChanges();
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;

            Config config = new Config();

            this.boardRenderers = new List<BoardRenderer>();
            this.boards = new List<Board>();
            this.players = new List<IPlayer>();

            int numPlayers = 2;
            int playerIndex = 0;

            this.boardRenderers.Add(new BoardRenderer(
                config,
                this.GraphicsDevice,
                position: new Vector2(
                    (GemSwap.ScreenWidth - config.BoardWidthInPixels * numPlayers)
                        / (numPlayers + 1.0f) * (playerIndex + 1)
                        + playerIndex * config.BoardWidthInPixels,
                    (GemSwap.ScreenHeight - config.BoardHeightInPixels) / 2.0f
                )
            ));
            Board board = new Board(config);
            this.boards.Add(board);
            this.players.Add(new KeyboardPlayer(board));

            playerIndex = 1;
            this.boardRenderers.Add(new BoardRenderer(
                config,
                this.GraphicsDevice,
                position: new Vector2(
                    (GemSwap.ScreenWidth - config.BoardWidthInPixels * numPlayers)
                        / (numPlayers + 1.0f) * (playerIndex + 1)
                        + playerIndex * config.BoardWidthInPixels,
                    (GemSwap.ScreenHeight - config.BoardHeightInPixels) / 2.0f
                )
            ));
            board = new Board(config);
            this.boards.Add(board);
            this.players.Add(new GamePadPlayer(board, PlayerIndex.One));
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.background = this.Content.Load<Texture2D>("background");

            for (int i = 0; i < this.boardRenderers.Count; i++)
            {
                this.boardRenderers[i].LoadContent(this.Content);
            }
        }

        protected override void UnloadContent()
        {
            this.Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            float ellapsedMilliseconds = gameTime.ElapsedGameTime.Milliseconds;

            TimerManager.Update(ellapsedMilliseconds);

            for (int i = 0; i < this.boards.Count; i++)
            {
                this.boards[i].Update(ellapsedMilliseconds);
            }

            for (int i = 0; i < this.players.Count; i++)
            {
                this.players[i].ProcessInput();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(new Color(0, 0, 0));

            this.spriteBatch!.Begin(samplerState: SamplerState.PointClamp);
            this.spriteBatch.Draw(
                this.background,
                new Rectangle(0, 0, GemSwap.ScreenWidth, GemSwap.ScreenHeight),
                Color.White
            );
            this.spriteBatch.End();

            for (int i = 0; i < this.boardRenderers.Count; i++)
            {
                this.boardRenderers[i].Draw(this.boards[i]);
            }

            base.Draw(gameTime);
        }
    }
}
