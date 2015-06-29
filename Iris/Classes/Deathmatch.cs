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
        public List<GameObject> GameObjects = new List<GameObject>();
        private Sprite mapSprite;
        private Sprite Sky;
        private byte[] mapBytes;
        private int mapWidth;
        private int mapHeight;

        public static Color hardCol = new Color(0, 0, 0, 255);
        public static Color softCol = new Color(0, 255, 0, 255);
        public static Color intCol = new Color(255, 0, 0, 0);
        public static Color anyCol = new Color(1, 1, 1, 0);

        public static float MAPYOFFSET = 297f;
        public static float MAPXOFFSET = 1300f;

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

            Image mapImg = new Image("Content/mapCol.png");
            Sky = new Sprite(Content.GetTexture("sky.png"));
            mapBytes = mapImg.Pixels;
            mapSprite = new Sprite(new Texture(mapImg));
            mapWidth = (int)mapImg.Size.X;
            mapHeight = (int)mapImg.Size.Y;

            Sky.Position = new Vector2f(0, -200);

            player = new ClientPlayer(this);
            player.Pos = new Vector2f(46, 62);
            Players.Add(player);

            MainGame.Camera.Center = player.Pos - new Vector2f(0, 90);

        }

        public void Update()
        {
            Mailman.HandleMessages();
            Players.ForEach(p => { p.Update(); });
            Projectiles.ForEach(p => { p.Update(); });
            GameObjects.ForEach(p => { p.Update(); });
            CheckProjectileCollisions();

            if (Input.isKeyTap(Keyboard.Key.Space) && !player.Alive)
            {
                player = new ClientPlayer(this);
                Players.Add(player);
                player.Pos = new Vector2f(46, 62);
                Mailman.SendRespawn(player.UID);
            }

            if (Input.isKeyDown(Keyboard.Key.R))
                MainGame.Camera.Center += new Vector2f(MainGame.rand.Next(-4, 5), MainGame.rand.Next(-4, 5));

        }

        public void Draw()
        {
            //MainGame.Camera.Center = player.Pos - new Vector2f(0,90);

            Vector2f focus = player.Core +
                new Vector2f(Input.screenMousePos.X - MainGame.window.Size.X / 2,
                Input.screenMousePos.Y - MainGame.window.Size.Y / 2) / 2;

            if (Helper.Distance(player.Core, focus) > 100)
                focus = player.Core + Helper.PolarToVector2(100, player.AimAngle, 1, 1);//player.Core + Helper.normalize(focus) * 100;

            focus.Y = player.Pos.Y - 90;
            Helper.MoveCameraTo(MainGame.Camera, focus, .04f);
            if (MainGame.Camera.Center.Y > 180 - 90)
                MainGame.Camera.Center = new Vector2f(MainGame.Camera.Center.X, 180 - 90);

            //Camera2D.returnCamera(player.ActorCenter +
            //            new Vector2(Main.screenMousePos.X - Main.graphics.PreferredBackBufferWidth / 2,
            //                Main.screenMousePos.Y - Main.graphics.PreferredBackBufferHeight / 2) *
            //                Main.graphics.GraphicsDevice.Viewport.AspectRatio / player.currentWeapon.viewDistance);

            MainGame.window.SetView(MainGame.Camera);
            //Console.WriteLine(player.Pos);
            Texture t = Content.GetTexture("sky.png");

            Render.Draw(t, new Vector2f(-t.Size.X, -MAPYOFFSET), Color.White, new Vector2f(0, 0), 1, 0, 1);
            Render.Draw(t, new Vector2f(0, -MAPYOFFSET), Color.White, new Vector2f(0, 0), 1, 0, 1);
            Render.Draw(t, new Vector2f(t.Size.X, -MAPYOFFSET), Color.White, new Vector2f(0, 0), 1, 0, 1);

            HandleBackground();
            BackgroundImagesFar.ForEach(p => { p.Draw(MainGame.window, RenderStates.Default); });
            BackgroundImages.ForEach(p => { p.Draw(MainGame.window, RenderStates.Default); });
            BackgroundTracks.ForEach(p => { p.Draw(MainGame.window, RenderStates.Default); });


            MainGame.window.Draw(new Sprite(Content.GetTexture("mapDecor.png")));
            if (MapCollide((int)player.Pos.X, (int)player.Pos.Y, intCol))
                MainGame.window.Draw(new Sprite(Content.GetTexture("mapInterior.png")));

            Players.ForEach(p => { p.Draw(); });
            Projectiles.ForEach(p => { p.Draw(); });
            GameObjects.ForEach(p => { p.Draw(); });

            if (!MapCollide((int)player.Pos.X, (int)player.Pos.Y, intCol))
                MainGame.window.Draw(new Sprite(Content.GetTexture("mapDecor.png")));
            //MainGame.window.Draw(mapSprite);

        }

        public bool MapCollide(int x, int y, Color c)
        {
            //check if OOB
            if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
            {
                return false;
            }
            int index = (y * mapWidth + x) * 4;
            byte r = mapBytes[index];
            byte g = mapBytes[index + 1];
            byte b = mapBytes[index + 2];
            byte a = mapBytes[index + 3];
            Color result = new Color(r, g, b, a);
            //only collide if black
            if (c.A == 0 && result.A != 0)
                return true;
            return result.Equals(c);
        }

        public Actor GetPlayerWithUID(long id)
        {
            return Players.FirstOrDefault(p => p.UID == id);
        }

        public void CheckProjectileCollisions()
        {
            for (int i = 0; i < Projectiles.Count; i++)
            {

                if (Helper.Distance(Projectiles[i].Pos, player.Core) < 20)
                {
                    //player.OnProjectileHit(Projectiles[i]);
                    //Projectiles[i].onPlayerHit(player);
                }

                for (int a = 0; a < Players.Count; a++)
                {
                    if (Helper.Distance(Projectiles[i].Pos, Players[a].Core) < 20)
                    {
                        if (Projectiles[i].OwnerUID != Players[a].UID)
                        {
                            //Projectiles[i].onPlayerHit(Players[a]);
                            //Players[a].OnProjectileHit(Projectiles[i]);
                        }
                    }
                }
            }
        }

        public void Close()
        {
            Mailman.Disconnect();
        }

        public void HandleBackground()
        {
            for (int i = 0; i < 8; i++)
            {
                if (BackgroundImages.Count < i)
                {
                    Sprite s = new Sprite(Content.GetTexture("background1.png"));
                    s.Position = new Vector2f((float)(s.Texture.Size.X * (i - 1)) - MAPXOFFSET, 225 - MAPYOFFSET);
                    BackgroundImages.Add(s);
                }
            }
            for (int i = 0; i < 16; i++)
            {
                if (BackgroundImagesFar.Count < i)
                {
                    Sprite s = new Sprite(Content.GetTexture("background1Far.png"));
                    s.Position = new Vector2f((float)(s.Texture.Size.X * (i - 1)) - MAPXOFFSET, 300 - MAPYOFFSET);
                    BackgroundImagesFar.Add(s);
                }
            }
            for (int i = 0; i < 25; i++)
            {
                if (BackgroundTracks.Count < i)
                {
                    Sprite s = new Sprite(Content.GetTexture("tracksBlur.png"));
                    s.Position = new Vector2f((float)((s.Texture.Size.X) * (i - 1)) - MAPXOFFSET, 515 - MAPYOFFSET);
                    BackgroundTracks.Add(s);
                }
            }

            for (int i = 0; i < BackgroundImages.Count; i++) //Main Background
            {
                BackgroundImages[i].Position -= new Vector2f(1, 0);
                if (BackgroundImages[i].Position.X < -BackgroundImages[i].Texture.Size.X - MAPXOFFSET)
                    BackgroundImages.RemoveAt(i);

            }
            for (int i = 0; i < BackgroundImagesFar.Count; i++) //Far Background
            {
                BackgroundImagesFar[i].Position -= new Vector2f(.4f, 0);
                if (BackgroundImagesFar[i].Position.X < -BackgroundImagesFar[i].Texture.Size.X - MAPXOFFSET)
                    BackgroundImagesFar.RemoveAt(i);
            }
            for (int i = 0; i < BackgroundTracks.Count; i++) //Tracks
            {
                BackgroundTracks[i].Position -= new Vector2f(10, 0);
                if (BackgroundTracks[i].Position.X < -BackgroundTracks[i].Texture.Size.X - MAPXOFFSET)
                    BackgroundTracks.RemoveAt(i);
            }

        }
    }
}
