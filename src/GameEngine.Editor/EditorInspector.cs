using GameEngine.components;
using GameEngine.components.Animations;
using GameEngine.Core.Utilities;
using GameEngine.ECS;
using ImGuiNET;
using System.Numerics;
using System.Reflection;

namespace GameEngine.Editor
{
    /// <summary>
    /// Handles all ImGui inspector rendering for the editor.
    /// Replaces the gui()/IMGUI() methods that were removed from the engine library.
    /// </summary>
    public static class EditorInspector
    {
        public static void DrawGameObject(GameObject go)
        {
            go.name = AJGui.inputText("Name", go.name);
            go.setZIndex(AJGui.dragInt("Layer:", go.zIndex()));

            foreach (var item in go.getAllComponents())
            {
                ImGuiTreeNodeFlags treeNodeFlags = ImGuiTreeNodeFlags.Framed | ImGuiTreeNodeFlags.SpanAvailWidth | ImGuiTreeNodeFlags.FramePadding | ImGuiTreeNodeFlags.AllowOverlap;
                if (ImGui.CollapsingHeader(item.GetType().Name, treeNodeFlags))
                    DrawComponent(item);
            }
        }

        public static void DrawComponent(Component component)
        {
            // Custom renderers for specific types
            if (component is Animator animator)
            {
                DrawAnimatorGui(animator);
                return;
            }

            // Default reflection-based rendering
            DrawComponentFields(component);
        }

        private static void DrawComponentFields(Component component)
        {
            try
            {
                FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                foreach (var field in fields)
                {
                    if (field.IsNotSerialized)
                    {
                        continue;
                    }
                    Type type = field.FieldType;
                    Object value = field.GetValue(component);
                    string name = field.Name;

                    if (type == typeof(int))
                    {
                        int val = (int)value;
                        field.SetValue(component, AJGui.dragInt(name, val));
                    }
                    else if (type == typeof(float))
                    {
                        float val = (float)value;
                        field.SetValue(component, AJGui.dragFloat(name, val));
                    }
                    else if (type == typeof(bool))
                    {
                        bool val = (bool)value;
                        if (ImGui.Checkbox(name + ":", ref val))
                        {
                            field.SetValue(component, !val);
                        }
                    }
                    else if (type == typeof(Vector3))
                    {
                        Vector3 val = (Vector3)value;
                        if (ImGui.DragFloat3(name + ":", ref val))
                        {
                            field.SetValue(component, val);
                        }
                    }
                    else if (type == typeof(Vector2))
                    {
                        Vector2 val = (Vector2)value;
                        AJGui.drawVec2Control(name + ":", ref val);
                        field.SetValue(component, val);
                    }
                    else if (name == "color")
                    {
                        Vector4 imColor = (Vector4)value;
                        if (ImGui.ColorEdit4("Color Picker", ref imColor))
                        {
                            field.SetValue(component, imColor);
                            component.gameObject.GetComponent<SpriteRenderer>().setDirty();
                        }
                    }
                    else if (type == typeof(Vector4) && name != "color")
                    {
                        Vector4 val = (Vector4)value;
                        if (ImGui.DragFloat4(name + ":", ref val))
                        {
                            field.SetValue(component, val);
                        }
                    }
                    else if (type.IsEnum)
                    {
                        string[] enumValues = GetEnumValues(type);
                        string enumType = ((Enum)value).ToString();
                        int index = IndexOf(enumType, enumValues);
                        Array s = type.GetEnumValues();
                        if (ImGui.Combo(field.Name, ref index, enumValues, enumValues.Length))
                        {
                            field.SetValue(component, s.GetValue(index));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("EditorInspector error: " + e.Message);
            }
        }

        private static void DrawAnimatorGui(Animator animator)
        {
            List<AnimationController> allControllers = AssetPool.animationControllers;
            if (allControllers == null) return;

            string[] names = new string[allControllers.Count];
            for (int i = 0; i < allControllers.Count; i++)
            {
                names[i] = allControllers[i].Name;
            }

            int index = 0;
            if (allControllers.Count > 0)
            {
                if (animator.controller != null)
                {
                    for (int i = 0; i < names.Length; i++)
                    {
                        if (animator.controller.Name == names[i])
                        {
                            index = i;
                            break;
                        }
                    }
                }
                else
                {
                    animator.controller = allControllers[0];
                }
            }

            if (ImGui.Combo("animations", ref index, names, names.Length))
            {
                if (allControllers.Count > 0)
                {
                    animator.controller = allControllers[index];
                }
            }
        }

        private static int IndexOf(string enumType, string[] enumValues)
        {
            for (int i = 0; i < enumValues.Length; i++)
            {
                if (enumType == enumValues[i])
                    return i;
            }
            return 0;
        }

        private static string[] GetEnumValues(Type type)
        {
            string[] enumValues = new string[type.GetEnumValues().Length];
            int i = 0;
            foreach (var t in type.GetEnumNames())
            {
                enumValues[i] = t;
                i++;
            }
            return enumValues;
        }
    }
}
