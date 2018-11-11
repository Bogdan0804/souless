using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Collections;
using RPG2D.GameEngine;
using RPG2D.GameEngine.Items;
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
        public Inventory Inventory { get; private set; }

        public InventoryUI()
        {
            Background = GameManager.Game.Content.Load<Texture2D>("ui/inventory_background");

            Inventory = new Inventory();
        }

        public override void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();

            if (oldState.IsKeyUp(Keys.Tab) && state.IsKeyDown(Keys.Tab))
            {
                GameManager.Game.InInventory = !GameManager.Game.InInventory;
            }

            oldState = state;
            Inventory.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (GameManager.Game.InInventory)
            {
                int bgWidth = (int)GameManager.Game.ScreenSize.X / 2;
                int bgHeight = (int)(GameManager.Game.ScreenSize.X / 3);
                Inventory.Position = new Vector2(GameManager.Game.ScreenSize.X / 2 - bgWidth / 2, GameManager.Game.ScreenSize.Y / 2 - bgHeight / 2);
                Inventory.Size = new Vector2(bgWidth, bgHeight);

                spriteBatch.Draw(Background, new Rectangle((int)GameManager.Game.ScreenSize.X / 2 - bgWidth / 2, (int)GameManager.Game.ScreenSize.Y / 2 - bgHeight / 2, bgWidth, bgHeight), Color.White);
                Inventory.Draw(gameTime, spriteBatch);
            }
            else
            {
            }

            base.Draw(gameTime, spriteBatch);
        }


    }
    public class Inventory
    {
        Texture2D invItemBackground, SelectedItemsItem;
        MouseState oldState;


        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public int SelectedItem = 0;
        public Bag<GameItem> Items = new Bag<GameItem>(6 * 7);


        public Inventory()
        {
            invItemBackground = new Texture2D(GameManager.Game.GraphicsDevice, 1, 1);
            invItemBackground.SetData<uint>(new uint[] { Color.Gray.PackedValue });
            SelectedItemsItem = GameManager.Game.Content.Load<Texture2D>("ui/selected_items_item");
            Items.Add(new DaggerSword_GameItem());
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var state = Mouse.GetState();
            int padding = 10;
            int initOffset = 20;
            int size = 48;

            for (int y = 0; y < 5; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    int xPos = (int)Position.X + x * size + (padding * (x + 1));
                    Rectangle rect = new Rectangle(xPos + initOffset, (int)Position.Y + 10 + y * size + (padding * y) + initOffset, size, size);
                    Color color = Color.White;
                    int i = x + 6 * y;



                    if (rect.Contains(state.Position))
                        color = Color.LightGray;

                    if (oldState.LeftButton == ButtonState.Released && state.LeftButton == ButtonState.Pressed && rect.Contains(state.Position))
                        SelectedItem = i;

                    if (SelectedItem == i)
                        color = Color.Gray;

                    spriteBatch.Draw(invItemBackground, rect, color);

                    if (Items[i] != null)
                    {
                        var selectedItm = Items[i];
                        spriteBatch.Draw(GlobalAssets.GameItemTextures[selectedItm.IconTextureName], rect, Color.White);
                    }

                    if (SelectedItem == i)
                    {
                        spriteBatch.Draw(SelectedItemsItem, new Rectangle(xPos + initOffset - 5, rect.Y - 5, size + 10, size + 10), Color.White);
                    }
                }
            }

            oldState = state;
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
