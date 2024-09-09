﻿using System.Text;
using System.Text.Json;
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
            InitConfig();
            var startupConfig = new ENetStartupOptions
            {
                ModulePath = Directory.GetCurrentDirectory() + "/enet.dll"
            };
            ManagedENet.Startup(startupConfig);
            var manager = new Manager();
            var bots = lib.utils.Config.GetBots();
            foreach (var bot in bots)
            {
                manager.AddBot(bot);
            }

            Menu renderer = new Menu();
            renderer.Start().Wait();
        }

        static void InitConfig()
        {
            if (File.Exists("config.json")) return;
            var config = new Config()
            {
                GameVersion = "4.64",
                Timeout = 5,
                FindPathDelay = 30,
                Bots = new List<BotConfig>(),
                SelectedBot = ""
            };

            var jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            using var writer = File.Create("config.json");
            writer.Write(Encoding.UTF8.GetBytes(jsonString));
        }
    }
}
