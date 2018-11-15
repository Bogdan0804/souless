using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG2D.GameEngine.Entities
{
    public class Torch : TileEntity
    {
        SoundEffect soundEffect;
        SoundEffectInstance soundEffectInstance;
        AudioListener listener = new AudioListener();
        AudioEmitter emitter = new AudioEmitter();

        public Torch()
            : base(new Frame(GameManager.Game.Content.Load<Texture2D>("torch0")), new Frame(GameManager.Game.Content.Load<Texture2D>("torch1")), new Frame(GameManager.Game.Content.Load<Texture2D>("torch2")))
        {
            soundEffect = GlobalAssets.SoundEffects["torch"];
            soundEffectInstance = soundEffect.CreateInstance();
            soundEffectInstance.IsLooped = true;
            soundEffectInstance.Play();
            soundEffectInstance.Volume = 1.0f;
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            base.Draw(gameTime, spriteBatch);
            emitter.Position = new Vector3(X, Y, 1);

            listener.Position = new Vector3(GameManager.Game.Player.Position + new Vector2(32), 1);
            listener.Velocity = new Vector3(GameManager.Game.Player.Velocity, 1);

            soundEffectInstance.Apply3D(listener, emitter);
        }
    }
}
