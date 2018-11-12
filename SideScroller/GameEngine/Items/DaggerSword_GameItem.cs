using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.GameEngine.Items
{
    public class DaggerSword_GameItem : GameItem
    {
        public override int ID { get => 1; }
        public override string IconTextureName { get => "dagger0"; }
        public override string TextureName { get => "dagger0"; }
        public override string Name { get => "Dagger"; }
        public override void Use()
        {
            Console.WriteLine("DJ Kahled");
        }
    }
}
