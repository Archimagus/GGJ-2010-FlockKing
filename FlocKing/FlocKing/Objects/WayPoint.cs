using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FlocKing.Animations;

namespace FlocKing.Objects
{
    public class WayPoint : ThreeDRenderable
    {
        public WayPoint(Vector3 position, Color color)
        {
            Position = position;
            Color = color;
            Loadcontent();
        }

        public override void Loadcontent()
        {
            Model = Content.Load<Model>("Mesh/wayPoint");//"Mesh/cursor1");//
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
