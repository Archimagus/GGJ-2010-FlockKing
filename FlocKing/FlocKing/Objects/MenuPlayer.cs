using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FlocKing.Helpers;
using Microsoft.Xna.Framework.Input;

namespace FlocKing.Objects
{
    class MenuPlayer : ThreeDRenderable
    {
        List<Boid> allBoids = new List<Boid>();
        List<Boid> boids = new List<Boid>();
        List<int> transitions = new List<int>();
        List<Vector3> transitionPosition = new List<Vector3>();

        public Color PlayerColor { get; set; }

        public PlayerIndex ControllerID { get; set; }
        public int SelectedIndex { get; set; }

        public int CenterOffset { get; set; }

        int currentIndex = 0;

        Vector3 center = new Vector3(700, 0, -90);

        Curve3D selectionPositions = new Curve3D();

        float transitionTime = -2000;
        float timeInTransition = 0;

        public int modelIndex = 0;

        public override void Loadcontent()
        {
            boids.Add(new Boid(Vector3.Zero, "Mesh/fastguy"));
            boids.Add(new Boid(Vector3.Zero, "Mesh/fastguy"));
            boids.Add(new Boid(Vector3.Zero, "Mesh/stealthguy"));
                        
            selectionPositions.AddPoint(new Vector3(center.X, center.Y + 20, center.Z + 25), 0);
            selectionPositions.AddPoint(new Vector3(center.X - 30, center.Y - 20, center.Z - 10), 700);
            selectionPositions.AddPoint(new Vector3(center.X + 20, center.Y - 20, center.Z - 10), 1400);
            selectionPositions.AddPoint(new Vector3(center.X, center.Y + 20, center.Z + 25), 2000);

            selectionPositions.SetTangents();

            for (int i = 0; i < boids.Count; ++i)
            {
                boids[i].Loadcontent();
                transitions.Add(i * 2000 / boids.Count);
                transitionPosition.Add(selectionPositions.GetPointOnCurve(i * 2000 / boids.Count));
            }


            var inputManager = ServiceProvider.GetService<InputManager>();
            inputManager.ButtonTriggered += new InputEventHandler(inputManager_ButtonTriggered);
        }

        public void Update(GameTime time)
        {
            if (SelectedIndex != currentIndex)
            {
                //if (SelectedIndex > currentIndex)
                {
                    //if (transitionTime >= 2000) 
                    //    transitionTime = 0;
                    transitionTime -= (float)time.ElapsedGameTime.TotalMilliseconds;

                }
                //else
                {
                    //if (transitionTime <= 0) 
                    //    transitionTime = 2000;
                  //  transitionTime -= (float)time.ElapsedGameTime.TotalMilliseconds;
                }

                timeInTransition += (float)time.ElapsedGameTime.TotalMilliseconds;

                if (timeInTransition >= 2000 / boids.Count)
                {
                    currentIndex = SelectedIndex;
                    timeInTransition = 0;
                }
            }

            for (int i = 0; i < boids.Count; ++i)
            {
                boids[i].Update(time, null);
            }

            
        }

        void inputManager_ButtonTriggered(InputManager sender, InputEventArgs e)
        {
            if (e.ControllerID == ControllerID)
            {
                switch (e.ButtonID)
                {
                    case ContolPadButtons.A:
                        if (currentIndex == SelectedIndex)
                        {
                            modelIndex--;
                            if (modelIndex < 0)
                                modelIndex = 2;
                            SelectedIndex--;
                        }
                        break;
                    case ContolPadButtons.B:
                        break;
                    case ContolPadButtons.Back:
                        break;
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
                        if (currentIndex == SelectedIndex)
                            SelectedIndex--;
                        break;
                    case ContolPadButtons.RightTrigger:
                        if (currentIndex == SelectedIndex)
                            SelectedIndex++;
                        break;
                    case ContolPadButtons.DPadDown:
                        break;
                    case ContolPadButtons.DPadLeft:
                        SelectedIndex--;
                        break;
                    case ContolPadButtons.DPadRight:
                        break;
                    case ContolPadButtons.DPadUp:
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


        public void Input()
        {
            var gamePad = GamePad.GetState(ControllerID);

            if (currentIndex == SelectedIndex)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    SelectedIndex++;
                    if (SelectedIndex >= this.boids.Count)
                        SelectedIndex = 0;
                }
                //else if (Keyboard.GetState().IsKeyDown(Keys.D))
                //{
                //    SelectedIndex++;
                //    if (SelectedIndex >= this.boids.Count)
                //        SelectedIndex = 0;
                //}
            }
        }
        public override void Render()
        {
            //base.Render();

            for(int i = 0; i < boids.Count; ++i)
            {
                //Console.WriteLine(transitionTime + i * (2000 / models.Count));
                Vector3 position = selectionPositions.GetPointOnCurve((transitionTime + i * (2000 / boids.Count)) % 2000);
                
                switch (ControllerID)
                {
                    case PlayerIndex.One:
                        break;
                    case PlayerIndex.Two:
                        position.X += 300;
                        break;
                    case PlayerIndex.Three:
                        position.Z += 70;
                        break;
                    case PlayerIndex.Four:
                        position.X += 300;
                        position.Z += 70;
                        break;
                    default:
                        break;
                }

                if (i == SelectedIndex)
                {
                    Vector3 diff = Camera.Instance.CameraPosition - position;
                    diff.Normalize();

                    //if(timeInTransition > 0)
                    position += diff * 400;
                }

                boids[i].Position = position;
                boids[i].Color = PlayerColor;

                
                boids[i].Render();
            }
        }
    }
}
