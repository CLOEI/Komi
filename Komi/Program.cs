using ENet.Managed;
using Komi.lib;
using Komi.lib.types;
using Komi.lib.gui;

namespace Komi
{
    class Program
    {
        static void Main(string[] args)
        {
            var startupConfig = new ENetStartupOptions
            {
                ModulePath = Directory.GetCurrentDirectory() + "/enet.dll"
            };
            ManagedENet.Startup(startupConfig);
            var manager = new Manager();
            var config = new BotConfig()
            {
                Username = "mxgr",
                Password = "mxgr@finally1",
                LoginMethod = ELoginMethod.Legacy
            };
            manager.AddBot(config);

            Menu renderer = new Menu();
            renderer.Start().Wait();
        }
    }
}
