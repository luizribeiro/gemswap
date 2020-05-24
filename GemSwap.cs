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
        Texture2D gemTexture;

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
            background = this.Content.Load<Texture2D>("background");
            gemTexture = this.Content.Load<Texture2D>("gems");
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
            spriteBatch.Draw(
                background,
                new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT),
                Color.White
            );
            for (int x = 0; x < 10; x++) {
                for (int y = 0; y < 10; y++) {
                    spriteBatch.Draw(
                        this.gemTexture,
                        position: new Vector2(x*64, y*64),
                        sourceRectangle: new Rectangle(
                            (x % Constants.NUM_GEMS) * Constants.GEM_WIDTH,
                            0,
                            Constants.GEM_WIDTH,
                            Constants.GEM_HEIGHT
                        ),
                        color: Color.White
                    );
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
