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
                    HandlePosMessage(UID_POS, xPos, yPos);
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

                default:
                    Console.WriteLine("Unrecognized Game Message Recieved: {0}\n{1}", msg.ToString(), messageType);
                    break;
            }
        }

        private void HandleLifeMessage(long uid, int health)
        {
            dm.GetPlayerWithUID(uid).Health = health;
        }

        private void HandleNameMessage(long uid, string newName)
        {
            dm.GetPlayerWithUID(uid).Name = newName;
        }

        private void HandlePosMessage(long uid, float x, float y)
        {
            Player plr = dm.GetPlayerWithUID(uid);
            if (plr != null) //stale POS message, player is already gone?
            {
                plr.Position = new Vector2f(x, y);
            }
        }

        private void HandleChatMessage(long uid, string message)
        {
            //TODO: add gui chat
            Player p = dm.GetPlayerWithUID(uid);
        }

        private void HandleJoinMessage(long uid)
        {
            dm.Players.Add(new NetPlayer(dm, uid));
        }

        private void HandlePartMessage(long uid)
        {
            dm.Players.Remove(dm.GetPlayerWithUID(uid));
        }

        public void SendPlayerPosMessage(long uid, Vector2f pos)
        {
            NetOutgoingMessage outGoingMessage = client.CreateMessage();
            outGoingMessage.Write("POS");
            outGoingMessage.Write(pos.X);
            outGoingMessage.Write(pos.Y);
            client.SendMessage(outGoingMessage, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
