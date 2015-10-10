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
        public static Model Char1Model, Char2Model;
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
            Char1Model = new Model();
            Char1Model.name = "Character_1_Model";
            Char1Model.gibArmFile = "gibArm.png";
            Char1Model.gibBodyFile = "gibBody.png";
            Char1Model.gibHeadFile = "gibHead.png";
            Char1Model.gibLowerLegFile = "gibLowerLeg.png";
            Char1Model.gibUpperLegFile = "gibUpperLeg.png";
            Char1Model.idleFile = "idle.png";
            Char1Model.jumpDownFile = "jumpDown.png";
            Char1Model.jumpUpFile = "jumpUp.png";
            Char1Model.runFile = "run.png";
            Char1Model.pistolHand = "pistolHand.png";

            Char2Model = new Model();
            Char2Model.name = "Character_2_Model";
            Char2Model.gibArmFile = "char2_gibArm.png";
            Char2Model.gibBodyFile = "char2_gibBody.png";
            Char2Model.gibHeadFile = "char2_gibHead.png";
            Char2Model.gibLowerLegFile = "char2_gibLowerLeg.png";
            Char2Model.gibUpperLegFile = "char2_gibUpperLeg.png";
            Char2Model.idleFile = "char2_idle.png";
            Char2Model.jumpDownFile = "char2_jumpDown.png";
            Char2Model.jumpUpFile = "char2_jumpUp.png";
            Char2Model.runFile = "char2_run.png";
            Char2Model.pistolHand = "char2_pistolHand.png";

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

        internal static bool containsProfanity(string newName)
        {
            return (newName.ToLower().Contains("fag") ||
                           newName.ToLower().Contains("nigger") ||
                           newName.ToLower().Contains("fuck") ||
                           newName.ToLower().Contains("shit") ||
                           newName.ToLower().Contains("cunt") ||
                           newName.ToLower().Contains("ass") ||
                           newName.ToLower().Contains("dick") ||
                           newName.ToLower().Contains("penis") ||
                           newName.ToLower().Contains("gay"));
        }
    }
}


