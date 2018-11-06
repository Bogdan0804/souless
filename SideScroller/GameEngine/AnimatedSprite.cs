using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.GameEngine
{
    public class AnimatedSprite : Sprite
    {
        public Dictionary<string, Animation> Animations = new Dictionary<string, Animation>();
        public string CurrentAnimation = "";

        public override Texture2D Texture { get { return GetTexture(); } }

        public bool Animate = true;

        public Frame CurrentAnimationFrame
        {
            get
            {
                return Animations[CurrentAnimation].GetFrame();
            }
        }
        public Texture2D GetTexture()
        {
            return CurrentAnimationFrame.Texture;
        }
        public virtual void Update(GameTime gameTime)
        {
            Animations[CurrentAnimation].Update(gameTime, CurrentAnimationFrame.FrameTime);
        }
    }
}
