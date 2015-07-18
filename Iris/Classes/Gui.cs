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
        public static List<FragText> FragTexts = new List<FragText>() { };
        public static bool ChatOpen = false;
        public static bool Composing = false;
        public static StringBuilder Draft = new StringBuilder();
        public static int ChatCloseDelay = 0;
        public static int maxCharacters = 50;
        public static int maxNameCharacters = 12;
        public static int FragTextRemoveTimer = 60 * 5;
        public static bool shopping;

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
            ChatCloseDelay--;
            if (FragTexts.Count > 0)
                FragTextRemoveTimer--;

            if (FragTextRemoveTimer <= 0 || FragTexts.Count > 5)
            {
                FragTextRemoveTimer = 60 * 5;
                if (FragTexts.Count > 0)
                    FragTexts.RemoveAt(0);
            }

            if (Input.isKeyTap(Keyboard.Key.E))
                shopping = !shopping;

            if (Chats.Count > 5)
                Chats.RemoveAt(Chats.Count - 1);


            if (!Composing)
                ChatOpen = false;

            if (ChatCloseDelay > 0)
                ChatOpen = true;

            CrosshairFireExpand *= .8f;
            MainGame.GuiCamera.Center = new Vector2f(MainGame.window.Size.X / 4, MainGame.window.Size.Y / 4);
        }

        public static void Draw()
        {
            if (shopping)
            {
                RectangleShape rectBG = new RectangleShape(new Vector2f(200, 80));
                rectBG.Position = new Vector2f(100, 10);
                rectBG.FillColor = new Color(10, 10, 10, 150);
                rectBG.Draw(MainGame.window, RenderStates.Default);

                int shotgunCost = 200;
                int machinegunCost = 275;
                int bombCost = 325;

                Render.Draw(Content.GetTexture("revolver.png"), new Vector2f(100, 40), new Color(255, 255, 255, (byte)(MainGame.dm.clientCoins > 0 ? 255 : 55)), new Vector2f(0, 0), 1, 0f);
                Render.DrawString(Content.GetFont("PixelSix.ttf"), "Free", new Vector2f(120, 60), new Color(255, 255, 255, (byte)(MainGame.dm.clientCoins > 0 ? 255 : 55)), .5f, true, 1);
                Render.DrawString(Content.GetFont("PixelSix.ttf"), "[1]", new Vector2f(120, 13), new Color(255, 255, 255, (byte)(MainGame.dm.clientCoins > 0 ? 255 : 55)), .5f, true, 1);

                Render.Draw(Content.GetTexture("shotgun.png"), new Vector2f(140, 40), new Color(255, 255, 255, (byte)(MainGame.dm.clientCoins > shotgunCost ? 255 : 55)), new Vector2f(0, 0), 1, 0f);
                if (MainGame.dm.player.weapons[1] == null)
                {
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), "" + shotgunCost + "c", new Vector2f(165, 60), new Color(255, 255, 255, (byte)(MainGame.dm.clientCoins > shotgunCost ? 255 : 55)), .5f, true, 1);
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), "[2]", new Vector2f(165, 13), new Color(255, 255, 255, (byte)(MainGame.dm.clientCoins > shotgunCost ? 255 : 55)), .5f, true, 1);
                }

                Render.Draw(Content.GetTexture("machinegun.png"), new Vector2f(200, 40), new Color(255, 255, 255, (byte)(MainGame.dm.clientCoins > machinegunCost ? 255 : 55)), new Vector2f(0, 0), 1, 0f);
                if (MainGame.dm.player.weapons[2] == null)
                {
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), "" + machinegunCost + "c", new Vector2f(225, 60), new Color(255, 255, 255, (byte)(MainGame.dm.clientCoins > machinegunCost ? 255 : 55)), .5f, true, 1);
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), "[3]", new Vector2f(225, 13), new Color(255, 255, 255, (byte)(MainGame.dm.clientCoins > machinegunCost ? 255 : 55)), .5f, true, 1);
                }

                Render.Draw(Content.GetTexture("bomb.png"), new Vector2f(260, 40), new Color(255, 255, 255, (byte)(MainGame.dm.clientCoins > bombCost ? 255 : 55)), new Vector2f(0, 0), 1, 0f);
                if (MainGame.dm.player.weapons[3] == null)
                {
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), "" + bombCost + "c", new Vector2f(275, 60), new Color(255, 255, 255, (byte)(MainGame.dm.clientCoins > bombCost ? 255 : 55)), .5f, true, 1);
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), "[4]", new Vector2f(275, 13), new Color(255, 255, 255, (byte)(MainGame.dm.clientCoins > bombCost ? 255 : 55)), .5f, true, 1);
                }

                if (Input.isKeyTap(Keyboard.Key.Num2))
                {
                    if (MainGame.dm.clientCoins > shotgunCost)
                    {
                        MainGame.dm.clientCoins -= shotgunCost;
                        MainGame.dm.player.weapons[1] = (new Shotgun(MainGame.dm.player));
                        MainGame.dm.Mailman.sendWeaponSwitch(1);
                        MainGame.dm.player.weapon = MainGame.dm.player.weapons[1];
                        shopping = false;
                    }
                }
                if (Input.isKeyTap(Keyboard.Key.Num3))
                {
                    if (MainGame.dm.clientCoins > machinegunCost)
                    {
                        MainGame.dm.clientCoins -= machinegunCost;
                        MainGame.dm.player.weapons[2] = (new MachineGun(MainGame.dm.player));
                        MainGame.dm.Mailman.sendWeaponSwitch(2);
                        MainGame.dm.player.weapon = MainGame.dm.player.weapons[2];
                        shopping = false;
                    }
                }
                if (Input.isKeyTap(Keyboard.Key.Num4))
                {
                    if (MainGame.dm.clientCoins > bombCost)
                    {
                        MainGame.dm.clientCoins -= bombCost;
                        MainGame.dm.player.weapons[3] = (new BombWeapon(MainGame.dm.player));
                        MainGame.dm.Mailman.sendWeaponSwitch(3);
                        MainGame.dm.player.weapon = MainGame.dm.player.weapons[3];
                        shopping = false;
                    }
                }
            }

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

            if (MainGame.dm.player.weapon.Ammo > 0)
                ammoBar.TextureRect = new IntRect(0, 10, (int)healthBar.Texture.Size.X, (int)(24 * ((float)MainGame.dm.player.weapon.Ammo / (float)MainGame.dm.player.weapon.MaxAmmo)));
            else
            {
                ammoBar.TextureRect = new IntRect(0, 10, (int)healthBar.Texture.Size.X, (int)(24 * ((float)(70 - MainGame.dm.player.weapon.ReloadTimer) / MainGame.dm.player.weapon.ReloadSpeed)));
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
            Render.Draw(Content.GetTexture("gibHead.png"), new Vector2f(0, 3), Color.White, new Vector2f(0, 0), 1, 0f, 1.5f);

            Render.DrawString(Content.GetFont("PixelSix.ttf"), MainGame.dm.clientCoins + "", new Vector2f(35, 30), Color.White, .5f, true, 1);
            Render.DrawString(Content.GetFont("PixelSix.ttf"), MainGame.dm.clientName, new Vector2f(2, 55), Color.White, .3f, false, 1);
            Render.DrawString(Content.GetFont("PixelSix.ttf"), "[E] Shop", new Vector2f(2, 65), Color.White, .25f, false, 1);

            if (!MainGame.dm.player.Alive)
            {
                if (MainGame.dm.player.respawnTimer > 0)
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), (MainGame.dm.player.respawnTimer / 60) + 1 + "", new Vector2f(200, 45), Color.White, .9f, true, 1);
                else
                {
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), "Respawn!", new Vector2f(200, 45), Color.White, .9f, true, 1);
                }
            }

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

            for (int i = 0; i < FragTexts.Count; i++)
            {
                int spacing = 4;
                int rightX = 390;

                Font font = Content.GetFont("OldNewspaperTypes.ttf");
                Text textKiller = new Text(FragTexts[i].killer.ToString(), font);
                Text textVictim = new Text(FragTexts[i].killer.ToString(), font);

                Render.DrawString(font, FragTexts[i].killer, new Vector2f(rightX - (textVictim.GetLocalBounds().Width * .35f) -
                    (textKiller.GetLocalBounds().Width * .35f) - (FragTexts[i].icon.Size.X + (spacing * 2)), 10 + i * 15), Color.White, .35f, false, 1);

                Render.Draw(FragTexts[i].icon, new Vector2f(rightX - (textVictim.GetLocalBounds().Width * .35f) - (FragTexts[i].icon.Size.X + spacing), 13 + i * 15), Color.White, new Vector2f(0, 0), 1, 0);


                Render.DrawString(font, FragTexts[i].victim, new Vector2f(rightX - (textVictim.GetLocalBounds().Width * .35f), 10 + i * 15), Color.White, .35f, false, 1);
            }

            if (MainGame.dm.tunnel)
            {
                Render.Draw(Content.GetTexture("caution.png"), new Vector2f(350, 190), Color.White, new Vector2f(0, 0), 1, 0f);
            }

            for (int i = 0; i < MainGame.dm.GameObjects.Count; i++)
            {
                if (MainGame.dm.GameObjects[i] is TreasureBox)
                {
                    TreasureBox box = MainGame.dm.GameObjects[i] as TreasureBox;
                    Vector2f drawPos = box.Pos;
                    if (Helper.Distance(box.Pos, MainGame.dm.player.Pos) > 320)
                    {
                        int flip = -1;
                        if (box.Pos.X < MainGame.dm.player.Pos.X)
                            drawPos.X = 40;
                        else
                        {
                            drawPos.X = 350;
                            flip = 1;
                        }

                        drawPos.Y = box.Pos.Y;

                        if (!box.goldDropped)
                            Render.Draw(Content.GetTexture("treasureBubble.png"), drawPos, Color.White, new Vector2f(0, 0), flip, 0f);
                    }
                   
                }
            }

            if (Input.isKeyDownOverride(Keyboard.Key.Tab))
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
                        string newName = "";

                        try
                        {
                            newName = Draft.ToString().Substring(9).Trim();
                        }
                        catch (Exception)
                        {
                            Chats.Insert(0, "Oops!");
                            return;
                        }
                        if (newName.Length > maxNameCharacters)
                            newName = newName.Substring(0, maxNameCharacters);


                        if (newName.ToLower().Contains("fag") ||
                            newName.ToLower().Contains("nigger") ||
                            newName.ToLower().Contains("fuck") ||
                            newName.ToLower().Contains("shit") ||
                            newName.ToLower().Contains("cunt") ||
                            newName.ToLower().Contains("ass") ||
                            newName.ToLower().Contains("gay"))
                        {
                            Chats.Insert(0, "Nope.");
                        }
                        else
                        {
                            MainGame.dm.Mailman.SendChat(MainGame.dm.clientName + " has changed their name to " + newName);
                            Chats.Insert(0, "You have changed your name to " + newName);
                            MainGame.dm.player.Name = newName;
                            MainGame.dm.clientName = newName;
                            MainGame.dm.Mailman.SendName(MainGame.dm.player.Name);
                        }
                    }
                    else
                    {
                        string completeMessage = MainGame.dm.clientName + ": " + Draft.ToString();
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
