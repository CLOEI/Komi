using ImGuiNET;
using Komi.lib.gui.pages.bot_tabs;

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
                    if (ImGui.BeginTabBar("BotTab"))
                    {
                        if (ImGui.BeginTabItem("Info"))
                        {
                            InfoPage.Render(manager, bot);
                            ImGui.EndTabItem();
                        }

                        if (ImGui.BeginTabItem("World"))
                        {
                            WorldPage.Render(manager, bot);
                            ImGui.EndTabItem();
                        }

                        if (ImGui.BeginTabItem("Inventory"))
                        {
                            InventoryPage.Render(manager, bot);
                            ImGui.EndTabItem();
                        }

                        ImGui.EndTabBar();
                    }
                }
            }

            ImGui.EndChild();
            ImGui.Columns(1);
        }
    }
}
