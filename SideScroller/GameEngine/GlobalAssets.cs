using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collections;
using RPG2D.GameEngine.Entities;
using RPG2D.GameEngine.Items;
using RPG2D.GameEngine.World;
using RPG2D.SGame.Screens;
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
        public static Dictionary<string, SoundEffect> SoundEffects = new Dictionary<string, SoundEffect>();
    }


    public class TileInfo
    {
        public Texture2D Texture { get; set; }
        public bool UsePPC { get; set; }
        public Func<Tile, bool> OnInteract;
        public string Tag { get; set; }
        public Tile Tile { get; set; }

        public void SetInteract(string tileName)
        {
            if (tileName == "door0")
            {
                OnInteract = new Func<Tile, bool>((tile) =>
                {

                    if (tile.Texture.Name == "door_0")
                    {
                        GlobalAssets.SoundEffects["doorOpen"].Play();
                        tile.Physics = false;
                        tile.Texture = GlobalAssets.WorldTiles["door1"].Texture;
                    }
                    else
                    {
                        GlobalAssets.SoundEffects["doorClose"].Play();
                        tile.Physics = true;
                        tile.Texture = GlobalAssets.WorldTiles["door0"].Texture;
                    }

                    return true;
                });
            }
            else if (tileName == "floorLadder")
            {
                Tile.Physics = false;

                OnInteract = new Func<Tile, bool>((tile) =>
                {
                    MessageBox.Show(Tag);

                    return true;
                });
            }
            else if (tileName == "wallTile1")
            {
                Tile.Physics = false;
            }

            if (Tag != null)
            {
                if (Tag.StartsWith("gototemplate:"))
                    OnInteract = new Func<Tile, bool>((tile) => {
                        GameManager.Game.World.LoadTemplate("SGame/Worlds/" + Tag.Split(':')[1] + ".xml");
                        ((MainGameScreen)GameManager.Game.GameScreen).FadeIn();
                        return true;
                    });
            }
        }
    }
}
