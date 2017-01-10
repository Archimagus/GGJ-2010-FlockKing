using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using FlocKing.Animations;
using System.Diagnostics;

namespace FlocKing.Objects
{
    public class Boid : ThreeDRenderable
    {
        #region Properties
        public Vector3 Velocity
        {
            get { return _velocity; }
            set 
            { 
                 Debug.Assert(!float.IsNaN(value.X));
                _velocity = value; 
            }
        }
        #endregion

        protected Vector3 _position = new Vector3();
        protected Vector3 _heading = new Vector3(1.0f, 0.0f, 0.0f);
        protected Vector3 _velocity = new Vector3();
        protected Vector3 _acceleration = new Vector3();

        Vector3 scratch = new Vector3();

        static float neighborhoodRadius = 25.0f;
        static float targetRadius = 7.0f;

        static float alignmentWeight = 0.001f;
        static float cohesionWeight = 0.2f;
        static float seperationWeight = 0.5f;
        static float targetWeight = 0.5f;

        static float maxVelocity = 20.0f;
        static float minVelocity = 10.0f;

        public bool Dead { get; set; }
        public bool IsReachedTarget { get; set; }
        public string ModelName { get; set; }

        public Boid(Vector3 posInit, string modelName)
        {
            _position = posInit;
            _heading.Z =  Game1.Instance.Random.Next(1, int.MaxValue) / (float)int.MaxValue;
            //heading.Y = FlocKing.Instance.Game1.Random.Next(1, int.MaxValue)/(float)int.MaxValue;
            _heading.X = Game1.Instance.Random.Next(1, int.MaxValue) / (float)int.MaxValue;
            _heading.Normalize();
            _velocity = _heading * maxVelocity;
            Debug.Assert(!float.IsNaN(_velocity.X));
            UpdateRenderable();
            IsReachedTarget = false;
            ModelName = modelName;
        }

        public void Update(GameTime time, Flock flock)
        {
            // Accelerate to a desired direction with a wighted magnitude
            _acceleration = Vector3.Zero;
            if (Animating)
                AnimationPlayer.Update(time.ElapsedGameTime, true, Matrix.Identity);
            float deltaTime = (float)time.ElapsedGameTime.TotalSeconds;
            if (AvoidingAnObstacle(deltaTime) == false && flock != null)
                FlockingUpdate(deltaTime, flock);

            // Add acceleration to the velocity
            _velocity += _acceleration * deltaTime;// *deltaTime;
            Debug.Assert(!float.IsNaN(_velocity.X));
            // Cap velocity
            ClampVelocity();
            // Update position
            _position += _velocity * deltaTime;
            UpdateRenderable();
        }

        static float lookAHeadDistance = 20.0f;
        private bool AvoidingAnObstacle(float deltaTime)
        {
            // Check for the nearest obstacle hitting a line along the heading
            if (GameplayState._world == null)
                return false;
            List<BoundingSphere> obstacles = GameplayState._world.CollisionObjects;//{ };//new BoundingSphere(Vector3.Zero, 30.0f) };
            float nearest = 1000000000.0f;
            BoundingSphere mostThreatening = new BoundingSphere(Vector3.Zero, 0.0f);
            for (int i = 0; i < obstacles.Count; i++)
            {
                BoundingSphere obstacle = obstacles[i];
                float? distance = obstacle.Intersects(new Ray(Position, Heading));
                if (distance != null && distance.Value < lookAHeadDistance)
                {
                    if (distance < nearest)
                    {
                        //Console.WriteLine("I see an obstacle at distance: " + distance);
                        nearest = distance.Value;
                        mostThreatening = obstacle;
                    }
                }
            }
            if (mostThreatening.Radius != 0.0f)
            {
                Vector3 fromBoidToObstacleCenter = Position - mostThreatening.Center;
                _acceleration = Vector3.Cross(fromBoidToObstacleCenter, Vector3.Up);
                _acceleration.Normalize();
                _acceleration *= maxVelocity;
                return true;
            }
            return false;
        }

        private void FlockingUpdate(float deltaTime, Flock flock)
        {
            // Gather alignment, cohesion and seperation vectors
            Vector3 alignment = getAlignment(flock.Boids);
            Vector3 cohesion = getCohesion(flock.Boids, flock.Target);
            Vector3 seperation = getSeperation(flock.Boids);
            // Apply these vectors on the acceleration with the weights 
            _acceleration += alignment * alignmentWeight;
            _acceleration += cohesion * cohesionWeight;
            _acceleration += seperation * seperationWeight;

            // Target position to flock to
            float distanceToTarget = Vector3.Distance(flock.Target, _position);
            if (distanceToTarget < (targetRadius + Bounds.Radius))
            {
                //if (!IsReachedTarget)
                //    Console.WriteLine("Boid reached target");
                IsReachedTarget = true;
            }
            //else
            {
                Vector3 target = getTargetHeading(flock.Target);
                _acceleration += target * targetWeight;
                _acceleration *= distanceToTarget;
            }
        }

        protected void UpdateRenderable()
        {
            _heading = _velocity;
            _heading.Normalize();

            Debug.Assert(!float.IsNaN(_heading.X));

             Heading = -_heading;
            //if (position.Y < 0.0f)
            //    position.Y = 0.0f;
            if (_position.X != float.NaN)
                Position = _position;
        }

        public override bool CollidesWith(ICOllideable target)
        {
            bool collision = base.CollidesWith(target);
            //if (collision)
            //    Console.WriteLine("Collision between " + this + " and " + target);
            return collision;
        }

        private Vector3 getTargetHeading(Vector3 target)
        {
            if (target.Equals(_position))
                return Heading;
            Vector3 targetHeading = target - _position;
            targetHeading.Normalize();
            return targetHeading;
        }

        public Vector3 getAlignment(List<Boid> flock)
        {
            scratch = _heading;
            int count = 0;
            for (int i = 0; i < flock.Count; i++)
            {
                Boid boid = flock[i];
                if (boid == this)
                    continue;
                float distance = Vector3.Distance(_position, boid.Position);
                if (distance != 0.0f && distance < neighborhoodRadius)
                {
                    count++;
                    scratch += boid.Heading;
                }
            }
            if (count > 0)
                scratch /= count;
            scratch.Normalize();
            return scratch;
        }

        public Vector3 getCohesion(List<Boid> flock, Vector3 target)
        {
            scratch = target;
            int count = 0;
            for (int i = 0; i < flock.Count; i++)
            {
                Boid boid = flock[i];
                if (boid == this)
                    continue;
                float distance = Vector3.Distance(_position, boid.Position);
                if (distance != 0.0f && distance < neighborhoodRadius)
                {
                    count++;
                    scratch += boid.Position;
                }
            }
            if (count > 0)
                scratch /= count;
            Vector3 steer = scratch - _position;
            if (steer.Equals(Vector3.Zero))
                return Vector3.Zero;
            steer.Normalize();
            return steer;
        }

        public Vector3 getSeperation(List<Boid> flock)
        {
            scratch = Vector3.Zero;
            Vector3 repulse;
            for (int i = 0; i < flock.Count; i++)
            {
                Boid boid = flock[i];
                if (boid == this)
                    continue;
                float distance = Vector3.Distance(_position, boid.Position);
                if (distance != 0.0f && distance < neighborhoodRadius)
                {
                    repulse = _position - boid.Position;
                    repulse.Normalize();
                    repulse /= distance;
                    scratch += repulse;
                }
            }
            return scratch;
        }

        protected void ClampVelocity()
        {
            float speed = _velocity.Length();

            if (speed > maxVelocity)
            {
                _velocity.Normalize();
                _velocity *= maxVelocity;
                Debug.Assert(!float.IsNaN(_velocity.X));
            }
            else if (speed < minVelocity)
            {
                _velocity.Normalize();
                _velocity *= minVelocity;
                Debug.Assert(!float.IsNaN(_velocity.X));
            }
        }

        public override void Loadcontent()
        {
            Model = Content.Load<Model>(ModelName);
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
        }
    }
}
