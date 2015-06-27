using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Linq;

namespace Iris
{
    public class Deathmatch
    {
        public ClientMailman Mailman { get; set; }
        public List<Player> Players { get; set; }
        public ClientPlayer ClientPlayer { get; set; }
        private Sprite mapSprite;
        private byte[] mapBytes;
        private int mapWidth;
        private int mapHeight;

        public Deathmatch()
        {
            Players = new List<Player>();
            Mailman = new ClientMailman(this);
            Mailman.Connect();

            Image mapImg = new Image("Content/map.png");
            mapBytes = mapImg.Pixels;
            mapSprite = new Sprite(new Texture(mapImg));
            mapWidth = (int)mapImg.Size.X;
            mapHeight = (int)mapImg.Size.Y;

            ClientPlayer plr = new ClientPlayer(this);
            plr.Position = new Vector2f(46, 62);
            Players.Add(plr);
        }

        public void Update()
        {
            Mailman.HandleMessages();
            Players.ForEach(p => { p.Update(); });
        }

        public void Draw()
        {
            Players.ForEach(p => { p.Draw(); } );
            MainGame.window.Draw(mapSprite);
        }

        public bool MapCollide(int x, int y)
        {
            //check if OOB
            if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
            {
                return false;
            }
            int index = (y * mapWidth + x) * 4;
            byte r = mapBytes[index];
            byte g = mapBytes[index+1];
            byte b = mapBytes[index+2];
            byte a = mapBytes[index+3];

            //only collide if black
            return r == 0 && g == 0 && b == 0 && a == 255;
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
