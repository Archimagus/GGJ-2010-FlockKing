using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace FlocKing.Objects
{
    public class MenuWorld : ThreeDRenderable
    {
        public override void Loadcontent()
        {
            Model = Content.Load<Model>("Mesh\\menuLevelV2");
            Color = Color.White;
        }
    }
}
