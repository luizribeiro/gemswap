namespace GemSwap
{
    using System.Collections.Generic;
    using GemSwap.Match;
    using GemSwap.Player;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class GemSwapGame : Game
    {
        private const int ScreenWidth = 1920;
        private const int ScreenHeight = 1080;

        private readonly GraphicsDeviceManager graphics;
        private readonly VersusMatch match;

        public GemSwapGame()
        {
            this.graphics = new GraphicsDeviceManager(this)
            {
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8,
            };
            this.graphics.PreferredBackBufferWidth = GemSwapGame.ScreenWidth;
            this.graphics.PreferredBackBufferHeight = GemSwapGame.ScreenHeight;
            this.graphics.ApplyChanges();
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
            this.match = new VersusMatch(
                this.GraphicsDevice,
                new List<IPlayer> {
                    new GamePadPlayer(PlayerIndex.One),
                    new KeyboardPlayer(),
                }
            );
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.match.LoadContent(this.Content);
        }

        protected override void UnloadContent()
        {
            this.Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            float ellapsedMilliseconds = gameTime.ElapsedGameTime.Milliseconds;
            this.match.Update(ellapsedMilliseconds);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.match.Draw();
            base.Draw(gameTime);
        }
    }
}
