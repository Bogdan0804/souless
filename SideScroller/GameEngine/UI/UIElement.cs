using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.GameEngine.UI
{
    public class UIElement
    {
        public virtual Rectangle Bounds { get; set; }
        public virtual Vector2 Position { get; set; }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) { }
        public virtual void Update(GameTime gameTime) { }
    }
}
