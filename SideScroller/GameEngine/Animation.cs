using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.GameEngine
{
    public class Animation
    {
        // Store all animations in a list
        public Bag<Frame> Frames = new Bag<Frame>();


        // The frame we are curently on
        public int Frame = 0;

        // The time past since we last changed frames
        private double TimePastSinceUpdate = 0;

        /// <summary>
        /// Initializes a new instance of the animation class.
        /// </summary>
        public Animation(params Frame[] frames)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                this.Frames.Add(frames[i]);
            }
        }

        /// Updates the frames
        public void Update(GameTime gameTime, float frameTime)
        {
            // Add time to the time
            TimePastSinceUpdate += gameTime.ElapsedGameTime.TotalSeconds;

            // CHeck if the time has passed and change the frame
            if (TimePastSinceUpdate > frameTime)
            {
                TimePastSinceUpdate = 0;

                if (Frame < Frames.Count - 1)
                    Frame++;
                else
                    Frame = 0;
            }
        }

        /// <summary>
        /// Gets the frame.
        /// </summary>
        /// <returns>The frame.</returns>
        public Frame GetFrame()
        {
            return Frames[Frame];
        }
    }
    /// A basic class the can store a texture and a point of origin
    public class Frame
    {
        public float FrameTime { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Origin
        {
            get
            {
                return new Vector2(Texture.Width / 2, Texture.Height / 2);
            }
        }

        public Frame(Texture2D texture, float frameTime = 0.100f)
        {
            this.Texture = texture;
            this.FrameTime = frameTime;
        }
    }
}
