using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using FlocKing.Helpers;


namespace FlocKing.Objects
{
    public class King : Boid
    {
        public bool DirectDrive { get; set; }
        private bool _revealed;
        Player player;
        public Flock nearestFlock;

        SoundEffect activeSoundEffect;

        public King(Vector3 posInit, Player player, string ModelName) : base(posInit, ModelName)
        {
            this.player = player;
            DirectDrive = false;
            Color = player.Color;
        }

        void OnKingIsDead(Player killer)
        {
            if (killer == null)
            {
                Console.WriteLine("THE KINGS ARE DEAD!\nTIE");
            }
            else
            {
                Console.WriteLine("THE KING IS DEAD!\nKilled by player " + killer.ControllerID);
            }
            Position = new Vector3(0.0f, -100000.0f, 0.0f);// prevent collisions with the king, since he is not deleted like other boids

            GameplayState.EvaluateEndGame();
            Content = ServiceProvider.GetService<ContentManager>();
            Content.Load<SoundEffect>("Sounds/King_Dies").Play(Game1.Instance.SoundEffectsVolume, 0, 0);
            Content.Load<SoundEffect>("Sounds/Explosion").Play(Game1.Instance.SoundEffectsVolume, 0, 0);
        }

        public bool Revealed
        {
            get { return _revealed; }
            set
            {
                if (value != _revealed)
                {
                    _revealed = value;
                    Console.WriteLine("King is revealed for player " + player.ControllerID);
                    player.KingIsRevealed();
                }
            }
        }

        public override void Loadcontent()
        {
            base.Loadcontent();
           // _alternateModel = Content.Load<Model>("Mesh/cursor1");
        }

        internal void Update(GameTime time)
        {
            if (DirectDrive)
            {
                float deltaTime = (float)time.ElapsedGameTime.TotalSeconds;
                // Cap velocity
                ClampVelocity();
                // Update position
                _position += _velocity * deltaTime;
                UpdateRenderable();
            }
            else
            {
                // Find the nearest flock and go with it
                nearestFlock = player.flocks[0];
                float nearestFlockDistance = Vector3.Distance(nearestFlock.FlockCenter, Position);
                for (int i = 1; i < player.flocks.Count; i++)
                {
                    Flock flock = player.flocks[i];
                    float distance = Vector3.Distance(flock.FlockCenter, Position);
                    if (distance < nearestFlockDistance)
                    {
                        nearestFlockDistance = distance;
                        nearestFlock = flock;
                    }
                }
                base.Update(time/*(float)time.ElapsedGameTime.TotalSeconds*/, nearestFlock);
            }
        }

        public override void Render()
        {
            if (Dead)
                return;
            if (nearestFlock.Selected)
                Color = nearestFlock.SelectedColor;
            else
                Color = nearestFlock.Color;
            base.Render();
        }

        internal void checkCollision(Player otherPlayer)
        {
            if (Dead)
                return;
            foreach (Flock flock in otherPlayer.flocks)
            {
                foreach (Boid boid in flock.Boids)
                {
                    if (!boid.Dead && !Dead)
                    {
                        bool collision = boid.CollidesWith(this);
                        if (collision)
                        {
                            boid.Dead = true;
                            Dead = true;
                            Flock.deathFire.AddParticles(boid.Position);
                            Flock.deathFire.AddParticles(this.Position);
                            OnKingIsDead(otherPlayer);
                        }
                    }
                }
                // King on King action
                if (otherPlayer.king.CollidesWith(this))
                {
                    otherPlayer.king.Dead = true;
                    Dead = true;
                    Flock.deathFire.AddParticles(otherPlayer.king.Position);
                    Flock.deathFire.AddParticles(this.Position);
                    OnKingIsDead(null);
                    otherPlayer.king.OnKingIsDead(null);
                }
            }
        }
    }
}
