using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FlocKing.Helpers;
using Microsoft.Xna.Framework.Content;
using FlocKing.Objects;

namespace FlocKing.StateMachine
{
    class EndGameState : IBaseState
    {
        #region IBaseState Members

        // Controllers that are in use.
        bool[] _activeControllers = new bool[4];

        // Disconnected controller detected.
        bool _disconnectDetected = false;

        Texture2D endGameTexture;
        Rectangle textureRect;
        Color color = Color.White;

        public void Enter()
        {
            GameplayState.paused = true;
            var content = ServiceProvider.GetService<ContentManager>();
            endGameTexture = content.Load<Texture2D>("Textures\\VictoryScreen");

            textureRect = new Rectangle(0, 0, endGameTexture.Width, endGameTexture.Height);
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
                    // main menu
                    //foreach (Player player in Game1.Players)
                    //    player.reset();
                    //CStateMachine.Instance.ChangeState(new MenuState());
                }
                if (gamePad.IsButtonDown(Buttons.X))
                {
                    // rematch
                    //Console.WriteLine("Rematch");
                    GameplayState game = new GameplayState();
                    if (GameplayState.GameSettings != null)
                        game.MapName = GameplayState.GameSettings.MapName;
                    else
                        game.MapName = "Mesh/plane";

                    foreach (Player player in Game1.Instance.Players)
                        player.reset();

                    CStateMachine.Instance.ChangeState(game);

                }
                if (gamePad.IsButtonDown(Buttons.Y))
                {
                    // credits
                    Console.WriteLine("Credits");
                    CStateMachine.Instance.PushState(new CreditsGameState());
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
            Game1.Instance.spriteBatch.Draw(endGameTexture, textureRect, color);
            Game1.Instance.spriteBatch.End();
        }

        public void Exit()
        {

        }

        #endregion
    }
}
