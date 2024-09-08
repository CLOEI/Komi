using ENet.Managed;
using Komi.lib;
using Komi.lib.bot;
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

            Menu renderer = new Menu();
            renderer.Start().Wait();
        }
    }
}
