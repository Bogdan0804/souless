using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collections;
using Penumbra;
using RPG2D.GameEngine;
using RPG2D.GameEngine.Screens;
using RPG2D.GameEngine.UI;
using RPG2D.GameEngine.World;
using RPG2D.SGame.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.SGame.Screens
{
    public class MainGameScreen : IGameScreen
    {
        private SoundEffect bgMusic;
        private Bag<UIElement> UI = new Bag<UIElement>();
        private NetPeerConfiguration config;
        private NetServer server;
        private FrameCounter fpsCounter;
        private NetworkPlayer player;
        private double fadeInTimer = 0;
        private bool doneFadeIn = false;
        private Texture2D vignette;
        private Light mouseLight;
        private double fadeOutTimer = 0;
        private bool doneFadeOut = false;
        private int fadeAlpha = 0;

        public void InitLighting()
        {
            mouseLight = new PointLight();
            mouseLight.CastsShadows = true;
            mouseLight.ShadowType = ShadowType.Solid;
            mouseLight.Scale = new Vector2(100);
            mouseLight.Intensity = 0.5f;
            GameManager.Game.Penumbra.Lights.Add(mouseLight);

        }

        public void Init(ContentManager content)
        {
            vignette = content.Load<Texture2D>("vignette");
            bgMusic = content.Load<SoundEffect>("sound/ambience_music");
            GameManager.Game.ConsoleInterpreter.RegisterCommand("debug", (o) =>
            {
                try
                {
                    GameManager.DebugMode = bool.Parse(o[0]);
                    GameManager.Game.Penumbra.Debug = GameManager.DebugMode;
                    return "Set debug mode to " + o[0];
                }
                catch
                {
                    return o[0] + " is not valid input.";
                }
            });
            GameManager.Game.ConsoleInterpreter.RegisterCommand("graphics", (o) =>
            {
                try
                {
                    switch (o[0])
                    {
                        case "lighting":
                            GameManager.Game.Penumbra.Visible = bool.Parse(o[1]);
                            return "Set lighting mode to " + o[1];
                        case "vignette":
                            GameManager.Game.GraphicsSettings.Vignette = bool.Parse(o[1]);
                            return "Set vignette mode to " + o[1];

                        default:
                            return o[0] + " is not valid input.";
                    }

                }
                catch
                {
                    return o[0] + " is not valid input.";
                }
            });

            GameManager.Game.World = new World("SGame/Worlds/world1.xml", false);
            GameManager.Game.Camera = new Camera2D(GameManager.Game.GraphicsDevice);
            GameManager.Game.Camera.ZoomIn(1f);
            GameManager.Game.Player = new Player.Player();
            GameManager.Game.Player.Init(content);
            fpsCounter = new FrameCounter();

            config = new NetPeerConfiguration("RPG2D");
            config.Port = 20666;

            server = new NetServer(config);
            server.Start();

            GameManager.Game.NetworkParser = new NetworkParser(server);
            player = new NetworkPlayer();

            GameManager.Game.Inventory = new UI.InventoryUI();
            GameManager.Game.Stats = new UI.StatsOverlay();
            UI.Add(GameManager.Game.Stats);
            UI.Add(GameManager.Game.Inventory);
            InitGameStory();
            var musicInstance = bgMusic.CreateInstance();
            musicInstance.IsLooped = true;
            musicInstance.Volume = 0.25f;
            musicInstance.Play();

            InitLighting();
            doneFadeOut = true;
        }

        private void InitGameStory()
        {
            GameManager.Game.InstructionDialog = new InstructionDialog();
            this.UI.Add(GameManager.Game.InstructionDialog);
        }

        public void FadeIn()
        {
            fadeInTimer = 0;
            fadeAlpha = 255;
            doneFadeIn = false;
        }
        public void Update(GameTime gameTime)
        {
            fadeInTimer += gameTime.ElapsedGameTime.TotalSeconds;
            fadeOutTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (fadeInTimer > 0.05d && fadeAlpha > 0 && !doneFadeIn)
                fadeAlpha -= 1;


            GameManager.Game.Penumbra.Transform = GameManager.Game.Camera.GetViewMatrix();
            mouseLight.Position = GameManager.Game.Camera.ScreenToWorld(GameManager.Game.MousePos);
            GameManager.Game.Camera.LookAt(GameManager.Game.Player.Position + (GameManager.Game.Player.Size / 2));
            GameManager.Game.Player.Update(gameTime);

            if (!GameManager.Game.InInventory) GameManager.Game.World.Update(gameTime);

            GameManager.Game.NetworkParser.Update(gameTime);
            player.Update(gameTime);

            foreach (var ui in UI)
            {
                ui.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GameManager.Game.Penumbra.BeginDraw();
            GameManager.Game.GraphicsDevice.Clear(new Color(28, 17, 23));


            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            fpsCounter.Update(deltaTime);

            spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: GameManager.Game.Camera.GetViewMatrix());


            var fps = string.Format("FPS: {0}", fpsCounter.AverageFramesPerSecond);
            GameManager.Game.World.Draw(gameTime, spriteBatch);

            if (GameManager.Game.NetworkParser.IsConnected)
            {
                player.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(GlobalAssets.Arial12, GameManager.Game.NetworkParser.Name, new Vector2(player.X - 16 + (GlobalAssets.Arial12.MeasureString(GameManager.Game.NetworkParser.Name).X / 2), player.Y - GlobalAssets.Arial12.MeasureString(GameManager.Game.NetworkParser.Name).Y), Color.Black);
            }
            spriteBatch.End();

            GameManager.Game.Penumbra.Draw(gameTime);

            spriteBatch.Begin(samplerState: SamplerState.PointWrap);

            if (GameManager.Game.GraphicsSettings.Vignette)
                spriteBatch.Draw(vignette, new Rectangle(0, 0, (int)GameManager.Game.ScreenSize.X, (int)GameManager.Game.ScreenSize.Y), new Color(Color.White, 150));

            spriteBatch.DrawString(GlobalAssets.Arsenal, GameManager.Game.Tooltip, new Vector2(GameManager.Game.ScreenSize.X / 2 - GlobalAssets.Arsenal.MeasureString(GameManager.Game.Tooltip).X / 2, GameManager.Game.ScreenSize.Y - (GlobalAssets.Arsenal.MeasureString(GameManager.Game.Tooltip).Y) - 105), Color.White);
            foreach (var ui in UI)
            {
                ui.Draw(gameTime, spriteBatch);
            }


            spriteBatch.Draw(GameManager.Black, new Rectangle(0, 0, (int)GameManager.Game.ScreenSize.X, (int)GameManager.Game.ScreenSize.Y), new Color(Color.Black, fadeAlpha));

            spriteBatch.DrawString(GlobalAssets.Arsenal, fps, new Vector2(1, 1), Color.White);
            spriteBatch.End();
        }
    }
}
