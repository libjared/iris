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
        public static bool shopping, emoteMenuOpen;
        public static List<Actor> highscore = new List<Actor>();

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
            highscore = MainGame.dm.Players;
            highscore.Sort(delegate (Actor x, Actor y)
            {
                if (x.gold == y.gold) return 0;
                else if (x.gold < y.gold) return -1;
                else if (x.gold > y.gold) return 1;
                else return x.gold.CompareTo(y.gold);
            });

            foreach (Actor a in highscore)
            {
                //Console.WriteLine(a.gold);
            }

            ChatCloseDelay--;
            if (FragTexts.Count > 0)
                FragTextRemoveTimer--;

            if (FragTextRemoveTimer <= 0 || FragTexts.Count > 5)
            {
                FragTextRemoveTimer = 60 * 5;
                if (FragTexts.Count > 0)
                    FragTexts.RemoveAt(0);
            }


            if (Chats.Count > 5)
                Chats.RemoveAt(Chats.Count - 1);


            if (!Composing)
                ChatOpen = false;

            if (ChatCloseDelay > 0)
                ChatOpen = true;

            if (Input.isKeyTap(Keyboard.Key.C))
            {
                shopping = !shopping;
                emoteMenuOpen = false;
            }

            if (Input.isKeyTap(Keyboard.Key.V))
            {
                emoteMenuOpen = !emoteMenuOpen;
                shopping = false;
            }


            CrosshairFireExpand *= .8f;
            MainGame.GuiCamera.Center = new Vector2f(MainGame.window.Size.X / 4, MainGame.window.Size.Y / 4);
        }

        public static void Draw()
        {
            MainGame.window.SetView(MainGame.GuiCamera);



            if (!shopping && !emoteMenuOpen)
            {
                if (MainGame.dm.roundStarted)
                {
                    float fakeTime = MainGame.dm.roundTimeLeft - 10;
                    if (fakeTime > 10)
                    {
                        if (fakeTime < 2500) //The timer will start at an
                        {
                            Color c = Color.White;
                            if (MainGame.dm.firstPlacePlayer.UID == MainGame.dm.player.UID)
                                c = Color.Yellow; //Changing this makes your name a different color if you're winning
                            Render.DrawString(Content.GetFont("PixelSix.ttf"), "Highest Bounty: " + MainGame.dm.firstPlacePlayer.Name, new Vector2f(200, 25),
                                c, .4f,
                                 true, 1);


                            Render.DrawString(Content.GetFont("OldNewspaperTypes.ttf"), + (int)(fakeTime - 10)/60 + ":" + 
                                String.Format("{0:D2}", (int)(fakeTime - 10) % 60) + " sec", new Vector2f(200, 3),
                            fakeTime < 30 ? Color.Yellow : Color.White,
                            fakeTime < 30 ? .7f : .6f,
                             true, 1);
                        }
                    }
                    else
                    {
                        Render.DrawString(Content.GetFont("OldNewspaperTypes.ttf"), +MainGame.dm.roundTimeLeft - 10 + " sec", new Vector2f(200, 3),
                           Color.Green,
                           .6f,
                            true, 1);

                        string t = MainGame.dm.firstPlacePlayer.Name + " wins the round!";
                        Color c = Color.White;
                        if (MainGame.dm.firstPlacePlayer.UID == MainGame.dm.player.UID)
                            c = Color.Yellow;
                        Render.DrawString(Content.GetFont("PixelSix.ttf"),t, new Vector2f(200, 25),
                           c, .4f,
                            true, 1);
                        Render.DrawString(Content.GetFont("PixelSix.ttf"), "Starting New Round", new Vector2f(200, 35),
                           Color.White, .4f,
                            true, 1);
                        
                    }


                }
            }

            if (emoteMenuOpen)
            {
                RectangleShape rectBG = new RectangleShape(new Vector2f(200, 60));
                rectBG.Position = new Vector2f(100, 10);
                rectBG.FillColor = new Color(10, 10, 10, 150);
                rectBG.Draw(MainGame.window, RenderStates.Default);

                Render.Draw(Content.GetTexture("emote_cool.png"), new Vector2f(105, 35), Color.White, new Vector2f(0, 0), 1, 0f);
                Render.DrawString(Content.GetFont("PixelSix.ttf"), "[1]", new Vector2f(120, 13), new Color(255, 255, 255), .5f, true, 1);

                Render.Draw(Content.GetTexture("emote_laugh.png"), new Vector2f(155, 35), Color.White, new Vector2f(0, 0), 1, 0f);
                Render.DrawString(Content.GetFont("PixelSix.ttf"), "[2]", new Vector2f(170, 13), new Color(255, 255, 255), .5f, true, 1);

                Render.Draw(Content.GetTexture("emote_silly.png"), new Vector2f(205, 35), Color.White, new Vector2f(0, 0), 1, 0f);
                Render.DrawString(Content.GetFont("PixelSix.ttf"), "[3]", new Vector2f(220, 13), new Color(255, 255, 255), .5f, true, 1);

                Render.Draw(Content.GetTexture("emote_shocked.png"), new Vector2f(255, 35), Color.White, new Vector2f(0, 0), 1, 0f);
                Render.DrawString(Content.GetFont("PixelSix.ttf"), "[4]", new Vector2f(270, 13), new Color(255, 255, 255), .5f, true, 1);

                if (MainGame.dm.player.Alive)
                {
                    if (Input.isKeyTap(Keyboard.Key.Num1))
                    {
                        string s = "cool";
                        MainGame.dm.GameObjects.Add(new EmoteBubble(s, MainGame.dm.player));
                        MainGame.dm.Mailman.SendEmote(s);
                        emoteMenuOpen = false;
                    }
                    if (Input.isKeyTap(Keyboard.Key.Num2))
                    {
                        string s = "laugh";
                        MainGame.dm.GameObjects.Add(new EmoteBubble(s, MainGame.dm.player));
                        MainGame.dm.Mailman.SendEmote(s);
                        emoteMenuOpen = false;
                    }
                    if (Input.isKeyTap(Keyboard.Key.Num3))
                    {
                        string s = "silly";
                        MainGame.dm.GameObjects.Add(new EmoteBubble(s, MainGame.dm.player));
                        MainGame.dm.Mailman.SendEmote(s);
                        emoteMenuOpen = false;
                    }
                    if (Input.isKeyTap(Keyboard.Key.Num4))
                    {
                        string s = "shocked";
                        MainGame.dm.GameObjects.Add(new EmoteBubble(s, MainGame.dm.player));
                        MainGame.dm.Mailman.SendEmote(s);
                        emoteMenuOpen = false;
                    }
                }

            }

            if (shopping)
            {

                RectangleShape rectBG = new RectangleShape(new Vector2f(200, 80));
                rectBG.Position = new Vector2f(100, 10);
                rectBG.FillColor = new Color(10, 10, 10, 150);
                rectBG.Draw(MainGame.window, RenderStates.Default);

                int shotgunCost = 200;
                int machinegunCost = 275;
                int bombCost = 325;

                Render.Draw(Content.GetTexture("revolver.png"), new Vector2f(100, 40), new Color(255, 255, 255, (byte)(MainGame.dm.player.gold > 0 ? 255 : 55)), new Vector2f(0, 0), 1, 0f);
                Render.DrawString(Content.GetFont("PixelSix.ttf"), "Free", new Vector2f(120, 60), new Color(255, 255, 255, (byte)(MainGame.dm.player.gold > 0 ? 255 : 55)), .5f, true, 1);
                Render.DrawString(Content.GetFont("PixelSix.ttf"), "[1]", new Vector2f(120, 13), new Color(255, 255, 255, (byte)(MainGame.dm.player.gold > 0 ? 255 : 55)), .5f, true, 1);

                Render.Draw(Content.GetTexture("shotgun.png"), new Vector2f(140, 40), new Color(255, 255, 255, (byte)(MainGame.dm.player.gold >= shotgunCost ? 255 : 55)), new Vector2f(0, 0), 1, 0f);
                if (MainGame.dm.player.weapons[1] == null)
                {
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), "" + shotgunCost + "c", new Vector2f(165, 60), new Color(255, 255, 255, (byte)(MainGame.dm.player.gold >= shotgunCost ? 255 : 55)), .5f, true, 1);
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), "[2]", new Vector2f(165, 13), new Color(255, 255, 255, (byte)(MainGame.dm.player.gold >= shotgunCost ? 255 : 55)), .5f, true, 1);
                }

                Render.Draw(Content.GetTexture("machinegun.png"), new Vector2f(200, 40), new Color(255, 255, 255, (byte)(MainGame.dm.player.gold >= machinegunCost ? 255 : 55)), new Vector2f(0, 0), 1, 0f);
                if (MainGame.dm.player.weapons[2] == null)
                {
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), "" + machinegunCost + "c", new Vector2f(225, 60), new Color(255, 255, 255, (byte)(MainGame.dm.player.gold >= machinegunCost ? 255 : 55)), .5f, true, 1);
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), "[3]", new Vector2f(225, 13), new Color(255, 255, 255, (byte)(MainGame.dm.player.gold >= machinegunCost ? 255 : 55)), .5f, true, 1);
                }

                Render.Draw(Content.GetTexture("bomb.png"), new Vector2f(260, 40), new Color(255, 255, 255, (byte)(MainGame.dm.player.gold >= bombCost ? 255 : 55)), new Vector2f(0, 0), 1, 0f);
                if (MainGame.dm.player.weapons[3] == null)
                {
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), "" + bombCost + "c", new Vector2f(275, 60), new Color(255, 255, 255, (byte)(MainGame.dm.player.gold >= bombCost ? 255 : 55)), .5f, true, 1);
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), "[4]", new Vector2f(275, 13), new Color(255, 255, 255, (byte)(MainGame.dm.player.gold >= bombCost ? 255 : 55)), .5f, true, 1);
                }

                if (Input.isKeyTap(Keyboard.Key.Num2))
                {
                    if (MainGame.dm.player.weapons[1] == null)
                    {
                        if (MainGame.dm.player.gold >= shotgunCost)
                        {
                            MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("cashReg.wav"), 1, 0, 5));
                            MainGame.dm.player.gold -= shotgunCost;
                            MainGame.dm.player.weapons[1] = (new Shotgun(MainGame.dm.player));
                            MainGame.dm.Mailman.sendWeaponSwitch(1);
                            MainGame.dm.player.weapon = MainGame.dm.player.weapons[1];
                            shopping = false;
                        }
                    }
                }
                if (Input.isKeyTap(Keyboard.Key.Num3))
                {
                    if (MainGame.dm.player.weapons[2] == null)
                    {
                        if (MainGame.dm.player.gold >= machinegunCost)
                        {
                            MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("cashReg.wav"), 1, 0, 5));
                            MainGame.dm.player.gold -= machinegunCost;
                            MainGame.dm.player.weapons[2] = (new MachineGun(MainGame.dm.player));
                            MainGame.dm.Mailman.sendWeaponSwitch(2);
                            MainGame.dm.player.weapon = MainGame.dm.player.weapons[2];
                            shopping = false;
                        }
                    }
                }
                if (Input.isKeyTap(Keyboard.Key.Num4))
                {
                    if (MainGame.dm.player.weapons[3] == null)
                    {
                        if (MainGame.dm.player.gold >= bombCost)
                        {
                            MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("cashReg.wav"), 1, 0, 5));
                            MainGame.dm.player.gold -= bombCost;
                            MainGame.dm.player.weapons[3] = (new BombWeapon(MainGame.dm.player));
                            MainGame.dm.Mailman.sendWeaponSwitch(3);
                            MainGame.dm.player.weapon = MainGame.dm.player.weapons[3];
                            shopping = false;
                        }
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
                ammoBar.TextureRect = new IntRect(0, 10, (int)healthBar.Texture.Size.X, (int)(24 * ((float)(MainGame.dm.player.weapon.Ammo / (float)MainGame.dm.player.weapon.MaxAmmo))));
            else
            {
                ammoBar.TextureRect = new IntRect(0, 10, (int)healthBar.Texture.Size.X, (int)(24 * ((float)(70 - (MainGame.dm.player.weapon.ReloadTimer / MainGame.dm.player.weapon.ReloadSpeed)))));
                ammoBar.Color = Color.Red;//new Color(0, 0, 0, 170);
            }

            ammoBar.Draw(MainGame.window, RenderStates.Default);

            //Render.Draw(Content.GetTexture("healthBarVert.png"), mouse, Color.White, crosshairOrigin, 1, 0, 1 + CrosshairFireExpand);
            //Render.Draw(Content.GetTexture("ammoBarVert.png"), mouse, Color.White, crosshairOrigin, 1, 0, 1 + CrosshairFireExpand);




            Sprite healthBar2 = new Sprite(Content.GetTexture("healthBar.png"));
            healthBar2.Position = new Vector2f(25, 3);
            healthBar2.TextureRect = new IntRect(0, 0, (int)(healthBar2.Texture.Size.X * ((float)MainGame.dm.player.Health / 100f)), (int)healthBar2.Texture.Size.Y);
            //healthBar2.Scale = new Vector2f(1, -1f);
            healthBar2.Draw(MainGame.window, RenderStates.Default);

            //shader.Shader.SetParameter("sampler", pistolHand);
            //Render.Draw(pistolHand, Core + Helper.PolarToVector2(holdDistance, AimAngle, 1, 1), Color.White, new Vector2f(2, 4), 1, AimAngle, 1, Facing == -1);


            Render.Draw(Content.GetTexture("characterUI.png"), new Vector2f(0, 0), Color.White, new Vector2f(0, 0), 1, 0f);
            Render.Draw(Content.GetTexture(MainGame.dm.player.model.gibHeadFile), new Vector2f(0, 3), Color.White, new Vector2f(0, 0), 1, 0f, 1.5f);

            Render.DrawString(Content.GetFont("PixelSix.ttf"), MainGame.dm.player.gold + "", new Vector2f(35, 30), Color.White, .5f, true, 1);
            Render.DrawString(Content.GetFont("PixelSix.ttf"), MainGame.dm.player.Name, new Vector2f(2, 55), Color.White, .3f, false, 1);
            Render.DrawString(Content.GetFont("PixelSix.ttf"), "[C] Shop", new Vector2f(2, 65), Color.White, .25f, false, 1);
            Render.DrawString(Content.GetFont("PixelSix.ttf"), "[V] Emotes", new Vector2f(2, 75), Color.White, .25f, false, 1);
            Render.DrawString(Content.GetFont("PixelSix.ttf"), "[E] Use Item" + (MainGame.dm.player.ItemTimer <= 0? "" : ": " + MainGame.dm.player.ItemTimer / 60 ), new Vector2f(2, 85), Color.White, .25f, false, 1);

            if (!MainGame.dm.player.Alive)
            {
                if (MainGame.dm.player.respawnTimer > 0)
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), (MainGame.dm.player.respawnTimer / 60) + 1 + "", new Vector2f(200, 45), Color.White, .9f, true, 1);
                else
                {
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), "Respawn!", new Vector2f(200, 45), Color.White, .9f, true, 1);
                    Render.DrawString(Content.GetFont("PixelSix.ttf"), "[Left Shift]", new Vector2f(200, 75), Color.White, .4f, true, 1);
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
                Text textVictim = new Text(FragTexts[i].victim.ToString(), font);
                float scale = .25f;
                int topY = 20;

                Render.DrawString(font, FragTexts[i].killer, new Vector2f(rightX - (textVictim.GetLocalBounds().Width * scale) -
                    (textKiller.GetLocalBounds().Width * scale) - (FragTexts[i].icon.Size.X + (spacing * 2)), topY + i * 15), Color.White, scale, false, 0);

                Render.Draw(FragTexts[i].icon, new Vector2f(rightX - (textVictim.GetLocalBounds().Width * scale) - (FragTexts[i].icon.Size.X + spacing), topY + 2 + i * 15), Color.White, new Vector2f(0, 0), 1, 0);


                Render.DrawString(font, FragTexts[i].victim, new Vector2f(rightX - (textVictim.GetLocalBounds().Width * scale), topY + i * 15), Color.White, scale, false, 0);


                //Font font = Content.GetFont("OldNewspaperTypes.ttf");
                //Text textKiller = new Text(FragTexts[i].killer.ToString(), font);
                //Text textVictim = new Text(FragTexts[i].victim.ToString(), font);
                //float scale = .3f;

                //Render.DrawString(font, FragTexts[i].victim, new Vector2f(rightX, 20), Color.White, scale, false, 0, true);
                //Render.Draw(FragTexts[i].icon, new Vector2f(rightX - (textVictim.GetLocalBounds().Width * scale) - spacing, 27), Color.White, (Vector2f)FragTexts[i].icon.Size / 2, 1, 0f);
                //Render.DrawString(font, FragTexts[i].killer, new Vector2f(rightX - (textKiller.GetLocalBounds().Width * scale) - (spacing * 2), 20), Color.White, scale, false, 0, true);
            }

            if (MainGame.dm.tunnel)
            {
                Render.Draw(Content.GetTexture("caution.png"), new Vector2f(350, 110), Color.White, new Vector2f(0, 0), 1, 0f);
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
                    //if (Draft.ToString().IndexOf("/setname") == 0)
                    //{
                    //    string newName = "";

                    //    try
                    //    {
                    //        newName = Draft.ToString().Substring(9).Trim();
                    //    }
                    //    catch (Exception)
                    //    {
                    //        Chats.Insert(0, "Oops!");
                    //        return;
                    //    }
                    //    if (newName.Length > maxNameCharacters)
                    //        newName = newName.Substring(0, maxNameCharacters);


                    //    if (MainGame.containsProfanity(newName))
                    //    {
                    //        Chats.Insert(0, "Nope.");
                    //    }
                    //    else
                    //    {
                    //        Chats.Insert(0, "You have changed your name to " + newName);
                    //        MainGame.dm.player.Name = newName;
                    //        MainGame.dm.Mailman.SendName(newName);
                    //    }
                    //}
                    //else
                    {
                        string completeMessage = Draft.ToString();
                        MainGame.dm.Mailman.SendChat(completeMessage);
                        Chats.Insert(0, MainGame.dm.player.Name + ": " + completeMessage);
                    }

                    Draft.Clear();
                    Input.isActive = true;
                    ChatOpen = false;
                    ChatCloseDelay = 60 * 4;
                    Composing = false;
                }
            }

            Render.Draw(Content.GetTexture("crosshair.png"), mouse, Color.White, crosshairOrigin, 1, 0, 1 + CrosshairFireExpand);
            Render.Draw(Content.GetTexture("crosshairBars.png"), mouse, Color.White, crosshairOrigin, 1, 0, 1);
        }
    }
}
