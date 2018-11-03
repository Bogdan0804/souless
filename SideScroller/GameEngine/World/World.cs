using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Collections;
using RPG2D.GameEngine.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RPG2D.GameEngine.World
{
    public class World
    {
        public Bag<Tile> Tiles { get; set; }
        public Bag<Tile> FloorTiles { get; set; }
        public Bag<Tile> Decor { get; set; }
        public Bag<Entities.Entity> Entity { get; set; }
        private static bool texturesLoaded = false;

        int renderDepthState = 0;

        public World(string worldSaveFile)
        {
            Tiles = new Bag<Tile>();
            FloorTiles = new Bag<Tile>();
            Decor = new Bag<Tile>();
            Entity = new Bag<Entities.Entity>();

            if (!texturesLoaded)
            {
                texturesLoaded = true;
                LoadTileset("SGame/Textures/tileset.xml");
                LoadEntityTextures("SGame/Textures/entitys.xml");
            }

            LoadWorld("SGame/Worlds/" + worldSaveFile);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entity in Entity)
            {
                entity.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var tile in FloorTiles)
            {
                spriteBatch.Draw(tile.Texture, tile.Bounds, Color.White);
            }

            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.S))
                renderDepthState = 1;
            else if (state.IsKeyDown(Keys.W))
                renderDepthState = 0;

            if (renderDepthState == 0)
            {
                foreach (var tile in Tiles)
                {
                    spriteBatch.Draw(tile.Texture, tile.Bounds, Color.White);
                }
                foreach (var tile in Entity)
                {
                    tile.Draw(gameTime, spriteBatch);
                }
                foreach (var tile in Decor)
                {
                    spriteBatch.Draw(tile.Texture, tile.Bounds, Color.White);
                }
                GameManager.Game.Player.Draw(gameTime, spriteBatch);

            }
            else
            {
                GameManager.Game.Player.Draw(gameTime, spriteBatch);

                foreach (var tile in Tiles)
                {
                    spriteBatch.Draw(tile.Texture, tile.Bounds, Color.White);
                }
                foreach (var tile in Entity)
                {
                    tile.Draw(gameTime, spriteBatch);
                }
                foreach (var tile in Decor)
                {
                    spriteBatch.Draw(tile.Texture, tile.Bounds, Color.White);
                }

            }


        }
        KeyboardState old;

        public void CheckInteractions(Vector2 pos, Vector2 size)
        {
            var state = Keyboard.GetState();

            Rectangle rect = new Rectangle(pos.ToPoint(), size.ToPoint());


            for (int i = 0; i < Tiles.Count; i++)
            {
                Tile t = Tiles[i];
                if (rect.Intersects(t.Bounds))
                {
                    if (t.TileInfo.OnInteract != null)
                    {
                        GameManager.Game.Tooltip = "Press E to interact";
                        if (state.IsKeyDown(Keys.E) && old.IsKeyUp(Keys.E))
                            t.TileInfo.OnInteract.Invoke(Tiles[i]);
                    }
                }
            }

            old = state;
        }

        public Tuple<bool, Tile> IsSpaceOpen(Vector2 pos, Vector2 size, SpriteBatch s = null)
        {
            Sprite spr = new Sprite();
            spr.X = (int)pos.X;
            spr.Y = (int)pos.Y;

            Texture2D tex = new Texture2D(GameManager.Game.GraphicsDevice, (int)size.X, (int)size.Y);
            uint[] pixels = new uint[(int)size.X * (int)size.Y];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Color.Red.PackedValue;
            tex.SetData(pixels);

            spr.Texture = tex;

            bool isOpen = true;
            Tile t = null;

            s?.Draw(GlobalAssets.WorldTiles["floor"].Texture, new Rectangle(pos.ToPoint(), size.ToPoint()), Color.Blue);

            for (int i = 0; i < Tiles.Count; i++)
            {

                if (spr.CollidesWith(Tiles[i]))
                {
                    if (Tiles[i].Physics)
                    {
                        t = Tiles[i];
                        isOpen = false;

                        if (t.TileInfo.UsePPC)
                        {
                            isOpen = !Sprite.PerPixelCollision(spr, Tiles[i]);
                        }

                    }
                    break;
                }
            }
            for (int i = 0; i < Entity.Count; i++)
            {

                if (spr.CollidesWith(Entity[i]) && Entity[i].EntityTexture.Colidable)
                {
                    isOpen = !Sprite.PerPixelCollision(spr, Entity[i]);
                    break;
                }
            }

            tex.Dispose();

            return new Tuple<bool, Tile>(isOpen, t);
        }

        private void LoadTileset(string file)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(file);

            foreach (XmlNode tile in xmlDocument["tileset"])
            {
                Texture2D texture = GameManager.Game.Content.Load<Texture2D>(tile["file"].InnerText);
                string name = tile["name"].InnerText;

                bool ppc = false;
                try
                {
                    if (tile.Attributes["ppc"].InnerText == "true")
                        ppc = true;
                }
                catch { }

                GlobalAssets.WorldTiles.Add(name, new TileInfo { Texture = texture, UsePPC = ppc });
            }

        }
        private void LoadEntityTextures(string file)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(file);

            foreach (XmlNode tile in xmlDocument["tileset"])
            {
                Texture2D texture = GameManager.Game.Content.Load<Texture2D>("entity/" + tile["file"].InnerText);
                string name = tile["name"].InnerText;

                int width = int.Parse(tile["size"]["width"].InnerText);
                int height = int.Parse(tile["size"]["height"].InnerText);
                bool colidable = bool.Parse(tile["colidable"].InnerText);

                GlobalAssets.EntityTextures.Add(name, new Entities.EntityTexture { Texture = texture, Size = new Vector2(width, height), Colidable = colidable });
            }

        }

        private void LoadWorld(string file)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(file);

            foreach (XmlNode tile in xmlDocument["world"]["tiles"])
            {
                Tile fTile = new Tile();
                fTile.X = int.Parse(tile["position"]["x"].InnerText) * 64;
                fTile.Y = int.Parse(tile["position"]["y"].InnerText) * 64;
                fTile.Texture = GlobalAssets.WorldTiles[tile["textureKey"].InnerText].Texture;
                fTile.TileInfo = GlobalAssets.WorldTiles[tile["textureKey"].InnerText];
                try
                {
                    fTile.TileInfo.ID = int.Parse(tile.Attributes["id"].InnerText);
                }
                catch
                {
                    fTile.TileInfo.ID = -1;
                }

                fTile.TileInfo.SetInteract(tile["textureKey"].InnerText);

                this.Tiles.Add(fTile);
            }
            foreach (XmlNode tile in xmlDocument["world"]["floorTiles"])
            {
                Tile fTile = new Tile();
                fTile.X = int.Parse(tile["position"]["x"].InnerText) * 64;
                fTile.Y = int.Parse(tile["position"]["y"].InnerText) * 64;
                fTile.Texture = GlobalAssets.WorldTiles[tile["textureKey"].InnerText].Texture;
                fTile.TileInfo = GlobalAssets.WorldTiles[tile["textureKey"].InnerText];

                this.FloorTiles.Add(fTile);
            }
            foreach (XmlNode tile in xmlDocument["world"]["decor"])
            {
                Tile fTile = new Tile();
                fTile.X = int.Parse(tile["position"]["x"].InnerText) * 64;
                fTile.Y = int.Parse(tile["position"]["y"].InnerText) * 64;
                fTile.Texture = GlobalAssets.WorldTiles[tile["textureKey"].InnerText].Texture;
                fTile.TileInfo = GlobalAssets.WorldTiles[tile["textureKey"].InnerText];

                this.Decor.Add(fTile);
            }
            foreach (XmlNode tile in xmlDocument["world"]["entitys"])
            {
                if (GlobalAssets.EntityTextures[tile["textureKey"].InnerText].Colidable)
                {
                    TileEntity fTile = new TileEntity(GlobalAssets.EntityTextures[tile["textureKey"].InnerText].Texture);
                    fTile.X = int.Parse(tile["position"]["x"].InnerText) * 64;
                    fTile.Y = int.Parse(tile["position"]["y"].InnerText) * 64;
                    fTile.Texture = GlobalAssets.EntityTextures[tile["textureKey"].InnerText].Texture;
                    fTile.Size = GlobalAssets.EntityTextures[tile["textureKey"].InnerText].Size;
                    fTile.EntityTexture = GlobalAssets.EntityTextures[tile["textureKey"].InnerText];
                    this.Entity.Add(fTile);
                }
                else
                {
                    switch (tile["entityType"].InnerText)
                    {
                        case "corphafon":
                            Corphafon fTile = new Corphafon();
                            fTile.X = int.Parse(tile["position"]["x"].InnerText) * 64;
                            fTile.Y = int.Parse(tile["position"]["y"].InnerText) * 64;
                            fTile.Texture = GlobalAssets.EntityTextures[tile["textureKey"].InnerText].Texture;
                            fTile.Size = GlobalAssets.EntityTextures[tile["textureKey"].InnerText].Size;
                            fTile.EntityTexture = GlobalAssets.EntityTextures[tile["textureKey"].InnerText];
                            fTile.Texture = fTile.EntityTexture.Texture;
                            this.Entity.Add(fTile);
                            break;
                    }
                }


            }
        }
    }
}
