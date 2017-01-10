using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;



namespace FlocKing
{
    public interface IBaseState
    {
         void Enter();
         bool Input();
         void Update(GameTime _Delta);
         void Render();
         void Exit();
    }
}
