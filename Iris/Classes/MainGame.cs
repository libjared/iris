using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iris
{
    public class MainGame
    {
        public static RenderWindow window;
        public static DateTime startTime;
        public static Random rand;
        public static View Camera;
        public static Deathmatch dm;
        public static Vector2f worldMousePos;
        public static DateTime oldDateTime;
        const double expectedTicks = (1000.0 / 63.0) * 10000.0;
        public static TimeSpan deltaTime
        {
            get
            {
                return DateTime.Now - oldDateTime;
            }
        }

        public static void StartGame()
        {
            PreRun();
            LoadContentInitialize();

            while (window.IsOpen)
            {
                UpdateDraw(window);
            }
        }

        private static void PreRun()
        {
            startTime = DateTime.Now;
            oldDateTime = DateTime.Now - new TimeSpan((long)expectedTicks);
            rand = new Random();
        }

        private static void LoadContentInitialize()
        {
            window = new RenderWindow(
                new VideoMode(800, 600), "Project Iris", Styles.Resize);

            window.SetFramerateLimit(60);
            window.SetMouseCursorVisible(false);
            window.Closed += (o, e) =>
            {
                dm.Close();
                window.Close();
            };

            window.GainedFocus += (o, e) => { Input.isActive = true; };
            window.LostFocus += (o, e) => { Input.isActive = false; };
            Camera = new View(window.DefaultView);
            Camera.Size = new Vector2f(800/2, 600/2);

            //GUICamera = new View(window.DefaultView);
            //GUICamera.Size = new Vector2f(800 / 2, 600 / 2);
            //Camera.Zoom(.25f); //Adjust as needed

            dm = new Deathmatch();
        }

        private static void UpdateDraw(RenderWindow window)
        {
            //float ratio = 800 / 600;
            window.Size = new Vector2u((uint)(800),(uint)(600));

            window.Clear(Color.Black);
            window.DispatchEvents();
            Input.Update();
            dm.Update();
            dm.Draw();
            Render.Draw(Content.GetTexture("crosshair.png"), worldMousePos, Color.White, new Vector2f(11, 11), 1, 0, 1 + dm.player.crosshairFireExpand);
            window.Display();
            updateWorldMousePos();

            Input.refreshState(); //Always call last

            if (Input.isKeyDown(Keyboard.Key.Escape))
            {
                if (dm.Mailman != null)
                {
                    dm.Close();
                }
                window.Close();
            }
        }

        public static void updateWorldMousePos()
        {
            worldMousePos = window.MapPixelToCoords(Input.screenMousePos, Camera);
        }
    }
}


