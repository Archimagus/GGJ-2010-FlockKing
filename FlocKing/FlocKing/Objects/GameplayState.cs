using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using FlocKing.Helpers;
using FlocKing.Objects;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;


namespace FlocKing
{
    public class GameplayState : IBaseState
    {
        World _world = new World();

        Cloud _cloud = new Cloud();
        // Controllers that are in use.
        bool[] _activeControllers = new bool[4];

        // Disconnected controller detected.
        bool _disconnectDetected = false;

        public void Enter()
        {
            ServiceProvider.GetService<GraphicsDeviceManager>().MinimumVertexShaderProfile = ShaderProfile.VS_2_0;
            ServiceProvider.GetService<GraphicsDeviceManager>().MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
            _cloud.Loadcontent();
            _world.Loadcontent();
            _world.Position = new Vector3(0, -10, 0);
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                _activeControllers[0] = true;
            }
            Game1.Players.Add(new Player() { ControllerID = PlayerIndex.One, Color = Color.Blue, Animating = false });
            Game1.Players.Add(new Player() { ControllerID = PlayerIndex.Two, Color = Color.Green });
            Game1.Players[0].Position = new Vector3(-50.0f, 0.0f, 0.0f);
            Game1.Players[1].Position = new Vector3(50.0f, 0.0f, 0.0f);
            foreach (Player player in Game1.Players)
            {
                player.initializeFlock(20);
                player.Loadcontent();
            }
        }
        public bool Input()
        {
            var ks = Keyboard.GetState();
            if(ks.IsKeyDown(Keys.Escape))
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
            }

            _cloud.GetInput();
            
            Camera.Instance.Input();


            foreach (Player p in Game1.Players)
            {
                p.Input();
            }
            return true;

        }
        public void Update(GameTime time)
        {
            //if (!_disconnectDetected)
            {
                Camera.Instance.Update(true);
                _cloud.Update(time);
                foreach (Player p in Game1.Players)
                {
                    p.Update(time);
                }

                Game1.collisionManager.update(time);
            }
        }
        public void Render()
        {
            _world.Render();
            foreach (Player p in Game1.Players)
            {
                p.Render();
            }
            _cloud.Render();
        }
        public void Exit()
        {
        }
    }
}
