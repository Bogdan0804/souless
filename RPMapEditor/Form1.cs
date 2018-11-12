using MonoGame.Extended.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace RPMapEditor
{
    public partial class Form1 : Form
    {
        Form previewForm = new Form();

        int x = 0;
        int y = 0;
        int size = 32;
        Graphics gPreview;
        bool saved = true;
        string loadedFile = "";
        string tileset = "";
        string entitiesset = "";
        string workingDir = @"C:\Users\Bogdan\Desktop\game\";
        int editingLayer = 0;

        Dictionary<string, Image> textures = new Dictionary<string, Image>();
        Dictionary<string, EntityInfo> entitytextures = new Dictionary<string, EntityInfo>();

        public Bag<Tile> Tiles = new Bag<Tile>();
        public Bag<Tile> Floor = new Bag<Tile>();
        public Bag<Tile> Decor = new Bag<Tile>();
        public Bag<Entity> Entitys = new Bag<Entity>();

        public Form1()
        {
            InitializeComponent();
            //drawPanel.BackColor = (Color.FromArgb(66, 40, 53));
            layersListBox.Items.Add("Tiles");
            layersListBox.Items.Add("Floor");
            layersListBox.Items.Add("Decor");

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            {
                FolderBrowserDialog diagf = new FolderBrowserDialog();

                if (diagf.ShowDialog() == DialogResult.OK)
                {
                    workingDir = diagf.SelectedPath;
                }
            }

            OpenFileDialog diag = new OpenFileDialog();
            diag.Filter = "RPG2D Engine Map File|*.xml";

            if (diag.ShowDialog() == DialogResult.OK)
            {
                loadedFile = diag.FileName;
                XmlDocument doc = new XmlDocument();
                doc.Load(loadedFile);
                tileset = doc["world"]["tileset"].InnerText;
                entitiesset = doc["world"]["entityset"].InnerText;

                LoadMap(loadedFile, tileset);
            }
        }

        private void LoadMap(string file, string tileset)
        {
            tileset = workingDir + tileset;
            XmlDocument tileDoc = new XmlDocument();
            tileDoc.Load(tileset);
            dataGridView1.Columns.Add("data", "Name");
            dataGridView1.Columns.Add("img", "Preview");

            foreach (XmlNode tile in tileDoc["tileset"])
            {
                Image texture = Image.FromFile(workingDir + @"\Assets\" + tile["file"].InnerText + ".png");

                string name = tile["name"].InnerText;

                textures.Add(name, texture);

                dataGridView1.Rows.Add(new object[] { name });
            }

            tileDoc = new XmlDocument();
            tileDoc.Load(workingDir + entitiesset);

            foreach (XmlNode tile in tileDoc["tileset"])
            {
                Image texture = Image.FromFile(workingDir + @"\Assets\entity\" + tile["file"].InnerText + ".png");

                string name = tile["name"].InnerText;
                int width = int.Parse(tile["size"]["width"].InnerText);
                int height = int.Parse(tile["size"]["height"].InnerText);

                entitytextures.Add(name, new EntityInfo { Height = height, Width = Width, Texture = texture, Name = name });
            }

            tileDoc = new XmlDocument();
            tileDoc.Load(file);

            foreach (XmlNode tile in tileDoc["world"]["tiles"])
            {
                Tile fTile = new Tile();
                fTile.X = int.Parse(tile["position"]["x"].InnerText);
                fTile.Y = int.Parse(tile["position"]["y"].InnerText);
                fTile.Texture = textures[tile["textureKey"].InnerText];
                fTile.TextureName = tile["textureKey"].InnerText;
                try
                {
                    fTile.ID = int.Parse(tile.Attributes["id"].InnerText);
                }
                catch
                {
                    fTile.ID = -1;
                }

                this.Tiles.Add(fTile);
            }
            foreach (XmlNode tile in tileDoc["world"]["floorTiles"])
            {
                Tile fTile = new Tile();
                fTile.X = int.Parse(tile["position"]["x"].InnerText);
                fTile.Y = int.Parse(tile["position"]["y"].InnerText);
                fTile.Texture = textures[tile["textureKey"].InnerText];
                fTile.TextureName = tile["textureKey"].InnerText;

                this.Floor.Add(fTile);
            }
            foreach (XmlNode tile in tileDoc["world"]["decor"])
            {
                Tile fTile = new Tile();
                fTile.X = int.Parse(tile["position"]["x"].InnerText);
                fTile.Y = int.Parse(tile["position"]["y"].InnerText);
                fTile.Texture = textures[tile["textureKey"].InnerText];
                fTile.TextureName = tile["textureKey"].InnerText;

                this.Decor.Add(fTile);
            }
            foreach (XmlNode tile in tileDoc["world"]["entitys"])
            {
                Entity fTile = new Entity();
                fTile.X = int.Parse(tile["position"]["x"].InnerText);
                fTile.Y = int.Parse(tile["position"]["y"].InnerText);
                fTile.Texture = entitytextures[tile["textureKey"].InnerText].Texture;
                fTile.TextureName = tile["textureKey"].InnerText;

                this.Entitys.Add(fTile);
            }
            DrawTiles(editingLayer);
        }

        private void DrawTiles(int layer)
        {
            var g = drawPanel.CreateGraphics();
            g.Clear(Color.FromArgb(28, 17, 23));

            if (layer == 0)
            {
                foreach (var tile in Tiles)
                {
                    g.DrawImage(tile.Texture, new Rectangle(tile.X * 16, tile.Y * 16, 16, 16));
                }
            }
            else if (layer == 1)
            {
                foreach (var tile in Floor)
                {
                    g.DrawImage(tile.Texture, new Rectangle(tile.X * 16, tile.Y * 16, 16, 16));
                }
            }
            else if (layer == 2)
            {
                foreach (var tile in Decor)
                {
                    g.DrawImage(tile.Texture, new Rectangle(tile.X * 16, tile.Y * 16, 16, 16));
                }
            }
            foreach (var tile in Entitys)
            {
                g.DrawImage(tile.Texture, new Rectangle(tile.X * 16, tile.Y * 16, tile.Texture.Width / 8, tile.Texture.Height / 8));
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
            saved = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!saved)
                Save();

            Environment.Exit(0);
        }

        private void Save()
        {
            XmlDocument doc = new XmlDocument();

            var root = doc.CreateElement("world");

            var tileset = doc.CreateElement("tileset");
            tileset.InnerText = this.tileset;

            var entityset = doc.CreateElement("entityset");
            entityset.InnerText = this.entitiesset;

            root.AppendChild(tileset);
            root.AppendChild(entityset);

            var tiles = doc.CreateElement("tiles");
            foreach (var tile in Tiles)
            {
                var tileNode = doc.CreateElement("tile");
                var position = doc.CreateElement("position");
                var xNode = doc.CreateElement("x");
                xNode.InnerText = tile.X.ToString();
                var yNode = doc.CreateElement("y");
                yNode.InnerText = tile.Y.ToString();
                position.AppendChild(xNode);
                position.AppendChild(yNode);
                var textureKey = doc.CreateElement("textureKey");
                textureKey.InnerText = tile.TextureName;

                if (tile.ID > -1)
                {
                    var atr = doc.CreateAttribute("id");
                    atr.InnerText = tile.ID.ToString();
                    tileNode.Attributes.Append(atr);
                }

                tileNode.AppendChild(position);
                tileNode.AppendChild(textureKey);

                tiles.AppendChild(tileNode);
            }

            var floorTiles = doc.CreateElement("floorTiles");
            foreach (var tile in Floor)
            {
                var tileNode = doc.CreateElement("tile");
                var position = doc.CreateElement("position");
                var xNode = doc.CreateElement("x");
                xNode.InnerText = tile.X.ToString();
                var yNode = doc.CreateElement("y");
                yNode.InnerText = tile.Y.ToString();
                position.AppendChild(xNode);
                position.AppendChild(yNode);
                var textureKey = doc.CreateElement("textureKey");
                textureKey.InnerText = tile.TextureName;


                tileNode.AppendChild(position);
                tileNode.AppendChild(textureKey);

                floorTiles.AppendChild(tileNode);
            }

            var decorTiles = doc.CreateElement("decor");
            foreach (var tile in Decor)
            {
                var tileNode = doc.CreateElement("tile");
                var position = doc.CreateElement("position");
                var xNode = doc.CreateElement("x");
                xNode.InnerText = tile.X.ToString();
                var yNode = doc.CreateElement("y");
                yNode.InnerText = tile.Y.ToString();
                position.AppendChild(xNode);
                position.AppendChild(yNode);
                var textureKey = doc.CreateElement("textureKey");
                textureKey.InnerText = tile.TextureName;


                tileNode.AppendChild(position);
                tileNode.AppendChild(textureKey);

                decorTiles.AppendChild(tileNode);
            }

            var entityTiles = doc.CreateElement("entitys");
            foreach (var tile in Entitys)
            {
                var tileNode = doc.CreateElement("tile");

                var position = doc.CreateElement("position");
                var xNode = doc.CreateElement("x");
                xNode.InnerText = tile.X.ToString();
                var yNode = doc.CreateElement("y");
                yNode.InnerText = tile.Y.ToString();
                position.AppendChild(xNode);
                position.AppendChild(yNode);

                var textureKey = doc.CreateElement("textureKey");
                textureKey.InnerText = tile.TextureName;


                tileNode.AppendChild(position);
                tileNode.AppendChild(textureKey);

                entityTiles.AppendChild(tileNode);
            }



            root.AppendChild(tiles);
            root.AppendChild(floorTiles);
            root.AppendChild(decorTiles);
            root.AppendChild(entityTiles);
            doc.AppendChild(root);

            doc.Save(this.loadedFile);
        }

        private void drawPanel_Click(object sender, EventArgs e)
        {
            saved = false;
            // Cast to MouseEventArgs
            MouseEventArgs mouse = (MouseEventArgs)e;
            int xRows = drawPanel.Width / 16;
            int yRows = drawPanel.Height / 16;

            for (int y = 0; y < yRows; y++)
            {
                for (int x = 0; x < xRows; x++)
                {
                    int imgWidth = 16, imgHeight = 16;
                    int imgOriginX = (x * 16);
                    int imgOriginY = (y * 16);

                    // If mouse is within image
                    try
                    {
                        if (mouse.X >= imgOriginX && mouse.Y >= imgOriginY && mouse.X < imgOriginX + imgWidth && mouse.Y < imgOriginY + imgHeight)
                        {
                            if (mouse.Button == MouseButtons.Left)
                            {
                                if (editingLayer == 0)
                                {
                                    foreach (var tile in Tiles)
                                    {
                                        if (tile.X == x && tile.Y == y)
                                            Tiles.Remove(tile);
                                    }
                                }
                                else if (editingLayer == 1)
                                {
                                    foreach (var tile in Floor)
                                    {
                                        if (tile.X == x && tile.Y == y)
                                            Floor.Remove(tile);
                                    }
                                }
                                else if (editingLayer == 2)
                                {
                                    foreach (var tile in Decor)
                                    {
                                        if (tile.X == x && tile.Y == y)
                                            Decor.Remove(tile);
                                    }
                                }
                            }
                            else if (mouse.Button == MouseButtons.Right)
                            {
                                Tile fTile = new Tile();
                                fTile.X = x;
                                fTile.Y = y;
                                fTile.Texture = textures[dataGridView1.SelectedRows[0].Cells[0].Value.ToString()];
                                fTile.TextureName = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();

                                if (editingLayer == 0)
                                    this.Tiles.Add(fTile);
                                else if (editingLayer == 1)
                                    this.Floor.Add(fTile);
                                else if (editingLayer == 2)
                                    this.Decor.Add(fTile);
                            }
                        }
                    }
                    catch { }
                }
            }

            DrawTiles(editingLayer);
            DrawPreviewForm();
        }

        private void DrawPreviewForm()
        {
            if (!previewToolStripMenuItem.Enabled)
            {
                gPreview = previewForm.CreateGraphics();
                gPreview.Clear(Color.FromArgb(28, 17, 23));
                foreach (var tile in Floor)
                {
                    gPreview.DrawImage(tile.Texture, new Rectangle((tile.X + x) * size, (tile.Y + y) * size, size, size));
                }
                foreach (var tile in Tiles)
                {
                    gPreview.DrawImage(tile.Texture, new Rectangle((tile.X + x) * size, (tile.Y + y) * size, size, size));
                }
                foreach (var tile in Decor)
                {
                    gPreview.DrawImage(tile.Texture, new Rectangle((tile.X + x) * size, (tile.Y + y) * size, size, size));
                }
                foreach (var tile in Entitys)
                {
                    //
                    //entitytextures[tile.TextureName].Height
                    gPreview.DrawImage(tile.Texture, new Rectangle((tile.X + x) * size, (tile.Y + y) * size, (1 / 3) * (tile.Texture.Width), (2 / 3) * (tile.Texture.Height)));
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!saved)
                Save();
        }
        private void layersListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            editingLayer = layersListBox.SelectedIndex;
            DrawTiles(editingLayer);
        }

        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            previewToolStripMenuItem.Enabled = false;
            previewForm.Width = 1280;
            previewForm.Height = 720;
            previewForm.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            previewForm.Show();
            gPreview = previewForm.CreateGraphics();


            previewForm.KeyDown += (o, eF) =>
            {
                if (eF.KeyCode == Keys.D)
                    x--;
                else if (eF.KeyCode == Keys.A)
                    x++;
                else if (eF.KeyCode == Keys.W)
                    y++;
                else if (eF.KeyCode == Keys.S)
                    y--;
                else if (eF.KeyCode == Keys.Add)
                    size += 8;
                else if (eF.KeyCode == Keys.Subtract)
                    if (size > 8) size -= 8;

                DrawPreviewForm();
            };

            DrawPreviewForm();
        }

        private void entitiesToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }

    internal class EntityInfo
    {
        public Image Texture { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; internal set; }
    }

    public class Tile
    {
        public Image Texture { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int ID { get; set; }
        public string TextureName { get; set; }
        public bool Decor { get; set; }
    }
    public class Entity
    {
        public Image Texture { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string TextureName { get; set; }
        public string EnyityType { get; set; }
    }

}
