using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Penumbra;
using RPG2D.GameEngine;
using RPG2D.GameEngine.Entities;
using RPG2D.GameEngine.Screens;
using RPG2D.GameEngine.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPG2D.GameEngine.Entities.Physics;

namespace RPG2D.SGame.Player
{
    public class Player : AnimatedSprite
    {
        private SoundEffect walkingEffect;
        private SoundEffectInstance walkingSoundEffectInstance;
        private PointLight playerLight;
        public Hull PlayerHull { get; private set; }


        int Speed = 250;

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

        public Vector2 Velocity { get; internal set; }

        Physics Physics;

        public void Init(ContentManager content)
        {
            // load in all textures and animations
            Animations.Add("walking_left", new Animation(new Frame
                (content.Load<Texture2D>("player/player_left1")), new Frame
                (content.Load<Texture2D>("player/player_left2")), new Frame
                (content.Load<Texture2D>("player/player_left3"))));

            Animations.Add("walking_right", new Animation(new Frame(content.Load<Texture2D>("player/player_right1")), new Frame(content.Load<Texture2D>("player/player_right2")), new Frame(content.Load<Texture2D>("player/player_right3"))));

            Animations.Add("walking_up", new Animation(new Frame
                (content.Load<Texture2D>("player/player_up1")), new Frame
                (content.Load<Texture2D>("player/player_up2")), new Frame
                (content.Load<Texture2D>("player/player_up3"))));

            Animations.Add("walking_down", new Animation(new Frame
                (content.Load<Texture2D>("player/player_down1")), new Frame
                (content.Load<Texture2D>("player/player_down2")), new Frame
                (content.Load<Texture2D>("player/player_down3"))));

            // set default animation
            CurrentAnimation = "walking_down";

            // walking sound effects
            walkingEffect = content.Load<SoundEffect>("sound/footsteps");
            walkingSoundEffectInstance = walkingEffect.CreateInstance();
            walkingSoundEffectInstance.Volume = 0.25f;

            // position and collision system
            Position = new Vector2(256);
            Physics = new Physics();
            Physics.GeneratePoints(this);

            // init lighting
            InitLighting();

        }

        public void InitLighting()
        {
            // Create a hull for player & map points
            PlayerHull = new Hull(new Vector2(12, 0), new Vector2(12, 55), new Vector2(14, 58), new Vector2(16, 59), new Vector2(20, 59), new Vector2(26, 56), new Vector2(37, 56), new Vector2(43, 59), new Vector2(47, 59), new Vector2(49, 58), new Vector2(51, 55), new Vector2(51, 0))
            {
                Scale = new Vector2(1),
                Origin = new Vector2(32)
            };

            // create light for player
            playerLight = new PointLight();
            playerLight.CastsShadows = true;
            playerLight.ShadowType = ShadowType.Solid;
            playerLight.Scale = new Vector2(400);
            playerLight.Intensity = 0.25f;
            playerLight.IgnoredHulls.Add(PlayerHull);

            GameManager.Game.Penumbra.Hulls.Add(PlayerHull);
            GameManager.Game.Penumbra.Lights.Add(playerLight);
        }

        bool movingUp, movingDown, movingLeft, movingRight;
        public override void Update(GameTime gameTime)
        {
            // Lighting
            PlayerHull.Position = GameManager.Game.Camera.ScreenToWorld(GameManager.Game.ScreenSize / 2);
            playerLight.Position = Position + new Vector2(32, 16);

            // Keypress states & gamepad
            var state = Keyboard.GetState();
            var gamepad = GamePad.GetState(PlayerIndex.One);
            // if (GameManager.Game.CanMove)
            UpdateKeypressStates(state, gamepad);
            if (!GameManager.Game.InInventory) HandleKeyPresses(state, gameTime, gamepad);

            // Interactions with the envoirnment
            GameManager.Game.World.CheckInteractions(Position - new Vector2(4), new Vector2(68, 68));

            base.Update(gameTime);
        }

        private void HandleKeyPresses(KeyboardState state, GameTime gameTime, GamePadState gamepad)
        {
            // get latest collition states before checking for movement
            UpdateColitions();
            float diagonalSpeedDivisor = 1.5f;

            var oldPos = Position;
            if (state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.A) || state.IsKeyDown(Keys.S) || state.IsKeyDown(Keys.D))
                walkingSoundEffectInstance.Play();

            float xSpeed = 0, ySpeed = 0;

            if (gamepad.IsConnected)
            {
                if (gamepad.ThumbSticks.Left.Y > 0.1f)
                {
                    if (Physics.CanUp)
                        ySpeed = -Speed * gamepad.ThumbSticks.Left.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    CurrentAnimation = "walking_up";
                }
                else if (gamepad.ThumbSticks.Left.Y < -0.1f)
                {
                    if (Physics.CanDown)
                        ySpeed = Speed * -gamepad.ThumbSticks.Left.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    CurrentAnimation = "walking_down";
                }

                if (gamepad.ThumbSticks.Left.X > 0.1f)
                {
                    if (Physics.CanRight)
                        xSpeed = Speed * gamepad.ThumbSticks.Left.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    CurrentAnimation = "walking_right";
                }
                else if (gamepad.ThumbSticks.Left.X < -0.1f)
                {
                    if (Physics.CanLeft)
                        xSpeed = -(Speed * -gamepad.ThumbSticks.Left.X * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    CurrentAnimation = "walking_left";
                }
            }
            else
            {
                if (state.IsKeyDown(Keys.W) && Physics.CanUp)
                {
                    ySpeed = -Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    CurrentAnimation = "walking_up";
                }
                else if (state.IsKeyDown(Keys.S) && Physics.CanDown)
                {
                    ySpeed = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    CurrentAnimation = "walking_down";

                }
                else if (state.IsKeyDown(Keys.A) && Physics.CanLeft)
                {
                    xSpeed = -Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    CurrentAnimation = "walking_left";

                }
                else if (state.IsKeyDown(Keys.D) && Physics.CanRight)
                {
                    xSpeed = +Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    CurrentAnimation = "walking_right";
                }

                if (state.IsKeyDown(Keys.A) && state.IsKeyDown(Keys.W))
                {
                    if (Physics.CanUp)
                        ySpeed = -Speed / diagonalSpeedDivisor * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Physics.CanLeft)
                        xSpeed = -Speed / diagonalSpeedDivisor * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    CurrentAnimation = "walking_left";
                }
                else if (state.IsKeyDown(Keys.D) && state.IsKeyDown(Keys.W))
                {
                    if (Physics.CanUp)
                        ySpeed = -Speed / diagonalSpeedDivisor * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Physics.CanRight)
                        xSpeed = Speed / diagonalSpeedDivisor * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    CurrentAnimation = "walking_right";
                }

                if (state.IsKeyDown(Keys.A) && state.IsKeyDown(Keys.S))
                {
                    if (Physics.CanDown)
                        ySpeed = Speed / diagonalSpeedDivisor * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Physics.CanLeft)
                        xSpeed = -Speed / diagonalSpeedDivisor * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    CurrentAnimation = "walking_left";
                }
                else if (state.IsKeyDown(Keys.D) && state.IsKeyDown(Keys.S))
                {
                    if (Physics.CanDown)
                        ySpeed = Speed / diagonalSpeedDivisor * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (Physics.CanRight)
                        xSpeed = Speed / diagonalSpeedDivisor * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    CurrentAnimation = "walking_right";
                }
            }

            this.Position += new Vector2(xSpeed, ySpeed);
            this.Velocity = oldPos - Position;
        }
        private void UpdateKeypressStates(KeyboardState state, GamePadState gamepad)
        {
            movingUp = false;
            movingDown = false;
            movingLeft = false;
            movingRight = false;


            if (state.IsKeyDown(Keys.W) || gamepad.ThumbSticks.Left.Y > 0.1f)
            {
                walkingSoundEffectInstance.Play();
                movingUp = true;
            }
            else if (state.IsKeyDown(Keys.S) || gamepad.ThumbSticks.Left.Y < -0.1f)
            {
                walkingSoundEffectInstance.Play();
                movingDown = true;
            }

            if (state.IsKeyDown(Keys.A) || gamepad.ThumbSticks.Left.X < -0.1f)
            {
                walkingSoundEffectInstance.Play();
                movingLeft = true;
            }
            else if (state.IsKeyDown(Keys.D) || gamepad.ThumbSticks.Left.X > 0.1f)
            {
                walkingSoundEffectInstance.Play();
                movingRight = true;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(GetTexture(), new Rectangle(Position.ToPoint(), Size.ToPoint()), Color.White);
        }

        private void UpdateColitions()
        {
            if (movingUp || movingDown || movingLeft || movingRight)
                Physics.UpdateCollitions(this);
        }
    }
}
