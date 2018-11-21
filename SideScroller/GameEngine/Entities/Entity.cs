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
        public Physics CollisionDetection { get; set; }
        public virtual EntityTexture EntityTexture { get; set; }
        public bool UseCollitions = false;

        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)X, (int)Y, (int)Size.X, (int)Size.Y);
            }
        }
        public virtual void AfterAdd() { }
        public Entity(string name, Animation animation)
        {
            this.Animations.Add(name, animation);
            this.CurrentAnimation = name;
            this.CollisionDetection = new Physics();
        }
        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Bounds, Color.White);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (UseCollitions)
                CollisionDetection.UpdateCollitions(this);
        }
    }
}
