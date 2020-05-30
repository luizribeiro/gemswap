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
        ) {
            this.config = config;
            this.graphicsDevice = graphicsDevice;
            this.translationMatrix = Matrix.CreateTranslation(
                position.X,
                position.Y,
                0.0f
            );
        }

        public void LoadContent(ContentManager contentManager) {
            this.spriteBatch = new SpriteBatch(this.graphicsDevice);
            this.gemTexture = contentManager.Load<Texture2D>("gems");
            this.cursorTexture = contentManager.Load<Texture2D>("cursor");

            int backgroundWidth = config.BoardWidth * config.GemWidth;
            int backgroundHeight =
                config.BoardHeight * config.GemHeight;
            this.backgroundTexture = new Texture2D(
                this.graphicsDevice,
                backgroundWidth,
                backgroundHeight
            );
            Color[] backgroundData = new Color[
                backgroundWidth * backgroundHeight
            ];
            Color backgroundColor = new Color(0, 0, 0, 100);
            for(int i = 0; i < backgroundData.Length; i++) {
                backgroundData[i] = backgroundColor;
            }
            this.backgroundTexture.SetData(backgroundData);
        }

        public void Draw(Board board) {
            var s1 = new DepthStencilState {
                StencilEnable = true,
                StencilFunction = CompareFunction.Always,
                StencilPass = StencilOperation.Replace,
                ReferenceStencil = 1,
                DepthBufferEnable = false,
            };
            var s2 = new DepthStencilState {
                StencilEnable = true,
                StencilFunction = CompareFunction.LessEqual,
                StencilPass = StencilOperation.Keep,
                ReferenceStencil = 1,
                DepthBufferEnable = false,
            };

            spriteBatch!.Begin(SpriteSortMode.Immediate, null, null, s1, null, null, this.translationMatrix);
            this.DrawBackground();
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, s2, null, null, this.translationMatrix);
            this.DrawBackground();
            this.DrawBoard(board);
            this.DrawCursor(board);
            spriteBatch.End();
        }

        private void DrawBackground() {
            spriteBatch!.Draw(
                this.backgroundTexture,
                new Vector2(0, 0),
                Color.Black
            );
        }

        private void DrawBoard(Board board) {
            float offset = board.GetOffset();

            for (int x = 0; x < config.BoardWidth; x++) {
                for (int y = 0; y < config.BoardHeight; y++) {
                    int gem = board.GetCell(x, y);
                    if (gem == Board.EMPTY) {
                        continue;
                    }

                    float cellOffsetX = board.GetCellOffsetX(x, y);
                    float cellOffsetY = board.GetCellOffsetY(x, y);

                    int a = board.GetCellAlpha(x, y);
                    this.spriteBatch!.Draw(
                        this.gemTexture,
                        position: new Vector2(
                            x * config.GemWidth + cellOffsetX,
                            y * config.GemHeight + cellOffsetY - offset
                        ),
                        sourceRectangle: new Rectangle(
                            gem * config.GemWidth,
                            0,
                            config.GemWidth,
                            config.GemHeight
                        ),
                        color: new Color(a, a, a, a)
                    );
                }
            }

            float heightLeft = config.GemHeight - offset;
            float pxSpeed = config.GemHeight / config.BoardSpeedRowPerMs;
            float msLeft = heightLeft / pxSpeed;
            int alpha = msLeft > config.AnimationGemFadeInMs
                ? 100
                : Convert.ToInt32(100.0 + 155.0 * (1.0 - msLeft / config.AnimationGemFadeInMs));
            Color upcomingGemColor = new Color(
                alpha,
                alpha,
                alpha,
                alpha
            );
            for (int x = 0; x < config.BoardWidth; x++) {
                int gem = board.GetUpcomingCell(x);

                this.spriteBatch!.Draw(
                    this.gemTexture,
                    position: new Vector2(
                        x * config.GemWidth,
                        config.BoardHeight * config.GemHeight - offset
                    ),
                    sourceRectangle: new Rectangle(
                        gem * config.GemWidth,
                        0,
                        config.GemWidth,
                        config.GemHeight
                    ),
                    color: upcomingGemColor
                );
            }
        }

        private void DrawCursor(Board board) {
            spriteBatch!.Draw(
                this.cursorTexture,
                new Vector2(
                    board.GetCursorX() * config.GemWidth
                        - config.CursorOffsetPx,
                    board.GetCursorY() * config.GemHeight
                        - config.CursorOffsetPx - board.GetOffset()
                ),
                Color.White
            );
        }
    }
}
