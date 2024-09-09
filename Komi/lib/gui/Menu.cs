using ClickableTransparentOverlay;
using ImGuiNET;
using System.Numerics;
using Komi.lib.gui.pages;

namespace Komi.lib.gui
{
    public class Menu(Manager manager) : Overlay
    {
        private Vector2 windowSize = new Vector2(700, 400);
        private Vector2 windowPosition = new Vector2(0, 0);
        private static bool p_open = true;
        private static bool m_init = true;
        private ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoCollapse;
        public Manager Manager = manager;

        protected unsafe override void Render()
        {
            if (m_init) {
                SetTheme();
                Console.WriteLine("[IMGUI] Has been initialized.");
                ImGuiIOPtr io = ImGui.GetIO();
                this.ReplaceFont(config =>
                {
                    var io = ImGui.GetIO();
                    if (File.Exists("verdana.ttf"))
                    {
                        io.Fonts.AddFontFromFileTTF("verdana.ttf", 17f, config, io.Fonts.GetGlyphRangesDefault());
                    }
                });
                Vector2 displaySize = io.DisplaySize;
                Console.WriteLine($"[IMGUI] Display size: {displaySize.X}x{displaySize.Y}");

                windowPosition = new Vector2(
                    (displaySize.X - windowSize.X) * 0.5f,
                    (displaySize.Y - windowSize.Y) * 0.5f
                );
                Console.WriteLine($"[IMGUI] Centered position: {windowPosition.X},{windowPosition.Y}");


                m_init = false;
            }

            if (!p_open)
                Close();

            ImGui.SetNextWindowSize(windowSize, ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowPos(windowPosition, ImGuiCond.Once);
            ImGui.Begin("Komi - github.com/CLOEI/Komi", ref p_open, windowFlags);

            if (ImGui.BeginTabBar("TabBar"))
            {
                if (ImGui.BeginTabItem("Bots"))
                {
                    Bots.Render(manager);
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Item Database"))
                {

                    pages.ItemDatabase.Render();
                    ImGui.EndTabItem();
                }

                if (ImGui.BeginTabItem("Settings"))
                {
                    Settings.Render();
                    ImGui.EndTabItem();
                }

                ImGui.EndTabBar();
            }

            ImGui.End();
        }

        private static void SetTheme()
        {
            ImGuiStylePtr style = ImGui.GetStyle();
            var colors = style.Colors;

            style.WindowRounding = 5.3f;
            style.FrameRounding = 2.3f;
            style.ScrollbarRounding = 0;

            colors[(int)ImGuiCol.WindowBg] = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
            colors[(int)ImGuiCol.TitleBg] = new Vector4(0.08f, 0.08f, 0.08f, 1.0f);
            colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.16f, 0.16f, 0.16f, 1.0f);
            colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.05f, 0.05f, 0.05f, 1.0f);
            colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.14f, 0.14f, 0.14f, 1.0f);
            colors[(int)ImGuiCol.Text] = new Vector4(0.95f, 0.96f, 0.98f, 1.0f);
            colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.36f, 0.42f, 0.47f, 1.0f);
            colors[(int)ImGuiCol.Border] = new Vector4(0.43f, 0.43f, 0.50f, 0.5f);
            colors[(int)ImGuiCol.FrameBg] = new Vector4(0.18f, 0.18f, 0.18f, 1.0f);
            colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.24f, 0.24f, 0.24f, 1.0f);
            colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.32f, 0.32f, 0.32f, 1.0f);
            colors[(int)ImGuiCol.Button] = new Vector4(0.24f, 0.24f, 0.24f, 1.0f);
            colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.35f, 0.35f, 0.35f, 1.0f);
            colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.40f, 0.40f, 0.40f, 1.0f);
            colors[(int)ImGuiCol.CheckMark] = new Vector4(0.0f, 0.78f, 0.78f, 1.0f);
            colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.24f, 0.52f, 0.88f, 1.0f);
            colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.26f, 0.56f, 0.98f, 1.0f);
            colors[(int)ImGuiCol.Tab] = new Vector4(0.18f, 0.18f, 0.18f, 1.0f);
            colors[(int)ImGuiCol.TabHovered] = new Vector4(0.24f, 0.52f, 0.88f, 1.0f);
            colors[(int)ImGuiCol.PlotLines] = new Vector4(0.61f, 0.61f, 0.61f, 1.0f);
            colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(1.0f, 0.43f, 0.35f, 1.0f);
            colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.9f, 0.7f, 0.0f, 1.0f);
            colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(1.0f, 0.6f, 0.0f, 1.0f);
            colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.24f, 0.52f, 0.88f, 0.35f);
        }
    }
}
