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
        public List<Sprite> BackgroundTracks { get; set; }
        private Sprite mapSprite;
        private Sprite Sky;
        private byte[] mapBytes;
        private int mapWidth;
        private int mapHeight;

        public float gravity = 1.1f;

        public Deathmatch()
        {
            Projectiles = new List<Projectile>();
            BackgroundImages = new List<Sprite>();
            BackgroundImagesFar = new List<Sprite>();
            BackgroundTracks = new List<Sprite>();
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
            
        }

        public void Draw()
        {
            //MainGame.Camera.Center = player.Pos;
            Vector2f focus = player.Core +
                new Vector2f(Input.screenMousePos.X - MainGame.window.Size.X / 2,
                Input.screenMousePos.Y - MainGame.window.Size.Y /2) / 2;
            if (Helper.Distance(player.Core, focus) > 100)
                focus = player.Core + Helper.PolarToVector2(100, player.AimAngle, 1, 1);//player.Core + Helper.normalize(focus) * 100;
            Helper.MoveCameraTo(MainGame.Camera, focus, .04f);

            //Camera2D.returnCamera(player.ActorCenter +
            //            new Vector2(Main.screenMousePos.X - Main.graphics.PreferredBackBufferWidth / 2,
            //                Main.screenMousePos.Y - Main.graphics.PreferredBackBufferHeight / 2) *
            //                Main.graphics.GraphicsDevice.Viewport.AspectRatio / player.currentWeapon.viewDistance);

            MainGame.window.SetView(MainGame.Camera);
            Sky.Draw(MainGame.window, RenderStates.Default);
            HandleBackground();
            BackgroundImagesFar.ForEach(p => { p.Draw(MainGame.window, RenderStates.Default); });
            BackgroundImages.ForEach(p => { p.Draw(MainGame.window, RenderStates.Default); });
            BackgroundTracks.ForEach(p => { p.Draw(MainGame.window, RenderStates.Default); });
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

        public void HandleBackground()
        {
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
                    s.Position = new Vector2f((float)(s.Texture.Size.X * (i - 1)), 150);
                    BackgroundImagesFar.Add(s);
                }
            }
            for (int i = 0; i < 25; i++)
            {
                if (BackgroundTracks.Count < i)
                {
                    Sprite s = new Sprite(Content.GetTexture("tracksBlur.png"));
                    s.Position = new Vector2f((float)((s.Texture.Size.X - 1) * (i - 1)), 515);
                    BackgroundTracks.Add(s);
                }
            }
            for (int i = 0; i < BackgroundImages.Count; i++) //Main Background
            {
                BackgroundImages[i].Position -= new Vector2f(2f, 0);
                if (BackgroundImages[i].Position.X < -BackgroundImages[i].Texture.Size.X)
                    BackgroundImages.RemoveAt(i);

            }
            for (int i = 0; i < BackgroundImagesFar.Count; i++) //Far Background
            {
                BackgroundImagesFar[i].Position -= new Vector2f(.8f, 0);
                if (BackgroundImagesFar[i].Position.X < -BackgroundImagesFar[i].Texture.Size.X)
                    BackgroundImagesFar.RemoveAt(i);
            }
            for (int i = 0; i < BackgroundTracks.Count; i++) //Tracks
            {
                BackgroundTracks[i].Position -= new Vector2f(8, 0);
                if (BackgroundTracks[i].Position.X < -BackgroundTracks[i].Texture.Size.X)
                    BackgroundTracks.RemoveAt(i);
            }

        }
    }
}
