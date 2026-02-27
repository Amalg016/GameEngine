using GameEngine.scenes;
using GameEngine.Core.Utilities;
using ImGuiNET;

namespace GameEngine.Editor.Window
{
    public class ContentBrowserWindow : IEditorWindow
    {
        string Path = "Assets";
        public DirectoryInfo directoryInfo;
        string currentPath;
        DirectoryInfo currentInfo;

        public string Title => "Content Browser";

        public ContentBrowserWindow()
        {
            directoryInfo = new DirectoryInfo(Path);
            currentInfo = directoryInfo;
            //   Directory.SetCurrentDirectory(directoryInfo.FullName);
        }
        public unsafe void Render()
        {
            ImGui.Begin(Title);
            //  var files = from file in
            //   Directory.EnumerateFiles(Path)
            //              select file;
            if (currentInfo.FullName != directoryInfo.FullName)
            {
                if (ImGui.Button("<-"))
                {
                    currentInfo = currentInfo.Parent;
                }
            }
            ImGui.NewLine();

            float padding = 16;
            float thumbnailSize = 120;
            float cellsize = thumbnailSize + padding;
            float panelWidth = ImGui.GetContentRegionAvail().X;
            int columnCount = (int)(panelWidth / cellsize);
            if (columnCount < 1)
            {
                columnCount = 1;
            }
            ImGui.Columns(columnCount, "s", false);
            foreach (var item in Directory.GetFileSystemEntries(currentInfo.FullName))
            {
                DirectoryInfo file = new DirectoryInfo(item);
                //  ImGui.Text(item);
                if (file.Extension == "")
                {

                    uint id = AssetPool.getTexture("Editor/Images/File.png", "File").getTexID();
                    ImGui.PushID(file.Name.ToString());
                    if (ImGui.ImageButton(file.Name.ToString(), (IntPtr)id, new System.Numerics.Vector2(50, 50)))
                    {
                        currentInfo = file;
                    }
                    ImGui.PopID();
                    //   if (ImGui.Button(file.Name))
                    //   {
                    //       currentInfo = file;
                    //   }
                }
                else
                {
                    // Console.WriteLine(file.Extension.ToString());
                    uint id = AssetPool.getTexture("Editor/Images/FileItem.png", "FileItem").getTexID();
                    ImGui.PushID(file.Name.ToString());
                    if (ImGui.ImageButton(file.Name.ToString(), (IntPtr)id, new System.Numerics.Vector2(50, 50)))
                    {

                    }
                    // if (ImGui.Button(file.Name.ToString()))
                    // {
                    //
                    // }
                    ImGui.PopID();
                    if (ImGui.BeginDragDropSource())
                    {

                        ImGui.SetDragDropPayload("Content Browser Item", IntPtr.Zero, 0);

                        if (file.Extension == ".scene")
                        {
                            SceneManager.ChangeScenePath(file);
                        }
                        ImGui.Text(file.Name);
                        ImGui.EndDragDropSource();
                    }
                }
                ImGui.TextWrapped(file.Name.ToString());


                ImGui.NextColumn();
            }
            ImGui.Columns(1);
            ImGui.End();
        }

    }

}

