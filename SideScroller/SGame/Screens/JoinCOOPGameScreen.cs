using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPG2D.GameEngine;
using RPG2D.GameEngine.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.SGame.Screens
{
    public class JoinCOOPGameScreen : IGameScreen
    {
        public string ip = "";
        public StringBuilder messageLog = new StringBuilder();
        public bool showingDiag = false;
        Texture2D titleTexture;

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GameManager.Game.GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin(samplerState: SamplerState.PointWrap);

            float titleTexX = GameEngine.GameManager.Game.ScreenSize.X / 2 - titleTexture.Width / 2;
            spriteBatch.Draw(titleTexture, new Vector2(titleTexX, 2), Color.White);
            spriteBatch.DrawString(GlobalAssets.Arial24, "IP: " + ip, new Vector2(GameManager.Game.ScreenSize.X / 2 - GlobalAssets.Arial24.MeasureString("IP: " + ip).X / 2, 200), Color.Black);
            spriteBatch.DrawString(GlobalAssets.Arial24, messageLog.ToString(), new Vector2(GameManager.Game.ScreenSize.X / 2 - GlobalAssets.Arial24.MeasureString(messageLog).X / 2, 250), Color.DarkGray);

            spriteBatch.End();
        }

        public void Init(ContentManager content)
        {
            titleTexture = content.Load<Texture2D>("title");

        }
        
        public void Update(GameTime gameTime)
        {
            if (showingDiag == false)
            {
                showingDiag = true;
                ip = KeyboardInput.Show("Server IP", "Enter the ip adress of the server host.", "127.0.0.1").Result;

                GameManager.Game.ChangeScreen(new COOPGameScreen(ip));
            }
        }
    }
}
