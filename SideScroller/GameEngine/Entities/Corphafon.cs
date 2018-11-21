using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using RPG2D.GameEngine.World;

namespace RPG2D.GameEngine.Entities
{
    public class Corphafon : Entity
    {

        public Corphafon()
            : base("walking_left", new Animation(new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_left1")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_left2")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_left3")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_left4")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_left5"))))
        {

            Animations.Add("walking_right", new Animation(new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_right1")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_right2")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_right3")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_right4")),
                new Frame(GameManager.Game.Content.Load<Texture2D>("entity/corphafon_right5"))));
        }
        
        double timer = 0;
        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
