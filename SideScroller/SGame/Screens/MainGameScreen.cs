using Lidgren.Network;
using Microsoft.Xna.Framework;
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
        private Bag<UIElement> UI = new Bag<UIElement>();
        private NetPeerConfiguration config;
        private NetServer server;
        private FrameCounter fpsCounter;
        private NetworkPlayer player;
        private double fadeInTimer = 0;
        private int fadeInAlpha = 255;
        private Texture2D vignette;
        private bool doneFade;
        private Hull playerHull;
        private PointLight mouseLight;

        public void Init(ContentManager content)
        {
            vignette = content.Load<Texture2D>("vignette");

            GameManager.Game.ConsoleInterpreter.RegisterCommand("debug", (o) =>
            {
                try
                {
                    GameManager.DebugMode = bool.Parse(o[0]);
                    return "Set debug mode to " + o[0];
                }
                catch
                {
                    return o[0] + " is not a valid input.";
                }
            });

            GameManager.Game.World = new World("SGame/Worlds/world1.xml", false);
            GameManager.Game.Camera = new Camera2D(GameManager.Game.GraphicsDevice);
            GameManager.Game.Camera.ZoomIn(1);
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
            playerHull = new Hull(new Vector2(12, 0), new Vector2(12, 55), new Vector2(14, 58), new Vector2(16, 59), new Vector2(20, 59), new Vector2(26, 56), new Vector2(37, 56), new Vector2(43, 59), new Vector2(47, 59), new Vector2(49, 58), new Vector2(51, 55), new Vector2(51, 0))
            {
                Scale = new Vector2(1),
                Origin=new Vector2(32)
            };

            mouseLight = new PointLight();
            mouseLight.CastsShadows = true;
            mouseLight.ShadowType = ShadowType.Solid;
            mouseLight.Scale = new Vector2(100);
            mouseLight.Intensity = 0.5f;

            GameManager.Game.Penumbra.Hulls.Add(playerHull);
            GameManager.Game.Penumbra.Lights.Add(mouseLight);
        }


        public void Update(GameTime gameTime)
        {
            fadeInTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (fadeInTimer > 0.10d && fadeInAlpha > 0 && !doneFade)
                fadeInAlpha -= 1;
            else doneFade = true;


            GameManager.Game.Player.Update(gameTime);
            GameManager.Game.Camera.LookAt(GameManager.Game.Player.Position + (GameManager.Game.Player.Size / 2));

            playerHull.Position = GameManager.Game.Camera.ScreenToWorld(GameManager.Game.ScreenSize / 2);
            GameManager.Game.Penumbra.Transform = GameManager.Game.Camera.GetViewMatrix();
            mouseLight.Position = GameManager.Game.Camera.ScreenToWorld(Mouse.GetState().Position.ToVector2());

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
            spriteBatch.Draw(vignette, new Rectangle(0, 0, (int)GameManager.Game.ScreenSize.X, (int)GameManager.Game.ScreenSize.Y), Color.White);

            spriteBatch.DrawString(GlobalAssets.Arial24, GameManager.Game.Tooltip, new Vector2(GameManager.Game.ScreenSize.X / 2 - GlobalAssets.Arial24.MeasureString(GameManager.Game.Tooltip).X / 2, GameManager.Game.ScreenSize.Y - (GlobalAssets.Arial24.MeasureString(GameManager.Game.Tooltip).Y) - 75), Color.White);
            foreach (var ui in UI)
            {
                ui.Draw(gameTime, spriteBatch);
            }


            spriteBatch.Draw(GameManager.Black, new Rectangle(0, 0, (int)GameManager.Game.ScreenSize.X, (int)GameManager.Game.ScreenSize.Y), new Color(Color.Black, fadeInAlpha));

            spriteBatch.DrawString(GlobalAssets.Arial12, fps, new Vector2(1, 1), Color.White);
            spriteBatch.End();
        }
    }
}
