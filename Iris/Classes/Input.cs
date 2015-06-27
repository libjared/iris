using SFML.System;
using SFML.Window;
using System.Collections.Generic;

namespace Iris
{
    public static class Input
    {
        public static List<string> PressedKeys = new List<string>() { };
        public static List<string> OldPressedKeys = new List<string>() { };

        public static List<string> PressedMouseButtons = new List<string>() { };
        public static List<string> OldPressedMouseButtons = new List<string>() { };

        public static bool isActive = true;

        public static Vector2i screenMousePos
        {
            get
            {
                return Mouse.GetPosition(MainGame.window);
            }
        }

        public static void Update()
        {
            refreshState();
            PressedKeys.Clear();
            PressedMouseButtons.Clear();

            for (int i = 0; i < 100; i++)
            {
                if (Keyboard.IsKeyPressed((Keyboard.Key)i))
                {
                    PressedKeys.Add(((Keyboard.Key)i).ToString());
                }
            }

            for (int i = 0; i < 6; i++)
            {
                if (Mouse.IsButtonPressed((Mouse.Button)i))
                {
                    PressedMouseButtons.Add(((Mouse.Button)i).ToString());
                }
            }
        }

        public static void refreshState()
        {
            OldPressedKeys = new List<string>(PressedKeys);
            OldPressedMouseButtons = new List<string>(PressedMouseButtons);
        }

        public static bool isKeyTap(Keyboard.Key key)
        {
            if (!isActive)
                return false;
            if (PressedKeys.Contains(key.ToString()) &&
                !OldPressedKeys.Contains(key.ToString()))
                return true;
            return false;
        }

        public static bool isMouseButtonTap(Mouse.Button key)
        {
            if (!isActive)
                return false;
            if (PressedMouseButtons.Contains(key.ToString()) &&
                !OldPressedMouseButtons.Contains(key.ToString()))
                return true;
            return false;
        }

        public static bool isKeyDown(Keyboard.Key key)
        {
            if (!isActive)
                return false;
            return Keyboard.IsKeyPressed(key);
        }

        public static bool isKeyUp(Keyboard.Key key)
        {
            if (!isActive)
                return false;
            return !Keyboard.IsKeyPressed(key);
        }
    }
}
