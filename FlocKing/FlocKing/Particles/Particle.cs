using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlocKing.Particles
{
    public class Particle
    {
        Vector3 m_position;
        Vector3 m_velocity;
        Vector3 m_acceleration;

        float m_life;

        float m_timeSinceStart;

        float m_scale;

        float m_rotation;

        float m_rotationSpeed;

        //We don't use a constructor because we recycle particles
        //Instead use this function to "reallocate" the partcle
        public void Initialize(Vector3 position, Vector3 velocity, Vector3 acceleration, float life, float scale, float rotationSpeed)
        {
            m_position = position;
            m_velocity = velocity;
            m_acceleration = acceleration;
            m_life = life;
            m_scale = scale;
            m_rotationSpeed = rotationSpeed;

            m_timeSinceStart = 0.0f;
        }

        public Vector3 Position
        {
            get { return m_position; }
            set { m_position = value; }
        }

        public Vector3 Velocity
        {
            get { return m_velocity; }
            set { m_velocity = value; }
        }

        Vector3 Acceleration
        {
            get { return m_acceleration; }
            set { m_acceleration = value; }
        }

        public float TotalLife
        {
            get { return m_life; }
        }

        public float TimeSinceStart
        {
            get { return m_timeSinceStart; }
        }

        public float Scale
        {
            get { return m_scale; }
            set { m_scale = value; }
        }

        public float Rotation
        {
            get { return m_rotation; }
            set { m_rotation = value; }
        }

        float RotationSpeed
        {
            get { return m_rotationSpeed; }
            set { m_rotationSpeed = value; }
        }

        public bool Active
        {
            get { return TimeSinceStart < TotalLife; }
        }



        public void Update(float dt)
        {
            Position += Acceleration * dt;
            Position += Velocity * dt;

            Rotation += RotationSpeed * dt;

            m_timeSinceStart += dt;
        }

    }
}
