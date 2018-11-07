using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPG2D.GameEngine.UI
{
    public class TextureButton : UIElement
    {
        public Texture2D Texture { get; set; }
        Color color = Color.White;

        public delegate void Click();
        public event Click OnClick;

        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle(Position.ToPoint(), new Point(Texture.Width, Texture.Height));
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Bounds, color);
        }

        MouseState oldState;
        bool click = false;
        public override void Update(GameTime gameTime)
        {
            var state = Mouse.GetState();

            if (Bounds.Contains(state.Position))
                color = Color.Gray;
            else
                color = Color.White;

            if (Bounds.Contains(state.Position) && state.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Released && !click)
            {
                click = true;
                OnClick();
            }
            else
                click = false;

            oldState = state;
        }
    }
}
