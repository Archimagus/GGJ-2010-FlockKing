using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using FlocKing;
using FlocKing.Helpers;

namespace FlocKing.Particles
{

    public class ParticleSystem
    {
        Texture2D m_texture;

        Vector2 m_origin;

        int m_numberEffects;

        Particle[] m_particles;

        VertexPositionColor[] SpriteArray;

        Queue<Particle> m_freeParticles;

        Effect PointSpritesEffect;

        private float m_minInitialSpeed;
        public float MinInitialSpeed
        {
            get { return m_minInitialSpeed; }
            set { m_minInitialSpeed = value; }
        }

        private float m_maxInitialSpeed;
        public float MaxInitialSpeed
        {
            get { return m_maxInitialSpeed; }
            set { m_maxInitialSpeed = value; }
        }

        private float m_minAcceleration;
        public float MinAcceleration
        {
            get { return m_minAcceleration; }
            set { m_minAcceleration = value; }
        }
        
        private float m_maxAcceleration;
        public float MaxAcceleration
        {
            get { return m_maxAcceleration; }
            set { m_maxAcceleration = value; }
        }
        
        private float m_minLifetime;
        public float MinLifetime
        {
            get { return m_minLifetime; }
            set { m_minLifetime = value; }
        }

        private float m_maxLifetime;
        public float MaxLifetime
        {
            get { return m_maxLifetime; }
            set { m_maxLifetime = value; }
        }

        private float m_minScale;
        public float MinScale
        {
            get { return m_minScale; }
            set { m_minScale = value; }
        }

        private float m_maxScale;
        public float MaxScale
        {
            get { return m_maxScale; }
            set { m_maxScale = value; }
        }

        private int m_minNumParticles;
        public int MinNumParticles
        {
            get { return m_minNumParticles; }
            set { m_minNumParticles = value; }
        }

        private int m_maxNumParticles;
        public int MaxNumParticles
        {
            get { return m_maxNumParticles; }
            set { m_maxNumParticles = value; }
        }

        private float m_minRotationSpeed;
        public float MinRotationSpeed
        {
            get { return m_minRotationSpeed; }
            set { m_minRotationSpeed = value; }
        }

        private float m_maxRotationSpeed;
        public float MaxRotationSpeed
        {
            get { return m_maxRotationSpeed; }
            set { m_maxRotationSpeed = value; }
        }

        private Vector3 m_minInitialAngle;
        public Vector3 MinInitialAngle
        {
            get { return m_minInitialAngle; }
            set { m_minInitialAngle = value; }
        }
        private Vector3 m_maxInitialAngle;
        public Vector3 MaxInitialAngle
        {
            get { return m_maxInitialAngle; }
            set { m_maxInitialAngle = value; }
        }

        private float scaleModifer;
        public float ScaleModifer
        {
            get { return scaleModifer; }
            set { scaleModifer = value; }
        }

        Color m_color;
        public Color Color
        {
            get { return m_color; }
            set { m_color = value; }
        }

        private Blend m_sourceBlend;
        protected Blend SourceBlend
        {
            get { return m_sourceBlend; }
            set { m_sourceBlend = value; }
        }

        private Blend m_destBlend;
        protected Blend DestBlend
        {
            get { return m_destBlend; }
            set { m_destBlend = value; }
        }

        public float AlphaModifier { get; set; }

        private Vector3 m_velocity;
        private Vector3 m_acceleration;
        protected string m_xmlFile;
        protected SpriteBlendMode m_spriteBlendMode;


        /// <summary>
        /// Gets the average velocity
        /// </summary>
        private Vector2 Velocity
        {
            get
            {
                float avgSpeed = (m_minInitialSpeed + m_maxInitialSpeed) / 2;
                return new Vector2(Direction.X * avgSpeed, Direction.Y * avgSpeed);

            }
        }

        public List<Vector3> ParticlePositions()
        {
            List<Vector3> positions = new List<Vector3>();
            foreach (Particle p in m_particles)
            {
                positions.Add(p.Position);
            }

            return positions;
        }

        public List<Particle> Particles()
        {
            return m_particles.ToList();
        }

        public void AdjustScale(float amount)
        {
            scaleModifer = amount;
        }

        private Vector3 m_direction;


        /// <summary>
        /// Gets direction of particles
        /// </summary>
        public Vector3 Direction
        {
            get { return m_direction; }
            set 
            {
                m_direction = value;                
            }
        }

        public Vector3 Acceleration
        {
            get { return m_acceleration; }
        }

        //public float SetRotation
        //{
        //    set
        //    {
        //        float temp = m_maxInitialAngle - m_minInitialAngle;

        //        m_maxInitialAngle = value - temp / 2;
        //        m_minInitialAngle = value + temp / 2;
        //    }
        //}

        public ParticleSystem(int howManyEffects, Texture2D texture, string XmlFileName)
        {
            m_numberEffects = howManyEffects;

            m_texture = texture;

            m_origin.X = m_texture.Width / 2;
            m_origin.Y = m_texture.Height / 2;
            var content = ServiceProvider.GetService<ContentManager>();
            PointSpritesEffect = content.Load<Effect>("Particles\\Particles");
            

            m_xmlFile = "..\\..\\..\\Content\\Particles\\" + XmlFileName;

            m_spriteBlendMode = SpriteBlendMode.Additive;

            AlphaModifier = 1.0f;
            scaleModifer = 1.0f;

            InitializeConstants();
            Initialize();

        }

        public void Initialize()
        {

            m_particles = new Particle[m_numberEffects * m_maxNumParticles];
            SpriteArray = new VertexPositionColor[m_numberEffects * m_maxNumParticles];
            m_freeParticles = new Queue<Particle>(m_numberEffects * m_maxNumParticles);
            for (int i = 0; i < m_particles.Length; i++)
            {
                m_particles[i] = new Particle();
                m_freeParticles.Enqueue(m_particles[i]);
                SpriteArray[i].Position = m_particles[i].Position;
                SpriteArray[i].Color = m_color;
            }

            Random r = new Random();
            m_direction = Vector3.Lerp(m_minInitialAngle, m_maxInitialAngle, (float)r.NextDouble());//new Vector3((float)Math.Cos(angle), 0, (float)Math.Sin(angle));
            //            _direction += new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0);
        }
        protected void InitializeConstants()
        {
            try
            {
                XmlTextReader reader = new XmlTextReader(m_xmlFile);

                int r = 0, g = 0, b = 0;

                while (reader.Read())
                {
                    if (reader.Name == "MaxInitialSpeed")
                        m_maxInitialSpeed = reader.ReadElementContentAsFloat();
                    else if (reader.Name == "MinInitialSpeed")
                        m_minInitialSpeed = reader.ReadElementContentAsFloat();

                    else if (reader.Name == "MinNumParticles")
                        m_minNumParticles = reader.ReadElementContentAsInt();
                    else if (reader.Name == "MaxNumParticles")
                        m_maxNumParticles = reader.ReadElementContentAsInt();

                    else if (reader.Name == "MinAcceleration")
                        m_minAcceleration = reader.ReadElementContentAsFloat();
                    else if (reader.Name == "MaxAcceleration")
                        m_maxAcceleration = reader.ReadElementContentAsFloat();

                    else if (reader.Name == "MinLifetime")
                        m_minLifetime = reader.ReadElementContentAsFloat();
                    else if (reader.Name == "MaxLifetime")
                        m_maxLifetime = reader.ReadElementContentAsFloat();

                    else if (reader.Name == "MinScale")
                        m_minScale = reader.ReadElementContentAsFloat();
                    else if (reader.Name == "MaxScale")
                        m_maxScale = reader.ReadElementContentAsFloat();


                    else if (reader.Name == "MinNumParticles")
                        m_minScale = reader.ReadElementContentAsFloat();
                    else if (reader.Name == "MaxNumParticles")
                        m_maxNumParticles = reader.ReadElementContentAsInt();

                    else if (reader.Name == "MinRotationSpeed")
                        m_minRotationSpeed = reader.ReadElementContentAsFloat();
                    else if (reader.Name == "MaxRotationSpeed")
                        m_maxRotationSpeed = reader.ReadElementContentAsFloat();

                    else if (reader.Name == "MinInitialAngleX")
                        m_minInitialAngle.X = reader.ReadElementContentAsFloat();
                    else if (reader.Name == "MaxInitialAngleX")
                        m_maxInitialAngle.X = reader.ReadElementContentAsFloat();

                    else if (reader.Name == "MinInitialAngleY")
                        m_minInitialAngle.Y = reader.ReadElementContentAsFloat();
                    else if (reader.Name == "MaxInitialAngleY")
                        m_maxInitialAngle.Y = reader.ReadElementContentAsFloat();

                    else if (reader.Name == "MinInitialAngleZ")
                        m_minInitialAngle.Z = reader.ReadElementContentAsFloat();
                    else if (reader.Name == "MaxInitialAngleZ")
                        m_maxInitialAngle.Z = reader.ReadElementContentAsFloat();

                    else if (reader.Name == "SourceBlendMode")
                        m_sourceBlend = (Blend)reader.ReadElementContentAsInt();
                    else if (reader.Name == "DestBlendMode")
                        m_destBlend = (Blend)reader.ReadElementContentAsInt();


                    else if (reader.Name == "ColorR")
                        r = reader.ReadElementContentAsInt();
                    else if (reader.Name == "ColorG")
                        g = reader.ReadElementContentAsInt();
                    else if (reader.Name == "ColorB")
                        b = reader.ReadElementContentAsInt();
                    


                }

                m_color = new Color(r, g, b);

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public bool HasFired()
        {
            return m_freeParticles.Count != m_numberEffects * m_maxNumParticles;
        }

        public void AddParticles(Vector3 addAt)
        {
            Random random = new Random();
            int numParticles = random.Next(m_minNumParticles, m_maxNumParticles);

            //m_numParticles = numParticles;

            // create that many particles, if you can.
            for (int i = 0; i < numParticles && m_freeParticles.Count > 0; i++)
            {
                // grab a particle from the freeParticles queue, and Initialize it.
                Particle p = m_freeParticles.Dequeue();
                InitializeParticle(p, addAt);
            }
        }

        protected virtual void InitializeParticle(Particle p, Vector3 addAt)
        {
            Vector3 direction;// Vector3.Lerp(m_minInitialAngle, m_maxInitialAngle, (float)FlocKing.Game1.Random.NextDouble());
            direction.X = FlocKing.Game1.RandomFloatBetween(m_minInitialAngle.X, m_maxInitialAngle.X);
            direction.Y = FlocKing.Game1.RandomFloatBetween(m_minInitialAngle.Y, m_maxInitialAngle.Y);
            direction.Z = FlocKing.Game1.RandomFloatBetween(m_minInitialAngle.Z, m_maxInitialAngle.Z);
            
            //direction += new Vector3((float)Math.Cos(angle), (float)Math.Sign(angle), 0);

            direction.Normalize();

            float velocity = RandomFloatBetween(m_minInitialSpeed, m_maxInitialSpeed);
            float acceleration = RandomFloatBetween(m_minAcceleration, m_maxAcceleration);
            float lifeTime = RandomFloatBetween(m_minLifetime, m_maxLifetime);
            float scale = RandomFloatBetween(m_minScale + scaleModifer, m_maxScale + scaleModifer);
            float rotationSpeed = RandomFloatBetween(m_minRotationSpeed, m_maxRotationSpeed);

            m_velocity = (velocity * direction);
            //m_velocity.Y = 0;
            m_acceleration = (acceleration * direction);
            addAt += m_velocity * FlocKing.Game1.RandomFloatBetween(0, m_maxLifetime);
            p.Initialize(addAt, m_velocity, m_acceleration, lifeTime, scale, rotationSpeed);

        }

        public void Update(GameTime gameTime)
        {

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;


            // go through all of the particles...
            for(int i = 0;i < m_particles.Length; i++)
            {
                Particle p = m_particles[i];
                if (p.Active)
                {
                    // ... and if they're active, update them.
                    p.Update(dt);
                    // if that update finishes them, put them onto the free particles
                    // queue.
                    if (!p.Active)
                    {
                        m_freeParticles.Enqueue(p);
                    }
                    SpriteArray[i].Position = p.Position;
                }
            }
        }


        public void Draw(SpriteBatch spritebatch)
        {
            //spritebatch.Begin(m_spriteBlendMode);

            //foreach (Particle p in m_particles)
            //{
            //    if (!p.Active) //if not active don't draw
            //        continue;

            //    float normalizedLifetime = p.TimeSinceStart / p.TotalLife;

            //    float alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);
            //    Color color = new Color(new Vector4(0.5f, 1, 1, alpha / 2));

            //    // make particles grow as they age. they'll start at 75% of their size,
            //    // and increase to 100% once they're finished.
            //    float scale = p.Scale * (.75f + .25f * normalizedLifetime);


            //    spritebatch.Draw(m_texture, p.Position, null, color,
            //        p.Rotation, m_origin, scale, SpriteEffects.None, 0.0f);

            //}

            //spritebatch.End();

            GraphicsDevice device = ServiceProvider.GetService<GraphicsDeviceManager>().GraphicsDevice;


            device.RenderState.PointSpriteEnable = true;
            
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SourceBlend = m_sourceBlend;
            device.RenderState.DestinationBlend = m_destBlend;
            device.RenderState.DepthBufferWriteEnable = false;

            

            device.VertexDeclaration
                = new VertexDeclaration(device, VertexPositionColor.VertexElements);

            Matrix WVPMatrix = Matrix.Identity * Camera.Instance.View * Camera.Instance.Projection;
            PointSpritesEffect.Parameters["WVPMatrix"].SetValue(WVPMatrix);
            PointSpritesEffect.Parameters["SpriteTexture"].SetValue(m_texture);
            

            PointSpritesEffect.Begin();
            for (int i = 0; i < m_particles.Length; i++)
            {
                Particle p = m_particles[i];

                if (!p.Active) //if not active don't draw
                    continue;

                float normalizedLifetime = p.TimeSinceStart / p.TotalLife;

                device.RenderState.PointSize = p.Scale * scaleModifer *  (.75f + .25f * normalizedLifetime);

                float alpha = 4 * normalizedLifetime * (1 - normalizedLifetime) * AlphaModifier;
                Color color = new Color(new Vector4(m_color.R, m_color.G, m_color.B, alpha / 4));

                PointSpritesEffect.Parameters["Color"].SetValue(color.ToVector4());

                foreach (EffectPass pass in PointSpritesEffect.CurrentTechnique.Passes)
                {
                    pass.Begin();
                    device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.PointList,
                        SpriteArray, i, 1);//SpriteArray.Length);
                    pass.End();
                }
            }
            PointSpritesEffect.End();

            device.RenderState.PointSpriteEnable = false;
            device.RenderState.DepthBufferWriteEnable = true;
            device.RenderState.SourceBlend = Blend.SourceAlpha;
            device.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

        }

        public float RandomFloatBetween(float min, float max)
        {
            return min + (float)Game1.Instance.Random.NextDouble() * (max - min);
        }
    }

}
