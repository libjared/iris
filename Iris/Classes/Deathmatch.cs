using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iris
{
    public class Deathmatch
    {
        public ClientMailman Mailman { get; set; }
        public ClientPlayer player;
        public List<Actor> Players { get; set; }
        public List<Projectile> Projectiles { get; set; }
        public List<Sprite> BackgroundImages { get; set; }
        public List<Sprite> BackgroundImagesFar { get; set; }
        private Sprite mapSprite;
        private Sprite Sky;
        private byte[] mapBytes;
        private int mapWidth;
        private int mapHeight;

        public float gravity = 1.3f;

        public Deathmatch()
        {
            Projectiles = new List<Projectile>();
            BackgroundImages = new List<Sprite>();
            BackgroundImagesFar = new List<Sprite>();
            Players = new List<Actor>();
            Mailman = new ClientMailman(this);
            Mailman.Connect();

            Image mapImg = new Image("Content/map.png");
            Sky = new Sprite(Content.GetTexture("sky.png"));
            mapBytes = mapImg.Pixels;
            mapSprite = new Sprite(new Texture(mapImg));
            mapWidth = (int)mapImg.Size.X;
            mapHeight = (int)mapImg.Size.Y;

            player = new ClientPlayer(this);
            player.Pos = new Vector2f(46, 62);
            Players.Add(player);

            
            
        }

        public void Update()
        {
            Mailman.HandleMessages();
            Players.ForEach(p => { p.Update(); });
            Projectiles.ForEach(p => { p.Update(); });


            for (int i = 0; i < 4; i++)
            {
                if (BackgroundImages.Count < i)
                {
                    Sprite s = new Sprite(Content.GetTexture("background1.png"));
                    s.Position = new Vector2f((float)(s.Texture.Size.X * (i - 1)), 0);
                    BackgroundImages.Add(s);
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (BackgroundImagesFar.Count < i)
                {
                    Sprite s = new Sprite(Content.GetTexture("background1Far.png"));
                    s.Position = new Vector2f((float)(s.Texture.Size.X * (i - 1)), 75);
                    BackgroundImagesFar.Add(s);
                }
            }
            for (int i = 0 ; i < BackgroundImages.Count; i++)
            {
                BackgroundImages[i].Position -= new Vector2f(2f, 0);
                if (BackgroundImages[i].Position.X < -BackgroundImages[i].Texture.Size.X)
                    BackgroundImages.RemoveAt(i);

            }
            for (int i = 0; i < BackgroundImagesFar.Count; i++)
            {
                BackgroundImagesFar[i].Position -= new Vector2f(1.5f, 0);
                if (BackgroundImagesFar[i].Position.X < -BackgroundImagesFar[i].Texture.Size.X)
                    BackgroundImagesFar.RemoveAt(i);
            }
        }

        public void Draw()
        {
            MainGame.Camera.Center = player.Pos;
            
            MainGame.window.SetView(MainGame.Camera);
            Sky.Draw(MainGame.window, RenderStates.Default);
            BackgroundImagesFar.ForEach(p => { p.Draw(MainGame.window, RenderStates.Default); });
            BackgroundImages.ForEach(p => { p.Draw(MainGame.window, RenderStates.Default); });
            Players.ForEach(p => { p.Draw(); } );
            Projectiles.ForEach(p => { p.Draw(); });
            
            
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

        public Actor GetPlayerWithUID(long id)
        {
            return Players.FirstOrDefault(p => p.UID == id);
        }

        public void Close()
        {
            Mailman.Disconnect();
        }
    }
}
