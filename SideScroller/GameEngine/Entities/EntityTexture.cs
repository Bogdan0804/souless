using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.GameEngine.Entities
{
    public class EntityTexture
    {
        public Texture2D Texture { get; set; }
        public Vector2 Size { get; set; }
        public bool Colidable { get; set; }
        public bool IsTile { get; set; }
    }
}
