using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using FlocKing.Particles;
using FlocKing.Helpers;
using Microsoft.Xna.Framework.Audio;
using System.Linq;
using System.Diagnostics;

namespace FlocKing.Objects
{
    public class Flock
    {

        #region Properties
        public List<Boid> Boids
        {
            get { return boids; }
        }
        public Vector3 Target
        {
            get { return target; }
            set { target = value; }
        }
        public bool Selected { get; set; }
        public Color SelectedColor { get; set; }
        public Color Color { get; set; }
        #endregion

        Player player;
        List<Boid> boids = new List<Boid>();
        Vector3 target = new Vector3();
        public List<WayPoint> wayPoints = new List<WayPoint>();
        public Vector3 FlockCenter { get; set; }
        public Texture2D DeathFireTexture { get; set; }
        public static ParticleSystem deathFire;

        ContentManager Content;
        SoundEffect activeSoundEffect;

        public Flock(Player player)
        {
            this.player = player;
            Content = ServiceProvider.GetService<ContentManager>();
        }

        void UpdateTargetReached()
        {
            int reached = 0;
            foreach (Boid boid in boids)
            {
                if (boid.IsReachedTarget)
                    reached++;
            }
            if (reached > (boids.Count * 0.7f))
            {
                // Reset
                foreach (Boid boid in boids)
                    boid.IsReachedTarget = false;
                if (wayPoints.Count > 0)
                {
                    wayPoints.RemoveAt(0);
                    Console.WriteLine("Target Reached!");
                    Content.Load<SoundEffect>("Sounds/Arrival_Toot").Play(Game1.Instance.SoundEffectsVolume, 0, 0);
                }
                // TODO SOUND target reached

            }
        }

        public void AddWayPoint(Vector3 point)
        {
            wayPoints.Add(new WayPoint(point, Color));
            // TODO SOUND
            Content.Load<SoundEffect>("Sounds/Silly_Toot_New").Play(Game1.Instance.SoundEffectsVolume, 0, 0);
        }

        public void ClearWayPoints()
        {
            wayPoints.Clear();
            Content.Load<SoundEffect>("Sounds/Cancel_New").Play(Game1.Instance.SoundEffectsVolume, 0, 0);
        }

        public void StopFlockInPlace()
        {
            ClearWayPoints();
            Debug.Assert(!float.IsNaN(FlockCenter.X));
            target = FlockCenter;
        }

        private Vector3 UpdateFlockCenter()
        {
            FlockCenter = Vector3.Zero;
            if (boids.Count > 0)
            {
                for (int i = 0; i < boids.Count; i++)
                {
                    Boid boid = boids[i];
                    FlockCenter += boid.Position;
                }
                FlockCenter /= boids.Count;
            }
            return FlockCenter;
        }

        public void Update(GameTime time)
        {
            if (wayPoints.Count > 0)
            {
                Debug.Assert(!float.IsNaN(wayPoints[0].Position.X));
                target = wayPoints[0].Position;
            }

            foreach (Boid boid in boids)
                boid.Update(time, this);

            UpdateFlockCenter();
            // update the king
            UpdateTargetReached();

            deathFire.Update(time);
            foreach (WayPoint point in wayPoints)
                point.AnimationPlayer.Update(time.ElapsedGameTime, true, Matrix.Identity);
        }

        public void Loadcontent()
        {
            foreach (Boid boid in boids)
                boid.Loadcontent();

            var content = ServiceProvider.GetService<ContentManager>();
            DeathFireTexture = content.Load<Texture2D>("Textures\\Blood");

            deathFire = new ParticleSystem(1, DeathFireTexture, "FireDeath.xml");
        }

        public void Render()
        {
            foreach (Boid boid in boids)
            {
                boid.Color = Selected ? SelectedColor : Color;
                boid.Render();
            }

            deathFire.Draw(null);
            foreach (WayPoint point in wayPoints)
                point.Render();
        }


        internal void FlockCollidesWith(Flock otherFlock)
        {
            foreach (Boid boid in boids)
            {
                if (!boid.Dead)
                {
                    foreach (Boid otherBoid in otherFlock.Boids)
                    {
                        // Finally! :D
                        if (!otherBoid.Dead)
                        {
                            bool collision = boid.CollidesWith(otherBoid);
                            if (collision)
                            {
                                // Check oreintation
                                //float dotHeading = Vector3.Dot(boid.Heading, otherBoid.Heading);
                                //if (dotHeading < 0.0f)
                                {
                                    // They are heading towards each other, kill them both
                                    Console.WriteLine("TWO Boids (and all humans) are DEAD");
                                    Content.Load<SoundEffect>("Sounds/Bamf_Wave_New").Play(Game1.Instance.SoundEffectsVolume, 0, 0);
                                    boid.Dead = true;
                                    otherBoid.Dead = true;
                                    deathFire.AddParticles(boid.Position);
                                    deathFire.AddParticles(otherBoid.Position);
                                }
                                //else
                                //{
                                //    // Who is behind who? He will survive
                                //    Vector3 collisionAxis = boid.Position - otherBoid.Position;
                                //    collisionAxis.Normalize();
                                //    float boidProj = Vector3.Dot(boid.Position, collisionAxis);
                                //    float otherBoidProj = Vector3.Dot(otherBoid.Position, collisionAxis);
                                //    if (otherBoidProj > boidProj)
                                //    {
                                //        // otherBoid pawns boid
                                //        Console.WriteLine("ONE Boid is DEAD (totally pawned)");
                                //        boid.Dead = true;
                                //        deathFire.AddParticles(boid.Position);
                                //    }
                                //    else
                                //    {
                                //        // boid pawns otherBoid
                                //        Console.WriteLine("ONE Boid is DEAD (totally pawned)");
                                //        otherBoid.Dead = true;
                                //        deathFire.AddParticles(otherBoid.Position);
                                //    }
                                //    activeSoundEffect = Content.Load<SoundEffect>("Sounds/Gotcha");
                                //    activeSoundEffect.Play();
                                //}

                            }
                        }
                    }
                }
            }
        }

        internal void CleanDeadBoids()
        {
            for (int i = 0; i < boids.Count; )
            {
                if (boids[i].Dead)
                    boids.RemoveAt(i);
                else
                    i++;
            }
        }
    }
}
