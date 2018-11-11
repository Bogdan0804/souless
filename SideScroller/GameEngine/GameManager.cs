using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Penumbra;
using QuakeConsole;
using RPG2D.GameEngine.Screens;
using RPG2D.SGame.Player;
using RPG2D.SGame.UI;
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
        public Game ThisGame { get; set; }
        public NetworkParser NetworkParser { get; set; }
        public ConsoleComponent Console { get; set; }
        public ManualInterpreter ConsoleInterpreter { get; set; }
        public static string Name
        {
            get
            {
                return RPG2D.Properties.Settings.Default.name;
            }
        }

        public Vector2 ScreenSize { get { return new Vector2(GraphicsDeviceManager.PreferredBackBufferWidth, GraphicsDeviceManager.PreferredBackBufferHeight); } }
        public World.World World { get; set; }
        public Player Player { get; set; }
        public string Tooltip = "";
        public static Texture2D Black { get; private set; }
        public InventoryUI Inventory { get; set; }
        public StatsOverlay Stats { get; set; }
        public PenumbraComponent Penumbra { get; set; }
        public Camera2D Camera { get; set; }

        private IGameScreen gameScreen;
        private KeyboardState oldState;
        private bool genPause = false;
        private Texture2D pausedMenuTitle;

        public bool Stopped { get; set; }
        public bool Paused { get; set; }
        public bool InInventory { get; set; }
        public static bool DebugMode { get; set; }

        public void Init(ContentManager content, SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Game game)
        {
            this.Stopped = false;
            this.ThisGame = game;
            this.Content = content;
            this.SpriteBatch = spriteBatch;
            this.GraphicsDeviceManager = graphics;

            GlobalAssets.Arial12 = Content.Load<SpriteFont>("arial12");
            GlobalAssets.Arial24 = Content.Load<SpriteFont>("arial24");
            Black = Content.Load<Texture2D>("black");
            pausedMenuTitle = Content.Load<Texture2D>("invTitle");

            gameScreen = new SGame.Screens.MenuGameScreen();
            gameScreen.Init(this.Content);

            GameManager.Game.ConsoleInterpreter.RegisterCommand("fullscreen", (o) =>
            {
                try
                {
                    GraphicsDeviceManager.IsFullScreen = bool.Parse(o[0]);
                    GraphicsDeviceManager.ApplyChanges();
                    return "Set fullscreen mode to " + o[0];
                }
                catch
                {
                    return o[0] + " is not a valid input.";
                }
            });
            GameManager.Game.ConsoleInterpreter.RegisterCommand("res", (o) =>
            {
                try
                {
                    GraphicsDeviceManager.PreferredBackBufferWidth = int.Parse(o[0]);
                    GraphicsDeviceManager.PreferredBackBufferHeight = int.Parse(o[1]);
                    GraphicsDeviceManager.ApplyChanges();
                    return "Set res to " + o[0] + "x" + o[1];
                }
                catch
                {
                    return o[0] + " is not a valid input.";
                }
            });
        }
        public void InitLighting()
        {
            Penumbra = new PenumbraComponent(ThisGame);
            ThisGame.Services.AddService(Penumbra);
            Penumbra.Initialize();
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

            if (Paused)
            {
                if (genPause)
                {
                    genPause = false;

                }

                float titleTexX = GameEngine.GameManager.Game.ScreenSize.X / 2 - pausedMenuTitle.Width / 2;

                SpriteBatch.Begin();
                SpriteBatch.Draw(Black, new Rectangle(0, 0, this.GraphicsDeviceManager.PreferredBackBufferWidth, this.GraphicsDeviceManager.PreferredBackBufferHeight), new Color(Color.White, 100));
                SpriteBatch.Draw(pausedMenuTitle, new Vector2(titleTexX, 2), Color.White);
                SpriteBatch.End();
            }
            else
                genPause = true;
        }

        public void Update(GameTime gameTime)
        {
            var kbState = Keyboard.GetState();

            Game.Tooltip = "";

            if (!Stopped && !Paused)
                gameScreen.Update(gameTime);

            if (kbState.IsKeyDown(Keys.Escape) && oldState.IsKeyUp(Keys.Escape)) Paused = !Paused;

            if (oldState.IsKeyUp(Keys.OemTilde) && kbState.IsKeyDown(Keys.OemTilde))
            {
                Console.ToggleOpenClose();
                Stopped = !Stopped;
            }

            oldState = kbState;
        }
    }
}
