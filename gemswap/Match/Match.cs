namespace GemSwap.Match
{
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;

    public abstract class Match
    {
        protected Match(GraphicsDevice graphicsDevice)
        {
            this.GraphicsDevice = graphicsDevice;
            this.Config = new Config();
        }

        protected Config Config { get; }

        protected GraphicsDevice GraphicsDevice { get; }

        protected SpriteBatch? SpriteBatch { get; private set; }

        protected int ScreenWidth => this.GraphicsDevice.Viewport.Width;

        protected int ScreenHeight => this.GraphicsDevice.Viewport.Height;

        public void LoadContent(ContentManager contentManager)
        {
            // contentManager may be used by methods that override LoadContent
            _ = contentManager;
            this.SpriteBatch = new SpriteBatch(this.GraphicsDevice);
        }

        public abstract void Update(float ellapsedMilliseconds);

        public abstract void Draw();
    }
}
