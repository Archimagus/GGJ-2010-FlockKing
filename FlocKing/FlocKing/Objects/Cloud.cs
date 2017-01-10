using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FlocKing.Helpers;

using FlocKing.Particles;


namespace FlocKing.Objects
{
    public class Cloud : ICOllideable
    {

        ParticleSystem particleSystem;
        ParticleSystem rainSplash;
        ParticleSystem rainSystem;

        public Texture2D CloudTexture { get; set; }
        public Texture2D SplashTexture { get; set; }
        public Texture2D RainTexture { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Position { get; set; }

        public float Scale { get; set; }
        float totalGameTime;
        float timeToNewPosition;

        public BoundingSphere WorldBounds { get; set; }

        bool roam = true;

        public void Loadcontent()
        {
            var content = ServiceProvider.GetService<ContentManager>();
            CloudTexture = content.Load<Texture2D>("Particles/smoke2");
            SplashTexture = content.Load<Texture2D>("Particles/fire");
            RainTexture = content.Load<Texture2D>("Particles/rain");

            
            Initialize();
        }

        public void Update(GameTime time)
        {
            Position += Velocity * (float)time.ElapsedGameTime.TotalSeconds;
            particleSystem.AddParticles(Position);
            particleSystem.Update(time);

            CreateRain(time);

            totalGameTime += (float)time.ElapsedGameTime.TotalSeconds;

            if (roam)
            {
                Scale = (totalGameTime / 20.0f);

                timeToNewPosition -= totalGameTime;

                if (timeToNewPosition < 0)
                {
                    var graphics = ServiceProvider.GetService<GraphicsDeviceManager>();

                    Vector3 TargetPosition = new Vector3(FlocKing.Game1.RandomFloatBetween(-WorldBounds.Radius, WorldBounds.Radius),
                        0,
                        FlocKing.Game1.RandomFloatBetween(-WorldBounds.Radius, WorldBounds.Radius));
                    Vector3 diff = TargetPosition - Position;
                    diff.Normalize();
                    diff.Y = 0;
                    diff *= 2;

                    Velocity = diff;

                    particleSystem.Direction += Velocity;

                    List<Particle> particles = particleSystem.Particles();

                    foreach (Particle p in particles)
                    {
                        p.Velocity = Velocity;
                    }

                    timeToNewPosition = FlocKing.Game1.RandomFloatBetween(1000, 20000);

                }

                if (Scale < 0)
                    Scale = 0;
                if (Scale > 3)
                    Scale = 3;

                particleSystem.AdjustScale(Scale);
            }

        }

        private void CreateRain(GameTime time)
        {
            List<Vector3> cloudPositions = particleSystem.ParticlePositions();

            foreach (Vector3 position in cloudPositions)
            {
                int random = Game1.Instance.Random.Next(100);
                if (random > 50)
                {
                    rainSystem.AddParticles(new Vector3(position.X + FlocKing.Game1.RandomFloatBetween(0, 30 * Scale), FlocKing.Game1.RandomFloatBetween(2, position.Y), position.Z + FlocKing.Game1.RandomFloatBetween(0, 15 * Scale)));
                    if (random > 90)
                    {

                        rainSplash.AddParticles(new Vector3(position.X + FlocKing.Game1.RandomFloatBetween(0, 30 * Scale), 0f, position.Z + FlocKing.Game1.RandomFloatBetween(0, 15 * Scale)));
                    }
                }
            }

            rainSystem.Update(time);
            rainSplash.Update(time);
        }

        public void GetInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Delete))
            {
                particleSystem.Initialize();
                rainSplash.Initialize();
                rainSystem.Initialize();
            }
        }

        public void Initialize()
        {
            particleSystem = new ParticleSystem(1, CloudTexture, "Cloud.xml");
            rainSplash = new ParticleSystem(1, CloudTexture, "Splash.xml");
            rainSystem = new ParticleSystem(1, RainTexture, "Rain.xml");
            Position = new Vector3(0, 100, 0);
            Velocity = new Vector3(FlocKing.Game1.RandomFloatBetween(0, 3),
                0,
                FlocKing.Game1.RandomFloatBetween(0, 3));

            particleSystem.AlphaModifier = 1.0f;
        }

        public void Render()
        {
            var spriteBatch = ServiceProvider.GetService<SpriteBatch>();
            rainSplash.Draw(spriteBatch);
            rainSystem.Draw(spriteBatch);
            particleSystem.Draw(spriteBatch);
        }

        #region ICOllideable Members

        public BoundingSphere Bounds
        {
            get { return new BoundingSphere(new Vector3(Position.X, 0, Position.Z), 10 * (1 + Scale)); }
        }

        public bool CollidesWith(ICOllideable target)
        {
            bool collision = false;
            if (Bounds != null && target.Bounds != null)
            {
                collision = Bounds.Intersects(target.Bounds);
            }
            //if (collision)
            //    Console.WriteLine("Collision between " + this + " and " + target);
            return collision;
        }

        #endregion
    }
}
