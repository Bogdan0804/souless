using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using RPG2D.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.SGame.Player
{
    public class NetworkPlayer : AnimatedSprite
    {
        public Vector2 Size { get; private set; }
        private Hull playerHull;

        public float oldX { get; set; }
        public float oldY { get; set; }

        public NetworkPlayer()
        {
            Size = new Vector2(64);

            Animations.Add("walking_left", new Animation(new Frame(GameManager.Game.Content.Load<Texture2D>("player/player2_left1")), new Frame(GameManager.Game.Content.Load<Texture2D>("player/player2_left2")), new Frame(GameManager.Game.Content.Load<Texture2D>("player/player2_left3"))));
            Animations.Add("walking_right", new Animation(new Frame(GameManager.Game.Content.Load<Texture2D>("player/player2_right1")), new Frame(GameManager.Game.Content.Load<Texture2D>("player/player2_right2")), new Frame(GameManager.Game.Content.Load<Texture2D>("player/player2_right3"))));
            Animations.Add("walking_up", new Animation(new Frame(GameManager.Game.Content.Load<Texture2D>("player/player2_up1")), new Frame(GameManager.Game.Content.Load<Texture2D>("player/player2_up2")), new Frame(GameManager.Game.Content.Load<Texture2D>("player/player2_up3"))));
            Animations.Add("walking_down", new Animation(new Frame(GameManager.Game.Content.Load<Texture2D>("player/player2_down1")), new Frame(GameManager.Game.Content.Load<Texture2D>("player/player2_down2")), new Frame(GameManager.Game.Content.Load<Texture2D>("player/player2_down3"))));
            CurrentAnimation = "walking_down";

            playerHull = new Hull(new Vector2(12, 0), new Vector2(12, 55), new Vector2(14, 58), new Vector2(16, 59), new Vector2(20, 59), new Vector2(26, 56), new Vector2(37, 56), new Vector2(43, 59), new Vector2(47, 59), new Vector2(49, 58), new Vector2(51, 55), new Vector2(51, 0))
            {
                Scale = new Vector2(1),
                Origin = new Vector2(32)
            };

            GameManager.Game.Penumbra.Hulls.Add(playerHull);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GetTexture(), Bounds, Color.White);
        }
        public override void Update(GameTime gameTime)
        {
            float x = GameManager.Game.NetworkParser.X;
            float y = GameManager.Game.NetworkParser.Y;

            playerHull.Position = new Vector2(x + 32, y + 32);

            this.X = x;
            this.Y = y;

            if (y < oldY)
                CurrentAnimation = "walking_up";
            else if (y > oldY)
                CurrentAnimation = "walking_down";


            if (x < oldX)
                CurrentAnimation = "walking_left";
            else if (x > oldX)
                CurrentAnimation = "walking_right";

            oldX = x;
            oldY = y;
            base.Update(gameTime);
        }
    }
}
