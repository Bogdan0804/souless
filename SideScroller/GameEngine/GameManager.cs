using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Penumbra;
using QuakeConsole;
using RPG2D.GameEngine.Screens;
using RPG2D.GameEngine.UI;
using RPG2D.SGame.Player;
using RPG2D.SGame.Screens;
using RPG2D.SGame.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPG2D.GameEngine
{
    public struct GraphicsSettings
    {
        public bool Vignette { get; set; }
    }

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
        public GraphicsSettings GraphicsSettings;
        public ContentManager Content { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public GraphicsDevice GraphicsDevice => GraphicsDeviceManager.GraphicsDevice;
        public Game ThisGame { get; set; }
        public NetworkParser NetworkParser { get; set; }
        public ConsoleComponent Console { get; set; }
        public ManualInterpreter ConsoleInterpreter { get; set; }
        public static string Name => RPG2D.Properties.Settings.Default.name;

        public Vector2 ScreenSize
        {
            get
            {
                return new Vector2(GraphicsDeviceManager.PreferredBackBufferWidth, GraphicsDeviceManager.PreferredBackBufferHeight);
            }
        }
        public World.World World { get; set; }
        public Player Player { get; set; }
        public string Tooltip = "";
        public static Texture2D Black { get; private set; }
        public InventoryUI Inventory { get; set; }
        public StatsOverlay Stats { get; set; }
        public PenumbraComponent Penumbra { get; set; }
        public Camera2D Camera { get; set; }
        public IGameScreen GameScreen;
        public Vector2 ControllerCursorPos;
        public InstructionDialog InstructionDialog { get; set; }
        public Vector2 MousePos
        {
            get
            {
                if (GamePad.GetState(PlayerIndex.One).IsConnected)
                    return ControllerCursorPos;
                else
                    return Mouse.GetState().Position.ToVector2();
            }
        }

        private KeyboardState oldState;
        private Texture2D pausedMenuTitle, controllerCursor;

        public bool Stopped { get; set; }
        //public bool CanMove { get; set; }
        public bool Paused { get; set; }
        public bool InInventory { get; set; }
        public static bool DebugMode { get; set; }

        public void Init(ContentManager content, SpriteBatch spriteBatch, GraphicsDeviceManager graphics, Game game)
        {
            this.GraphicsSettings = new GraphicsSettings();
            GraphicsSettings.Vignette = true;
            this.Stopped = false;
            this.ThisGame = game;
            this.Content = content;
            this.SpriteBatch = spriteBatch;
            this.GraphicsDeviceManager = graphics;

            // load essential assets and sounds.
            GlobalAssets.SoundEffects.Add("torch", Content.Load<SoundEffect>("sound/torch"));
            GlobalAssets.SoundEffects.Add("doorOpen", Content.Load<SoundEffect>("sound/door_open"));
            GlobalAssets.SoundEffects.Add("doorClose", Content.Load<SoundEffect>("sound/door_close"));
            GlobalAssets.Arial12 = Content.Load<SpriteFont>("arial12");
            GlobalAssets.Arial24 = Content.Load<SpriteFont>("arial24");
            GlobalAssets.Arial18 = Content.Load<SpriteFont>("arial18");
            GlobalAssets.Arsenal = Content.Load<SpriteFont>("Arsenal");
            Black = Content.Load<Texture2D>("black");
            pausedMenuTitle = Content.Load<Texture2D>("invTitle");
            controllerCursor = Content.Load<Texture2D>("ui/controller_mouse");
            GlobalAssets.SpeechDialogTexture = Content.Load<Texture2D>("ui/speech_dialog");

            GameScreen = new SGame.Screens.MenuGameScreen();
            GameScreen.Init(this.Content);

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
                    return "Not a valid input.";
                }
            });

            this.ControllerCursorPos = ScreenSize / 2;
        }
        public void InitLighting()
        {
            Penumbra = new PenumbraComponent(ThisGame);
            ThisGame.Services.AddService(Penumbra);
            Penumbra.Initialize();
            Penumbra.AmbientColor = Color.Black;
        }
        public void ChangeScreen(IGameScreen screen)
        {
            Stopped = true;

            GameScreen = screen;
            GameScreen.Init(this.Content);

            Stopped = false;
        }
        public void Draw(GameTime gameTime)
        {
            if (!Stopped)
                GameScreen.Draw(gameTime, SpriteBatch);

            if (Paused)
            {
                float titleTexX = ScreenSize.X / 2 - pausedMenuTitle.Width / 2;

                SpriteBatch.Begin();
                SpriteBatch.Draw(Black, new Rectangle(0, 0, this.GraphicsDeviceManager.PreferredBackBufferWidth, this.GraphicsDeviceManager.PreferredBackBufferHeight), new Color(Color.White, 100));
                SpriteBatch.Draw(pausedMenuTitle, new Vector2(titleTexX, 2), Color.White);
                SpriteBatch.End();
            }

            SpriteBatch.Begin(samplerState: SamplerState.PointWrap);
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
                SpriteBatch.Draw(controllerCursor, MousePos, Color.White);
            SpriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            var state = GamePad.GetState(PlayerIndex.One);
            var kbState = Keyboard.GetState();

            Game.Tooltip = "";

            if (!Stopped && !Paused)
                GameScreen.Update(gameTime);

            if (kbState.IsKeyDown(Keys.Escape) && oldState.IsKeyUp(Keys.Escape)) Paused = !Paused;

            if (oldState.IsKeyUp(Keys.OemTilde) && kbState.IsKeyDown(Keys.OemTilde))
            {
                Console.ToggleOpenClose();
                Stopped = !Stopped;
            }

            if (state.IsConnected)
            {
                ControllerCursorPos.X += state.ThumbSticks.Right.X * 5;
                ControllerCursorPos.Y += -state.ThumbSticks.Right.Y * 5;
            }
            oldState = kbState;
        }

        public static void VibrateFor(int time, bool left = true, bool right = true)
        {
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
                new Thread(new ThreadStart(delegate
                {
                    GamePad.SetVibration(Microsoft.Xna.Framework.PlayerIndex.One, right ? 1 : 0, left ? 1 : 0);
                    Thread.Sleep(time);
                    GamePad.SetVibration(Microsoft.Xna.Framework.PlayerIndex.One, 0, 0);
                })).Start();
        }
    }
}
