using ENet.Managed;
using Komi.lib.bot;
using Komi.lib.types;

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
        }
    }
}

