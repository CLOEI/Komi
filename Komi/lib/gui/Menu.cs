using ClickableTransparentOverlay;
using ImGuiNET;
using System.Numerics;
using Komi.lib.gui.pages;

namespace Komi.lib.gui
{
    public class Menu(Manager manager) : Overlay
    {
        private Vector2 windowSize = new Vector2(800, 500);
        private Vector2 windowPosition = new Vector2(0, 0);
        private static bool p_open = true;
        private static bool m_init = true;

        private ImGuiWindowFlags windowFlags =
            ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoSavedSettings;

        protected unsafe override void Render()
        {
            if (m_init)
            {
                SetTheme();
                ImGuiIOPtr io = ImGui.GetIO();
                ReplaceFont(config =>
                {
                    var io = ImGui.GetIO();
                    if (File.Exists("Roboto-Regular.ttf"))
                    {
                        io.Fonts.AddFontFromFileTTF("Roboto-Regular.ttf", 18f, config,
                            io.Fonts.GetGlyphRangesDefault());
                    }
                });
                Vector2 displaySize = io.DisplaySize;

                windowPosition = new Vector2(
                    (displaySize.X - windowSize.X) * 0.5f,
                    (displaySize.Y - windowSize.Y) * 0.5f
                );

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
                    ItemDatabase.Render();
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

            colors[(int)ImGuiCol.WindowBg] = new Vector4(0.1f, 0.1f, 0.13f, 1.0f);
            colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
            colors[(int)ImGuiCol.Border] = new Vector4(0.44f, 0.37f, 0.61f, 0.29f);
            colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.0f, 0.0f, 0.0f, 0.24f);
            colors[(int)ImGuiCol.Text] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            colors[(int)ImGuiCol.Header] = new Vector4(0.13f, 0.13f, 0.17f, 1.0f);
            colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.19f, 0.2f, 0.25f, 1.0f);
            colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
            colors[(int)ImGuiCol.Button] = new Vector4(0.13f, 0.13f, 0.17f, 1.0f);
            colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.19f, 0.2f, 0.25f, 1.0f);
            colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
            colors[(int)ImGuiCol.CheckMark] = new Vector4(0.74f, 0.58f, 0.98f, 1.0f);
            colors[(int)ImGuiCol.PopupBg] = new Vector4(0.1f, 0.1f, 0.13f, 0.92f);
            colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.44f, 0.37f, 0.61f, 0.54f);
            colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.74f, 0.58f, 0.98f, 0.54f);
            colors[(int)ImGuiCol.FrameBg] = new Vector4(0.13f, 0.13f, 0.17f, 1.0f);
            colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.19f, 0.2f, 0.25f, 1.0f);
            colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
            colors[(int)ImGuiCol.Tab] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
            colors[(int)ImGuiCol.TabHovered] = new Vector4(0.24f, 0.24f, 0.32f, 1.0f);
            colors[(int)ImGuiCol.TitleBg] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
            colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
            colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
            colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.1f, 0.1f, 0.13f, 1.0f);
            colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
            colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.19f, 0.2f, 0.25f, 1.0f);
            colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.24f, 0.24f, 0.32f, 1.0f);
            colors[(int)ImGuiCol.Separator] = new Vector4(0.44f, 0.37f, 0.61f, 1.0f);
            colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.74f, 0.58f, 0.98f, 1.0f);
            colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.84f, 0.58f, 1.0f, 1.0f);
            colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.44f, 0.37f, 0.61f, 0.29f);
            colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.74f, 0.58f, 0.98f, 0.29f);
            colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.84f, 0.58f, 1.0f, 0.29f);
            colors[(int)ImGuiCol.DockingPreview] = new Vector4(0.44f, 0.37f, 0.61f, 1.0f);

            style.TabRounding = 4;
            style.ScrollbarRounding = 9;
            style.WindowRounding = 7;
            style.GrabRounding = 3;
            style.FrameRounding = 3;
            style.PopupRounding = 4;
            style.ChildRounding = 4;
        }
    }
}