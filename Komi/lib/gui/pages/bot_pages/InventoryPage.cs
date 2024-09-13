using ImGuiNET;
using Komi.lib.bot;
using System.Numerics;

namespace Komi.lib.gui.pages.bot_pages
{
    internal class InventoryPage
    {
        public static void Render(Manager manager, Bot bot)
        {
            ImGui.Text($"Inventory Size: {bot.Inventory.ItemCount}/{bot.Inventory.Size}");
            if (ImGui.BeginChild("##inv", new Vector2(630, 330), ImGuiChildFlags.Border))
            {
                foreach (var items in bot.Inventory.Items.Values)
                {
                    ImGui.TextUnformatted($"{items.Amount}x | {bot.ItemDatabase.GetItem(items.Id).Name} [{items.Id}]");
                    ImGui.Separator();
                }
                ImGui.EndChild();
            }
        }
    }
}
