using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.GameEngine.Items
{
    public class GameItem
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string TextureName { get; set; }
        public virtual string IconTextureName { get; set; }
        public virtual void Use() { }
    }
}
