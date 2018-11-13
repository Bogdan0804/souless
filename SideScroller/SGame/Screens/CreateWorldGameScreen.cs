using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using RPG2D.GameEngine;
using RPG2D.GameEngine.Screens;
using RPG2D.GameEngine.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.SGame.Screens
{
    public class CreateWorldGameScreen : IGameScreen
    {
        public void Init(ContentManager content)
        {
            GameManager.Game.World = new World("SGame/Worlds/world1.xml", false);

            Tile tile_bricks_0 = new Tile() {
                Physics=true,
                TextureName = "wallTile1",
                TileInfo= GlobalAssets.WorldTiles["wallTile1"], 
            };

            string mapContents = System.IO.File.ReadAllText("SGame/Worlds/map.txt");
            for (int y = 0; y < mapContents.Split('\n').Length; y++)
            {
                string line = mapContents.Split('\n')[y];

                for (int x = 0; x < line.Split('\t').Length; x++)
                {
                    string c = line.Split('\t')[x];
                    

                    if (c == "")
                    {


                        var tile = tile_bricks_0.Clone();

                        tile.X = x * 64;
                        tile.Y = y * 64;
                        GameManager.Game.World.Tiles.Add(tile);
                    }
                }
                
            }

            GameManager.Game.ChangeScreen(new MainGameScreen());
        }

        public void Update(GameTime gameTime)
        {

        }
        
    
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            GameManager.Game.GraphicsDevice.Clear(new Color(28, 17, 23));
            spriteBatch.Begin(samplerState: SamplerState.PointWrap);

            spriteBatch.DrawString(GlobalAssets.Arial24, "Generating World", new Vector2(GameManager.Game.ScreenSize.X / 2, GameManager.Game.ScreenSize.Y / 4) - GlobalAssets.Arial24.MeasureString("Generating World") / 2, Color.White);

            spriteBatch.End();

        }
    }
}
