﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gemswap
{
    public class GemSwap : Game
    {
        const int SCREEN_WIDTH = 1920;
        const int SCREEN_HEIGHT = 1080;

        GraphicsDeviceManager graphics;
        SpriteBatch? spriteBatch;
        Texture2D? background;
        Board board;
        GamePadPlayer player;
        BoardRenderer boardRenderer;

        public GemSwap()
        {
            this.graphics = new GraphicsDeviceManager(this) {
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
            };
            this.graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            this.graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            this.graphics.ApplyChanges();
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;

            Config config = new Config();
            this.board = new Board(config);
            this.boardRenderer = new BoardRenderer(
                config,
                GraphicsDevice,
                position: new Vector2(
                    (SCREEN_WIDTH - config.BoardWidthInPixels) / 2.0f,
                    (SCREEN_HEIGHT - config.BoardHeightInPixels) / 2.0f
                )
            );
            this.player = new GamePadPlayer(this.board, PlayerIndex.One);
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            this.background = this.Content.Load<Texture2D>("background");
            this.boardRenderer.LoadContent(this.Content);
        }

        protected override void UnloadContent()
        {
            this.Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {

            float ellapsedMilliseconds =
                (float)gameTime.ElapsedGameTime.Milliseconds;

            TimerManager.Update(ellapsedMilliseconds);

            this.board.Update(ellapsedMilliseconds);

            this.player.ProcessInput();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(new Color(0, 0, 0));

            this.spriteBatch!.Begin(samplerState: SamplerState.PointClamp);
            this.spriteBatch.Draw(
                this.background,
                new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT),
                Color.White
            );
            this.spriteBatch.End();

            this.boardRenderer.Draw(board);

            base.Draw(gameTime);
        }
    }
}
