using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPG2D.GameEngine.UI
{
    public class InstructionDialog : UIElement
    {
        public bool IsGameMode = true;
        public int Progress = 0;

        private KeyboardState oldKeyState;
        private GamePadState oldControllerState;
        private bool canContinue = true;

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

        }
        public override void Update(GameTime gameTime)
        {
            var keystate = Keyboard.GetState();
            var controllerState = GamePad.GetState(PlayerIndex.One);
            string text = "";

            if (Progress == 0)
            {
                if (controllerState.IsConnected)
                    text = "Welcome to Soulless! (press A to continue)";
                else
                    text = "Welcome to Soulless! (press enter to continue)";
            }
            if (Progress == 1)
            {
                IsGameMode = false;
            }

            if (keystate.IsKeyDown(Keys.Enter) && oldKeyState.IsKeyUp(Keys.Enter) && !controllerState.IsConnected || controllerState.IsButtonDown(Buttons.A) && oldControllerState.IsButtonUp(Buttons.A))
                if (canContinue)
                    Progress++;

            oldControllerState = controllerState;
            oldKeyState = keystate;

            if (IsGameMode)
                GameManager.Game.Tooltip = text;
        }
    }
}
