using GameEngine.components;
using GameEngine.components.Animations;
using GameEngine.Physics2D.components;
using GameEngine.util;
using ImGuiNET;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using ImGuiCond = ImGuiNET.ImGuiCond;
using ImGuiTreeNodeFlags = ImGuiNET.ImGuiTreeNodeFlags;
using ImGuiWindowFlags = ImGuiNET.ImGuiWindowFlags;

namespace GameEngine.editor
{
    public class AnimationWindow
    {
        private GameObject activeGameobject;
        public void Update()
        {
        }
        Animator animator;
        Spritesheet spritesheet;
        int spriteIndex;
        float frameRate;
        string Name = "default";
        Animation anim;
        int d = 0;
        public AnimationWindow()
        {
            // spritesheet = AssetPool.spritesheets.ElementAt(0).Value;
        }
        bool create = false;
        Vector2 uV0 = new Vector2(), uV1 = new Vector2(); int k = 0;
        public unsafe void imgui()
        {

            ImGui.Begin("Animations", ImGuiWindowFlags.HorizontalScrollbar);

            string[] names = GetNames(AssetPool.spritesheets);

            if (ImGui.BeginTabBar("AnimWindow TabBar"))
            {
                if (ImGui.BeginTabItem("Animation Section"))
                {
                    Vector2 WindowPos = ImGui.GetWindowPos();
                    Vector2 WindowSize = ImGui.GetWindowSize();
                    Vector2 WindowMin = WindowPos + ImGui.GetCursorPos();
                    Vector2 WindowMax = WindowPos + WindowSize;
                    Vector2 WindowSpace = WindowMax - WindowMin;

                    ImGui.BeginChild("##Child1", new Vector2(0.5f * WindowSpace.X, 0.5f * WindowSpace.Y), true);

                    //Setting Index of spritesheet;
                    int index = 0;
                    if (spritesheet != null)
                    {
                        index = indexof(spritesheet.Name, AssetPool.spritesheets);
                    }
                    else
                    {
                        spritesheet = AssetPool.spritesheets.ElementAt(index).Value;
                    }

                    if (ImGui.Combo(": Spritesheets", ref index, names, names.Length))
                    {
                        spritesheet = AssetPool.spritesheets.ElementAt(index).Value;
                        create = false;
                    }

                    ImGui.NewLine();
                    List<Animation> anims = AssetPool.allAnimations;
                    int i = 0;
                    if (anims != null)
                    {



                        if (anims.Count != 0)
                        {
                            if (anim == null)
                            {
                                anim = anims[0];
                            }

                            i = indexof(anim.Name, anims);
                        }

                        string[] N = GetNames(anims);
                        if (ImGui.Combo("animations", ref i, N, N.Length))
                        {
                            anim = anims[i];
                            play = false;
                            k = 0;
                        }

                        ImGui.SameLine();

                        if (ImGui.Button("Edit"))
                        {
                            create = true;
                        }

                        ImGui.SameLine();



                        if (ImGui.Button("Create New"))
                        {
                            anim = new Animation();
                            anim.Name = Name;
                            AssetPool.allAnimations.Add(anim);
                            // Animator.AddBackUp(anim);
                            create = true;
                        }

                        if (create)
                        {
                            Name = AJGui.inputText("Animation Name", anim.Name);
                            anim.Name = Name;
                            bool val = anim.Loop;
                            if (ImGui.Checkbox("Loop :", ref val))
                            {
                                anim.Loop = val;
                            }

                            int j = 0;
                            if (anim != null)
                            {
                                j = indexof(d, anim.frames);
                                string[] na = GetNames(anim.frames);

                                if (ImGui.ListBox("Indexes", ref j, na, na.Length))
                                {
                                    d = j;
                                }

                                ImGui.SameLine();
                                if (ImGui.Button("Delete"))
                                {
                                    if (anim.frames.Count > 0)
                                    {
                                        anim.DeleteFrame(j);
                                    }
                                }
                            }

                            ImGui.NewLine();
                            spriteIndex = AJGui.dragInt("SpriteIndex", spriteIndex);
                            frameRate = AJGui.dragFloat("FrameRate", frameRate);

                            if (anim != null)
                            {

                                anim.Name = Name;

                                if (ImGui.Button("Add Frame"))
                                {
                                    anim.Name = Name;
                                    if (spritesheet != null)
                                    {
                                        if (spriteIndex >= spritesheet.size())
                                        {
                                            return;
                                        }

                                        anim.AddFrame(spritesheet, spriteIndex, frameRate);
                                        anim.Loop = val;
                                    }
                                }
                            }
                        }

                        // ImGui.TextUnformatted("Child1");
                        ImGui.EndChild();

                        ImGui.SameLine();
                        ImGui.BeginChild("##Child2", new Vector2(0.5f * WindowSpace.X, 0.5f * WindowSpace.Y), true);
                        ImGui.TextUnformatted("Viewer");
                        ImGui.NewLine();
                        if (spritesheet != null)
                        {
                            int count = anim.frames.Count;
                            if (count > d)
                            {
                                Sprite sprite = anim.frames[d].sprite;
                                int id = sprite.GetTexID();
                                var texCoords = sprite.getTexCoords();
                                Vector2 uv0 = new Vector2(texCoords[1].X, texCoords[1].Y);
                                Vector2 uv1 = new Vector2(texCoords[3].X, texCoords[3].Y);


                                ImGui.Image((IntPtr)id, new Vector2(150, 150), uv1, uv0);
                            }
                        }



                        ImGui.EndChild();

                        ImGui.BeginChild("##Child3", new Vector2(0, 0.5f * WindowSpace.Y), true);
                        if (spritesheet != null)
                        {
                            int count = anim.frames.Count;
                            if (count > k)
                            {
                                Sprite sprite = anim.frames[k].sprite;
                                int id = sprite.GetTexID();
                                var texCoords = sprite.getTexCoords();
                                Vector2 uv0 = new Vector2(texCoords[1].X, texCoords[1].Y);
                                Vector2 uv1 = new Vector2(texCoords[3].X, texCoords[3].Y);


                                // ImGui.Image((IntPtr)id, new Vector2(150, 150), uv1, uv0);

                                if (ImGui.Button("Play"))
                                {
                                    play = !play;
                                }

                                if (play)
                                {
                                    timeTracker -= Time.deltaTime;
                                }

                                if (ImGui.SliderInt("Frames", ref k, 0, count - 1))
                                {

                                }

                                if (anim.frames.Count > 0)
                                {
                                    var currentFrame = anim.frames[k];
                                    if (timeTracker <= 0)
                                    {
                                        if (k < anim.frames.Count - 1)
                                        {
                                            k++;
                                        }
                                        else if (k == count - 1 && anim.Loop)
                                        {
                                            k = (k + 1) % count;
                                        }

                                        timeTracker = currentFrame.frameRate;
                                    }

                                    var tex = currentFrame.sprite.getTexCoords();
                                    uV0 = new Vector2(tex[1].X, tex[1].Y);
                                    uV1 = new Vector2(tex[3].X, tex[3].Y);
                                    ImGui.PushID(k);
                                    ImGui.Image((IntPtr)id, new Vector2(150, 150), uV1, uV0);
                                    ImGui.PopID();
                                }
                            }

                            ImGui.EndChild();
                        }

                        ImGui.EndTabItem();
                        }
                    }


                    //Animation Controller

                    if (ImGui.BeginTabItem("AnimationController"))
                    {
                        Vector2 WindowPos = ImGui.GetWindowPos();
                        Vector2 WindowSize = ImGui.GetWindowSize();
                        Vector2 WindowMin = WindowPos + ImGui.GetCursorPos();
                        Vector2 WindowMax = WindowPos + WindowSize;
                        Vector2 WindowSpace = WindowMax - WindowMin;


                        ImGui.BeginChild("##Child4", new Vector2(0, 0.2f * WindowSpace.Y), true);
                        List<AnimationController> controllers = AssetPool.animationControllers;
                        if (controllers != null)
                        {


                            string[] AnimNames = GetNames(controllers);
                            int d = 0;
                            if (controllers.Count > 0)
                            {
                                d = indexof(currentController.Name, AnimNames);
                            }

                            if (ImGui.Combo("Controller", ref d, AnimNames, AnimNames.Length))
                            {
                                currentController = controllers[d];
                            }

                            ImGui.SameLine();
                            if (ImGui.Button("Create New"))
                            {
                                var c = new AnimationController();
                                AssetPool.animationControllers.Add(c);
                                currentController = c;
                            }

                            //       ImGui.PopID();
                            if (currentController != null)
                            {
                                currentController.Name = AJGui.inputText("Controller Name", currentController.Name);
                            }
                        }

                        ImGui.EndChild();

                        //   if (activeGameobject != null)
                        //   {
                        //       Animator animator = activeGameobject.GetComponent<Animator>();
                        //       if (animator != null)
                        //       {
                        //           if (animator.controller != null)
                        //           {
                        //               currentController = animator.controller;
                        //           }
                        //       }                       
                        //   }
                        ImGui.BeginChild("##Child5", new Vector2(0.5f * WindowSpace.X, 0.5f * WindowSpace.Y), true);
                        if (currentController != null)
                        {
                            List<AnimationState> states = currentController.States;
                            int j = 0;
                            string[] stateNames = GetNames(states);
                            if (states.Count > 0)
                            {
                                if (states.Count == 1)
                                {
                                    currentState = states[0];
                                }

                                j = indexof(currentState.Name, stateNames);
                            }
                            // ImGui.Separator();

                            //  ImGui.Unindent();
                            if (ImGui.Combo("States", ref j, stateNames, stateNames.Length))
                            {
                                currentState = states[j];
                            }

                            ImGui.SameLine();
                            //   ImGui.PushID(add);
                            if (ImGui.Button("+"))
                            {
                                currentState = new AnimationState();
                                if (currentState == null)
                                {
                                    throw new Exception();
                                }

                                currentController.States.Add(currentState);
                            }

                            //  ImGui.PopID(); 
                            ImGui.EndChild();
                            ImGui.SameLine();
                            ImGui.BeginChild("##Child6", new Vector2(0.5f * WindowSpace.X, 0.5f * WindowSpace.Y), true);
                            /// States Editor 
                            if (currentState != null)
                            {
                                currentState.Name = AJGui.inputText("State Name", currentState.Name);
                                AddingAnimation();
                            }


                            // ImGui.PopID();
                            //  Debug.Log(k);
                            ImGui.EndChild();
                        }
                    

                    ImGui.EndTabItem();
                    
                }

                activeGameobject = PropertiesWindow.window.getActiveGameObject();
                ImGui.EndTabBar();
            }
                ImGui.End();
        }
            string add = "+";
        bool play = false;
            float timeTracker = 0;
            AnimationController currentController;
            AnimationState currentState;
            List<Animation> allAnimations = new List<Animation>();
            void AddingAnimation()
            {

                allAnimations = AssetPool.allAnimations;

                if (currentState.animation == null)
                {
                    if (allAnimations.Count > 0)
                    {
                        currentState.animation = allAnimations[0];
                    }
                }
                string[] N = GetNames(allAnimations);
                int f = 0;
                if (currentState.animation != null)
                {
                    f = indexof(currentState.animation.Name, N);
                }
                if (ImGui.Combo("Animation", ref f, N, N.Length))
                {
                    currentState.animation = allAnimations[f];
                }
            }


            private int indexof(string Name, Dictionary<string, Spritesheet> spritesheets)
            {
                for (int i = 0; i < spritesheets.Count; i++)
                {
                    if (Name == spritesheets.ElementAt(i).Key)
                    {
                        return i;
                    }
                }
                return 0;
            }
            private int indexof(int index, List<Frame> frames)
            {
                for (int i = 0; i < frames.Count; i++)
                {
                    if (index == i)
                    {
                        return i;
                    }
                }
                return 0;
            }

            private int indexof(string enumType, List<Animation> enumValues)
            {
                for (int i = 0; i < enumValues.Count; i++)
                {
                    if (enumType == enumValues[i].Name)
                    {
                        return i;
                    }
                }
                return 0;
            }
            private int indexof(string enumType, string[] enumValues)
            {
                for (int i = 0; i < enumValues.Length; i++)
                {
                    if (enumType == enumValues[i])
                    {
                        return i;
                    }
                }
                return 0;
            }
            public string[] GetNames(Dictionary<string, Spritesheet> animations)
            {
                string[] names = new string[animations.Count];
                int i = 0;
                foreach (var item in animations)
                {
                    names[i] = item.Key;
                    i++;
                }
                return names;
            }
            public string[] GetNames(List<Frame> frames)
            {
                string[] names = new string[frames.Count];
                int i = 0;
                foreach (var item in frames)
                {
                    names[i] = i.ToString();
                    i++;
                }
                return names;
            }
            public string[] GetNames(List<Animation> animations)
            {
                string[] names = new string[animations.Count];
                int i = 0;
                foreach (var item in animations)
                {
                    names[i] = item.Name;
                    i++;
                }
                return names;
            }
            public string[] GetNames(List<AnimationState> animations)
            {
                string[] names = new string[animations.Count];
                int i = 0;
                foreach (var item in animations)
                {
                    names[i] = item.Name;
                    i++;
                }
                return names;
            }
            public string[] GetNames(List<AnimationController> animations)
            {
                string[] names = new string[animations.Count];
                int i = 0;
                foreach (var item in animations)
                {
                    names[i] = item.Name;
                    i++;
                }
                return names;
            }
            private bool doTreeNode(Animation obj, int index)
            {
                int i = index;
                ImGui.PushID(index);
                bool treeNodeOpen = ImGui.TreeNodeEx(obj.Name, ImGuiTreeNodeFlags.DefaultOpen |
                    ImGuiTreeNodeFlags.FramePadding |
                    ImGuiTreeNodeFlags.OpenOnArrow |
                    ImGuiTreeNodeFlags.SpanAvailWidth, obj.Name);

                ImGui.PopID();
                if (ImGui.IsAnyItemActive())
                {

                }
                //  if (ImGui.BeginDragDropSource())
                //  {
                //      ImGui.SetDragDropPayload(Name, (IntPtr)(&i), sizeof(int));
                //
                //      ImGui.Text(obj.name);
                //      ImGui.EndDragDropSource();
                //
                //  }
                //  if (ImGui.BeginDragDropTarget())
                //  {
                //      ImGuiPayloadPtr payload = ImGui.AcceptDragDropPayload(Name);
                //      if (payload.NativePtr != null)
                //      {
                //          var dataPtr = (int*)payload.Data;
                //          int src = dataPtr[0];
                //          var srcItem = list[src];
                //          Console.WriteLine("Payload accepted" + srcItem.name + "_");
                //      }
                //
                //      ImGui.EndDragDropTarget();
                //  }

                return treeNodeOpen;
            }
        }

    } 
