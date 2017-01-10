using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FlocKing.Objects
{
    public class PlayerSettings
    {
        public PlayerIndex ControllerID { get; set; }
        public Color Color { get; set; }
        public string ModelName { get; set; }
    }
}
