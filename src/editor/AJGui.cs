using ImGuiNET;
using System.Numerics;

namespace GameEngine.Editor
{
    public class AJGui
    {
        public static float defaultWidth = 220;
        public static void drawVec2Control(string Label, ref Vector2 values)
        {
            drawVec2Control(Label, ref values, 0, defaultWidth);
        }
        public static void drawVec2Control(string Label, ref Vector2 values, float resetValue)
        {
            drawVec2Control(Label, ref values, resetValue, defaultWidth);

        }

        public static void drawVec2Control(string Label, ref Vector2 values, float resetValue, float columnWidth)
        {
            ImGui.PushID(Label);

            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, columnWidth / 2);
            ImGui.Text(Label);
            ImGui.NextColumn();

            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
            float lineHeight = ImGui.GetFontSize() + ImGui.GetStyle().FramePadding.Y * 2;
            Vector2 buttonSize = new Vector2(lineHeight + 3, lineHeight);
            float widthEach = (ImGui.CalcItemWidth() - buttonSize.X * 2f) / 2;
            ImGui.PushItemWidth(widthEach * 1.5f);
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.8f, 0.1f, 0.15f, 1));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.9f, 0.2f, 0.2f, 1));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.8f, 0.1f, 0.15f, 1));
            if (ImGui.Button("x", buttonSize))
            {
                values.X = resetValue;
            }
            ImGui.PopStyleColor(3);
            ImGui.SameLine();
            float vecValuesX = values.X;
            ImGui.DragFloat("##x", ref vecValuesX, 0.2f);
            ImGui.PopItemWidth();
            ImGui.SameLine();

            ImGui.PushItemWidth(widthEach * 2);
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.2f, 0.7f, 0.2f, 1));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.8f, 0.3f, 1));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.2f, 0.7f, 0.2f, 1));
            if (ImGui.Button("y", buttonSize))
            {
                values.Y = resetValue;
            }
            ImGui.PopStyleColor(3);
            ImGui.SameLine();
            float vecValuesY = values.Y;
            ImGui.DragFloat("##y", ref vecValuesY, 0.1f);

            ImGui.PopItemWidth();
            ImGui.NextColumn();

            values.X = vecValuesX;
            values.Y = vecValuesY;
            ImGui.PopStyleVar();
            ImGui.Columns(1);
            ImGui.PopID();
        }



        public static float dragFloat(string Label, float values)
        {
            ImGui.PushID(Label);

            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, defaultWidth / 2);
            ImGui.Text(Label);
            ImGui.NextColumn();

            float valArr = values;
            ImGui.DragFloat("##dragFloat", ref valArr);


            ImGui.Columns(1);
            ImGui.PopID();
            return valArr;
        }
        public static int dragInt(string Label, int values)
        {
            ImGui.PushID(Label);

            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, defaultWidth / 2);
            ImGui.Text(Label);
            ImGui.NextColumn();

            int valArr = values;
            ImGui.DragInt("##dragInt", ref valArr);


            ImGui.Columns(1);
            ImGui.PopID();
            return valArr;
        }
        public static string inputText(string Label, string text)
        {
            ImGui.PushID(Label);

            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, defaultWidth / 2);
            ImGui.Text(Label);
            ImGui.NextColumn();

            string valArr = text;
            if (ImGui.InputText("##" + Label, ref valArr, 32))
            {
                ImGui.Columns(1);
                ImGui.PopID();
                return valArr;
            }

            ImGui.Columns(1);
            ImGui.PopID();
            return text;
        }
    }
}
