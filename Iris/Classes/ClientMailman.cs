using Lidgren.Network;
using SFML.System;
using System;

namespace Iris
{
    public class ClientMailman
    {
        private NetClient client;
        private Deathmatch dm;

        public ClientMailman(Deathmatch dm)
        {
            this.dm = dm;
        }

        public void Disconnect()
        {
            client.Disconnect("Bye");
        }

        public void Connect()
        {
            NetPeerConfiguration config = new NetPeerConfiguration("bandit");
            config.EnableMessageType(NetIncomingMessageType.ConnectionLatencyUpdated);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            string ip = "giga.krash.net"; //Jared's IP
            int port = 5635;
            client = new NetClient(config);
            client.Start();

            //start processing messages
            client.Connect(ip, port);
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
                    MainGame.dm.Projectiles.Add(new Bullet(UID_BULLET, BULLETangle, new Vector2f(xBULLET, yBULLET), 6, 40)); //No damage yet
                    break;
                case "RESPAWN": 
                    long UID_RESPAWN = msg.ReadInt64();
                    HandleJoinMessage(UID_RESPAWN);
                    break;
                case "COIN":
                    long UID_COIN = msg.ReadInt64();
                    float xCOIN = msg.ReadFloat();
                    float yCOIN = msg.ReadFloat();
                    float COINangle = msg.ReadFloat();
                    float COINspeed = msg.ReadFloat();
                    MainGame.dm.GameObjects.Add(new Coin(new Vector2f(xCOIN, yCOIN), COINspeed, COINangle));
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
            dm.GetPlayerWithUID(uid).Name = newName;
        }

        private void HandlePosMessage(long uid, float x, float y, int facing, float aimAngle)
        {
            Actor plr = dm.GetPlayerWithUID(uid);
            if (plr != null) //stale POS message, player is already gone?
            {
                plr.Pos = new Vector2f(x, y);
                plr.Facing = facing;
                plr.AimAngle = aimAngle;
            }
        }

        private void HandleChatMessage(long uid, string message)
        {
            //TODO: add gui chat
            Actor p = dm.GetPlayerWithUID(uid);
        }

        private void HandleJoinMessage(long uid)
        {
            dm.Players.Add(new NetPlayer(dm, uid));
        }

        private void HandlePartMessage(long uid)
        {
            dm.Players.Remove(dm.GetPlayerWithUID(uid));
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

        public void SendCoinCreate(Coin c)
        {
            NetOutgoingMessage outGoingMessage = client.CreateMessage();
            outGoingMessage.Write("COIN");
            outGoingMessage.Write(c.Pos.X);
            outGoingMessage.Write(c.Pos.Y);
            outGoingMessage.Write(c.angle);
            outGoingMessage.Write(c.speed);
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
    }
}
