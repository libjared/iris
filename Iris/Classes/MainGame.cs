using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Iris
{
    class MainGame
    {
        public static RenderWindow window;
        public static DateTime startTime;
        public static Vector2f windowSize;
        public static Random rand;
        public static View Camera;
        public static Deathmatch dm;

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
            rand = new Random();
        }

        private static void LoadContentInitialize()
        {
            window = new RenderWindow(
                new VideoMode(800, 600), "Project Iris", Styles.Titlebar);

            windowSize = new Vector2f(800, 600);
            window.SetFramerateLimit(60);

            window.Closed += (o, e) =>
            {
                dm.Close();
                window.Close();
            };

            window.GainedFocus += (o, e) => { Input.isActive = true; };
            window.LostFocus += (o, e) => { Input.isActive = false; };
            Camera = new View();
            Camera.Zoom(.4f);
            dm = new Deathmatch();
        }

        private static void UpdateDraw(RenderWindow window)
        {

            

            window.Clear(Color.Blue);
            window.DispatchEvents();
            Input.Update();
            dm.Update();
            dm.Draw();
            window.Display();
            Input.refreshState(); //Always call last

            if (Input.isKeyDown(Keyboard.Key.Escape))
            {
                if (dm.Mailman != null)
                {
                    dm.Mailman.Disconnect();
                }
                window.Close();
            }
        }
    }
}


