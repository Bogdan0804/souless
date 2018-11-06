using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.GameEngine.Entities
{
    public  class Torch : TileEntity
    {
        public Torch()
            : base(new Frame(GameManager.Game.Content.Load<Texture2D>("torch0")), new Frame(GameManager.Game.Content.Load<Texture2D>("torch1")), new Frame(GameManager.Game.Content.Load<Texture2D>("torch2")))
        {
            
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
        }
    }
}
