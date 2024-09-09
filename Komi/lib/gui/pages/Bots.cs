using ImGuiNET;

namespace Komi.lib.gui.pages
{
    internal class Bots
    {
        private static string _selectedBot = utils.Config.GetSelectedBot();
        private static string _worldName = "";
        public static void Render(Manager manager)
        {
            ImGui.Columns(2, "bots", false);
            ImGui.SetColumnWidth(0, 150);
            ImGui.BeginChild("scroll");

            var bots = utils.Config.GetBots();
            foreach (var bot in bots)
            {
                var payload = utils.TextParse.ParseAndStoreAsList(bot.Payload);
                if (ImGui.Selectable(payload[0]))
                {
                    _selectedBot = payload[0];
                    utils.Config.EditSelectedBot(_selectedBot);
                }
            }

            ImGui.EndChild();
            ImGui.NextColumn();
            ImGui.BeginChild("info");

            if (!string.IsNullOrEmpty(_selectedBot))
            {
                var bot = manager.GetBot(_selectedBot);
                if (bot != null)
                {
                    ImGui.Text($"Display name: {bot.Info.LoginInfo.TankIdName}");
                    ImGui.Text($"Status: {bot.Info.Status}");
                    ImGui.SameLine();
                    ImGui.Text($"| Timeout: {bot.Info.Timeout}");
                    ImGui.Text($"Token: {bot.Info.Token}");
                    ImGui.Text($"World: {bot.World.Name}");
                    ImGui.Text($"Position: {(int)(bot.Position.X / 32)}, {(int)(bot.Position.Y / 32)}");
                    ImGui.Text($"Ping: {bot.Info.Ping}");
                    ImGui.Text($"RID: {bot.Info.LoginInfo.Rid}");

                    if (ImGui.Button("Reconnect"))
                    {
                        // bot.Reconnect();
                    }
                    
                    ImGui.InputText("World name", ref _worldName, 10);
                    if (ImGui.Button("Warp"))
                    {
                        Thread thread = new Thread(() => bot.Warp(_worldName))
                        {
                            IsBackground = true
                        };
                        thread.Start();
                    }
                }
            }

            ImGui.EndChild();
            ImGui.Columns(1);
        }
    }
}
