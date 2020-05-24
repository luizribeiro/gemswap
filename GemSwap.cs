using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gemswap
{
    public class GemSwap : Game
    {
        const int SCREEN_WIDTH = 1920;
        const int SCREEN_HEIGHT = 1080;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D background;
        Board board;
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

            this.board = new Board();
            this.boardRenderer = new BoardRenderer(GraphicsDevice);
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
            GamePadState gamePadState1 = GamePad.GetState(PlayerIndex.One);
            if (
                gamePadState1.Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape)
            ) {
                Exit();
            }

            float ellapsedMilliseconds =
                (float)gameTime.ElapsedGameTime.Milliseconds;

            this.board.Update(ellapsedMilliseconds);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(new Color(0, 0, 0));

            this.spriteBatch.Begin(samplerState: SamplerState.PointClamp);
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
