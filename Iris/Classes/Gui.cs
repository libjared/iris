using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iris
{
    public static class Gui
    {
        //for things only gui should hold
        public static float CrosshairFireExpand = 0f;

        //for things normally held in other classes; updated framely
        //private static 
        public static List<String> Chats = new List<string>() { };
        public static bool ChatOpen = false;
        public static bool Composing = false;
        public static StringBuilder Draft = new StringBuilder();
        public static int ChatCloseDelay = 0;
        public static int maxCharacters = 50;
        public static int maxNameCharacters = 12;

        static Gui()
        {
            MainGame.window.TextEntered += (object sender, TextEventArgs e) =>
            {
                if (Composing)
                {
                    if (Keyboard.IsKeyPressed(Keyboard.Key.BackSpace))
                    {
                        if (Draft.Length > 0)
                            Draft = Draft.Remove(Draft.Length - 1, 1);
                    }
                    else if (Keyboard.IsKeyPressed(Keyboard.Key.Return))
                    {
                    }
                    else if (Keyboard.IsKeyPressed(Keyboard.Key.LControl))
                    {
                    }
                    else if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
                    {
                        ChatOpen = false;
                        Input.isActive = true;
                    }
                    else if (Keyboard.IsKeyPressed(Keyboard.Key.Tab))
                    {
                    }
                    else
                    {
                        if (Draft.Length < maxCharacters)
                            Draft.Append(e.Unicode);
                    }
                }
            };
        }

        public static void Update()
        {
            if (Chats.Count > 5)
                Chats.RemoveAt(Chats.Count - 1);

            ChatCloseDelay--;
            if (!Composing)
                ChatOpen = false;

            if (ChatCloseDelay > 0)
                ChatOpen = true;

            CrosshairFireExpand *= .8f;
            MainGame.GuiCamera.Center = new Vector2f(MainGame.window.Size.X / 4, MainGame.window.Size.Y / 4);
        }

        public static void Draw()
        {


            Vector2f mouse = MainGame.window.MapPixelToCoords(Input.screenMousePos);

            Vector2u crosshairOriginU = Content.GetTexture("crosshair.png").Size / 2;
            Vector2f crosshairOrigin = new Vector2f(crosshairOriginU.X, crosshairOriginU.Y);

            Sprite healthBar = new Sprite(Content.GetTexture("healthBarVert.png"));
            healthBar.Position = mouse;
            healthBar.Origin = crosshairOrigin - new Vector2f(0, 10);
            healthBar.TextureRect = new IntRect(0, 10, (int)healthBar.Texture.Size.X, (int)(24 * ((float)MainGame.dm.player.Health / 100f)));
            healthBar.Scale = new Vector2f(1, -1f);
            healthBar.Draw(MainGame.window, RenderStates.Default);

            Sprite ammoBar = new Sprite(Content.GetTexture("ammoBarVert.png"));
            ammoBar.Position = mouse;
            ammoBar.Origin = crosshairOrigin - new Vector2f(0, 10);
            ammoBar.Scale = new Vector2f(1, -1f);

            if (MainGame.dm.player.AMMO_Bullet > 0)
                ammoBar.TextureRect = new IntRect(0, 10, (int)healthBar.Texture.Size.X, (int)(24 * ((float)MainGame.dm.player.AMMO_Bullet / 8f)));
            else
            {
                ammoBar.TextureRect = new IntRect(0, 10, (int)healthBar.Texture.Size.X, (int)(24 * ((float)(70 - MainGame.dm.player.ReloadTimer) / 70f)));
                ammoBar.Color = Color.Red;//new Color(0, 0, 0, 170);
            }

            ammoBar.Draw(MainGame.window, RenderStates.Default);

            //Render.Draw(Content.GetTexture("healthBarVert.png"), mouse, Color.White, crosshairOrigin, 1, 0, 1 + CrosshairFireExpand);
            //Render.Draw(Content.GetTexture("ammoBarVert.png"), mouse, Color.White, crosshairOrigin, 1, 0, 1 + CrosshairFireExpand);
            Render.Draw(Content.GetTexture("crosshair.png"), mouse, Color.White, crosshairOrigin, 1, 0, 1 + CrosshairFireExpand);
            Render.Draw(Content.GetTexture("crosshairBars.png"), mouse, Color.White, crosshairOrigin, 1, 0, 1);



            Sprite healthBar2 = new Sprite(Content.GetTexture("healthBar.png"));
            healthBar2.Position = new Vector2f(25, 3);
            healthBar2.TextureRect = new IntRect(0, 0, (int)(healthBar2.Texture.Size.X * ((float)MainGame.dm.player.Health / 100f)), (int)healthBar2.Texture.Size.Y);
            //healthBar2.Scale = new Vector2f(1, -1f);
            healthBar2.Draw(MainGame.window, RenderStates.Default);

            //shader.Shader.SetParameter("sampler", pistolHand);
            //Render.Draw(pistolHand, Core + Helper.PolarToVector2(holdDistance, AimAngle, 1, 1), Color.White, new Vector2f(2, 4), 1, AimAngle, 1, Facing == -1);


            Render.Draw(Content.GetTexture("characterUI.png"), new Vector2f(0, 0), Color.White, new Vector2f(0, 0), 1, 0f);
            Render.Draw(Content.GetTexture("gibHead.png"), new Vector2f(1, 4), Color.White, new Vector2f(0, 0), 1, 0f, 1.5f);

            Render.DrawString(Content.GetFont("PixelSix.ttf"), MainGame.dm.clientCoins + "", new Vector2f(35, 30), Color.White, .5f, true, 1);

            if (ChatOpen)
            {
                RectangleShape rectBG = new RectangleShape(new Vector2f(350, 85));
                rectBG.Position = new Vector2f(1, 198);
                rectBG.FillColor = new Color(10, 10, 10, 150);
                rectBG.Draw(MainGame.window, RenderStates.Default);

                RectangleShape rectBG2 = new RectangleShape(new Vector2f(350, 12));
                rectBG2.Position = new Vector2f(1, 285);
                rectBG2.FillColor = new Color(10, 10, 10, 200);
                rectBG2.Draw(MainGame.window, RenderStates.Default);

                Font font = Content.GetFont("OldNewspaperTypes.ttf");
                for (int i = 0; i < Chats.Count; i++)
                {
                    Render.DrawString(font, Chats[i], new Vector2f(10, 265 - (i * 15)), Color.White, .35f, false, 1);
                }



                //int subStringStart = Draft.Length > 50 ? Draft.Length - 50 : 0;


                Render.DrawString(font, Draft.ToString(), new Vector2f(10, 283), Color.White, .35f, false, 1);

                if (Composing)
                {
                    Text textBar = new Text(Draft.ToString(), font);

                    Render.DrawString(font, "|", new Vector2f(10 + textBar.GetLocalBounds().Width * .35f, 283), Color.White, .35f, false, 1);
                }
            }

            if (MainGame.dm.tunnel)
            {
                Render.Draw(Content.GetTexture("caution.png"), new Vector2f(550, 300), Color.White, new Vector2f(0, 0), 1, 0f);

            }

            if (Input.isKeyDown(Keyboard.Key.Tab))
            {
                RectangleShape rectBG = new RectangleShape(new Vector2f(190, MainGame.dm.Players.Count * 20));
                rectBG.Position = new Vector2f(100, 70);
                rectBG.FillColor = new Color(10, 10, 10, 150);
                rectBG.Draw(MainGame.window, RenderStates.Default);
                Font font = Content.GetFont("PixelSix.ttf");

                for (int i = 0; i < MainGame.dm.Players.Count; i++)
                {
                    Render.DrawString(font, MainGame.dm.Players[i].Name, new Vector2f(200, 70 + (i * 20)), Color.White, .45f, true, 1);
                }
            }

            if (Input.isKeyOverride(Keyboard.Key.Return))
            {
                if (!ChatOpen || !Composing)
                {
                    Input.isActive = false;
                    ChatOpen = true;
                    Composing = true;
                }
                else if (Draft.Length == 0)
                {
                    Input.isActive = true;
                    ChatOpen = false;
                    Composing = false;
                }
                else
                {
                    if (Draft.ToString().IndexOf("/setname") == 0)
                    {
                        string newName = Draft.ToString().Substring(9).Trim();
                        if (newName.Length > maxNameCharacters)
                            newName = newName.Substring(0, maxNameCharacters);


                        if (newName.ToLower().Contains("fag") ||
                            newName.ToLower().Contains("nigge") ||
                            newName.ToLower().Contains("fuck") ||
                            newName.ToLower().Contains("shit") ||
                            newName.ToLower().Contains("gay"))
                        {
                            Chats.Insert(0, "Nope.");
                        }
                        else
                        {
                            MainGame.dm.Mailman.SendChat(MainGame.dm.player.Name + " has changed their name to " + newName);
                            Chats.Insert(0, "You have changed your name to " + newName);
                            MainGame.dm.player.Name = newName;
                            MainGame.dm.Mailman.SendName(MainGame.dm.player.Name);
                        }
                    }
                    else
                    {
                        string completeMessage = MainGame.dm.player.Name + ": " + Draft.ToString();
                        MainGame.dm.Mailman.SendChat(completeMessage);
                        Chats.Insert(0, completeMessage);
                    }

                    Draft.Clear();
                    Input.isActive = true;
                    ChatOpen = false;
                    ChatCloseDelay = 60 * 4;
                    Composing = false;
                }
            }
        }
    }
}
