using ImGuiNET;
using Komi.lib.bot;
using System.Numerics;

namespace Komi.lib.gui.pages.bot_tabs
{
    internal class WorldPage
    {
        public static void Render(Manager manager, Bot bot)
        {
            var draw_list = ImGui.GetWindowDrawList();
            var p = ImGui.GetCursorScreenPos();
            var size = ImGui.GetContentRegionAvail();
            var min = new Vector2(p.X, p.Y);
            var max = new Vector2(p.X + size.X, p.Y + size.Y);

            draw_list.AddRectFilled(min, max, ImGui.GetColorU32(new Vector4(1.0f, 1.0f, 1.0f, 1.0f)));

            var world = bot.World;
            if (world != null)
            {
                var cellWidth = size.X / world.Width;
                var cellHeight = size.Y / world.Height;

                for (int y = 0; y < world.Height; y++)
                {
                    for (int x = 0; x < world.Width; x++)
                    {
                        var cellMin = new Vector2(min.X + x * cellWidth, min.Y + y * cellHeight);
                        var cellMax = new Vector2(cellMin.X + cellWidth, cellMin.Y + cellHeight);

                        if (y * world.Width + x >= world.TileCount)
                        {
                            draw_list.AddRectFilled(cellMin, cellMax, ImGui.GetColorU32(new Vector4(1.0f, 0.843f, 0.0f, 1.0f)));
                            continue;
                        }

                        var tile = world.GetTile((uint)x, (uint)y);
                        var item = bot.ItemDatabase.GetItem((uint)tile.ForegroundItemId);
                        var color = bot.ItemDatabase.GetItem((uint)(tile.ForegroundItemId + 1)).OverlayColor;

                        var r = ((color >> 24) & 0xFF) / 255.0f;
                        var g = ((color >> 16) & 0xFF) / 255.0f;
                        var b = ((color >> 8) & 0xFF) / 255.0f;
                        var a = (color & 0xFF) / 255.0f;

                        draw_list.AddRectFilled(cellMin, cellMax, ImGui.GetColorU32(new Vector4(r, g, b, a)));


                        var botPosition = bot.Position;
                        if (Math.Abs(botPosition.X / 32.0f - x) < 0.01f && Math.Abs(botPosition.Y / 32.0f - y) < 0.01f)
                        {
                            draw_list.AddRectFilled(cellMin, cellMax, ImGui.GetColorU32(new Vector4(1.0f, 0.0f, 0.0f, 1.0f)));
                        }

                        var io = ImGui.GetIO();
                        if (ImGui.IsMouseHoveringRect(cellMin, cellMax))
                        {
                            ImGui.BeginTooltip();
                            ImGui.Text($"Position: {x}|{y}");
                            ImGui.Text($"Item name: {item.Name}");
                            ImGui.Text($"Collision type: {item.CollisionType}");
                            ImGui.EndTooltip();

                            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                            {
                                Console.WriteLine($"Clicked on tile: {x}|{y}");
                            }
                        }
                    }
                }
            }
        }
    }
}
