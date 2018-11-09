using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.GameEngine
{
    public class Spritesheet
    {
        public string Path
        {
            get;
            set;
        }

        public Microsoft.Xna.Framework.Vector2 TileSize
        {
            get;
            set;
        }

        public Spritesheet(string path, int tileSize)
        {
            this.Path = path;
            this.TileSize = new Microsoft.Xna.Framework.Vector2(tileSize);
        }
        public Spritesheet(string path, int x, int y)
        {
            this.Path = path;
            this.TileSize = new Microsoft.Xna.Framework.Vector2(x, y);
        }

        public Texture2D GetSprite(int xPos, int yPos)
        {
            var img = GameManager.Game.Content.Load<Texture2D>(this.Path);

            /// Get the data from the original texture and place it in an array
            Bitmap o = new Bitmap((int)img.Width, (int)img.Height);
            Microsoft.Xna.Framework.Color[] colorData = new Microsoft.Xna.Framework.Color[img.Width * img.Height];
            img.GetData(colorData);

            for (int x = 0; x < o.Width; x++)
            {
                for (int y = 0; y < o.Height; y++)
                {
                    o.SetPixel(x, y, System.Drawing.Color.FromArgb(colorData[x * o.Height + y].A, colorData[x * o.Height + y].R, colorData[x * o.Height + y].G, colorData[x * o.Height + y].B));
                }
            }


            Bitmap g = new Bitmap((int)TileSize.X, (int)TileSize.Y);
            for (int x = xPos; x < (int)TileSize.X - xPos; x++)
            {
                for (int y = yPos; y < (int)TileSize.Y - yPos; y++)
                {
                    g.SetPixel(x, y, (o.GetPixel(x, y)));
                }
            }

            return GetTexture(GameManager.Game.GraphicsDevice, g);
        }
        private Texture2D GetTexture(GraphicsDevice dev, System.Drawing.Bitmap bmp)
        {
            int[] imgData = new int[bmp.Width * bmp.Height];
            Texture2D texture = new Texture2D(dev, bmp.Width, bmp.Height);

            unsafe
            {
                // lock bitmap
                System.Drawing.Imaging.BitmapData origdata =
                    bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

                uint* byteData = (uint*)origdata.Scan0;
                

                // copy data
                System.Runtime.InteropServices.Marshal.Copy(origdata.Scan0, imgData, 0, bmp.Width * bmp.Height);

                byteData = null;

                // unlock bitmap
                bmp.UnlockBits(origdata);
            }

            texture.SetData(imgData);

            return texture;
        }
    }
}
