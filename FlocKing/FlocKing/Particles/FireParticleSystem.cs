using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;

namespace FlocKing.Particles
{
    class FlameThrowerParticleSystem : ParticleSystem
    {

        public FlameThrowerParticleSystem(int howManyEffects, Texture2D texture, string XmlFile)
            : base(howManyEffects, texture, XmlFile)
        {
            Initialize();
        }


        protected override void InitializeConstants()
        {
            XmlTextReader reader = new XmlTextReader(m_xmlFile);

            
            while (reader.Read())
            {
                if (reader.Name == "MaxInitialSpeed")
                    m_maxInitialSpeed = reader.ReadElementContentAsFloat();
                if (reader.Name == "MinInitialSpeed")
                    m_minInitialSpeed = reader.ReadElementContentAsFloat();

                if (reader.Name == "MinAcceleration")
                    m_minAcceleration = reader.ReadElementContentAsFloat();
                if (reader.Name == "MaxAcceleration")
                    m_maxAcceleration = reader.ReadElementContentAsFloat();
            }
            //m_minInitialSpeed = (float)Convert.ToDouble(reader["FileParticleSystem.MinInitialSpeed"]);//= 70;
            //m_maxInitialSpeed = 115;

            //m_minAcceleration = 0;
            //m_maxAcceleration = 0;

            //m_minLifetime = 1f;
            //m_maxLifetime = 5f;

            //m_minScale = 0.4f;
            //m_maxScale = 0.7f;

            //m_minNumParticles = 30;
            //m_maxNumParticles = 50;

            //m_minRotationSpeed = -3.0f;
            //m_maxRotationSpeed = 3.0f;

            //m_minInitialAngle = 0.0f;
            //m_maxInitialAngle = 0.3f;

            m_spriteBlendMode = Microsoft.Xna.Framework.Graphics.SpriteBlendMode.Additive;
        }
    }
}
