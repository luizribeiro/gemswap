using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace gemswap {
    public class BoardRenderer
    {
        GraphicsDevice graphicsDevice;
        SpriteBatch spriteBatch;
        Texture2D gemTexture;

        public BoardRenderer(GraphicsDevice graphicsDevice) {
            this.graphicsDevice = graphicsDevice;
        }

        public void LoadContent(ContentManager contentManager) {
            this.spriteBatch = new SpriteBatch(this.graphicsDevice);
            this.gemTexture = contentManager.Load<Texture2D>("gems");
        }

        public void Draw(Board board) {
            this.spriteBatch.Begin();
            for (int x = 0; x < Constants.BOARD_WIDTH; x++) {
                for (int y = 0; y < Constants.BOARD_HEIGHT; y++) {
                    int gem = board.getCell(x, y);
                    if (gem == Board.EMPTY) {
                        continue;
                    }

                    this.spriteBatch.Draw(
                        this.gemTexture,
                        position: new Vector2(
                            x * Constants.GEM_WIDTH,
                            y * Constants.GEM_HEIGHT
                        ),
                        sourceRectangle: new Rectangle(
                            gem * Constants.GEM_WIDTH,
                            0,
                            Constants.GEM_WIDTH,
                            Constants.GEM_HEIGHT
                        ),
                        color: Color.White
                    );
                }
            }
            this.spriteBatch.End();
        }
    }
}
