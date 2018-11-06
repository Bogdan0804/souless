using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using RPG2D.GameEngine;
using RPG2D.GameEngine.Screens;
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
        NetPeerConfiguration config;
        NetServer server;
        Camera2D camera;
        FrameCounter fpsCounter;
        NetworkPlayer player;


        public void Init(ContentManager content)
        {
            GameManager.Game.ConsoleInterpreter.RegisterCommand("debug", (o) => {
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

            GameManager.Game.World = new World("world1.xml");
            camera = new Camera2D(GameManager.Game.GraphicsDevice);
            GameManager.Game.Player = new Player.Player();
            GameManager.Game.Player.Init(content);
            fpsCounter = new FrameCounter();

            config = new NetPeerConfiguration("RPG2D");
            config.Port = 20666;

            server = new NetServer(config);
            server.Start();

            GameManager.Game.NetworkParser = new NetworkParser(server);
            player = new NetworkPlayer();
        }

        public void Update(GameTime gameTime)
        {
            GameManager.Game.Player.Update(gameTime);
            camera.LookAt(GameManager.Game.Player.Position + (GameManager.Game.Player.Size / 2));
            GameManager.Game.World.Update(gameTime);

            GameManager.Game.NetworkParser.Update(gameTime);
            player.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GameManager.Game.GraphicsDevice.Clear(new Color(28, 17, 23));


            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            fpsCounter.Update(deltaTime);

            spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: camera.GetViewMatrix());


            var fps = string.Format("FPS: {0}", fpsCounter.AverageFramesPerSecond);
            GameManager.Game.World.Draw(gameTime, spriteBatch);
            if (GameManager.Game.NetworkParser.IsConnected)
            {
                player.Draw(gameTime, spriteBatch);
                spriteBatch.DrawString(GlobalAssets.Arial12, GameManager.Game.NetworkParser.Name, new Vector2(player.X - 16 + (GlobalAssets.Arial12.MeasureString(GameManager.Game.NetworkParser.Name).X / 2), player.Y - GlobalAssets.Arial12.MeasureString(GameManager.Game.NetworkParser.Name).Y), Color.Black);
            }

            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointWrap);

            spriteBatch.DrawString(GlobalAssets.Arial12, fps, new Vector2(1, 1), Color.White);

            spriteBatch.DrawString(GlobalAssets.Arial24, GameManager.Game.Tooltip, new Vector2(GameManager.Game.ScreenSize.X / 2 - GlobalAssets.Arial24.MeasureString(GameManager.Game.Tooltip).X / 2, GameManager.Game.ScreenSize.Y - GlobalAssets.Arial24.MeasureString(GameManager.Game.Tooltip).Y), Color.White);

            spriteBatch.End();
        }
    }
}
