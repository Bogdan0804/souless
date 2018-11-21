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
        Texture2D HeartIcon, Overlay, UIItemSelector, SelectedItemsItem, ManaIcon;
        int selectedItemIndex = 0;
        MouseState oldState;
        GamePadState oldGamepadState;

        public StatsOverlay()
        {
            Overlay = GameManager.Game.Content.Load<Texture2D>("ui/overlay_stats");
            HeartIcon = GameManager.Game.Content.Load<Texture2D>("ui/heart_ico");
            ManaIcon = GameManager.Game.Content.Load<Texture2D>("ui/mana_ico");
            UIItemSelector = GameManager.Game.Content.Load<Texture2D>("ui/ui_items_inventory");
            SelectedItemsItem = GameManager.Game.Content.Load<Texture2D>("ui/selected_items_item");
        }

        public override void Update(GameTime gameTime)
        {
            var gamepad = GamePad.GetState(PlayerIndex.One);
            var mouseState = Mouse.GetState();



            if ((mouseState.ScrollWheelValue > oldState.ScrollWheelValue || gamepad.IsButtonDown(Buttons.LeftShoulder) && oldGamepadState.IsButtonUp(Buttons.LeftShoulder)) && selectedItemIndex > 0)
            {
                selectedItemIndex--;
                GameManager.VibrateFor(150, false, true);
            }

            else if ((mouseState.ScrollWheelValue < oldState.ScrollWheelValue || gamepad.IsButtonDown(Buttons.RightShoulder) && oldGamepadState.IsButtonUp(Buttons.RightShoulder)) && selectedItemIndex < 4)
            {
                selectedItemIndex++;
                GameManager.VibrateFor(150, true, false);
            }

            if (mouseState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released && !gamepad.IsConnected || gamepad.IsButtonDown(Buttons.LeftTrigger) && oldGamepadState.IsButtonUp(Buttons.LeftTrigger))
            {
                GameManager.Game.Inventory.Inventory.Items[selectedItemIndex]?.Use();
                GameManager.VibrateFor(200);
            }

            base.Update(gameTime);
            oldState = mouseState;
            oldGamepadState = gamepad;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 uiItemSelectorPos = new Vector2(GameManager.Game.ScreenSize.X / 2 - UIItemSelector.Width * 1.5f, GameManager.Game.ScreenSize.Y - 100);
            spriteBatch.Draw(UIItemSelector, new Rectangle(uiItemSelectorPos.ToPoint(), new Point((int)GameManager.Game.ScreenSize.X / 2, 100)), Color.White);


            Rectangle rect = new Rectangle((int)uiItemSelectorPos.X + 6 + (int)(selectedItemIndex * SelectedItemsItem.Width * 1.5f) - (4 * selectedItemIndex - 1), (int)uiItemSelectorPos.Y + 15, (int)(SelectedItemsItem.Width * 1.5f), (int)(SelectedItemsItem.Height * 1.5f));
            spriteBatch.Draw(SelectedItemsItem, rect, Color.White);

            for (int i = 0; i < 5; i++)
                if (GameManager.Game.Inventory.Inventory.Items[i] != null)
                    spriteBatch.Draw(GlobalAssets.GameItemTextures[GameManager.Game.Inventory.Inventory.Items[i].IconTextureName], new Rectangle((int)uiItemSelectorPos.X + i * 46 + 20, rect.Y + 20, 40, 40), Color.White);

            for (int i = 0; i < 7; i++)
                spriteBatch.Draw(HeartIcon, new Vector2(((i - 1) * 32), 12) + uiItemSelectorPos + new Vector2(85 * 3.4f, 5 * 1.5f), Color.White);

            for (int i = 0; i < 7; i++)
                spriteBatch.Draw(ManaIcon, new Vector2(((i - 1) * 32), 48) + uiItemSelectorPos + new Vector2(85 * 3.4f, 5 * 1.5f), Color.White);

            base.Draw(gameTime, spriteBatch);
        }
    }
}
