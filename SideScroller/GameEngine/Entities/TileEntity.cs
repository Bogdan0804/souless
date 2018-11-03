using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.GameEngine.Entities
{
    public class TileEntity : Entity
    {
        public bool Colidable { get; set; }

        public TileEntity(Texture2D texture)
            :base("default", new Animation(new Frame(texture)))
        {

        }
    }
}
