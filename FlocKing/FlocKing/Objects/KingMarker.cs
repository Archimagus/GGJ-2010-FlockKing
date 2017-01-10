using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using FlocKing.Animations;
using Microsoft.Xna.Framework;

namespace FlocKing.Objects
{
    public class KingMarker : ThreeDRenderable
    {
        Boid king;
        float timeCounter = 0.0f;
        float lifeTime = 3.0f;
        bool revealed = false;

        public KingMarker(Boid king)
        {
            this.king = king;
            Position = king.Position;
            Loadcontent();
        }

        public void ShowMarker(Boid king)
        {
            if (!revealed)
            {
                this.king = king;
                timeCounter = 0.0f;
            }
        }

        public void Update(GameTime time)
        {
            if (!revealed)
                timeCounter += (float)time.ElapsedGameTime.TotalSeconds;
          
           Position = new Vector3(king.Position.X, king.Position.Y +17 , king.Position.Z);
            //AnimationPlayer.Update(time.ElapsedGameTime, true, Matrix.Identity);
        }

        public override void Render()
        {
            Color = king.Color;
            if (timeCounter < lifeTime)
            {
                AlphaValue = 1.0f;// timeCounter / lifeTime;
                base.Render();
            }
        }

        public override void Loadcontent()
        {
            Model = Content.Load<Model>("Mesh/crown");

            Animating = false;
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

        internal void KingIsRevealed(King king)
        {
            ShowMarker(king);
            revealed = true;
        }
    }
}
