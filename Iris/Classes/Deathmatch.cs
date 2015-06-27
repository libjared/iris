using System;
using System.Linq;
using System.Collections.Generic;

namespace Iris
{
    public class Deathmatch
    {
        public List<Player> Players { get; set; }

        public Deathmatch()
        {
            Players = new List<Player>();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void Draw()
        {
            throw new NotImplementedException();
        }

        public Player GetPlayerWithUID(long id)
        {
            return Players.FirstOrDefault(p => p.UID == id);
        }
    }
}
