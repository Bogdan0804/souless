using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPG2D.GameEngine.Entities
{
    public class Entity : AnimatedSprite
    {
        public Vector2 Size { get; set; }
        public EntityTexture EntityTexture { get; set; }

        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)X, (int)Y, (int)Size.X, (int)Size.Y);
            }
        }

        public Entity(string name, Animation animation)
        {
            this.Animations.Add(name, animation);
            this.CurrentAnimation = name;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Bounds, Color.White);
        }
    }
}
