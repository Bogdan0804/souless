using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPG2D.GameEngine;
using RPG2D.GameEngine.Screens;
using RPG2D.GameEngine.UI;
using RPG2D.SGame.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.SGame.Screens
{
    public class MenuGameScreen : IGameScreen
    {
        private SoundEffect bgMusic;
        private TextureButton buttonSettings;
        private Texture2D titleTexture, settings_icon;
        private int SelectedOption = 0;
        private int MenuItemsCount = 2;
        SoundEffectInstance mgInstance;
        KeyboardState oldState;
        private Texture2D titleBG;

        public void Init(ContentManager content)
        {
            bgMusic = content.Load<SoundEffect>("sound/menu_bg");
            titleTexture = content.Load<Texture2D>("title");
            settings_icon = content.Load<Texture2D>("ui/settings_gear");
            titleBG = content.Load<Texture2D>("ui/title_bg");
            buttonSettings = new TextureButton();
            buttonSettings.Texture = settings_icon;
            buttonSettings.Position = GameManager.Game.ScreenSize - new Vector2(64);
            buttonSettings.OnClick += () =>
            {
                var settings = new SettingsForm();
                settings.ShowDialog();
            };

            mgInstance = bgMusic.CreateInstance();
            mgInstance.IsLooped = true;
            mgInstance.Play();
        }
        public void Update(GameTime gameTime)
        {
            buttonSettings.Update(gameTime);

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
                        mgInstance.Volume = 0.25f;
                        GameManager.Game.ChangeScreen(new MainGameScreen());
                        break;

                    case 1:
                        mgInstance.Volume = 0.25f;
                        GameManager.Game.ChangeScreen(new JoinCOOPGameScreen());
                        break;

                    case 2:
                        Environment.Exit(0);
                        break;
                }
            }

            oldState = state;
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GameManager.Game.GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            spriteBatch.Draw(titleBG, new Rectangle(0, 0, (int)GameManager.Game.ScreenSize.X, (int)GameManager.Game.ScreenSize.Y), Color.White);
            float titleTexX = GameEngine.GameManager.Game.ScreenSize.X / 2 - titleTexture.Width / 2;
            spriteBatch.Draw(titleTexture, new Vector2(titleTexX, 2), Color.White);

            spriteBatch.DrawString(GlobalAssets.Arial24, "Play", new Vector2(GameManager.Game.ScreenSize.X / 2 - GlobalAssets.Arial24.MeasureString("Play").X / 2, 300), Color.White);
            spriteBatch.DrawString(GlobalAssets.Arial24, "COOP", new Vector2(GameManager.Game.ScreenSize.X / 2 - GlobalAssets.Arial24.MeasureString("Play").X / 2, 350), Color.White);
            spriteBatch.DrawString(GlobalAssets.Arial24, "Exit", new Vector2(GameManager.Game.ScreenSize.X / 2 - GlobalAssets.Arial24.MeasureString("Exit.").X / 2, 400), Color.White);
            spriteBatch.DrawString(GlobalAssets.Arial24, ">", new Vector2(GameManager.Game.ScreenSize.X / 2 - GlobalAssets.Arial24.MeasureString("######").X / 2, 300 + (50 * SelectedOption)), Color.White);
            buttonSettings.Draw(gameTime, spriteBatch);

            spriteBatch.End();
        }
    }
}
