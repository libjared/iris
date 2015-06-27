using System;

namespace Iris
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length >= 1 && args[0] == "server")
            {
                Server s = new Server();
                s.Start();
                return;
            }

            using (var game = new MainGame())
                game.Run();
        }
    }
}
