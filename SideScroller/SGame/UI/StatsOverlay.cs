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
    public class StatsOverlay : UIElement
    {
        Texture2D HeartIcon, Overlay, UIItemSelector, SelectedItemsItem;
        int selectedItemIndex = 0;
        MouseState oldState;

        public StatsOverlay()
        {
            Overlay = GameManager.Game.Content.Load<Texture2D>("ui/overlay_stats");
            HeartIcon = GameManager.Game.Content.Load<Texture2D>("ui/heart_ico");
            UIItemSelector = GameManager.Game.Content.Load<Texture2D>("ui/ui_items_inventory");
            SelectedItemsItem = GameManager.Game.Content.Load<Texture2D>("ui/selected_items_item");
        }

        public override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            if (mouseState.ScrollWheelValue > oldState.ScrollWheelValue)
                selectedItemIndex--;
            else if (mouseState.ScrollWheelValue < oldState.ScrollWheelValue)
                selectedItemIndex++;

            base.Update(gameTime);
            oldState = mouseState;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Overlay, Vector2.Zero, Color.White);
            for (int i = 0; i < 7; i++)
            {
                spriteBatch.Draw(HeartIcon, new Vector2(5 + ((i - 1) * 32), 12), Color.White);
            }
            Vector2 uiItemSelectorPos = new Vector2(GameManager.Game.ScreenSize.X / 2 - (UIItemSelector.Width * 1.5f / 2), GameManager.Game.ScreenSize.Y - (UIItemSelector.Height * 1.5f));
            spriteBatch.Draw(UIItemSelector, new Rectangle(uiItemSelectorPos.ToPoint(), new Point((int)(UIItemSelector.Width * 1.5f), (int)(UIItemSelector.Height * 1.5f))), Color.White);

            spriteBatch.Draw(SelectedItemsItem, new Rectangle((int)uiItemSelectorPos.X + (int)(selectedItemIndex * SelectedItemsItem.Width * 1.5f) - ((selectedItemIndex - 1) * 2), (int)uiItemSelectorPos.Y, (int)(SelectedItemsItem.Width * 1.5f), (int)(SelectedItemsItem.Height* 1.5f)), Color.White);

            base.Draw(gameTime, spriteBatch);
        }
    }
}
