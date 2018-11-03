using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RPG2D.GameEngine;
using RPG2D.GameEngine.Screens;
using RPG2D.GameEngine.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.SGame.Player
{
    public class Player : Sprite
    {
        Texture2D texUp, texDown, texLeft, texRight, curTex;
        int Speed = 5;
        public Vector2 Position {
            get
            {
                return new Vector2(X, Y);
            }
            set
            {
                this.X = value.X;
                this.Y = value.Y;
            }
        }
        public Vector2 Size { get; private set; }
        public override Rectangle Bounds
        {
            get
            {
                return new Rectangle(Position.ToPoint(), Size.ToPoint());
            }
        }
        public Rectangle EntityBounds
        {
            get
            {
                return new Rectangle(new Point((int)Position.X - 32, (int)Position.Y - 32), Size.ToPoint());
            }
        }

        public struct CollitionDetection
        {
            public CollitionPoint Up;
            public CollitionPoint Down;
            public CollitionPoint Left;
            public CollitionPoint Right;
        }
        public CollitionDetection Collitions = new CollitionDetection();
        bool canUp = true, canDown = true, canLeft = true, canRight = true;
        public class CollitionPoint
        {
            public Tuple<bool, Tile> Point1, Point2;

            public CollitionPoint(Tuple<bool, Tile> p1, Tuple<bool, Tile> p2)
            {
                Point1 = p1;
                Point2 = p2;
            }
        }

        public void Init(ContentManager content)
        {
            texUp = content.Load<Texture2D>("player/player_up");
            texDown = content.Load<Texture2D>("player/player_down");
            texLeft = content.Load<Texture2D>("player/player_left");
            texRight = content.Load<Texture2D>("player/player_right");
            curTex = texDown;
            this.Texture = curTex;

            Size = new Vector2(64);
            this.Position = new Vector2(256);


            int xSize = 20;
            Collitions.Up = new CollitionPoint(
                   GameManager.Game.World.IsSpaceOpen(Position + new Vector2(xSize, 0), new Vector2(Size.X / 2 - (xSize / 2), 1)),
                   GameManager.Game.World.IsSpaceOpen(Position + new Vector2(Size.X / 2, 0), new Vector2(Size.X / 2 - (xSize / 2), 1))
                   );
            Collitions.Down = new CollitionPoint(
                GameManager.Game.World.IsSpaceOpen(Position + new Vector2(xSize, Size.Y - 1), new Vector2(Size.X / 2 - (xSize / 2), 1)),
                GameManager.Game.World.IsSpaceOpen(Position + new Vector2(Size.X / 2, Size.Y - 1), new Vector2(Size.X / 2 - (xSize / 2), 1))
                );
            Collitions.Left = new CollitionPoint(
                GameManager.Game.World.IsSpaceOpen(Position + new Vector2(0, xSize), new Vector2(1, Size.Y / 2 - (xSize / 2))),
                GameManager.Game.World.IsSpaceOpen(Position + new Vector2(0, Size.Y / 2), new Vector2(1, Size.Y / 2 - (xSize / 2)))
                );
            Collitions.Right = new CollitionPoint(
                GameManager.Game.World.IsSpaceOpen(Position + new Vector2(Size.X - 1, xSize), new Vector2(1, Size.Y / 2 - (xSize / 2))),
                GameManager.Game.World.IsSpaceOpen(Position + new Vector2(Size.X - 1, Size.Y / 2), new Vector2(1, Size.Y / 2 - (xSize / 2)))
                );
        }
        bool movingUp, movingDown, movingLeft, movingRight;
        public void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();

            UpdateKeypressStates();

            if (state.IsKeyDown(Keys.W) && canUp)
            {
                Position += (new Vector2(0, -Speed));
                curTex = texUp;
                this.Texture = curTex;

            }
            else if (state.IsKeyDown(Keys.S) && canDown)
            {
                Position += (new Vector2(0, Speed));
                curTex = texDown;
                this.Texture = curTex;

            }

            if (state.IsKeyDown(Keys.A) && canLeft)
            {
                Position += (new Vector2(-Speed, 0));
                curTex = texLeft;
                this.Texture = curTex;

            }
            else if (state.IsKeyDown(Keys.D) && canRight)
            {
                Position += (new Vector2(Speed, 0));
                curTex = texRight;
                this.Texture = curTex;
            }
            ///
            GameManager.Game.World.CheckInteractions(Position - new Vector2(4), new Vector2(68, 68));
        }

        private void UpdateKeypressStates()
        {
            movingUp = false;
            movingDown = false;
            movingLeft = false;
            movingRight = false;


            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.W))
            {
                movingUp = true;
            }
            else if (state.IsKeyDown(Keys.S))
            {
                movingDown = true;
            }

            if (state.IsKeyDown(Keys.A))
            {
                movingLeft = true;
            }
            else if (state.IsKeyDown(Keys.D))
            {
                movingRight = true;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(curTex, new Rectangle(Position.ToPoint(), Size.ToPoint()), Color.White);
            UpdateColitions(spriteBatch);
            //spriteBatch.Draw(GlobalAssets.WorldTiles["floor"].Texture, new Rectangle(Position.ToPoint() + new Point(0, (int)Size.Y + 1), new Point(64, 2)), Color.Blue);
            //spriteBatch.Draw(GlobalAssets.WorldTiles["floor"].Texture, new Rectangle(Position.ToPoint(), new Point(64, 2)), Color.Blue);
        }

        private void UpdateColitions(SpriteBatch s)
        {
            int xSize = 20;

            if (movingUp)
                Collitions.Up = new CollitionPoint(
                   GameManager.Game.World.IsSpaceOpen(Position + new Vector2(xSize, 10), new Vector2(Size.X / 6, 5), s),
                   GameManager.Game.World.IsSpaceOpen(Position + new Vector2(32, 10), new Vector2(Size.X / 6, 5), s)
                   );
            if (movingDown)
                Collitions.Down = new CollitionPoint(
                    GameManager.Game.World.IsSpaceOpen(Position + new Vector2(xSize, Size.Y-5), new Vector2(Size.X / 6, 5), s),
                    GameManager.Game.World.IsSpaceOpen(Position + new Vector2(32, Size.Y -5), new Vector2(Size.X / 6, 5), s)
                    );

            if (movingLeft)
                Collitions.Left = new CollitionPoint(
                    GameManager.Game.World.IsSpaceOpen(Position + new Vector2(10, xSize), new Vector2(5, 16), s),
                    GameManager.Game.World.IsSpaceOpen(Position + new Vector2(10, Size.Y / 2), new Vector2(5, 16), s)
                    );
            if (movingRight)
                Collitions.Right = new CollitionPoint(
                    GameManager.Game.World.IsSpaceOpen(Position + new Vector2(Size.X - 10, xSize), new Vector2(5, 16), s),
                    GameManager.Game.World.IsSpaceOpen(Position + new Vector2(Size.X - 10, Size.Y / 2), new Vector2(5, 16), s)
                    );

            canUp = Collitions.Up.Point1.Item1 & Collitions.Up.Point2.Item1;
            canLeft = Collitions.Left.Point1.Item1 & Collitions.Left.Point2.Item1;
            canRight = Collitions.Right.Point1.Item1 & Collitions.Right.Point2.Item1;
            canDown = Collitions.Down.Point1.Item1 & Collitions.Down.Point2.Item1;

        }
    }
}
