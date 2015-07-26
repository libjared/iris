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
        public static View GuiCamera;
        public static Deathmatch dm;
        public static Menu mm;
        public static Gamestate gamestate;
        public static Vector2f worldMousePos;
        public static DateTime oldDateTime;
        public static int gibCount = 20;
        const double expectedTicks = (1000.0 / 63.0) * 10000.0;
        public static List<SoundInstance> soundInstances;
        public static TimeSpan deltaTime
        {
            get
            {
                return DateTime.Now - oldDateTime;
            }
        }
        public static Vector2i WindowSize
        {
            get
            {
                return new Vector2i((int)window.Size.X, (int)window.Size.Y);
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

            window.GainedFocus += (o, e) =>
            {
                Input.isActive = true;
            };
            window.LostFocus += (o, e) =>
            {
                Input.isActive = false;
            };

            Camera = new View(window.DefaultView);
            Camera.Size = new Vector2f(800 / 2, 600 / 2);
            GuiCamera = new View(window.DefaultView);
            GuiCamera.Size = new Vector2f(800 / 2, 600 / 2);
            soundInstances = new List<SoundInstance>() { };
            dm = new Deathmatch();
            mm = new Menu();
            gamestate = mm;
        }

        private static void UpdateDraw(RenderWindow window)
        {
            //float ratio = 800 / 600;
            window.Size = new Vector2u(800u, 600u);

            window.Clear(Color.Black);
            window.DispatchEvents();
            UpdateSoundInstances();

            Input.Update();
            updateWorldMousePos();
            gamestate.Update();
           

            gamestate.Draw();
            window.SetView(GuiCamera);
            
            window.SetView(Camera);

            window.Display();

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

        private static void UpdateSoundInstances()
        {
            for (int i = 0; i < soundInstances.Count; i++)
            {
                soundInstances[i].Update();
            }
            if (soundInstances.Count > 200)
                soundInstances.Clear(); // Oh shit, too many noises
        }
    }
}


