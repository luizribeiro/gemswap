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

        public GemSwap()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            Content.Unload();
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0, 0, 0));

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
