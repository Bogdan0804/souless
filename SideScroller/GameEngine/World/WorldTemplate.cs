using RPG2D.SGame.Screens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.GameEngine.World
{
    public class WorldTemplate
    {
        public string Name { get; set; }
        public string MainWorld { get; set; }

        public void ParseTemplateTag(Tile tile)
        {
            if (tile.TileInfo.Tag != null)
            {
                string type = tile.TileInfo.Tag.Split(':')[1];

                if (type == "back")
                {
                    tile.TileInfo.OnInteract = new Func<Tile, bool>((t) => {
                        GameManager.Game.World = new World(MainWorld, false);
                        ((MainGameScreen)GameManager.Game.GameScreen).FadeIn();
                        return true;
                    });
                }
            }
        }
    }
}
