using ImGuiNET;
using Komi.lib.bot;
using System.Numerics;
using Komi.lib.bot.features;

namespace Komi.lib.gui.pages.bot_pages
{
    internal class WorldPage
    {
        public static void Render(Manager manager, Bot bot)
        {
            var drawList = ImGui.GetWindowDrawList();
            var p = ImGui.GetCursorScreenPos();
            var size = ImGui.GetContentRegionAvail();
            var min = new Vector2(p.X, p.Y);
            var max = new Vector2(p.X + size.X, p.Y + size.Y);

            drawList.AddRectFilled(min, max, ImGui.GetColorU32(new Vector4(1.0f, 1.0f, 1.0f, 1.0f)));

            var world = bot.World;
            if (world == null) return;
            var cellWidth = size.X / world.Width;
            var cellHeight = size.Y / world.Height;

            for (var y = 0; y < world.Height; y++)
            {
                for (var x = 0; x < world.Width; x++)
                {
                    var cellMin = new Vector2(min.X + x * cellWidth, min.Y + y * cellHeight);
                    var cellMax = new Vector2(cellMin.X + cellWidth, cellMin.Y + cellHeight);

                    if (y * world.Width + x >= world.TileCount)
                    {
                        drawList.AddRectFilled(cellMin, cellMax,
                            ImGui.GetColorU32(new Vector4(1.0f, 0.843f, 0.0f, 1.0f)));
                        continue;
                    }

                    var tile = world.GetTile((uint)x, (uint)y);
                    var item = bot.ItemDatabase.GetItem(tile.ForegroundItemId);
                    var color = bot.ItemDatabase.GetItem((uint)(tile.ForegroundItemId + 1)).OverlayColor;

                    var r = ((color >> 24) & 0xFF) / 255.0f;
                    var g = ((color >> 16) & 0xFF) / 255.0f;
                    var b = ((color >> 8) & 0xFF) / 255.0f;
                    var a = (color & 0xFF) / 255.0f;

                    drawList.AddRectFilled(cellMin, cellMax, ImGui.GetColorU32(new Vector4(r, g, b, a)));

                    foreach (var player in bot.Players)
                    {
                        if (player.Position.X / 32 == x && player.Position.Y / 32 == y)
                        {
                            drawList.AddRectFilled(cellMin, cellMax,
                                ImGui.GetColorU32(new Vector4(1.0f, 0.843f, 0.0f, 1.0f)));
                        }
                    }

                    var botPosition = bot.Position;
                    if (Math.Abs(botPosition.X / 32.0f - x) < 0.01f && Math.Abs(botPosition.Y / 32.0f - y) < 0.01f)
                    {
                        drawList.AddRectFilled(cellMin, cellMax,
                            ImGui.GetColorU32(new Vector4(1.0f, 0.0f, 0.0f, 1.0f)));
                    }

                    if (!ImGui.IsMouseHoveringRect(cellMin, cellMax)) continue;
                    ImGui.BeginTooltip();
                    ImGui.Text($"Position: {x}|{y}");
                    ImGui.Text($"Item name: {item.Name}");
                    ImGui.Text($"Collision type: {item.CollisionType}");
                    ImGui.EndTooltip();

                    if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                    {
                        bot.LogInfo($"Clicked on tile: {x}|{y}");
                        var x1 = x;
                        var y1 = y;
                        var thread = new Thread(() => bot.FindPath((uint)x1, (uint)y1))
                        {
                            IsBackground = true
                        };
                        thread.Start();
                    }
                }
            }

            ImGui.Begin("World",
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize);
            ImGui.Text($"World size: {world.Width}x{world.Height}");
            ImGui.Text($"Bot position: {bot.Position.X / 32.0f}|{bot.Position.Y / 32.0f}");
            ImGui.Text($"Floating count: {world.Dropped.ItemsCount}");
            ImGui.Text($"Tile count: {world.TileCount}");
            if (ImGui.Button("Lock the world"))
            {
                AutoTutorial.LockTheWorld(bot);
            }

            if (ImGui.Button("Auto clear world"))
            {
                var thread = new Thread(() => AutoClearWorld.Start(bot))
                {
                    IsBackground = true
                };
                thread.Start();
            }

            ImGui.End();
        }
    }
}