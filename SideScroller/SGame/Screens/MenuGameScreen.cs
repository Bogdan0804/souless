﻿using Microsoft.Xna.Framework;
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
    public class MenuGameScreen : IGameScreen
    {
        Texture2D titleTexture;
        int SelectedOption = 0;
        int MenuItemsCount = 1;

        KeyboardState oldState;

        public void Init(ContentManager content)
        {
            titleTexture = content.Load<Texture2D>("title");
        }
        public void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up))
            {
                if ((SelectedOption - 1) >= 0)
                    SelectedOption -= 1;
            }
            else if (state.IsKeyDown(Keys.Down) && oldState.IsKeyUp(Keys.Down))
            {
                if ((SelectedOption + 1) <= MenuItemsCount)
                    SelectedOption += 1;
            }


            if (state.IsKeyDown(Keys.Enter) && oldState.IsKeyUp(Keys.Enter))
            {
                switch (SelectedOption)
                {
                    case 0:
                        GameManager.Game.ChangeScreen(new MainGameScreen());
                        break;

                    case 1:
                        Environment.Exit(0);
                        break;
                }
            }

            oldState = state;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GameManager.Game.GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin(samplerState: SamplerState.PointWrap);

            float titleTexX = GameEngine.GameManager.Game.ScreenSize.X / 2 - titleTexture.Width / 2;
            spriteBatch.Draw(titleTexture, new Vector2(titleTexX, 2), Color.White);

            spriteBatch.DrawString(GlobalAssets.Arial24, "Play", new Vector2(GameManager.Game.ScreenSize.X / 2 - GlobalAssets.Arial24.MeasureString("Host").X / 2, 300), Color.Black);
            spriteBatch.DrawString(GlobalAssets.Arial24, "Exit", new Vector2(GameManager.Game.ScreenSize.X / 2 - GlobalAssets.Arial24.MeasureString("Exit.").X / 2, 350), Color.Black);
            spriteBatch.DrawString(GlobalAssets.Arial24, ">", new Vector2(GameManager.Game.ScreenSize.X / 2 - GlobalAssets.Arial24.MeasureString("######").X / 2, 300 + (50 * SelectedOption)), Color.Black);

            spriteBatch.End();
        }
    }
}