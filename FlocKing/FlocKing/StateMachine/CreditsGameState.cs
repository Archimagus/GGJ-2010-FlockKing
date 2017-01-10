using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FlocKing.Helpers;
using Microsoft.Xna.Framework.Content;

namespace FlocKing.StateMachine
{
    class CreditsGameState : IBaseState
    {
        #region IBaseState Members

        // Controllers that are in use.
        bool[] _activeControllers = new bool[4];

        // Disconnected controller detected.
        bool _disconnectDetected = false;

        Texture2D creditsGameTexture;
        Rectangle textureRect;
        Color color = Color.White;

        public void Enter()
        {
            GameplayState.paused = true;
            var content = ServiceProvider.GetService<ContentManager>();
            creditsGameTexture = content.Load<Texture2D>("Textures\\CreditsScreen");

            textureRect = new Rectangle(0, 0, creditsGameTexture.Width, creditsGameTexture.Height);
        }

        public bool Input()
        {
            var ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Escape))
            {
                return false;
            }

            PlayerIndex index = PlayerIndex.One;
            for (int i = 0; i < 4; i++, index++)
            {
                if (_activeControllers[i] &&
                    !GamePad.GetState(index).IsConnected)
                {
                    _disconnectDetected = true;
                }

                var gamePad = GamePad.GetState(index);

                if (gamePad.IsButtonDown(Buttons.B))
                {
                    CStateMachine.Instance.PopState(this);
                }

            }

            return true;
        }

        public void Update(Microsoft.Xna.Framework.GameTime _Delta)
        {
            //if (!_disconnectDetected)
            {
                Camera.Instance.Update(true);
                var graphics = ServiceProvider.GetService<GraphicsDeviceManager>();
                textureRect = new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            }
        }

        public void Render()
        {
            Game1.Instance.spriteBatch.Begin();
            Game1.Instance.spriteBatch.Draw(creditsGameTexture, textureRect, color);
            Game1.Instance.spriteBatch.End();
        }

        public void Exit()
        {
        }

        #endregion
    }
}