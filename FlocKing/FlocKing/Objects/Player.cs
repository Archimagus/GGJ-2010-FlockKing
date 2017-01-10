using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using FlocKing.Helpers;
using Microsoft.Xna.Framework.Input;
using FlocKing.Animations;
using FlocKing.StateMachine;
using Microsoft.Xna.Framework.Audio;




namespace FlocKing.Objects
{
    public class Player : ThreeDRenderable
    {
        public Vector3 Velocity { get; set; }
        public List<Flock> flocks = new List<Flock>();
        public King king;
        KingMarker kingMarker;
        public PlayerIndex ControllerID { get; set; }
        public int SelectedFlock { get; set; }
        public string ModelName { get; set; }


        SoundEffect activeSoundEffect;


        public Player(string ModelName)
        {
            this.ModelName = ModelName;
        }

        public void initializeFlock(int howManyBoids)
        {
            // Initialize the first flock


            Flock flock = new Flock(this);
            flock.Target = Position;
            Vector3 posInit = new Vector3(0.0f, 0.0f, 0.0f);
            for (int i = 0; i < howManyBoids; i++)
            {
                Boid boid = new Boid(Position + posInit, ModelName);
                flock.Boids.Add(boid);
                // Change position of the next one
                posInit.X += 1.0f;
                posInit.Z += 1.0f;
            }
            flocks.Add(flock);

            // Initialize the king
            king = new King(Position, this, ModelName)
            {
                Color = this.Color
            };
            kingMarker = new KingMarker(king);

            var inputManager = ServiceProvider.GetService<InputManager>();
            inputManager.ButtonTriggered += new InputEventHandler(inputManager_ButtonTriggered);
        }

        public override void Loadcontent()
        {
           // Model = Content.Load<Model>("Mesh/cursor1");
            Model = Content.Load<Model>("Mesh/curser");
           // Model = Content.Load<Model>("Mesh/wayPoint");
            Animating = true;
            if (Animating)
            {
                SkinningData skinningData = Model.Tag as SkinningData;
                if (skinningData == null)
                    throw new InvalidOperationException
                        ("This model does not contain a SkinningData tag.");

                // Create an animation player, and start decoding an animation clip.
                AnimationPlayer = new AnimationPlayer(skinningData);

                AnimationClip clip = skinningData.AnimationClips["Take 001"];

                AnimationPlayer.StartClip(clip);
            }
            

            foreach (Flock flock in flocks)
                flock.Loadcontent();
            king.Loadcontent();
            kingMarker.Loadcontent();
        }
        public void Input()
        {
            var gamePad = GamePad.GetState(ControllerID);
            Velocity = new Vector3(gamePad.ThumbSticks.Left.X, 0, -gamePad.ThumbSticks.Left.Y);
            Velocity *= 150.0f;
            if (gamePad.ThumbSticks.Right.LengthSquared() > 0)
            {
                king.DirectDrive = true;
                king.Velocity = new Vector3(gamePad.ThumbSticks.Right.X, 0, -gamePad.ThumbSticks.Right.Y) * 150.0f;
            }
            else
            {
                king.DirectDrive = false;
            }
        }

        void inputManager_ButtonTriggered(InputManager sender, InputEventArgs e)
        {
            if (GameplayState.paused)
                return;
            if (e.ControllerID == ControllerID)
            {
                switch (e.ButtonID)
                {
                    case ContolPadButtons.A:
                        PlaceWaypoint();
                        break;
                    case ContolPadButtons.B:
                        ClearWaypoints();
                        break;
                    case ContolPadButtons.Back:
                        break;
                    case ContolPadButtons.LeftShoulder:
                        BumpSelectedFlock(-1);
                        break;
                    case ContolPadButtons.LeftStick:
                        break;
                    case ContolPadButtons.RightShoulder:
                        BumpSelectedFlock(1);
                        break;
                    case ContolPadButtons.RightStick:
                        break;
                    case ContolPadButtons.Start:
                        CStateMachine.Instance.PushState(new PauseHelpGameState());
                        break;
                    case ContolPadButtons.X:
                        SplitFlock();
                        break;
                    case ContolPadButtons.Y:
                        MergeFlock();
                        break;
                    case ContolPadButtons.LeftTrigger:
                        // Reveal false (deceptfull) king
                        kingMarker.ShowMarker(getFalseKing());
                        break;
                    case ContolPadButtons.RightTrigger:
                        // Reveal king
                        kingMarker.ShowMarker(king);
                        break;
                    case ContolPadButtons.DPadDown:
                        break;
                    case ContolPadButtons.DPadLeft:
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

        private Boid getFalseKing()
        {
            int index = Game1.Instance.Random.Next(0, flocks.Count - 1);
            Flock flock = flocks[index];
            int kingIndex = Game1.Instance.Random.Next(0, flock.Boids.Count - 1);
            return flock.Boids[kingIndex];
        }

        private void ClearWaypoints()
        {
            Flock f = flocks[SelectedFlock];
            f.StopFlockInPlace();
        }

        private void PlaceWaypoint()
        {
            Flock f = flocks[SelectedFlock];
            f.AddWayPoint(Position);
        }

        int minimumBoids = 5;
        private void SplitFlock()
        {
            var selectedFlock = flocks[SelectedFlock];
            if (selectedFlock.Boids.Count > minimumBoids)
            {
                var splitPoint = selectedFlock.Boids.Count / 2;
                var flock = new Flock(this);
                flock.Target = selectedFlock.Target;
                for (int i = selectedFlock.Boids.Count-1; i > splitPoint; i--)
                {
                    flock.Boids.Add(selectedFlock.Boids[i]);
                    selectedFlock.Boids.RemoveAt(i);
                }
                flocks.Add(flock);
                Content.Load<SoundEffect>("Sounds/Split").Play(Game1.Instance.SoundEffectsVolume, 0, 0);
            }
        }

        private void MergeFlock()
        {
            var selectedFlock = flocks[SelectedFlock];
            var selectedPosition = selectedFlock.Target;
            float nearest = float.MaxValue;
            Flock nearestFlock = null;
            foreach (var flock in flocks)
            {
                float distanceSquared = (flock.Target - selectedPosition).LengthSquared();
                if (flock != selectedFlock &&  distanceSquared < nearest)
                {
                    nearest = distanceSquared;
                    nearestFlock = flock;
                }
            }
            if(nearestFlock != null)
            {
                foreach (var boid in nearestFlock.Boids)
                {
                    selectedFlock.Boids.Add(boid);
                }
                selectedPosition = selectedPosition + ((nearestFlock.Target - selectedPosition) * 0.5f);
                selectedFlock.Target = selectedPosition;
                flocks.Remove(nearestFlock);
                SelectedFlock = flocks.IndexOf(selectedFlock);
                Content.Load<SoundEffect>("Sounds/Merge").Play(Game1.Instance.SoundEffectsVolume, 0, 0);
            }

            // Clean empty flocks - this was a fix for something that turned out to be something else... probably not neccarassryt anymore
            for (int i = 0; i < flocks.Count; )
            {
                Flock flock = flocks[i];
                if (flock.Boids.Count == 0)
                {
                    flocks.RemoveAt(i);
                }
                else
                    i++;
            }
        }

        private void BumpSelectedFlock(int amount)
        {
            SelectedFlock += amount;
            if (SelectedFlock < 0)
                SelectedFlock = flocks.Count - 1;
            if (SelectedFlock > flocks.Count - 1)
                SelectedFlock = 0;
                Content.Load<SoundEffect>("Sounds/ChangeFlock_New").Play(Game1.Instance.SoundEffectsVolume, 0, 0);
        }

        public void Update(GameTime time)
        {
            if ((float)time.ElapsedGameTime.TotalSeconds == 0.0f)
                return;
            Vector3 step = Velocity * (float)time.ElapsedGameTime.TotalSeconds;
            //ContainmentType? con = GameplayState._world.WorldBounds.Contains(Position + step);
            //if (con != null && con.Value == ContainmentType.Contains)
                Position += step;
           if(Animating)
                AnimationPlayer.Update(time.ElapsedGameTime, true, Matrix.Identity);

            if (SelectedFlock > flocks.Count - 1)
                SelectedFlock = 0;


            foreach (Flock flock in flocks)
            {
                flock.Selected = false;
                flock.Color = Color;
                flock.SelectedColor = Color.Lerp(Color, Color.White, 0.5f);
                flock.Update(time);
            }
            flocks[SelectedFlock].Selected = true;
            king.Update(time);
            kingMarker.Update(time);
        }
        public override void Render()
        {
            base.Render();
            foreach (Flock flock in flocks)
                flock.Render();
            king.Render();
            kingMarker.Render();
        }

        internal void FlocksCollidesWith(Player otherPlayer)
        {
            foreach (Flock flock in flocks)
            {
                foreach (Flock otherFlock in otherPlayer.flocks)
                {
                    flock.FlockCollidesWith(otherFlock);
                }
            }
            king.checkCollision(otherPlayer);
        }

        internal void CleanDeadBoids()
        {
            foreach (Flock flock in flocks)
            {
                flock.CleanDeadBoids();
            }
        }

        internal void KingIsRevealed()
        {
            kingMarker.KingIsRevealed(king);
        }

        internal void reset()
        {
            flocks.Clear();
        }
    }
}
