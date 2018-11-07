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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.SGame.Screens
{
    public class COOPGameScreen : IGameScreen
    {
        NetPeerConfiguration config;
        NetClient client;
        Camera2D camera;
        FrameCounter fpsCounter;

        string ip, name = "";
        NetworkPlayer player;
        double fadeInTimer = 0;
        int fadeInAlpha = 255;



        public COOPGameScreen(string ip, string name, string xml)
        {
            this.ip = ip;
            this.name = name;
            File.WriteAllText("globe.xml", xml);
            GameManager.Game.World = new World("globe.xml", false);
        }

        public void Init(ContentManager content)
        {
            camera = new Camera2D(GameManager.Game.GraphicsDevice);
            GameManager.Game.Player = new Player.Player();
            GameManager.Game.Player.Init(content);
            fpsCounter = new FrameCounter();

            camera.ZoomIn(1);

            ConnectToServer(ip);
            GameManager.Game.NetworkParser = new NetworkParser(client);
            GameManager.Game.NetworkParser.IP = ip;
            player = new NetworkPlayer();
        }

        public void ConnectToServer(string ip)
        {
            config = new NetPeerConfiguration("RPG2D");
            client = new NetClient(config);
            client.Start();

            int port = 20666;
            client.Connect(ip, port);
        }

        public void Update(GameTime gameTime)
        {
            fadeInTimer += gameTime.ElapsedGameTime.TotalSeconds;

            if (fadeInTimer > 0.10d && fadeInAlpha > 0)
                fadeInAlpha -= 1;

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
            spriteBatch.DrawString(GlobalAssets.Arial12, name, new Vector2(player.X - 16 + (GlobalAssets.Arial12.MeasureString(name).X / 2), player.Y - GlobalAssets.Arial12.MeasureString(name).Y), Color.Black);
            player.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            spriteBatch.Begin(samplerState: SamplerState.PointWrap);

            spriteBatch.DrawString(GlobalAssets.Arial12, fps, new Vector2(1, 1), Color.White);

            spriteBatch.DrawString(GlobalAssets.Arial24, GameManager.Game.Tooltip, new Vector2(GameManager.Game.ScreenSize.X / 2 - GlobalAssets.Arial24.MeasureString(GameManager.Game.Tooltip).X / 2, GameManager.Game.ScreenSize.Y - GlobalAssets.Arial24.MeasureString(GameManager.Game.Tooltip).Y), Color.White);
            spriteBatch.Draw(GameManager.Black, new Rectangle(0, 0, (int)GameManager.Game.ScreenSize.X, (int)GameManager.Game.ScreenSize.Y), new Color(Color.Black, fadeInAlpha));

            spriteBatch.End();
        }
    }
}
