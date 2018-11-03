using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.GameEngine
{
    public class Sprite
    {
        public virtual Texture2D Texture { get; set; }
        public float X = 0;
        public float Y = 0;
        public int ZDepth { get; set; }

        public Sprite()
        {
            ZDepth = 0;
        }

        public virtual Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    ((int)X),
                    ((int)Y),
                    Texture.Width,
                    Texture.Height);
            }

        }


        public bool CollidesWith(Sprite other)
        {
            return CollidesWith(other, true);
        }

        public bool CollidesWith(Sprite other, bool calcPerPixel)
        {
            // Get dimensions of texture
            int widthOther = other.Bounds.Width;
            int heightOther = other.Bounds.Height;
            int widthMe = Bounds.Width;
            int heightMe = Bounds.Height;

            if (calcPerPixel &&                                // if we need per pixel
                ((Math.Min(widthOther, heightOther) > 100) ||  // at least avoid doing it
                (Math.Min(widthMe, heightMe) > 100)))          // for small sizes (nobody will notice :P)
            {
                return Bounds.Intersects(other.Bounds) // If simple intersection fails, don't even bother with per-pixel
                    && PerPixelCollision(this, other);
            }

            return Bounds.Intersects(other.Bounds);
        }

        public static bool PerPixelCollision(Sprite a, Sprite b)
        {
            // Get Color data of each Texture
            Color[] bitsA = new Color[a.Bounds.Width * a.Bounds.Height];
            a.Texture.GetData(bitsA);
            Color[] bitsB = new Color[b.Bounds.Width * b.Bounds.Height];
            b.Texture.GetData(bitsB);

            // Calculate the intersecting rectangle
            int x1 = Math.Max(a.Bounds.X, b.Bounds.X);
            int x2 = Math.Min(a.Bounds.X + a.Bounds.Width, b.Bounds.X + b.Bounds.Width);

            int y1 = Math.Max(a.Bounds.Y, b.Bounds.Y);
            int y2 = Math.Min(a.Bounds.Y + a.Bounds.Height, b.Bounds.Y + b.Bounds.Height);

            // For each single pixel in the intersecting rectangle
            for (int y = y1; y < y2; ++y)
            {
                for (int x = x1; x < x2; ++x)
                {
                    // Get the color from each texture
                    Color ac = bitsA[(x - a.Bounds.X) + (y - a.Bounds.Y) * a.Bounds.Width];
                    Color bc = bitsB[(x - b.Bounds.X) + (y - b.Bounds.Y) * b.Bounds.Width];

                    if (ac.A != 0 && bc.A != 0) // If both colors are not transparent (the alpha channel is not 0), then there is a collision
                    {
                        return true;
                    }
                }
            }
            // If no collision occurred by now, we're clear.
            return false;
        }
    }
}
