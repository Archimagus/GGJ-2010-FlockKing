using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FlocKing
{
    public class Camera
    {
        static readonly Camera instance = new Camera();

        public static Camera Instance { get { return instance; } }
        Viewport viewport;
        float aspectRatio;
        Matrix view;
        Matrix projection;
        float Dist = 500;
        float Rot;
        float CameraRotateSpeed = 1.0f;
        float CameraZoomSpeed = 5.0f;
        Vector3 cameraPosition;
        Vector3 lookAt;
        public Viewport Viewport
        {
            set { viewport = value; }
            get { return viewport; }

        }
        public float AspectRatio
        {
            set { aspectRatio = value; }
            get { return aspectRatio; }

        }
        public Matrix View
        {
            set { view = value; }
            get { return view; }

        }
        public Matrix Projection
        {
            set { projection = value; }
            get { return projection; }

        }

        public Vector3 CameraPosition
        {
            set { cameraPosition = value; }
            get { return cameraPosition; }
        }

        public Vector3 CameraLookAt
        {
            set { lookAt = value; }
            get { return lookAt; }
        }
        public void Init(GraphicsDevice device)
        {
            viewport = device.Viewport;

            aspectRatio = (float)viewport.Width / (float)viewport.Height;


             cameraPosition = new Vector3(0, Dist, 100);
              lookAt = new Vector3(0, 0, 0);
            // view = Matrix.CreateLookAt(cameraPosition,
            //                                  lookAt,
            //                                  Vector3.Cross(Vector3.UnitX, lookAt - cameraPosition));

            //projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
            //                                                       aspectRatio,
            //                                                       10,
            //                                                       10000);
        }
        public void Update(bool createLookAt)
        {
            
            if(createLookAt)
                view = Matrix.CreateLookAt(cameraPosition,
                                         lookAt,
                                         Vector3.Cross(Vector3.UnitX, lookAt - cameraPosition));

            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                   aspectRatio,
                                                                   10,
                                                                   10000);
        }
        public void Input()
        {
            var ks = Keyboard.GetState();

          //  float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //Check for input to rotate the camera around the model.
          

            //Check for input to zoom camera in and out.
            if (ks.IsKeyDown(Keys.Z))
                Dist +=  CameraZoomSpeed;

            if (ks.IsKeyDown(Keys.X))
                Dist -=  CameraZoomSpeed;

            cameraPosition.Y = Dist;
        }
    }
}