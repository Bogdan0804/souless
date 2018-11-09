using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPG2D.GameEngine;
using RPG2D.GameEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.SGame.UI
{
    public class InventoryUI : UIElement
    {
        Texture2D Background;
        KeyboardState oldState;


        public InventoryUI()
        {
            Background = GameManager.Game.Content.Load<Texture2D>("ui/inventory_background");
        }

        public override void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();

            if (oldState.IsKeyUp(Keys.Tab) && state.IsKeyDown(Keys.Tab))
                GameManager.Game.InInventory = !GameManager.Game.InInventory;

            oldState = state;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (GameManager.Game.InInventory)
            {
                int bgWidth = (int)GameManager.Game.ScreenSize.X / 2;
                int bgHeight = (int)(GameManager.Game.ScreenSize.X / 3);

                spriteBatch.Draw(Background, new Rectangle((int)GameManager.Game.ScreenSize.X / 2 - bgWidth / 2, (int)GameManager.Game.ScreenSize.Y / 2 - bgHeight / 2, bgWidth, bgHeight), Color.White);
            }

            base.Draw(gameTime, spriteBatch);
        }
    }
}
