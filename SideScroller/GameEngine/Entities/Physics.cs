using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RPG2D.GameEngine.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.GameEngine.Entities
{
    public class Physics
    {
        public class CollitionPoint
        {
            public Tuple<bool, Tile> Point1, Point2;

            public CollitionPoint(Tuple<bool, Tile> p1, Tuple<bool, Tile> p2)
            {
                Point1 = p1;
                Point2 = p2;
            }
        }
        public struct CollitionDetection
        {
            public CollitionPoint Up;
            public CollitionPoint Down;
            public CollitionPoint Left;
            public CollitionPoint Right;
        }

        public bool CanUp
        {
            get
            {
                try
                {
                    return Collitions.Up.Point1.Item1 & Collitions.Up.Point2.Item1;
                }

                catch
                {
                    return false;
                }
            }
        }
        public bool CanDown
        {
            get
            {
                try
                {
                    return Collitions.Down.Point1.Item1 & Collitions.Down.Point2.Item1;
                }

                catch
                {
                    return false;
                }
            }
        }
        public bool CanLeft
        {
            get
            {
                try
                {
                    return Collitions.Left.Point1.Item1 & Collitions.Left.Point2.Item1;
                }

                catch
                {
                    return false;
                }
            }
        }
        public bool CanRight
        {
            get
            {
                try
                {
                    return Collitions.Right.Point1.Item1 & Collitions.Right.Point2.Item1;
                }
                catch
                {
                    return false;
                }
            }
        }

        public CollitionDetection Collitions;

        public Physics()
        {
            Collitions = new CollitionDetection();
        }

        public void GeneratePoints(Sprite sprite)
        {
            int xSize = 20;

            Collitions.Up = new CollitionPoint(
                    GameManager.Game.World.IsSpaceOpen(sprite.Position + new Vector2(xSize, 0), new Vector2(sprite.Size.X / 2 - (xSize / 2), 1)),
                    GameManager.Game.World.IsSpaceOpen(sprite.Position + new Vector2(sprite.Size.X / 2, 0), new Vector2(sprite.Size.X / 2 - (xSize / 2), 1))
                    );
            Collitions.Down = new CollitionPoint(
                GameManager.Game.World.IsSpaceOpen(sprite.Position + new Vector2(xSize, sprite.Size.Y - 1), new Vector2(sprite.Size.X / 2 - (xSize / 2), 1)),
                GameManager.Game.World.IsSpaceOpen(sprite.Position + new Vector2(sprite.Size.X / 2, sprite.Size.Y - 1), new Vector2(sprite.Size.X / 2 - (xSize / 2), 1))
                );
            Collitions.Left = new CollitionPoint(
                GameManager.Game.World.IsSpaceOpen(sprite.Position + new Vector2(0, xSize), new Vector2(1, sprite.Size.Y / 2 - (xSize / 2))),
                GameManager.Game.World.IsSpaceOpen(sprite.Position + new Vector2(0, sprite.Size.Y / 2), new Vector2(1, sprite.Size.Y / 2 - (xSize / 2)))
                );
            Collitions.Right = new CollitionPoint(
                GameManager.Game.World.IsSpaceOpen(sprite.Position + new Vector2(sprite.Size.X - 1, xSize), new Vector2(1, sprite.Size.Y / 2 - (xSize / 2))),
                GameManager.Game.World.IsSpaceOpen(sprite.Position + new Vector2(sprite.Size.X - 1, sprite.Size.Y / 2), new Vector2(1, sprite.Size.Y / 2 - (xSize / 2)))
                );
        }

        public void UpdateCollitions(Sprite sprite)
        {
            GeneratePoints(sprite);
        }
    }
}
