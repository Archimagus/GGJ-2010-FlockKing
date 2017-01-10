using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FlocKing.Objects;

namespace FlocKing.Managers
{
    public class CollisionManager
    {
        public CollisionManager()
        {
        }

        public void update(GameTime time)
        {
            for (int i = 0; i < Game1.Instance.Players.Count; i++)
            {
                Player player = Game1.Instance.Players[i];
                for (int j = 1; j < Game1.Instance.Players.Count; j++)
                {
                    Player otherPlayer = Game1.Instance.Players[j];
                    if (player != otherPlayer)
                        player.FlocksCollidesWith(otherPlayer);
                }

                if (GameplayState.goCloudGo && player.king.CollidesWith(GameplayState._cloud))
                {                    
                    player.king.Revealed = true;
                }

            }

            // clean the dead boids
            foreach (Player player in Game1.Instance.Players)
            {
                player.CleanDeadBoids();
            }

        }
    }
}
