using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Iris
{
    public class Menu : Gamestate
    {
        private static RenderStates shader;
        public string usernameField = "Username";
        public string ipField = "Server IP";

        public bool composingUsername = false;
        public bool composingIP = false;

        public float trainPosX = -250;

        public StringBuilder activeField = new StringBuilder("");

        private RectangleShape rectConnect;
        private RectangleShape rectIP;
        private RectangleShape rectUsername;
        private bool submitted;

        public Menu()
            : base()
        {
            shader = new RenderStates(new Shader(null, "Content/bgPrlx.frag"));

            rectConnect = new RectangleShape()
            {
                Size = new Vector2f(150, 30),
                Position = new Vector2f(-25, 70)
            };
            rectIP = new RectangleShape()
            {
                Size = new Vector2f(150, 20),
                Position = new Vector2f(-25, 40)
            };
            rectUsername = new RectangleShape()
            {
                Size = new Vector2f(150, 20),
                Position = new Vector2f(-25, 10)
            };

            MainGame.window.TextEntered += TextEnteredEvent;
        }

        public void TextEnteredEvent(Object sender, TextEventArgs e)
        {
            if (Keyboard.IsKeyPressed(Keyboard.Key.BackSpace))
            {
                if (activeField.Length > 0)
                {
                    activeField = activeField.Remove(activeField.Length - 1, 1);
                    MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("dryFireSfx.wav"), 1, 0, 1));
                }
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Return))
            {
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.LControl))
            {
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
            {
            }
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Tab))
            {
            }
            else
            {
                if (activeField.Length < 20)
                {
                    activeField.Append(e.Unicode);
                    MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("dryFireSfx.wav"), 1, 0, 1));
                }
            }
        }

        public override void Update()
        {
            //move the train
            trainPosX += .7f;
            if (trainPosX > 400)
                trainPosX = -250f;

            if (!submitted)
            {
                UpdateMenuGui();
            } else if (MainGame.dm.Mailman.FullyConnected)
            {
                MainGame.window.TextEntered -= TextEnteredEvent;
                MainGame.dm.player.Name = usernameField;
                MainGame.gamestate = MainGame.dm;
            }
        }

        private void UpdateMenuGui()
        {
            //handle paste
            if (Clipboard.ContainsText())
            {
                if (Input.isKeyDown(Keyboard.Key.LControl) && Input.isKeyTap(Keyboard.Key.V))
                {
                    activeField.Append(Clipboard.GetText());
                }
            }

            //max length for any active field
            if (activeField.Length > 20)
            {
                activeField.Remove(20, activeField.Length - 20);
            }

            //tab switching
            if (Input.isKeyTap(Keyboard.Key.Tab))
            {
                if (composingUsername)
                {
                    MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("click.wav"), 1, 0, 5));
                    composingUsername = false;
                    composingIP = true;
                }
                else if (composingIP)
                {
                    MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("click.wav"), 1, 0, 5));
                    composingUsername = false;
                    composingIP = false;
                }

                activeField.Clear();
            }

            //update the textboxes display
            if (composingIP)
                ipField = activeField.ToString();
            if (composingUsername)
                usernameField = activeField.ToString();

            //reset click
            if (Input.isMouseButtonTap(Mouse.Button.Left))
            {
                composingUsername = false;
                composingIP = false;
                if (usernameField.Trim().Equals(""))
                    usernameField = "Username";
                if (ipField.Trim().Equals(""))
                    ipField = "Server IP";
                activeField.Clear();

                MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("click.wav"), 1, 0, 5));

                //click to activate username textbox
                if (rectUsername.GetGlobalBounds().Contains(MainGame.worldMousePos.X, MainGame.worldMousePos.Y))
                {
                    composingUsername = true;
                    if (usernameField.Equals("Username"))
                    {
                        usernameField = "";
                    }
                }

                //else, click to activate ip textbox
                else if (rectIP.GetGlobalBounds().Contains(MainGame.worldMousePos.X, MainGame.worldMousePos.Y))
                {
                    composingIP = true;
                    if (ipField.Equals("Server IP"))
                    {
                        ipField = "";
                    }
                }

                //else, click to activate connect button
                else if (rectConnect.GetGlobalBounds().Contains(MainGame.worldMousePos.X, MainGame.worldMousePos.Y))
                {
                    if (BoxesValid)
                    {
                        Submit();
                    }
                }
            }
        }

        public override void Draw()
        {
            //blue sky
            MainGame.window.SetView(MainGame.window.DefaultView);
            shader.Shader.SetParameter("offsetY", MainGame.Camera.Center.Y);
            RectangleShape rs = new RectangleShape
            {
                Size = new Vector2f(800, 600)
            };
            MainGame.window.Draw(rs, shader);
            MainGame.window.SetView(MainGame.Camera);

            //background
            Render.Draw(Content.GetTexture("background1Far.png"), new Vector2f(-200, -100), Color.White, new Vector2f(0, 0), 1, 0f);
            Render.Draw(Content.GetTexture("background1Far.png"), new Vector2f(145, -100), Color.White, new Vector2f(0, 0), 1, 0f);
            Render.Draw(Content.GetTexture("background1.png"), new Vector2f(-200, -150), Color.White, new Vector2f(0, 0), 1, 0f);

            //tracks
            RectangleShape tracks = new RectangleShape(new Vector2f(800, .5f));
            tracks.Position = new Vector2f(-400, -49);
            tracks.FillColor = new Color(10, 10, 10, 50);
            tracks.Draw(MainGame.window, RenderStates.Default);

            //train
            Render.Draw(Content.GetTexture("mapDecor.png"), new Vector2f(trainPosX, -55), new Color(255, 255, 255, 200), new Vector2f(0, 0), 1, 0f, .03f);

            //title
            Render.Draw(Content.GetTexture("title.png"), new Vector2f(-50, -190), new Color(255, 255, 255, 240), new Vector2f(0, 0), 1, 0f, .4f);

            //menubox
            RectangleShape rectBG = new RectangleShape(new Vector2f(200, 110));
            rectBG.Position = new Vector2f(-50, 0);
            rectBG.FillColor = new Color(10, 10, 10, 100);
            rectBG.Draw(MainGame.window, RenderStates.Default);

            if (!submitted)
            {
                //menu username
                rectUsername.FillColor = new Color(10, 10, 10, (byte)(composingUsername ? 150 : 50));
                rectUsername.Draw(MainGame.window, RenderStates.Default);

                //menu ip
                rectIP.FillColor = new Color(10, 10, 10, (byte)(composingIP ? 150 : 50));
                rectIP.Draw(MainGame.window, RenderStates.Default);

                //menu connect button
                rectConnect.FillColor = new Color(10, 255, 10,
                    (byte)(rectConnect.GetGlobalBounds().Contains(
                    (int)MainGame.worldMousePos.X, (int)MainGame.worldMousePos.Y) ? 150 : 70));
                rectConnect.Draw(MainGame.window, RenderStates.Default);

                //text: username, ip, connect button
                Render.DrawString(Content.GetFont("OldNewspaperTypes.ttf"), usernameField, new Vector2f(50, 15), Color.White, .3f, true, 1);
                Render.DrawString(Content.GetFont("OldNewspaperTypes.ttf"), ipField, new Vector2f(50, 45), Color.White, .3f, true, 1);
                Render.DrawString(Content.GetFont("OldNewspaperTypes.ttf"), "Connect", new Vector2f(50, 77), Color.White, .4f, true, 1);
            }
            else
            {
                Render.DrawString(Content.GetFont("OldNewspaperTypes.ttf"), "Connecting...", new Vector2f(50, 15), Color.White, .3f, true, 1);
            }

            //cursor
            Render.Draw(Content.GetTexture("cursorPointer.png"), MainGame.worldMousePos, Color.White, new Vector2f(0, 0), 1, 0f);
        }

        public bool BoxesValid
        {
            get
            {
                return ipField.Trim() != "Server IP" && usernameField.Trim() != "Username";
            }
        }

        public void Submit()
        {
            ClientMailman.ip = ipField;

            if (MainGame.dm.Mailman.Connect())
            {
                submitted = true;
            }
            else
            {
                MainGame.soundInstances.Add(new SoundInstance(Content.GetSound("error.wav"), 1, 0, 10));
                ipField = "Failed to Connect";
            }
        }
    }
}
