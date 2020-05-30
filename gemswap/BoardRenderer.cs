namespace GemSwap
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public class BoardRenderer
    {
        private readonly GraphicsDevice graphicsDevice;
        private readonly Config config;
        private SpriteBatch? spriteBatch;
        private Texture2D? backgroundTexture;
        private Texture2D? gemTexture;
        private Texture2D? cursorTexture;
        private Matrix translationMatrix;

        public BoardRenderer(
            Config config,
            GraphicsDevice graphicsDevice,
            Vector2 position
        )
        {
            this.config = config;
            this.graphicsDevice = graphicsDevice;
            this.translationMatrix = Matrix.CreateTranslation(
                position.X,
                position.Y,
                0.0f
            );
        }

        public void LoadContent(ContentManager contentManager)
        {
            this.spriteBatch = new SpriteBatch(this.graphicsDevice);
            this.gemTexture = contentManager.Load<Texture2D>("gems");
            this.cursorTexture = contentManager.Load<Texture2D>("cursor");

            int backgroundWidth = this.config.BoardWidth * this.config.GemWidth;
            int backgroundHeight =
                this.config.BoardHeight * this.config.GemHeight;
            this.backgroundTexture = new Texture2D(
                this.graphicsDevice,
                backgroundWidth,
                backgroundHeight
            );
            Color[] backgroundData = new Color[
                backgroundWidth * backgroundHeight
            ];
            Color backgroundColor = new Color(0, 0, 0, 100);
            for (int i = 0; i < backgroundData.Length; i++)
            {
                backgroundData[i] = backgroundColor;
            }

            this.backgroundTexture.SetData(backgroundData);
        }

        public void Draw(Board board)
        {
            DepthStencilState s1 = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false,
            };
            DepthStencilState s2 = new DepthStencilState
            {
                StencilEnable = true,
                StencilFunction = CompareFunction.LessEqual,
                StencilPass = StencilOperation.Keep,
                ReferenceStencil = 1,
                DepthBufferEnable = false,
            };

            this.spriteBatch!.Begin(
                SpriteSortMode.Immediate,
                null,
                null,
                s1,
                null,
                null,
                this.translationMatrix
            );
            this.DrawBackground();
            this.spriteBatch.End();

            this.spriteBatch.Begin(
                SpriteSortMode.Immediate,
                null,
                null,
                s2,
                null,
                null,
                this.translationMatrix
            );
            this.DrawBackground();
            this.DrawBoard(board);
            this.DrawCursor(board);
            this.spriteBatch.End();
        }

        private void DrawBackground()
        {
            this.spriteBatch!.Draw(
                this.backgroundTexture,
                new Vector2(0, 0),
                Color.Black
            );
        }

        private void DrawBoard(Board board)
        {
            float offset = board.GetOffset();

            for (int x = 0; x < this.config.BoardWidth; x++)
            {
                for (int y = 0; y < this.config.BoardHeight; y++)
                {
                    int gem = board.GetCell(x, y);
                    if (gem == Board.EMPTY)
                    {
                        continue;
                    }

                    float cellOffsetX = board.GetCellOffsetX(x, y);
                    float cellOffsetY = board.GetCellOffsetY(x, y);

                    int a = board.GetCellAlpha(x, y);
                    this.spriteBatch!.Draw(
                        this.gemTexture,
                        position: new Vector2(
                            x * this.config.GemWidth + cellOffsetX,
                            y * this.config.GemHeight + cellOffsetY - offset
                        ),
                        sourceRectangle: new Rectangle(
                            gem * this.config.GemWidth,
                            0,
                            this.config.GemWidth,
                            this.config.GemHeight
                        ),
                        color: new Color(a, a, a, a)
                    );
                }
            }

            float heightLeft = this.config.GemHeight - offset;
            float pxSpeed = this.config.GemHeight / this.config.BoardSpeedRowPerMs;
            float msLeft = heightLeft / pxSpeed;
            int alpha = msLeft > this.config.AnimationGemFadeInMs
                ? 100
                : Convert.ToInt32(100.0 + 155.0 * (1.0 - msLeft / this.config.AnimationGemFadeInMs));
            Color upcomingGemColor = new Color(
                alpha,
                alpha,
                alpha,
                alpha
            );
            for (int x = 0; x < this.config.BoardWidth; x++)
            {
                int gem = board.GetUpcomingCell(x);

                this.spriteBatch!.Draw(
                    this.gemTexture,
                    position: new Vector2(
                        x * this.config.GemWidth,
                        this.config.BoardHeight * this.config.GemHeight - offset
                    ),
                    sourceRectangle: new Rectangle(
                        gem * this.config.GemWidth,
                        0,
                        this.config.GemWidth,
                        this.config.GemHeight
                    ),
                    color: upcomingGemColor
                );
            }
        }

        private void DrawCursor(Board board)
        {
            this.spriteBatch!.Draw(
                this.cursorTexture,
                new Vector2(
                    board.GetCursorX() * this.config.GemWidth
                        - this.config.CursorOffsetPx,
                    board.GetCursorY() * this.config.GemHeight
                        - this.config.CursorOffsetPx - board.GetOffset()
                ),
                Color.White
            );
        }
    }
}
