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
    enum Screens {Main, PlayerSelect, MapSelect, MapSelect1, MapSelect2};
    public class MenuState : IBaseState
    {
        
        MenuWorld world = new MenuWorld();

        List<MenuPlayer> Players = new List<MenuPlayer>();
        List<int> PlayersReader = new List<int>();

        List<Color> PlayerColors = new List<Color>();
        Vector3 screen1Position;
        Vector3 screen2Position;
        Vector3 screen3Position;
        Vector3 screen4Position;
        Vector3 screen5Position;

        Screens currentScreen = Screens.Main;
        Screens targetScreen = Screens.Main;

        Vector3 cameraToTarget;


        Curve3D mainToCharacterPosition = new Curve3D();
        Curve3D mainToCharacterLookat = new Curve3D();

        Curve3D CharacterToMapPosition = new Curve3D();
        Curve3D CharacterToMapLookat = new Curve3D();

        Curve3D Map1ToMap2Position = new Curve3D();
        Curve3D Map1ToMap2Lookat = new Curve3D();

        Curve3D Map1ToMap3Position = new Curve3D();
        Curve3D Map1ToMap3Lookat = new Curve3D();

        Curve3D currentPositionCurve = null;
        Curve3D currentLookAtCurve = null;

        Texture2D texture;

        const float transitionPeriod = 2.0f;
        float time = 0.0f;

        // Controllers that are in use.
        bool[] _activeControllers = new bool[4];

        // Disconnected controller detected.
        bool _disconnectDetected = false;

        public void Enter()
        {
            _activeControllers[0] = true;
            //Game1.Players.Add(new Player() { ControllerID = PlayerIndex.One, Color = Color.Blue });
            
            var content = ServiceProvider.GetService<ContentManager>();

            world.Loadcontent();
            

            texture = content.Load<Texture2D>("Textures/scales");

            var inputManager = ServiceProvider.GetService<InputManager>();
            inputManager.ButtonTriggered += new InputEventHandler(inputManager_ButtonTriggered);
            inputManager.ControllerConnected += new ControllerEventHandler(inputManager_ControllerConnected);
            inputManager.ControllerDisconnected += new ControllerEventHandler(inputManager_ControllerDisconnected);

            Players.Add(new MenuPlayer());
            Players.Add(new MenuPlayer());
            Players.Add(new MenuPlayer());
            Players.Add(new MenuPlayer());

            for(int i = 0; i < Players.Count; ++i)//foreach (MenuPlayer player in Players)
            {
                MenuPlayer player = Players[i];
                player.Loadcontent();
                player.ControllerID = (PlayerIndex)i;
            }
            //world.Position = new Vector3(0, 0, 0);

            Vector3 screen1Lookat = Vector3.Zero;
            Vector3 screen2Lookat = Vector3.Zero;
            Vector3 screen3Lookat = Vector3.Zero;
            Vector3 screen4Lookat = Vector3.Zero;
            Vector3 screen5Lookat = Vector3.Zero;

            ModelMesh camBox;
            bool HasCameraData;
            HasCameraData = world.Model.Meshes.TryGetValue("camBox11inv", out camBox);
            if (HasCameraData)
                screen1Position = camBox.ParentBone.Transform.Translation;
            HasCameraData = world.Model.Meshes.TryGetValue("camBox12inv", out camBox);
            if (HasCameraData)
                screen2Position = camBox.ParentBone.Transform.Translation;
            HasCameraData = world.Model.Meshes.TryGetValue("camBox13inv", out camBox);
            if (HasCameraData)
                screen3Position = camBox.ParentBone.Transform.Translation;
            HasCameraData = world.Model.Meshes.TryGetValue("camBox14inv", out camBox);
            if (HasCameraData)
                screen4Position = camBox.ParentBone.Transform.Translation;
            HasCameraData = world.Model.Meshes.TryGetValue("camBox15inv", out camBox);
            if (HasCameraData)
                screen5Position = camBox.ParentBone.Transform.Translation;

            HasCameraData = world.Model.Meshes.TryGetValue("camTarget11inv", out camBox);
            if (HasCameraData)
                screen1Lookat = camBox.ParentBone.Transform.Translation;
            HasCameraData = world.Model.Meshes.TryGetValue("camTarget12inv", out camBox);
            if (HasCameraData)
                screen2Lookat = camBox.ParentBone.Transform.Translation;
            HasCameraData = world.Model.Meshes.TryGetValue("camTarget13inv", out camBox);
            if (HasCameraData)
                screen3Lookat = camBox.ParentBone.Transform.Translation;
            HasCameraData = world.Model.Meshes.TryGetValue("camTarget14inv", out camBox);
            if (HasCameraData)
                screen4Lookat = camBox.ParentBone.Transform.Translation;


            screen5Position = new Vector3(screen3Position.X, screen3Position.Y, screen3Position.Z + 650);
            screen5Lookat = new Vector3(screen3Lookat.X, screen3Lookat.Y, screen3Lookat.Z + 650);
            //HasCameraData = world.Model.Meshes.TryGetValue("camTarget15inv", out camBox);
            //if (HasCameraData)
            //    screen5Lookat = camBox.ParentBone.Transform.Translation;
            
                        
            //screen1Position = new Vector3(-350, 500, 0);
            //screen1Lookat = new Vector3(-350, 0, 0);
            //screen2Position = new Vector3(800, 500, 0);
            //screen3Position = new Vector3(1800, 500, 0);
            //screen4Position = new Vector3(1800, 500, 400);
            //screen5Position = new Vector3(1800, 500, -400);

            
            Camera.Instance.CameraPosition = screen1Position;
            cameraToTarget = screen1Lookat;

            Camera.Instance.View = Matrix.CreateLookAt(screen1Position, screen1Lookat,
                    Vector3.Cross(Vector3.UnitX, screen1Lookat - screen1Position));//new Vector3(0.0f, 1.0f, 0.0f));


            time = 0;
            mainToCharacterPosition.AddPoint(screen1Position, time);
            mainToCharacterLookat.AddPoint(screen1Lookat, time);
            time += 500;

            mainToCharacterPosition.AddPoint(new Vector3(50, 200, 400), time);
            mainToCharacterLookat.AddPoint(new Vector3(200, 0, 0), time);
            time += 1000;

            mainToCharacterPosition.AddPoint(new Vector3(250, 200, 200), time);
            mainToCharacterLookat.AddPoint(new Vector3(350, 0, 0), time);
            time += 300;

            mainToCharacterPosition.AddPoint(screen2Position, time);
            mainToCharacterLookat.AddPoint(screen2Lookat, time);


            time = 0;
            CharacterToMapPosition.AddPoint(screen2Position, time);
            CharacterToMapLookat.AddPoint(screen2Lookat, time);
            time += 500;

            CharacterToMapPosition.AddPoint(new Vector3(screen3Position.X - 200, 100, 400), time);
            CharacterToMapLookat.AddPoint(new Vector3(screen3Position.X - 150, 0, 0), time);
            time += 1000;

            CharacterToMapPosition.AddPoint(new Vector3(screen3Position.X -25, 100, 200), time);
            CharacterToMapLookat.AddPoint(new Vector3(screen3Position.X - 100, 0, 0), time);
            time += 300;

            CharacterToMapPosition.AddPoint(screen3Position, time);
            CharacterToMapLookat.AddPoint(screen3Lookat, time);


            time = 0;
            Map1ToMap2Position.AddPoint(screen3Position, time);
            Map1ToMap2Lookat.AddPoint(screen3Lookat, time);
            time += 500;

            Map1ToMap2Position.AddPoint(new Vector3(screen4Position.X, 200, screen3Position.Z), time);
            Map1ToMap2Lookat.AddPoint(new Vector3(screen4Position.X, 0, screen4Position.Z), time);
            time += 1000;

            Map1ToMap2Position.AddPoint(new Vector3(screen4Position.X, 200, screen4Position.Z), time);
            Map1ToMap2Lookat.AddPoint(screen4Lookat, time);
            time += 300;

            Map1ToMap2Position.AddPoint(screen4Position, time);
            Map1ToMap2Lookat.AddPoint(screen4Lookat, time);


            time = 0;
            Map1ToMap3Position.AddPoint(screen3Position, time);
            Map1ToMap3Lookat.AddPoint(screen3Lookat, time);
            time += 500;

            Map1ToMap3Position.AddPoint(new Vector3(screen3Position.X, 200, screen5Position.Z - 300), time);
            Map1ToMap3Lookat.AddPoint(screen4Lookat, time);
            time += 1000;

            Map1ToMap3Position.AddPoint(new Vector3(screen5Position.X, 200, screen5Position.Z), time);
            Map1ToMap3Lookat.AddPoint(screen3Lookat, time);
            time += 300;

            Map1ToMap3Position.AddPoint(screen5Position, time);
            Map1ToMap3Lookat.AddPoint(screen5Lookat, time);
           
            mainToCharacterPosition.SetTangents();
            mainToCharacterLookat.SetTangents();

            CharacterToMapLookat.SetTangents();
            CharacterToMapPosition.SetTangents();

            Map1ToMap2Lookat.SetTangents();
            Map1ToMap2Position.SetTangents();

            Map1ToMap3Lookat.SetTangents();
            Map1ToMap3Position.SetTangents();


            PlayerColors.Add(Color.Blue);
            PlayerColors.Add(Color.Gold);
            PlayerColors.Add(Color.Green);
            PlayerColors.Add(Color.Yellow);

        }

        void inputManager_ControllerDisconnected(InputManager sender, ControllerEventArgs e)
        {
            _activeControllers[e.ControllerID] = false;
        }

        void inputManager_ControllerConnected(InputManager sender, ControllerEventArgs e)
        {
            _activeControllers[e.ControllerID] = true;
        }

        public bool Input()
        {
            var ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.Escape))
            {
                return false;
            }


            if (ks.IsKeyDown(Keys.Right))
            {
                if (currentScreen == Screens.Main && targetScreen == Screens.Main)
                {
                    targetScreen = Screens.PlayerSelect;
                    currentPositionCurve = mainToCharacterPosition;
                    currentLookAtCurve = mainToCharacterLookat;
                    time = 0;
                }
                else if (currentScreen == Screens.PlayerSelect && targetScreen == Screens.PlayerSelect)
                {
                    targetScreen = Screens.MapSelect;
                    currentPositionCurve = CharacterToMapPosition;
                    currentLookAtCurve = CharacterToMapLookat;
                    time = 0;
                }
            }
            else if (ks.IsKeyDown(Keys.Left))
            {
                if (currentScreen == Screens.PlayerSelect && targetScreen == Screens.PlayerSelect)
                {
                    targetScreen = Screens.Main;
                    currentPositionCurve = mainToCharacterPosition;
                    currentLookAtCurve = mainToCharacterLookat;
                    time = 1800;
                }
                else if (currentScreen == Screens.MapSelect && targetScreen == Screens.MapSelect)
                {
                    targetScreen = Screens.PlayerSelect;
                    currentPositionCurve = CharacterToMapPosition;
                    currentLookAtCurve = CharacterToMapLookat;
                    time = 1800;
                }
            }
            else if (ks.IsKeyDown(Keys.Up))
            {
                if (currentScreen == Screens.MapSelect && currentScreen == Screens.MapSelect)
                {
                    targetScreen = Screens.MapSelect1;
                    currentPositionCurve = Map1ToMap2Position;
                    currentLookAtCurve = Map1ToMap2Lookat;
                    time = 0;
                }
                else if (currentScreen == Screens.MapSelect2 && currentScreen == Screens.MapSelect2)
                {
                    targetScreen = Screens.MapSelect;
                    currentPositionCurve = Map1ToMap3Position;
                    currentLookAtCurve = Map1ToMap3Lookat;
                    time = 1800;
                }
            }
            else if (ks.IsKeyDown(Keys.Down))
            {
                if (currentScreen == Screens.MapSelect1 && targetScreen == Screens.MapSelect1)
                {
                    targetScreen = Screens.MapSelect;
                    currentPositionCurve = Map1ToMap2Position;
                    currentLookAtCurve = Map1ToMap2Lookat;
                    time = 1800;
                }
                else if (currentScreen == Screens.MapSelect && targetScreen == Screens.MapSelect)
                {
                    targetScreen = Screens.MapSelect2;
                    currentPositionCurve = Map1ToMap3Position;
                    currentLookAtCurve = Map1ToMap3Lookat;
                    time = 0;
                }
            }
            else if (ks.IsKeyDown(Keys.Enter))
            {
                if ((int)currentScreen >= (int)Screens.MapSelect && currentScreen == targetScreen)
                {
                    OnMapSelect();
                }
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

            //Camera.Instance.Input();
            if (currentScreen == Screens.PlayerSelect)
            {
                for (int i = 0; i < Players.Count; ++i)
                {
                    MenuPlayer player = Players[i];
                    //player.CenterOffset = i * 60;
                    player.Input();
                }
            }

            
            return true;

        }

        void inputManager_ButtonTriggered(InputManager sender, InputEventArgs e)
        {
            //if (e.ControllerID == ControllerID)
            {
                switch (e.ButtonID)
                {
                    case ContolPadButtons.A:
                        if ((int)currentScreen >= (int)Screens.MapSelect)
                        {
                            OnMapSelect();
                        }

                        if (currentScreen == Screens.Main && targetScreen == Screens.Main)
                        {
                            targetScreen = Screens.PlayerSelect;
                            currentPositionCurve = mainToCharacterPosition;
                            currentLookAtCurve = mainToCharacterLookat;
                            time = 0;
                        }
                        else if (currentScreen == Screens.PlayerSelect && targetScreen == Screens.PlayerSelect)
                        {
                            targetScreen = Screens.MapSelect;
                            currentPositionCurve = CharacterToMapPosition;
                            currentLookAtCurve = CharacterToMapLookat;
                            time = 0;
                        }
                        break;
                    case ContolPadButtons.B:
                        if (currentScreen == Screens.PlayerSelect && targetScreen == Screens.PlayerSelect)
                        {
                            targetScreen = Screens.Main;
                            currentPositionCurve = mainToCharacterPosition;
                            currentLookAtCurve = mainToCharacterLookat;
                            time = 1800;
                        }
                        else if (currentScreen == Screens.MapSelect && targetScreen == Screens.MapSelect)
                        {
                            targetScreen = Screens.PlayerSelect;
                            currentPositionCurve = CharacterToMapPosition;
                            currentLookAtCurve = CharacterToMapLookat;
                            time = 1800;
                        }
                        break;
                    case ContolPadButtons.Back:
                        return;
                    case ContolPadButtons.LeftShoulder:
                        break;
                    case ContolPadButtons.LeftStick:
                        break;
                    case ContolPadButtons.RightShoulder:
                        break;
                    case ContolPadButtons.RightStick:
                        break;
                    case ContolPadButtons.Start:
                        break;
                    case ContolPadButtons.X:
                        break;
                    case ContolPadButtons.Y:
                        break;
                    case ContolPadButtons.LeftTrigger:
                        break;
                    case ContolPadButtons.RightTrigger:
                        break;
                    case ContolPadButtons.DPadDown:
                        if (currentScreen == Screens.MapSelect1 && targetScreen == Screens.MapSelect1)
                        {
                            targetScreen = Screens.MapSelect;
                            currentPositionCurve = Map1ToMap2Position;
                            currentLookAtCurve = Map1ToMap2Lookat;
                            time = 1800;
                        }
                        else if (currentScreen == Screens.MapSelect && targetScreen == Screens.MapSelect)
                        {
                            targetScreen = Screens.MapSelect2;
                            currentPositionCurve = Map1ToMap3Position;
                            currentLookAtCurve = Map1ToMap3Lookat;
                        }
                        break;
                    case ContolPadButtons.DPadLeft:

                        break;
                    case ContolPadButtons.DPadRight:
 
                        break;
                    case ContolPadButtons.DPadUp:
                        if (currentScreen == Screens.MapSelect && currentScreen == Screens.MapSelect)
                        {
                            targetScreen = Screens.MapSelect1;
                            currentPositionCurve = Map1ToMap2Position;
                            currentLookAtCurve = Map1ToMap2Lookat;
                            time = 0;
                        }
                        else if (currentScreen == Screens.MapSelect2 && currentScreen == Screens.MapSelect2)
                        {
                            targetScreen = Screens.MapSelect;
                            currentPositionCurve = Map1ToMap3Position;
                            currentLookAtCurve = Map1ToMap3Lookat;
                            time = 1800;
                        }
                        break;
                    case ContolPadButtons.LeftThumbStickX:
                        break;
                    case ContolPadButtons.LeftThumbStickY:
                        break;
                    case ContolPadButtons.RightThumbStickX:
                        break;
                    case ContolPadButtons.RightThumbStickY:
                        break;
                    case ContolPadButtons.NumControlPadButtons:
                        break;
                    default:
                        break;
                }
            }
        }

        private void OnMapSelect()
        {
            GameplayState game = new GameplayState();

            switch (currentScreen)
            {
                case Screens.MapSelect:
                    game.MapName = "mesh\\plane";
                    break;
                case Screens.MapSelect1:
                    game.MapName = "mesh\\marMapv2";
                    break;
                case Screens.MapSelect2:
                    game.MapName = "mesh\\techDomeV3";
                    break;
            }

            GameplayState.GameSettings = new GameSettings();
            GameplayState.GameSettings.MapName = game.MapName;
            //game.GameSettings.MapName = game.MapName;

            for (int i = 0; i < Players.Count; ++i)
            {
                if (_activeControllers[i])
                {
                    PlayerSettings setting = new PlayerSettings();
                    setting.ControllerID = Players[i].ControllerID;
                    setting.Color = Players[i].PlayerColor;
                    game._playerSettings.Add(setting);
                    switch (Players[i].modelIndex)
                    {
                        case 0:
                            setting.ModelName = "Mesh/fastguy";
                            break;
                        case 2:
                            setting.ModelName = "Mesh/fastguy";
                            break;
                        case 1:
                            setting.ModelName = "Mesh/stealthguy";
                            break;
                        default:
                            setting.ModelName = "Mesh/fastguy";
                            break;
                    }
                }
            }
            CStateMachine.Instance.ChangeState(game);
        }

        public void Update(GameTime time)
        {
#if XBOX
            if (!_disconnectDetected)
#endif
            {
                if (targetScreen != currentScreen)
                {
                    switch (targetScreen)
                    {
                        case Screens.Main:
                            cameraToTarget = screen1Position;
                            break;
                        case Screens.PlayerSelect:
                            cameraToTarget = screen2Position;
                            break;
                        case Screens.MapSelect:
                            cameraToTarget = screen3Position;
                            break;
                        case Screens.MapSelect1:
                            cameraToTarget = screen4Position;
                            break;
                        case Screens.MapSelect2:
                            cameraToTarget = screen5Position;
                            break;
                        default:
                            break;
                    }

                    if (CameraAtPosition())
                    {
                        currentScreen = targetScreen;
                    }

                }
                UpdateCameraCurve(time);
                Camera.Instance.Update(false);

                for(int i = 0; i < Players.Count; ++i)
                {
                    MenuPlayer player = Players[i];
                    player.PlayerColor = PlayerColors[i];
                    player.Update(time);
                }
            }
        }

        bool CameraAtPosition()
        {
            if ((Camera.Instance.CameraPosition - cameraToTarget).Length() < 5f)
            {
                Camera.Instance.CameraPosition = cameraToTarget;
                currentLookAtCurve = null;
                currentPositionCurve = null;
                return true;
            }
            else
                return false;

        }

        void UpdateCameraCurve(GameTime gameTime)
        {
            // Calculate the camera's current position.

            Vector3 cameraPosition = Camera.Instance.CameraPosition;
            Vector3 cameraLookAt = Camera.Instance.CameraLookAt;

            if (CameraAtPosition() == false && currentPositionCurve != null)
            {
                cameraPosition = currentPositionCurve.GetPointOnCurve((float)time);
                cameraLookAt =
                    currentLookAtCurve.GetPointOnCurve((float)time);

                // Set up the view matrix and projection matrix.
                Camera.Instance.View = Matrix.CreateLookAt(cameraPosition, cameraLookAt,
                    Vector3.Cross(Vector3.UnitX, cameraLookAt - cameraPosition));//new Vector3(0.0f, 1.0f, 0.0f));

                Camera.Instance.CameraPosition = cameraPosition;
                Camera.Instance.CameraLookAt = cameraLookAt;
            }
            else
            {
                //time = 0.0f;
            }

            time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        public void Render()
        {
            world.Render();
            for (int i = 0; i < Players.Count; ++i )
            {
                if (_activeControllers[i])
                {
                    MenuPlayer player = Players[i];
                    player.Render();
                }
            }
        }

        public void Exit()
        {
            var inputManager = ServiceProvider.GetService<InputManager>();
            inputManager.ButtonTriggered -= new InputEventHandler(inputManager_ButtonTriggered);
            inputManager.ControllerConnected -= new ControllerEventHandler(inputManager_ControllerConnected);
            inputManager.ControllerDisconnected -= new ControllerEventHandler(inputManager_ControllerDisconnected);
        }

    }
}
