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
        GamePadState previousGamePadState;

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
            this.previousGamePadState = GamePad.GetState(PlayerIndex.One);
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

            if (
                gamePadState1.DPad.Left == ButtonState.Pressed
                && gamePadState1.DPad.Left != previousGamePadState.DPad.Left
            ) {
                this.board.MoveCursor(dx: -1, dy: 0);
            }
            if (
                gamePadState1.DPad.Right == ButtonState.Pressed
                && gamePadState1.DPad.Right != previousGamePadState.DPad.Right
            ) {
                this.board.MoveCursor(dx: +1, dy: 0);
            }
            if (
                gamePadState1.DPad.Down == ButtonState.Pressed
                && gamePadState1.DPad.Down != previousGamePadState.DPad.Down
            ) {
                this.board.MoveCursor(dx: 0, dy: +1);
            }
            if (
                gamePadState1.DPad.Up == ButtonState.Pressed
                && gamePadState1.DPad.Up != previousGamePadState.DPad.Up
            ) {
                this.board.MoveCursor(dx: 0, dy: -1);
            }
            if (
                gamePadState1.Buttons.A == ButtonState.Pressed
                && gamePadState1.Buttons.A != previousGamePadState.Buttons.A
            ) {
                this.board.Swap();
            }

            float ellapsedMilliseconds =
                (float)gameTime.ElapsedGameTime.Milliseconds;

            this.board.Update(ellapsedMilliseconds);

            this.previousGamePadState = gamePadState1;

            TimerManager.Update(ellapsedMilliseconds);

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
