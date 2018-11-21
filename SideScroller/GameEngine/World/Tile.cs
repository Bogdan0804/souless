using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.GameEngine.World
{
    public class Tile : Sprite
    {
        public TileInfo TileInfo { get; set; }
        public bool Physics { get; set; }

        public Tile(bool usePhysics=true)
        {
            Physics = usePhysics;
        }
        
        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)X , (int)Y, 64, 64);
            }
        }
    }
}
