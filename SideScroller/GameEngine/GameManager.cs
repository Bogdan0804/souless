using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using RPG2D.GameEngine.Screens;
using RPG2D.SGame.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.GameEngine
{
    public class GameManager
    {
        private static GameManager _manager;
        public static GameManager Game
        {
            get
            {
                if (_manager == null)
                    _manager = new GameManager();

                return _manager;
            }
        }

        public ContentManager Content { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public GraphicsDevice GraphicsDevice { get { return GraphicsDeviceManager.GraphicsDevice; } }

        public Vector2 ScreenSize { get { return new Vector2(GraphicsDeviceManager.PreferredBackBufferWidth, GraphicsDeviceManager.PreferredBackBufferHeight); } }
        public World.World World { get; set; }
        public Player Player { get; set; }
        public string Tooltip = "";

        private IGameScreen gameScreen;

        public bool Stopped { get; set; }

        public void Init(ContentManager content, SpriteBatch spriteBatch, GraphicsDeviceManager graphics)
        {
            this.Stopped = false;

            this.Content = content;
            this.SpriteBatch = spriteBatch;
            this.GraphicsDeviceManager = graphics;

            GlobalAssets.Arial12 = Content.Load<SpriteFont>("arial12");
            GlobalAssets.Arial24 = Content.Load<SpriteFont>("arial24");

            gameScreen = new SGame.Screens.MenuGameScreen();
            gameScreen.Init(this.Content);
        }
        public void ChangeScreen(IGameScreen screen)
        {
            Stopped = true;

            gameScreen = screen;
            gameScreen.Init(this.Content);

            Stopped = false;
        }
        public void Draw(GameTime gameTime)
        {

            if (!Stopped)
                gameScreen.Draw(gameTime, SpriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            Game.Tooltip = "";

            if (!Stopped)
                gameScreen.Update(gameTime);
        }
    }
}
