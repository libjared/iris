using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Iris.Server
{
    class Server
    {
        const int LOOT_INTERVAL = 1000 * 30;
        const int TIME_INTERVAL = 1000;
        const int FRAME_INTERVAL = 16;
        private NetServer server;
        private AutoResetEvent quitter = new AutoResetEvent(false);
        private Random rand = new Random();
        private DateTime gameStarted;
        private Timer lootTimer;
        private Timer timeTimer;
        private Timer frameTimer;

        //local data
        List<Player> dActors = new List<Player>();
        List<Generator> dGens = new List<Generator>();
        List<Landmine> dLands = new List<Landmine>();

        public Server()
        {
            NetPeerConfiguration cfg = new NetPeerConfiguration("bandit");
            cfg.Port = 5635;
            server = new NetServer(cfg);
            server.RegisterReceivedCallback(new SendOrPostCallback(GotLidgrenMessage), new SynchronizationContext());
            gameStarted = DateTime.Now;
            lootTimer = new Timer(o => PlaceLoot(), null, 0, LOOT_INTERVAL);
            timeTimer = new Timer(o => UpdateTime(), null, 0, TIME_INTERVAL);
            frameTimer = new Timer(o => EveryFrame(), null, 0, FRAME_INTERVAL);
        }

        public void Start()
        {
            server.Start();

            Console.WriteLine("Built at [{0}]", GetSelfBuildDate());
            Console.WriteLine("Welcome to Iris");

            //block until quit
            quitter.WaitOne();

            Console.WriteLine("Exiting");
        }

        private string GetSelfBuildDate()
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            string loc = asm.Location;

            var dateMod = System.IO.File.GetLastWriteTime(loc);
            return dateMod.ToString();
        }

        public void GotLidgrenMessage(object peer)
        {
            NetIncomingMessage msg;
            while ((msg = server.ReadMessage()) != null)
            {
                switch (msg.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Console.WriteLine(msg.ReadString());
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        var newStatus = (NetConnectionStatus)msg.ReadByte();
                        if (newStatus == NetConnectionStatus.Connected)
                        {
                            OnConnect(msg);
                        }
                        else if (newStatus == NetConnectionStatus.Disconnected)
                        {
                            OnDisconnect(msg);
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        HandleGameMessage(msg);
                        break;
                    default:
                        Console.WriteLine(string.Format("Unhandled type: {0}", msg.MessageType));
                        break;
                }
                server.Recycle(msg);
            }
        }

        private void PlaceLoot()
        {
            int seed = rand.Next(int.MinValue, int.MaxValue);

            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("LOOT");
            outMsg.Write(seed);
            server.SendToAll(outMsg, null, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void UpdateTime()
        {
            //tell every client the time
            //to account for latency, subtract the standard by rtt / 2
            float secondsSinceStart = (float)((DateTime.Now - gameStarted).TotalSeconds);
            foreach (NetConnection client in server.Connections)
            {
                float adjustedTime = secondsSinceStart - (client.AverageRoundtripTime / 2f);
                NetOutgoingMessage outMsg = server.CreateMessage();
                outMsg.Write("TIME");
                outMsg.Write(adjustedTime);
                server.SendMessage(outMsg, client, NetDeliveryMethod.ReliableOrdered);
            }
        }

        private void EveryFrame()
        {
            RemoveOldGenerators();
        }

        private void RemoveOldGenerators()
        {
            int killed = dGens.RemoveAll((o) =>
            {
                return DateTime.Now > o.AddedAt.AddMilliseconds(Generator.LifeInMS);
            });
            if (killed > 0) { Console.WriteLine("Killed {0} old generators from newbie info.", killed); }
        }

        private void HandleGameMessage(NetIncomingMessage msg)
        {
            string type = msg.ReadString();

            switch (type)
            {
                case "POS":
                    HandlePOS(msg);
                    break;
                case "LIFE":
                    HandleLIFE(msg);
                    break;
                case "NAME":
                    HandleNAME(msg);
                    break;
                case "BULLET":
                    HandleBULLET(msg);
                    break;
                case "RESPAWN":
                    HandleRESPAWN(msg);
                    break;
                case "COIN":
                    HandleCOIN(msg);
                    break;
                case "CHAT":
                    HandleCHAT(msg);
                    break;
                case "SWITCHWEAPON":
                    HandleSWITCHWEAPON(msg);
                    break;
                case "BOMB":
                    HandleBOMB(msg);
                    break;
                case "KILLER":
                    HandleKILLER(msg);
                    break;
                case "EMOTE":
                    HandleEMOTE(msg);
                    break;
                case "MODEL":
                    HandleMODEL(msg);
                    break;
                case "GOLDCOUNT":
                    HandleGOLDCOUNT(msg);
                    break;
                case "LANDMINE":
                    HandleLANDMINE(msg);
                    break;
                case "GENERATOR":
                    HandleGENERATOR(msg);
                    break;
                case "LANDMINE_TRIGGER":
                    HandleLANDMINE_TRIGGER(msg);
                    break;
                default:
                    Console.WriteLine(string.Format("Bad message type {0} from player {1}",
                        type, msg.SenderConnection.RemoteUniqueIdentifier));
                    break;
            }
        }

        private void HandleLANDMINE_TRIGGER(NetIncomingMessage msg)
        {
            int lmid = msg.ReadInt32();

            //kill object in list
            int killed = dLands.RemoveAll((o) => { return o.LMID == lmid; });
            Console.WriteLine("Killed {0} mines by blowing them up.", killed);

            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("LANDMINE_TRIGGER");
            outMsg.Write(lmid);
            server.SendToAll(outMsg, null, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleGENERATOR(NetIncomingMessage msg)
        {
            long owner = msg.SenderConnection.RemoteUniqueIdentifier;

            float x = msg.ReadFloat();
            float y = msg.ReadFloat();
            int type = msg.ReadInt32();

            Console.WriteLine("GENERATOR: {0},{1} of type {2}", x, y, type);

            //save it
            dGens.Add(new Generator()
            {
                AddedAt = DateTime.Now,
                Owner = owner,
                Type = type,
                X = x,
                Y = y
            });

            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("GENERATOR");
            outMsg.Write(owner);
            outMsg.Write(x);
            outMsg.Write(y);
            outMsg.Write(type);
            outMsg.Write(DateTime.Now.Ticks);
            server.SendToAll(outMsg, null, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleLANDMINE(NetIncomingMessage msg)
        {
            long owner = msg.SenderConnection.RemoteUniqueIdentifier;

            float x = msg.ReadFloat();
            float y = msg.ReadFloat();
            int lmid = msg.ReadInt32();

            Console.WriteLine("LANDMINE: {0},{1} with id {2}", x, y, lmid);

            //save it
            dLands.Add(new Landmine()
            {
                Owner = owner,
                X = x,
                Y = y,
                LMID = lmid
            });

            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("LANDMINE");
            outMsg.Write(owner);
            outMsg.Write(x);
            outMsg.Write(y);
            outMsg.Write(lmid);
            server.SendToAll(outMsg, null, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleMODEL(NetIncomingMessage msg)
        {
            long owner = msg.SenderConnection.RemoteUniqueIdentifier;
            string model = msg.ReadString();

            //save
            GetPlayerFromUID(msg.SenderConnection.RemoteUniqueIdentifier).ModelName = model;

            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("MODEL");
            outMsg.Write(owner);
            outMsg.Write(model);
            server.SendToAll(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleKILLER(NetIncomingMessage msg)
        {
            long owner = msg.SenderConnection.RemoteUniqueIdentifier;
            long victim = msg.ReadInt64();

            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("KILLER");
            outMsg.Write(owner);
            outMsg.Write(victim);
            server.SendToAll(outMsg, null, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleEMOTE(NetIncomingMessage msg)
        {
            long owner = msg.SenderConnection.RemoteUniqueIdentifier;
            string emoteType = msg.ReadString();

            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("EMOTE");
            outMsg.Write(owner);
            outMsg.Write(emoteType);
            server.SendToAll(outMsg, null, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleGOLDCOUNT(NetIncomingMessage msg)
        {
            long owner = msg.SenderConnection.RemoteUniqueIdentifier;
            int goldCount = msg.ReadInt32();

            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("GOLDCOUNT");
            outMsg.Write(owner);
            outMsg.Write(goldCount);
            server.SendToAll(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleBOMB(NetIncomingMessage msg)
        {
            long owner = msg.SenderConnection.RemoteUniqueIdentifier;
            float x = msg.ReadFloat();
            float y = msg.ReadFloat();
            float angle = msg.ReadFloat();

            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("BOMB");
            outMsg.Write(owner);
            outMsg.Write(x);
            outMsg.Write(y);
            outMsg.Write(angle);
            server.SendToAll(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleSWITCHWEAPON(NetIncomingMessage msg)
        {
            long who = msg.SenderConnection.RemoteUniqueIdentifier;
            int wep = msg.ReadInt32();

            string hisName = GetPlayerFromUID(who).Name;
            Console.WriteLine(string.Format("SWITCHWEAPON {0}: {1}", hisName, wep));

            //TODO: newbie

            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("SWITCHWEAPON");
            outMsg.Write(who);
            outMsg.Write(wep);
            server.SendToAll(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleCHAT(NetIncomingMessage msg)
        {
            long who = msg.SenderConnection.RemoteUniqueIdentifier;
            string text = msg.ReadString();

            string hisName = GetPlayerFromUID(who).Name;
            Console.WriteLine(string.Format("CHAT {0}: {1}", hisName, text));

            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("CHAT");
            outMsg.Write(who);
            outMsg.Write(text);
            server.SendToAll(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleCOIN(NetIncomingMessage msg)
        {
            //no uid
            float x = msg.ReadFloat();
            float y = msg.ReadFloat();
            int count = msg.ReadInt32();

            //TODO: newbie

            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("COIN");
            outMsg.Write(x);
            outMsg.Write(y);
            outMsg.Write(count);
            server.SendToAll(outMsg, null, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleRESPAWN(NetIncomingMessage msg)
        {
            long who = msg.SenderConnection.RemoteUniqueIdentifier;

            Console.WriteLine(string.Format("RESPAWN: {0}", who));

            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("RESPAWN");
            outMsg.Write(who);
            server.SendToAll(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleBULLET(NetIncomingMessage msg)
        {
            long owner = msg.SenderConnection.RemoteUniqueIdentifier;
            float x = msg.ReadFloat();
            float y = msg.ReadFloat();
            float angle = msg.ReadFloat();

            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("BULLET");
            outMsg.Write(owner);
            outMsg.Write(x);
            outMsg.Write(y);
            outMsg.Write(angle);
            server.SendToAll(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleNAME(NetIncomingMessage msg)
        {
            string newName = msg.ReadString();
            string oldName = GetPlayerFromUID(msg.SenderConnection.RemoteUniqueIdentifier).Name;
            Console.WriteLine(string.Format("NAME: {0} changed {1}", oldName, newName));

            //save name in dict
            GetPlayerFromUID(msg.SenderConnection.RemoteUniqueIdentifier).Name = newName;

            //inform ALL clients about his name change
            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("NAME");
            outMsg.Write(msg.SenderConnection.RemoteUniqueIdentifier);
            outMsg.Write(newName);
            server.SendToAll(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleLIFE(NetIncomingMessage msg)
        {
            int newHp = msg.ReadInt32();
            Console.WriteLine(string.Format("LIFE: {0}: {1}", msg.SenderConnection.RemoteUniqueIdentifier, newHp));

            //save value
            GetPlayerFromUID(msg.SenderConnection.RemoteUniqueIdentifier).Life = newHp;

            //inform ALL clients about his pining for the fjords
            NetOutgoingMessage outMsgLife = server.CreateMessage();
            outMsgLife.Write("LIFE");
            outMsgLife.Write(msg.SenderConnection.RemoteUniqueIdentifier);
            outMsgLife.Write(newHp);
            server.SendToAll(outMsgLife, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandlePOS(NetIncomingMessage msg)
        {
            float newX = msg.ReadFloat();
            float newY = msg.ReadFloat();
            int facing = msg.ReadInt32();
            float angle = msg.ReadFloat();

            //inform ALL clients about position change
            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("POS");
            outMsg.Write(msg.SenderConnection.RemoteUniqueIdentifier);
            outMsg.Write(newX);
            outMsg.Write(newY);
            outMsg.Write(facing);
            outMsg.Write(angle);
            server.SendToAll(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableUnordered, 0);
        }

        private Player GetPlayerFromUID(long remoteUniqueIdentifier)
        {
            return dActors.FirstOrDefault(p => p.UID == remoteUniqueIdentifier);
        }

        private void OnDisconnect(NetIncomingMessage msg)
        {
            NetOutgoingMessage outMsg = server.CreateMessage();
            outMsg.Write("PART");
            outMsg.Write(msg.SenderConnection.RemoteUniqueIdentifier);

            server.SendToAll(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
            Console.WriteLine(string.Format("PART: {0}", msg.SenderConnection.RemoteUniqueIdentifier));

            //remove datas
            dActors.Remove((Player)msg.SenderConnection.Tag);
        }

        private void OnConnect(NetIncomingMessage msg)
        {
            //tell everyone else he joined
            {
                NetOutgoingMessage outMsg = server.CreateMessage();
                outMsg.Write("JOIN");
                outMsg.Write(msg.SenderConnection.RemoteUniqueIdentifier);
                server.SendToAll(outMsg, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
            }

            Console.WriteLine(string.Format("JOIN: {0}", msg.SenderConnection.RemoteUniqueIdentifier));

            InformNewbieState(msg);

            //intial data finished sending; add him to the player list, tag his Player for easy access
            Player thisPlayer = new Player();
            thisPlayer.UID = msg.SenderConnection.RemoteUniqueIdentifier;
            dActors.Add(thisPlayer);
            msg.SenderConnection.Tag = thisPlayer;
        }

        private void InformNewbieState(NetIncomingMessage msg)
        {
            NetOutgoingMessage newbieState = server.CreateMessage();

            newbieState.Write("MULTI_ON");

            foreach (var actor in dActors) //not using server.Connections
            {
                Player plr = (Player)actor;
                newbieState.Write("JOIN");
                newbieState.Write(plr.UID);

                newbieState.Write("NAME");
                newbieState.Write(plr.UID);
                newbieState.Write(plr.Name);

                newbieState.Write("LIFE");
                newbieState.Write(plr.UID);
                newbieState.Write(plr.Life);

                newbieState.Write("MODEL");
                newbieState.Write(plr.UID);
                newbieState.Write(plr.ModelName);
            }

            foreach (var lm in dLands)
            {
                newbieState.Write("LANDMINE");
                newbieState.Write(lm.Owner);
                newbieState.Write(lm.X);
                newbieState.Write(lm.Y);
                newbieState.Write(lm.LMID);
            }

            foreach (var gen in dGens)
            {
                newbieState.Write("GENERATOR");
                newbieState.Write(gen.Owner);
                newbieState.Write(gen.X);
                newbieState.Write(gen.Y);
                newbieState.Write(gen.Type);
                newbieState.Write(gen.AddedAt.Ticks);
            }

            newbieState.Write("MULTI_OFF");

            server.SendMessage(newbieState, msg.SenderConnection, NetDeliveryMethod.ReliableOrdered);
        }
    }

    public class Player
    {
        public long UID { get; set; }
        public string Name { get; set; }
        public int Life { get; set; }
        public string ModelName { get; set; }

        public Player()
        {
            Name = "UNNAMED!";
            Life = 100;
        }
    }

    public class Generator
    {
        public const int LifeInMS = 23300;
        public long Owner { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int Type { get; set; }
        public DateTime AddedAt { get; set; }
    }

    public class Landmine
    {
        public long Owner { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public int LMID { get; set; }
    }
}
