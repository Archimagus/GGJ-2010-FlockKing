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
using FlocKing.StateMachine;
using Microsoft.Xna.Framework.Audio;


namespace FlocKing
{
    public class GameplayState : IBaseState
    {
        public static World _world;

        public static Cloud _cloud = new Cloud();
        // Controllers that are in use.
        bool[] _activeControllers = new bool[4];

        public List<PlayerSettings> _playerSettings = new List<PlayerSettings>();

       
        // Disconnected controller detected.
        bool _disconnectDetected = false;

        public static bool paused = false;

        float matchTime = 0.0f;
        public static bool goCloudGo = false;

        public string MapName { get; set; }
        public static GameSettings GameSettings { get; set; }

        public ContentManager Content { get; set; }
        SoundEffect activeSoundEffect;

        public void Enter()
        {
            paused = false;
            if (string.IsNullOrEmpty(MapName))
                MapName = "mesh\\marsMap2";

            MapName = "mesh\\marMapv4";

            _world = new World(MapName);
            ServiceProvider.GetService<GraphicsDeviceManager>().MinimumVertexShaderProfile = ShaderProfile.VS_2_0;
            ServiceProvider.GetService<GraphicsDeviceManager>().MinimumPixelShaderProfile = ShaderProfile.PS_2_0;
            _cloud.Loadcontent();
            _world.Loadcontent();
            _world.Position = new Vector3(0, 0, 0);


            foreach (PlayerSettings ps in _playerSettings)
            {
                Game1.Instance.Players.Add(new Player(ps.ModelName) { ControllerID = ps.ControllerID, Color = ps.Color, Animating = true });
            }
            if (Game1.Instance.Players.Count == 0)
            {
                Game1.Instance.Players.Add(new Player("Mesh/fastguy") { ControllerID = PlayerIndex.One, Color = Color.Blue, Animating = true });
                Game1.Instance.Players.Add(new Player("Mesh/stealthguy") { ControllerID = PlayerIndex.Two, Color = Color.Green, Animating = true });
            }
            else if (Game1.Instance.Players.Count == 1)
            {
                PlayerIndex index = (Game1.Instance.Players[0].ControllerID == PlayerIndex.One ? PlayerIndex.Two : PlayerIndex.One);

                Game1.Instance.Players.Add(new Player("Mesh/fastguy") { ControllerID = index, Color = Color.Honeydew, Animating = true });
            }

            foreach (Player player in Game1.Instance.Players)
            {
                if (_world.HasPlayerData)
                {
                    switch (player.ControllerID)
                    {
                        case PlayerIndex.One:
                            player.Position = _world.PlayerStart1;
                            _activeControllers[0] = true;
                            break;
                        case PlayerIndex.Two:
                            player.Position = _world.PlayerStart2;
                            _activeControllers[1] = true;
                            break;
                        case PlayerIndex.Three:
                            player.Position = _world.PlayerStart3;
                            _activeControllers[2] = true;
                            break;
                        case PlayerIndex.Four:
                            player.Position = _world.PlayerStart4;
                            _activeControllers[3] = true;
                            break;
                        default:
                            break;
                    }

                }
                else
                {
                    switch (player.ControllerID)
                    {
                        case PlayerIndex.One:
                            player.Position = new Vector3(-50, 0, 0);
                            _activeControllers[0] = true;
                            break;
                        case PlayerIndex.Two:
                            player.Position = new Vector3(50, 0, 0);
                            _activeControllers[1] = true;
                            break;
                        case PlayerIndex.Three:
                            player.Position = new Vector3(0, 0, -50);
                            _activeControllers[2] = true;
                            break;
                        case PlayerIndex.Four:
                            player.Position = new Vector3(0, 0, 50);
                            _activeControllers[3] = true;
                            break;
                        default:
                            break;
                    }
                }
            }


            if (Camera.Instance.CameraLookAt != Vector3.Zero)
            {
                Camera.Instance.CameraLookAt = Vector3.Zero;
                Camera.Instance.CameraPosition = new Vector3(0, 350, 100);
            }
            if (_world.HasCameraData)
            {
                Camera.Instance.CameraPosition = _world.CameraPosition;
                Camera.Instance.CameraLookAt = _world.CameraTarget;
            }

            foreach (Player player in Game1.Instance.Players)
            {
                player.initializeFlock(20);
                player.Loadcontent();
            }
            matchTime = 0.0f;

            Content = ServiceProvider.GetService<ContentManager>();
            Content.Load<SoundEffect>("Sounds/Screech").Play(Game1.Instance.SoundEffectsVolume, 0, 0);
            Content.Load<SoundEffect>("Sounds/Wind_New").Play(Game1.Instance.SoundEffectsVolume, 0, 0);
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


            foreach (Player p in Game1.Instance.Players)
            {
                p.Input();
            }
            return true;

        }
        public void Update(GameTime time)
        {
            paused = false;
            matchTime += (float)time.ElapsedGameTime.TotalSeconds;
            //if (!_disconnectDetected)
            //if (_disconnectDetected)
            //{
            //    _disconnectDetected = false;
            //    CStateMachine.Instance.PushState(new StateMachine.PauseHelpGameState() { });
            //    return;
            //}
            
            Camera.Instance.Update(true);
            if (matchTime > 30.0f)
            {
                _cloud.Update(time);
                goCloudGo = true;
            }
            foreach (Player p in Game1.Instance.Players)
                p.Update(time);

            Game1.Instance.collisionManager.update(time);
            
        }
        public void Render()
        {
            _world.Render();
            foreach (Player p in Game1.Instance.Players)
            {
                p.Render();
            }
            _cloud.Render();
        }
        public void Exit()
        {
        }

        internal static void EvaluateEndGame()
        {
            Player winner = Game1.Instance.Players[0];
            int numberOfPlayers = Game1.Instance.Players.Count;
            int numberOfDeadKings = 0;
            foreach (Player player in Game1.Instance.Players)
            {
                if (player.king.Dead)
                    numberOfDeadKings++;
                else
                    winner = player;
            }
            if (numberOfDeadKings == numberOfPlayers - 1)
                GameEnded(winner);
            else if (numberOfDeadKings == numberOfPlayers)
                GameEnded(null);
        }

        private static void GameEnded(Player winner)
        {
            if (winner != null)
                Console.WriteLine("We have a WINNER!\nPlayer " + winner.ControllerID + "!!!");
            
            CStateMachine.Instance.PushState(new EndGameState());
        }
    }
}
