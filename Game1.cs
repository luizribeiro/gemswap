using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace mgsb
{
    public class Game1 : Game
    {
        const int SCREEN_WIDTH = 1920;
        const int SCREEN_HEIGHT = 1080;

        const int SPRITE_FPS = 15;
        const int SPRITE_NUM_FRAMES = 14;
        const int SPRITE_NUM_COLUMNS = 8;
        const int SPRITE_WIDTH = 64;
        const int SPRITE_HEIGHT = 64;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D texture;
        int currentFrame;
        float timer;
        bool isJumping;

        public Game1()
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
            currentFrame = 0;
            timer = 0;
            isJumping = false;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            texture = this.Content.Load<Texture2D>("bob-jumping");
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (isJumping) {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (timer >= 1000f/SPRITE_FPS) {
                    currentFrame++;
                    if (currentFrame == SPRITE_NUM_FRAMES) {
                        currentFrame = 0;
                        isJumping = false;
                    }
                    timer = 0f;
                }
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) {
                isJumping = true;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(225, 222, 178));

            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(
                texture,
                position: new Vector2((SCREEN_WIDTH - SPRITE_WIDTH)/2, (SCREEN_HEIGHT - SPRITE_HEIGHT)*2/3),
                sourceRectangle: new Rectangle(
                    (currentFrame % SPRITE_NUM_COLUMNS) * SPRITE_WIDTH,
                    (currentFrame / SPRITE_NUM_COLUMNS) * SPRITE_HEIGHT,
                    SPRITE_WIDTH,
                    SPRITE_HEIGHT
                )
            );
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
