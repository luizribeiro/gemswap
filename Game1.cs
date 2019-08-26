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

        const int TILESET_NUM_COLUMNS = 4;
        const int TILESET_WIDTH = 64;
        const int TILESET_HEIGHT = 64;

        private static int[,] map = {
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        };

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D bobSprite;
        Texture2D tileset;
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
            bobSprite = this.Content.Load<Texture2D>("bob-jumping");
            tileset = this.Content.Load<Texture2D>("Tileset");
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
            DrawMap();
            DrawPlayer();
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void DrawPlayer() {
            spriteBatch.Draw(
                bobSprite,
                position: new Vector2((SCREEN_WIDTH - SPRITE_WIDTH)/2, 11 * SPRITE_HEIGHT),
                sourceRectangle: new Rectangle(
                    (currentFrame % SPRITE_NUM_COLUMNS) * SPRITE_WIDTH,
                    (currentFrame / SPRITE_NUM_COLUMNS) * SPRITE_HEIGHT,
                    SPRITE_WIDTH,
                    SPRITE_HEIGHT
                )
            );
        }

        protected void DrawMap()
        {
            for (int y = 0; y < map.GetLength(0); y++) {
                for (int x = 0; x < map.GetLength(1); x++) {
                    int tileIndex = GetTileIndex(x, y);
                    spriteBatch.Draw(
                        tileset,
                        position: new Vector2(x * TILESET_WIDTH, y * TILESET_HEIGHT),
                        sourceRectangle: new Rectangle(
                            (tileIndex % TILESET_NUM_COLUMNS) * TILESET_WIDTH,
                            (tileIndex / TILESET_NUM_COLUMNS) * TILESET_HEIGHT,
                            TILESET_WIDTH,
                            TILESET_HEIGHT
                        )
                    );

                    if (map[y, x] == 1) {
                        if (map[y-1, x-1] == 0 && map[y-1, x] == 1 && map[y, x-1] == 1) {
                            spriteBatch.Draw(
                                tileset,
                                position: new Vector2(x * TILESET_WIDTH, y * TILESET_HEIGHT),
                                sourceRectangle: new Rectangle(
                                    (16 % TILESET_NUM_COLUMNS) * TILESET_WIDTH,
                                    (16 / TILESET_NUM_COLUMNS) * TILESET_HEIGHT,
                                    TILESET_WIDTH / 2,
                                    TILESET_HEIGHT / 2
                                )
                            );
                        }

                        if (map[y-1, x+1] == 0 && map[y-1, x] == 1 && map[y, x+1] == 1) {
                            spriteBatch.Draw(
                                tileset,
                                position: new Vector2(x * TILESET_WIDTH + TILESET_WIDTH / 2, y * TILESET_HEIGHT),
                                sourceRectangle: new Rectangle(
                                    (16 % TILESET_NUM_COLUMNS) * TILESET_WIDTH + TILESET_WIDTH / 2,
                                    (16 / TILESET_NUM_COLUMNS) * TILESET_HEIGHT,
                                    TILESET_WIDTH / 2,
                                    TILESET_HEIGHT / 2
                                )
                            );
                        }

                        if (map[y+1, x-1] == 0 && map[y+1, x] == 1 && map[y, x-1] == 1) {
                            spriteBatch.Draw(
                                tileset,
                                position: new Vector2(x * TILESET_WIDTH, y * TILESET_HEIGHT + TILESET_HEIGHT / 2),
                                sourceRectangle: new Rectangle(
                                    (16 % TILESET_NUM_COLUMNS) * TILESET_WIDTH,
                                    (16 / TILESET_NUM_COLUMNS) * TILESET_HEIGHT + TILESET_HEIGHT / 2,
                                    TILESET_WIDTH / 2,
                                    TILESET_HEIGHT / 2
                                )
                            );
                        }

                        if (map[y+1, x+1] == 0 && map[y+1, x] == 1 && map[y, x+1] == 1) {
                            spriteBatch.Draw(
                                tileset,
                                position: new Vector2(x * TILESET_WIDTH + TILESET_WIDTH / 2, y * TILESET_HEIGHT + TILESET_HEIGHT / 2),
                                sourceRectangle: new Rectangle(
                                    (16 % TILESET_NUM_COLUMNS) * TILESET_WIDTH + TILESET_WIDTH / 2,
                                    (16 / TILESET_NUM_COLUMNS) * TILESET_HEIGHT + TILESET_HEIGHT / 2,
                                    TILESET_WIDTH / 2,
                                    TILESET_HEIGHT / 2
                                )
                            );
                        }
                    }
                }
            }
        }

        protected int GetTileIndex(int x, int y) {
            if (map[y, x] == 0) {
                return 17;
            }

            /*
             * -, 1, -
             * 2, -, 4
             * -, 8, -
             */
            int[] tilesetValues = new int[] {
                12, 14, 10, 8,
                13, 15, 11, 9,
                5, 7, 3, 1,
                4, 6, 2, 0
            };
            return System.Array.IndexOf(
                tilesetValues,
                map[y-1, x]*1 + map[y, x-1]*2 + map[y, x+1]*4 + map[y+1, x]*8
            );
        }
    }
}
