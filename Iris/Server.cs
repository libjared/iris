using Lidgren.Network;
using System;
using System.Threading;

namespace Iris
{
    class Server
    {
        const int MS_BETWEEN = 1600;
        const int QUIT_DELAY = 100;
        private NetServer server;
        private bool quit = false;
        private Timer timer;

        public Server()
        {
            NetPeerConfiguration cfg = new NetPeerConfiguration("bandit");
            cfg.Port = 5635;
            server = new NetServer(cfg);
        }

        public void Start()
        {
            server.Start();

            timer = new Timer(EveryFrame, null, MS_BETWEEN, Timeout.Infinite);

            //block until quit
            while (!quit)
            {
                Thread.Sleep(QUIT_DELAY);
            }

            Console.WriteLine("Exiting");
        }

        public void EveryFrame(object state)
        {
            Console.WriteLine("Every frame");

            if (!quit)
            {
                timer.Change(MS_BETWEEN, Timeout.Infinite);
            }
        }
    }
}
