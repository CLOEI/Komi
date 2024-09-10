using ImGuiNET;
using Komi.lib.bot;

namespace Komi.lib.gui.pages.bot_pages
{
    internal class InfoPage
    {
        private static string _worldName = "";

        public static void Render(Manager manager, Bot bot)
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

            ImGui.InputText("##23414", ref _worldName, 24);
            ImGui.SameLine();
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
}
