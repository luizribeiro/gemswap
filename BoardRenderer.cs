using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace gemswap {
    public class BoardRenderer
    {
        GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;
        Texture2D backgroundTexture;
        Texture2D gemTexture;
        Texture2D cursorTexture;

        public BoardRenderer(GraphicsDevice graphicsDevice) {
            this.graphicsDevice = graphicsDevice;
        }

        public void LoadContent(ContentManager contentManager) {
            this.spriteBatch = new SpriteBatch(this.graphicsDevice);
            this.gemTexture = contentManager.Load<Texture2D>("gems");
            this.cursorTexture = contentManager.Load<Texture2D>("cursor");

            int backgroundWidth = Constants.BOARD_WIDTH * Constants.GEM_WIDTH;
            int backgroundHeight =
                Constants.BOARD_HEIGHT * Constants.GEM_HEIGHT;
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

            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, s1, null, null);
            this.DrawBackground();
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, s2, null, null);
            this.DrawBackground();
            this.DrawBoard(board);
            this.DrawCursor(board);
            spriteBatch.End();
        }

        private void DrawBackground() {
            spriteBatch.Draw(
                this.backgroundTexture,
                new Vector2(0, 0),
                Color.Black
            );
        }

        private void DrawBoard(Board board) {
            float offset = board.getOffset();

            for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
                for (int y = 0; y < Constants.BOARD_HEIGHT; y++) {
                    int gem = board.getCell(x, y);
                    if (gem == Board.EMPTY) {
                        continue;
                    }

                    float cellOffsetX = board.GetCellOffsetX(x, y);
                    float cellOffsetY = board.GetCellOffsetY(x, y);

                    int a = board.GetCellAlpha(x, y);
                    this.spriteBatch.Draw(
                        this.gemTexture,
                        position: new Vector2(
                            x * Constants.GEM_WIDTH + cellOffsetX,
                            y * Constants.GEM_HEIGHT + cellOffsetY - offset
                        ),
                        sourceRectangle: new Rectangle(
                            gem * Constants.GEM_WIDTH,
                            0,
                            Constants.GEM_WIDTH,
                            Constants.GEM_HEIGHT
                        ),
                        color: new Color(a, a, a, a)
                    );
                }
            }

            float heightLeft = Constants.GEM_HEIGHT - offset;
            float pxSpeed = Constants.GEM_HEIGHT / Constants.BOARD_SPEED_ROW_PER_MS;
            float msLeft = heightLeft / pxSpeed;
            int alpha = msLeft > Constants.ANIMATION_GEM_FADE_IN_MS
                ? 100
                : Convert.ToInt32(100.0 + 155.0 * (1.0 - msLeft / Constants.ANIMATION_GEM_FADE_IN_MS));
            Color upcomingGemColor = new Color(
                alpha,
                alpha,
                alpha,
                alpha
            );
            for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
                int gem = board.getUpcomingCell(x);

                this.spriteBatch.Draw(
                    this.gemTexture,
                    position: new Vector2(
                        x * Constants.GEM_WIDTH,
                        Constants.BOARD_HEIGHT * Constants.GEM_HEIGHT - offset
                    ),
                    sourceRectangle: new Rectangle(
                        gem * Constants.GEM_WIDTH,
                        0,
                        Constants.GEM_WIDTH,
                        Constants.GEM_HEIGHT
                    ),
                    color: upcomingGemColor
                );
            }
        }

        private void DrawCursor(Board board) {
            spriteBatch.Draw(
                this.cursorTexture,
                new Vector2(
                    board.getCursorX() * Constants.GEM_WIDTH
                        - Constants.CURSOR_OFFSET_PX,
                    board.getCursorY() * Constants.GEM_HEIGHT
                        - Constants.CURSOR_OFFSET_PX - board.getOffset()
                ),
                Color.White
            );
        }
    }
}
