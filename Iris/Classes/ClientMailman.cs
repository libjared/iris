using Lidgren.Network;
using SFML.System;
using System;

namespace Iris
{
    public class ClientMailman
    {
        private NetClient client;
        private Deathmatch dm;
        public static string ip = "giga.krash.net";

        public ClientMailman(Deathmatch dm)
        {
            this.dm = dm;
        }

        public void Disconnect()
        {
            try
            {
                client.Disconnect("Bye"); //Throws an exception when exiting from the menu
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public bool Connect()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("bandit");
            config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            int port = 5635;
            client = new NetClient(config);
            client.Start();
            MainGame.dm.player.UID = MainGame.dm.Mailman.client.UniqueIdentifier;
            

            //start processing messages
            try
            {
                client.Connect(ip, port);
            }
            catch (Exception)
            {
                Console.WriteLine("Check your connection");
                return false;
            }
            return true;
        }

        public bool FullyConnected
        {
            get
            {
                return client.ConnectionsCount == 1;
            }
        }

        public void HandleMessages()
        {
            NetIncomingMessage msg;
            while ((msg = client.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        break;
                    case NetIncomingMessageType.ConnectionLatencyUpdated:
                        break;
                    case NetIncomingMessageType.Data:
                        //multiple game messages in a single packet
                        if (msg.PeekString() == "MULTI_ON")
                        {
                            //consume start marker
                            msg.ReadString();
                            //read until end marker is reached
                            while (msg.PeekString() != "MULTI_OFF")
                            {
                                HandleAGameMessage(msg);
                            }
                        }
                        else //regular single message
                        {
                            HandleAGameMessage(msg);
                        }
                        break;
                    default:
                        Console.WriteLine("Unrecognized Lidgren Message Recieved: {0}", msg.MessageType);
                        break;
                }
                client.Recycle(msg);
            }
        }

        private void HandleAGameMessage(NetIncomingMessage msg)
        {
            string messageType = msg.ReadString();

            switch (messageType)
            {
                case "LIFE":
                    long UID_LIFE = msg.ReadInt64();
                    int hp = msg.ReadInt32();
                    HandleLifeMessage(UID_LIFE, hp);
                    break;

                case "NAME":
                    long UID_NAME = msg.ReadInt64();
                    string newName = msg.ReadString();
                    HandleNameMessage(UID_NAME, newName);
                    break;

                case "POS": //Update a player's position
                    long UID_POS = msg.ReadInt64();
                    float xPos = msg.ReadFloat();
                    float yPos = msg.ReadFloat();
                    int facing = msg.ReadInt32();
                    float aimAngle = msg.ReadFloat();
                    HandlePosMessage(UID_POS, xPos, yPos, facing, aimAngle);
                    break;

                case "JOIN": //Add a player
                    long UID_JOIN = msg.ReadInt64();
                    HandleJoinMessage(UID_JOIN);
                    break;

                case "CHAT": //Add chat
                    long UID_CHAT = msg.ReadInt64();
                    string message = msg.ReadString();
                    HandleChatMessage(UID_CHAT, message);
                    break;

                case "PART": //Remove a player
                    long UID_PART = msg.ReadInt64();
                    HandlePartMessage(UID_PART);
                    break;

                case "INFO": //Recieved when server has completed sending all newbie initialization
                    break;
                case "BULLET":
                    long UID_BULLET = msg.ReadInt64();
                    float xBULLET = msg.ReadFloat();
                    float yBULLET = msg.ReadFloat();
                    float BULLETangle = msg.ReadFloat();
                    Bullet b = new Bullet(UID_BULLET, BULLETangle, new Vector2f(xBULLET, yBULLET));
                    HandleBulletCreate(b, UID_BULLET);
                    break;
                case "RESPAWN":
                    long UID_RESPAWN = msg.ReadInt64();
                    HandleRespawnMessage(UID_RESPAWN);
                    break;
                case "COIN":
                    //long UID_COIN = msg.ReadInt64();
                    float xCOIN = msg.ReadFloat();
                    float yCOIN = msg.ReadFloat();
                    int countCOIN = msg.ReadInt32();

                    HandleCoinCreate(countCOIN, xCOIN, yCOIN);

                    break;
                case "SWITCHWEAPON":
                    long UID_WEPSWITCH = msg.ReadInt64();
                    int WEAPONID = msg.ReadInt32();
                    HandleNetPlayerWeaponSwitch(UID_WEPSWITCH, WEAPONID);
                    break;
                case "KILLER":
                    long UID_VICTIM = msg.ReadInt64();
                    long UID_KILLER = msg.ReadInt64();
                    HandleKillerMessage(UID_VICTIM, UID_KILLER);
                    break;
                case "LOOT":
                    Console.WriteLine("LOOT Recieved");
                    int LOOTseed = msg.ReadInt32();
                    Console.WriteLine(LOOTseed);
                    Random rand = new Random(LOOTseed);
                    MainGame.dm.GameObjects.Add(new TreasureBox(new Vector2f(rand.Next(40,1600), 180)));
                    break;
                case "EMOTE":
                    long UID_EMOTE = msg.ReadInt64();
                    string EMOTE_TYPE = msg.ReadString();
                    MainGame.dm.GameObjects.Add(new EmoteBubble(EMOTE_TYPE, MainGame.dm.GetPlayerWithUID(UID_EMOTE)));;
                    break;
                case "BOMB":
                    long UID_EXPLOSIVE = msg.ReadInt64();
                    float xEXP = msg.ReadFloat();
                    float yEXP = msg.ReadFloat();
                    float EXPangle = msg.ReadFloat();
                    BombInstance bomb = new BombInstance(UID_EXPLOSIVE, EXPangle, new Vector2f(xEXP, yEXP));
                    HandleBombCreate(bomb, UID_EXPLOSIVE);
                    break;
                case "TIME":
                    float TIME_MESSAGE = msg.ReadFloat();
                    HandleTimeMessage(TIME_MESSAGE);
                    break;
                default:
                    Console.WriteLine("Unrecognized Game Message Recieved: {0}\n{1}", msg.ToString(), messageType);
                    break;
            }
        }

        private void HandleLifeMessage(long uid, int health)
        {
            if (dm.GetPlayerWithUID(uid) != null)
                dm.GetPlayerWithUID(uid).Health = health;
        }

        private void HandleNameMessage(long uid, string newName)
        {
            Actor who = dm.GetPlayerWithUID(uid);
            if (who != null)
            {
                string oldName = who.Name;
                who.Name = newName;
                Gui.Chats.Insert(0, oldName + " has changed their name to " + newName);
            }

        }

        private void HandlePosMessage(long uid, float x, float y, int facing, float aimAngle)
        {
            Actor plr = dm.GetPlayerWithUID(uid);
            
            if (plr != null) //stale POS message, player is already gone?
            {
                //Console.WriteLine(plr.Name);
                plr.Pos = new Vector2f(x, y);
                plr.Facing = facing;
                plr.AimAngle = aimAngle;

                plr.UpdateCollisionBox();
            }
        }

        private void HandleBulletCreate(Bullet b, long uid)
        {
            Actor shooter = ((Actor)dm.GetPlayerWithUID(uid));
            MainGame.dm.Projectiles.Add(b);

            MainGame.dm.GameObjects.Add(new GunSmoke(shooter.Core + Helper.PolarToVector2(32, shooter.AimAngle, 1, 1) + (shooter.Velocity), shooter.AimAngle));
            MainGame.dm.GameObjects.Add(new GunFlash(shooter.Core + Helper.PolarToVector2(32, shooter.AimAngle, 1, 1) + (shooter.Velocity), shooter.AimAngle));
        }

        private void HandleTimeMessage(float TIME_MESSAGE)
        {
            MainGame.dm.roundTimeLeft = (float)((int)((60 * 3) - (TIME_MESSAGE % (60 * 3))));
        }


        private void HandleBombCreate(BombInstance b, long uid)
        {
            Actor shooter = ((Actor)dm.GetPlayerWithUID(uid));
            MainGame.dm.Projectiles.Add(b);
        }

        private void HandleCoinCreate(int countCOIN, float xCOIN, float yCOIN)
        {
            Random r = new Random(countCOIN);

            for (int i = 0; i < countCOIN; i++)
            {
                Coin c = new Coin(new Vector2f(xCOIN, yCOIN), 2.5f + (float)r.NextDouble() * 1.5f, (float)
                (-Math.PI / 2 + .35 * (r.NextDouble() - .5)));
                dm.GameObjects.Add(c);
            }
        }

        private void HandleChatMessage(long uid, string message)
        {
            Actor who = dm.GetPlayerWithUID(uid);
            if (who != null)
            {
                Gui.Chats.Insert(0, who.Name + ": " + message);
                Gui.ChatCloseDelay = 300;
            }
        }

        private void HandleRespawnMessage(long uid)
        {
            dm.Players.Add(new NetPlayer(dm, uid));
        }

        private void HandleKillerMessage(long victimUID, long killerUID)
        {
            Console.WriteLine("Killer UID: " + killerUID);
            Console.WriteLine("VICTIM UID: " + victimUID);
            

            Actor killer = MainGame.dm.GetPlayerWithUID(killerUID);
            Actor victim = MainGame.dm.GetPlayerWithUID(victimUID);
            string killerName = killer != null ? killer.Name : "Universe";
            string victimName = victim != null ? victim.Name : "Universe";
            Gui.FragTexts.Add(new FragText(killerName, victimName,
                Content.GetTexture("skullIcon.png")));

            Gui.Chats.Insert(0, "[Server] Killer UID: " + killerName);
            Gui.Chats.Insert(0, "[Server] Player UID: " + victimName);
        }

        private void HandleJoinMessage(long uid)
        {
            dm.Players.Add(new NetPlayer(dm, uid));

            Gui.Chats.Insert(0, "[Server]: " + Math.Abs(uid).ToString().Substring(0, 4) + " has connected");
            Gui.ChatCloseDelay = 200;
        }

        private void HandlePartMessage(long uid)
        {
            dm.Players.Remove(dm.GetPlayerWithUID(uid));

            Gui.Chats.Insert(0, "[Server]: " + Math.Abs(uid).ToString().Substring(0, 4) + " has disconnected");
            Gui.ChatCloseDelay = 200;
        }

        public void SendPlayerPosMessage(long uid, Vector2f pos, int facing, float aimAngle)
        {
            NetOutgoingMessage outGoingMessage = client.CreateMessage();
            outGoingMessage.Write("POS");
            outGoingMessage.Write(pos.X);
            outGoingMessage.Write(pos.Y);
            outGoingMessage.Write(facing);
            outGoingMessage.Write(aimAngle);
            client.SendMessage(outGoingMessage, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendBulletCreate(Bullet b)
        {
            NetOutgoingMessage outGoingMessage = client.CreateMessage();
            outGoingMessage.Write("BULLET");
            outGoingMessage.Write(b.Pos.X);
            outGoingMessage.Write(b.Pos.Y);
            outGoingMessage.Write(b.Angle);
            client.SendMessage(outGoingMessage, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendBombCreate(BombInstance b)
        {
            NetOutgoingMessage outGoingMessage = client.CreateMessage();
            outGoingMessage.Write("BOMB");
            outGoingMessage.Write(b.Pos.X);
            outGoingMessage.Write(b.Pos.Y);
            outGoingMessage.Write(b.Angle);
            client.SendMessage(outGoingMessage, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendCoinCreate(Vector2f pos, int count)
        {
            NetOutgoingMessage outGoingMessage = client.CreateMessage();
            outGoingMessage.Write("COIN");
            outGoingMessage.Write(pos.X);
            outGoingMessage.Write(pos.Y);
            outGoingMessage.Write(count);
            client.SendMessage(outGoingMessage, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendHealth(int life)
        {
            NetOutgoingMessage outGoingMessage = client.CreateMessage();
            outGoingMessage.Write("LIFE");
            outGoingMessage.Write(life);
            client.SendMessage(outGoingMessage, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendRespawn(long uid)
        {
            NetOutgoingMessage outGoingMessage = client.CreateMessage();
            outGoingMessage.Write("RESPAWN");
            client.SendMessage(outGoingMessage, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendKillerMessage(long killerUID)
        {
            NetOutgoingMessage outGoingMessage = client.CreateMessage();
            outGoingMessage.Write("KILLER");
            outGoingMessage.Write(killerUID);
            client.SendMessage(outGoingMessage, NetDeliveryMethod.ReliableOrdered);

            Console.WriteLine(killerUID);
            Console.WriteLine(MainGame.dm.player.UID);
        }

        public void SendName(string name)
        {
            NetOutgoingMessage outGoingMessage = client.CreateMessage();
            outGoingMessage.Write("NAME");
            outGoingMessage.Write(name);
            client.SendMessage(outGoingMessage, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendModel(string modelName)
        {
            NetOutgoingMessage outGoingMessage = client.CreateMessage();
            outGoingMessage.Write("MODEL");
            outGoingMessage.Write(modelName);
            client.SendMessage(outGoingMessage, NetDeliveryMethod.ReliableOrdered);
        }


        public void SendChat(string message)
        {
            NetOutgoingMessage outGoingMessage = client.CreateMessage();
            outGoingMessage.Write("CHAT");
            outGoingMessage.Write(message);
            client.SendMessage(outGoingMessage, NetDeliveryMethod.ReliableOrdered);
        }

        internal void sendWeaponSwitch(int p)
        {
            NetOutgoingMessage outGoingMessage = client.CreateMessage();
            outGoingMessage.Write("SWITCHWEAPON");
            outGoingMessage.Write(p);
            client.SendMessage(outGoingMessage, NetDeliveryMethod.ReliableOrdered);
        }

        internal void HandleNetPlayerWeaponSwitch(long UID, int index)
        {
            NetPlayer np = MainGame.dm.GetPlayerWithUID(UID) as NetPlayer;
            np.weaponIndex = index;
        }

        internal void SendEmote(string e)
        {
            NetOutgoingMessage outGoingMessage = client.CreateMessage();
            outGoingMessage.Write("EMOTE");
            outGoingMessage.Write(e);
            client.SendMessage(outGoingMessage, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
