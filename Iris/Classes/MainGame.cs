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
        public static ClientMailman mailman;

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
        }

        static void window_LostFocus(object sender, EventArgs e)
        {
            Input.isActive = false;
        }

        static void window_GainedFocus(object sender, EventArgs e)
        {
            Input.isActive = true;
        }

        private static void LoadContentInitialize()
        {
            mailman = new ClientMailman(dm);

            window = new RenderWindow(
                new VideoMode(800, 600), "Project Iris", Styles.Titlebar);

            windowSize = new Vector2f(800, 600);
            window.SetFramerateLimit(60);

            window.Closed += (a, b) =>
            {
                mailman.Disconnect();
                window.Close();
            };

            window.GainedFocus += new EventHandler(window_GainedFocus);
            window.LostFocus += new EventHandler(window_LostFocus);
            
            dm = new Deathmatch();

            mailman.Connect();
        }

        private static void UpdateDraw(RenderWindow window)
        {
            window.Clear(Color.Black);
            mailman.HandleMessages();
            window.DispatchEvents();
            Input.Update();
            dm.Update();
            dm.Draw();
            window.Display();

            if (Input.isKeyDown(Keyboard.Key.Escape))
            {
                window.Close();
            }
        }
    }
}


