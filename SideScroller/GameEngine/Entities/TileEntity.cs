using Microsoft.Xna.Framework;
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

        public override EntityTexture EntityTexture
        {
            get
            {
                return new EntityTexture { Colidable = true, Texture = this.Texture, IsTile = true, Size = Size };
            }
        }

        public TileEntity(params Frame[] frames)
            : base("default", new Animation(frames))
        {
            this.Size = new Vector2(64);
        }
    }
}
