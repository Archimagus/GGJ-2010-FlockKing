using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using FlocKing.Helpers;

namespace FlocKing.Objects
{
    public abstract class ThreeDRenderable : ICOllideable
    {
        private BoundingSphere _boundingSphere;
        private Vector3 _color;
        private bool _boundingDirty;
         private bool animating = false;
         private float alphaValue = 1.0f;
        private Animations.AnimationPlayer animationPlayer = null;



        public bool Animating { get { return animating; } set { animating = value; } }
        public Animations.AnimationPlayer AnimationPlayer { get { return animationPlayer; } set { animationPlayer = value; } }
        public float AlphaValue { get { return alphaValue; } set { alphaValue = value; } }
        public Vector3 Position { get; set; }
        public Vector3 Heading { get; set; }
        public bool Colliding { get; set; }
        public Model Model { get; set; }
        public ContentManager Content { get; set; }
        public Color Color 
        {
            get { return new Color(_color); }
            set { _color = value.ToVector3(); }
        }

        public abstract void Loadcontent();
        public ThreeDRenderable()
        {
            Content = ServiceProvider.GetService<ContentManager>();
            Color = Color.Green;
            _boundingDirty = true;
            Heading = Vector3.Forward;
        }
        public virtual void Render()
        {
            Matrix[] transforms;

            
            var transform = Matrix.CreateWorld(Position, Heading, Vector3.Up);

            if (animating)  
            {
                transforms = animationPlayer.GetSkinTransforms();
            }
            else
            {
                 transforms = new Matrix[Model.Bones.Count];
                Model.CopyAbsoluteBoneTransformsTo(transforms);
            }
                foreach (ModelMesh mesh in Model.Meshes)
                {
                        if (mesh.Name.EndsWith("inv"))
                            continue;
                    foreach (Effect effect in mesh.Effects)
                    {

                        effect.GraphicsDevice.RenderState.DepthBufferEnable = true;
                        

                        //effect.GraphicsDevice.
                       effect.Parameters["Highlight"].SetValue(new Vector4(_color, 1));
                       effect.Parameters["CameraPos"].SetValue(Camera.Instance.CameraPosition);


                       if (Model.Meshes.Count < 60)
                       {
                           effect.Parameters["Bones"].SetValue(transforms);
                       }
                       //effect.Parameters["Bones"].SetValue(transforms);
                        effect.Parameters["View"].SetValue(Camera.Instance.View);
                        effect.Parameters["Projection"].SetValue(Camera.Instance.Projection);
                        effect.Parameters["World"].SetValue(transforms[mesh.ParentBone.Index] * transform);
                        effect.Parameters["Alpha"].SetValue(AlphaValue);
                        effect.Parameters["Animating"].SetValue(Animating);



                        effect.Parameters["LightPosition"].SetValue(new Vector3(0, 40, 50));
                        effect.Parameters["LightPower"].SetValue(1.0f);
                        effect.Parameters["Ambient"].SetValue(0.2f);

                    }
                    mesh.Draw();
                }
            
        }
        #region ICOllideable Members

        public virtual BoundingSphere Bounds
        {
            get
            {
                if (_boundingDirty)
                {
                    if (Model.Meshes.Count > 0)
                    {
                        _boundingSphere = Model.Meshes[0].BoundingSphere;
                        foreach (ModelMesh mesh in Model.Meshes)
                        {
                            _boundingSphere = BoundingSphere.CreateMerged(mesh.BoundingSphere, _boundingSphere);
                        }
                    }
                    _boundingDirty = false;
                }
                _boundingSphere.Center = Position;
                return _boundingSphere;
            }
        }

        public virtual bool CollidesWith(ICOllideable target)
        {
            if (Bounds != null && target.Bounds != null)
            {
                return Bounds.Intersects(target.Bounds);
            }
            return false;
        }

        #endregion
    }
}
