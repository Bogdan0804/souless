using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPG2D.GameEngine.Entities
{
    public class Corphafon : Entity
    {
        private bool moving = true;

        public Corphafon()
            :base("walking_left", new Animation(new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_left1")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_left2")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_left3")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_left4")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_left5")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_left6"))))
        {

            Animations.Add("walking_right", new Animation(new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_right1")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_right2")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_right3")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_right4")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_right5")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_right6"))));

        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (this.Bounds.Intersects(GameManager.Game.Player.Bounds))
            {
                if (Sprite.PerPixelCollision(GameManager.Game.Player, this))
                    moving = false;
                else
                    moving = true;
            }


            if (moving)
            {
                if (X < GameManager.Game.Player.Position.X)
                {
                    X += 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    CurrentAnimation = "walking_right";
                }
                else if (X > GameManager.Game.Player.Position.X)
                {
                    X -= 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    CurrentAnimation = "walking_left";
                }

                if (Y < GameManager.Game.Player.Position.Y)
                    Y += 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                else if (Y > GameManager.Game.Player.Position.Y)
                    Y -= 10f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}
