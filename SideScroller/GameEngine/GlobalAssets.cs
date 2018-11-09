using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using RPG2D.GameEngine.Entities;
using RPG2D.GameEngine.Items;
using RPG2D.GameEngine.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RPG2D.GameEngine
{
    public static class GlobalAssets
    {
        public static SpriteFont Arial12, Arial24;
        public static Dictionary<string, TileInfo> WorldTiles = new Dictionary<string, TileInfo>();
        public static Dictionary<string, EntityTexture> EntityTextures = new Dictionary<string, EntityTexture>();
        public static Dictionary<string, Texture2D> GameItemTextures = new Dictionary<string, Texture2D>();
        public static Dictionary<string, GameItem> GameItems = new Dictionary<string, GameItem>();
    }


    public class TileInfo
    {
        public Texture2D Texture { get; set; }
        public bool UsePPC { get; set; }
        public Func<Tile, bool> OnInteract;
        public int ID { get; set; }

        public void SetInteract(string tileName)
        {
            if (tileName == "door0")
            {
                OnInteract = new Func<Tile, bool>((tile) =>
                {

                    if (tile.Texture.Name == "door_0")
                    {
                        tile.Physics = false;
                        tile.Texture = GlobalAssets.WorldTiles["door1"].Texture;
                    }
                    else
                    {
                        tile.Physics = true;
                        tile.Texture = GlobalAssets.WorldTiles["door0"].Texture;
                    }

                    return true;
                });
            }
        }
    }
}
