using System.Collections.Generic;
using System.Linq;

namespace Iris
{
    public class Deathmatch
    {
        public static ClientMailman Mailman { get; set; }
        public List<Player> Players { get; set; }

        public Deathmatch()
        {
            Players = new List<Player>();
            Mailman = new ClientMailman(this);
            Mailman.Connect();
            Players.Add(new ClientPlayer());
        }

        public void Update()
        {
            Mailman.HandleMessages();
            Players.ForEach(p => { p.Update(); });
        }

        public void Draw()
        {
            Players.ForEach(p => { p.Draw(); } );
        }

        public Player GetPlayerWithUID(long id)
        {
            return Players.FirstOrDefault(p => p.UID == id);
        }

        public void Close()
        {
            Mailman.Disconnect();
        }
    }
}
