using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Collections;
using Penumbra;
using RPG2D.GameEngine.Entities;
using RPG2D.SGame.Screens;
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
        private bool UseTemplate { get; set; }

        public Bag<Tile> Tiles { get; set; }
        public Bag<Tile> FloorTiles { get; set; }
        public Bag<Tile> Decor { get; set; }
        public Bag<Entities.Entity> Entity { get; set; }
        public WorldTemplate WorldTemplate { get; set; }

        private static bool texturesLoaded = false;
        private KeyboardState old;
        private GamePadState oldState;
        private string worldFile;

        public World(string worldSaveFile, bool v)
        {
            this.worldFile = worldSaveFile;
            Tiles = new Bag<Tile>();
            FloorTiles = new Bag<Tile>();
            Decor = new Bag<Tile>();
            Entity = new Bag<Entities.Entity>();

            if (!texturesLoaded)
            {
                texturesLoaded = true;
                LoadTileset("SGame/Textures/tileset.xml");
                LoadEntityTextures("SGame/Textures/entitys.xml");
                LoadGameItems(GameManager.Game.Content);
                GameManager.Game.InitLighting();

                GameManager.Game.ConsoleInterpreter.RegisterCommand("world", (o) =>
                {
                    Tiles = new Bag<Tile>();
                    FloorTiles = new Bag<Tile>();
                    Decor = new Bag<Tile>();
                    Entity = new Bag<Entities.Entity>();

                    LoadWorld("SGame/Worlds/" + o[0].ToString(), false);
                });
            }

            LoadWorld(worldSaveFile, v);
        }

        public void LoadTemplate(string path)
        {
            // Reset all the layers
            Tiles = new Bag<Tile>();
            FloorTiles = new Bag<Tile>();
            Decor = new Bag<Tile>();
            Entity = new Bag<Entities.Entity>();

            // Create and load the world template
            WorldTemplate = new WorldTemplate();
            WorldTemplate.Name = path;
            WorldTemplate.MainWorld = this.worldFile;

            LoadTemplateFile(path);
        }
        private void LoadTemplateFile(string path)
        {
            // Reset the lighting
            GameManager.Game.Penumbra.Lights.Clear();
            GameManager.Game.Player.InitLighting();
            GameManager.Game.Penumbra.Hulls.Clear();
            ((MainGameScreen)GameManager.Game.GameScreen).InitLighting();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path);

            foreach (XmlNode tile in xmlDocument["world"]["tiles"])
            {
                // Add a tile to the colidable tiles layer
                Tile fTile = ParseTile(tile);

                fTile.TileInfo.Tile = fTile;
                WorldTemplate.ParseTemplateTag(fTile);
                this.Tiles.Add(fTile);
            }
            foreach (XmlNode tile in xmlDocument["world"]["floorTiles"])
            {
                // Add a tile to the floor layer
                this.FloorTiles.Add(ParseTile(tile));
            }
            foreach (XmlNode tile in xmlDocument["world"]["decor"])
            {
                //Custom entitity tiles
                if (tile["textureKey"].InnerText == "torch")
                {
                    // Torch

                    Torch torch = new Torch();
                    torch.X = int.Parse(tile["position"]["x"].InnerText) * 64;
                    torch.Y = int.Parse(tile["position"]["y"].InnerText) * 64;
                    Entity.Add(torch);
                    GameManager.Game.Penumbra.Lights.Add(new PointLight
                    {
                        Position = new Vector2(torch.X + 32, torch.Y + 32),
                        Scale = new Vector2(600),
                        ShadowType = ShadowType.Illuminated,
                        CastsShadows = true,
                        Intensity = 0.5f
                    });
                }
                else
                {
                    // Normal entity-tile
                    Tile fTile = ParseTile(tile);
                    this.Decor.Add(fTile);
                }

            }
            foreach (XmlNode tile in xmlDocument["world"]["entitys"])
            {
                if (GlobalAssets.EntityTextures[tile["textureKey"].InnerText].IsTile)
                {

                    TileEntity fTile = new TileEntity(new Frame(GlobalAssets.EntityTextures[tile["textureKey"].InnerText].Texture));
                    fTile.X = int.Parse(tile["position"]["x"].InnerText) * 64;
                    fTile.Y = int.Parse(tile["position"]["y"].InnerText) * 64;
                    fTile.Texture = GlobalAssets.EntityTextures[tile["textureKey"].InnerText].Texture;
                    fTile.Size = GlobalAssets.EntityTextures[tile["textureKey"].InnerText].Size;
                    fTile.EntityTexture = GlobalAssets.EntityTextures[tile["textureKey"].InnerText];
                    fTile.Colidable = GlobalAssets.EntityTextures[tile["textureKey"].InnerText].Colidable;
                    this.Entity.Add(fTile);

                    fTile.AfterAdd();

                }
                else
                {
                    switch (tile["textureKey"].InnerText.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9'))
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
                            fTile.AfterAdd();
                            break;
                    }
                }


            }
        }

        private void LoadGameItems(ContentManager content)
        {
            // Load in game items.

            // Dagger:
            GlobalAssets.GameItemTextures.Add("dagger0", content.Load<Texture2D>("items/dagger_0"));
            GlobalAssets.GameItemTextures.Add("dagger1", content.Load<Texture2D>("items/dagger_1"));
            GlobalAssets.GameItems.Add("dagger", new GameEngine.Items.DaggerSword_GameItem());
        }

        public void Update(GameTime gameTime)
        {
            // Loop through and update entities.
            foreach (var entity in Entity)
            {
                entity.Update(gameTime);
            }
        }
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Create sorting layers.
            Bag<Tile> abovePlayerTiles = new Bag<Tile>();
            Bag<Tile> belowPlayerTiles = new Bag<Tile>();
            Bag<Entities.Entity> abovePlayerEntities = new Bag<Entity>();
            Bag<Entities.Entity> belowPlayerEntities = new Bag<Entity>();

            // Draw the floor below everything
            foreach (var tile in FloorTiles)
            {
                spriteBatch.Draw(tile.Texture, tile.Bounds, Color.White);
            }

            // Loop through tiles to sort them
            foreach (var tile in Tiles)
            {
                // Dont draw the tile if its a barrier block but do if we are in debug mode
                if ((tile.TileInfo.Texture.Name != "barrier") || GameManager.DebugMode)
                {
                    // Check if the tile is colidable
                    if (tile.Physics)
                        if (tile.Y + 32 > GameManager.Game.Player.Y + 32) // Check if it should be rendered under us
                            belowPlayerTiles.Add(tile);
                        else
                            abovePlayerTiles.Add(tile); // If it is on a higher level then the mid point then it is higher than us
                    else
                        abovePlayerTiles.Add(tile);

                }
            }
            foreach (var tile in Entity)
            {
                // Same with entities.
                if (tile.Y + tile.Texture.Height / 2 > GameManager.Game.Player.Y + 32)
                    belowPlayerEntities.Add(tile);
                else
                    abovePlayerEntities.Add(tile);
            }

            // Loop through everything in oder and draw it to the screen.
            foreach (var tile in abovePlayerTiles) // Above tiles
            {
                spriteBatch.Draw(tile.Texture, tile.Bounds, Color.White);
            }
            foreach (var entity in abovePlayerEntities) // Above entities.
            {
                entity.Draw(gameTime, spriteBatch);
            }

            GameManager.Game.Player.Draw(gameTime, spriteBatch); // Draw player between whats under and above us.

            foreach (var tile in belowPlayerTiles) // Below tiles
            {
                spriteBatch.Draw(tile.Texture, tile.Bounds, Color.White);
            }
            foreach (var entity in belowPlayerEntities) // Below entities.
            {
                entity.Draw(gameTime, spriteBatch);
            }

            // Always draw decor tiles last
            foreach (var tile in Decor)
            {
                spriteBatch.Draw(tile.Texture, tile.Bounds, Color.White);
            }
        }

        public void CheckInteractions(Vector2 pos, Vector2 size)
        {
            // Store keyboard and gamepad states as to use for interaction testing.
            var state = Keyboard.GetState();
            var gamepad = GamePad.GetState(PlayerIndex.One);

            // The hitbox rectangle for testing.
            Rectangle rect = new Rectangle(pos.ToPoint(), size.ToPoint());

            for (int i = 0; i < Tiles.Count; i++) // Loop through tiles
            {
                Tile t = Tiles[i];
                if (rect.Intersects(t.Bounds)) // If we are touching this tile
                {
                    if (t.TileInfo.OnInteract != null) // And if its can be interacted with
                    {
                        if (!GameManager.Game.InstructionDialog.IsGameMode) // Check if we are in dialog mode
                            if (gamepad.IsConnected) // If gamepad is conected, show its variant of the controlls
                                GameManager.Game.Tooltip = "Press B to interact";
                            else // Normal keyboard one.
                                GameManager.Game.Tooltip = "Press E to interact";

                        // Detect if we pressed out interact key on this tile.
                        if (state.IsKeyDown(Keys.E) && old.IsKeyUp(Keys.E) && !gamepad.IsConnected || gamepad.IsButtonDown(Buttons.B) && oldState.IsButtonUp(Buttons.B))
                        {
                            // Invoke the tiles interact function passinhg it into it
                            t.TileInfo.OnInteract.Invoke(t);
                            GameManager.Game.NetworkParser.InteractWith(t); // Update the network
                        }
                    }
                }
            }

            oldState = gamepad;
            old = state;
        }
        public Tuple<bool, Tile> IsSpaceOpen(Vector2 pos, Vector2 size, SpriteBatch s = null)
        {
            // Create a sprite with the collition points size and position
            Sprite spr = new Sprite();
            spr.X = (int)pos.X;
            spr.Y = (int)pos.Y;

            // Create a texture to fill it.
            // TODO: This is very RAM and CPU intensive and should be cleaned out.
            Texture2D tex = new Texture2D(GameManager.Game.GraphicsDevice, (int)size.X, (int)size.Y);
            uint[] pixels = new uint[(int)size.X * (int)size.Y];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Color.Red.PackedValue;
            tex.SetData(pixels);
            spr.Texture = tex;

            // Testing variables.
            bool isOpen = true;
            Tile t = null;

            // Draw the collition point if the passed spritebatch is not null
            s?.Draw(GlobalAssets.WorldTiles["floor"].Texture, new Rectangle(pos.ToPoint(), size.ToPoint()), Color.Blue);

            foreach (Tile tile in Tiles) // Loop through tiles.
            {
                if (spr.CollidesWith(tile)) // Check if we are just generally intersecting it
                {
                    if (tile.Physics) // Check if it is actually collidable
                    {
                        // set testing variables.
                        t = tile;
                        isOpen = false;

                        // Check if this tile needs to use per pixel colition.
                        if (t.TileInfo.UsePPC)
                        {
                            isOpen = !Sprite.PerPixelCollision(spr, tile);
                        }

                    }
                    break;
                }
            }

            // Same with entities.
            foreach (Entity ent in Entity)
            {
                if (spr.CollidesWith(ent) && ent.EntityTexture.Colidable)
                {
                    isOpen = !Sprite.PerPixelCollision(spr, ent);
                    break;
                }
            }

            // Dispose the texture to free RAM.
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
                if (tile.Attributes["ppc"]?.InnerText == "true")
                    ppc = true;


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
                bool isTile = bool.Parse(tile["tileMode"].InnerText);

                GlobalAssets.EntityTextures.Add(name, new Entities.EntityTexture { Texture = texture, Size = new Vector2(width, height), Colidable = colidable, IsTile = isTile });
            }

        }
        public static Tile ParseTile(XmlNode tile)
        {
            Tile fTile = new Tile();

            fTile.X = int.Parse(tile["position"]["x"]?.InnerText) * 64;
            fTile.Y = int.Parse(tile["position"]["y"]?.InnerText) * 64;
            fTile.Texture = GlobalAssets.WorldTiles[tile["textureKey"]?.InnerText]?.Texture;
            fTile.TileInfo = GlobalAssets.WorldTiles[tile["textureKey"]?.InnerText];
            fTile.TileInfo.Tag = tile["tag"]?.InnerText;

            return fTile;
        }
        private void LoadWorld(string file, bool useXml = false)
        {
            GameManager.Game.Penumbra.Lights.Clear();
            GameManager.Game.Penumbra.Hulls.Clear();
            GameManager.Game.Player?.InitLighting();
            ((MainGameScreen)GameManager.Game.GameScreen)?.InitLighting();

            XmlDocument xmlDocument = new XmlDocument();
            if (useXml)
                xmlDocument.LoadXml(file);
            else
                xmlDocument.Load(file);

            foreach (XmlNode tile in xmlDocument["world"]["tiles"])
            {
                Tile fTile = ParseTile(tile);

                fTile.TileInfo.Tag = tile["tag"]?.InnerText;

                fTile.TileInfo.Tile = fTile;
                fTile.TileInfo.SetInteract(tile["textureKey"].InnerText);

                this.Tiles.Add(fTile);
            }
            foreach (XmlNode tile in xmlDocument["world"]["floorTiles"])
            {
                this.FloorTiles.Add(ParseTile(tile));
            }
            foreach (XmlNode tile in xmlDocument["world"]["decor"])
            {
                if (tile["textureKey"].InnerText == "torch")
                {
                    Torch torch = new Torch();
                    torch.X = int.Parse(tile["position"]["x"].InnerText) * 64;
                    torch.Y = int.Parse(tile["position"]["y"].InnerText) * 64;
                    Entity.Add(torch);
                    GameManager.Game.Penumbra.Lights.Add(new PointLight
                    {
                        Position = new Vector2(torch.X + 32, torch.Y + 32),
                        Scale = new Vector2(600),
                        ShadowType = ShadowType.Illuminated,
                        CastsShadows = true,
                        Intensity = 0.5f
                    });
                }
                else
                {
                    Tile fTile = ParseTile(tile);

                    this.Decor.Add(fTile);

                }

            }
            foreach (XmlNode tile in xmlDocument["world"]["entitys"])
            {
                if (GlobalAssets.EntityTextures[tile["textureKey"].InnerText].IsTile)
                {

                    TileEntity fTile = new TileEntity(new Frame(GlobalAssets.EntityTextures[tile["textureKey"].InnerText].Texture));
                    fTile.X = int.Parse(tile["position"]["x"].InnerText) * 64;
                    fTile.Y = int.Parse(tile["position"]["y"].InnerText) * 64;
                    fTile.Texture = GlobalAssets.EntityTextures[tile["textureKey"].InnerText].Texture;
                    fTile.Size = GlobalAssets.EntityTextures[tile["textureKey"].InnerText].Size;
                    fTile.EntityTexture = GlobalAssets.EntityTextures[tile["textureKey"].InnerText];
                    fTile.Colidable = GlobalAssets.EntityTextures[tile["textureKey"].InnerText].Colidable;
                    this.Entity.Add(fTile);

                }
                else
                {
                    switch (tile["textureKey"].InnerText.TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9'))
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