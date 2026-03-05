using GameEngine.components;
using GameEngine.components.Animations;
using GameEngine.Core.Utilities;
using ImGuiNET;
using System.Numerics;

namespace GameEngine.Editor.Window
{
    /// <summary>
    /// Renders a Unity-style visual node graph for AnimationController editing.
    /// Uses ImGui's ImDrawList for custom rendering of nodes, transitions, and grid.
    /// </summary>
    public class AnimationControllerGraphRenderer
    {
        // ── State ──────────────────────────────────────────────
        private AnimationController? _controller;
        private AnimationState? _selectedState;
        private int _selectedTransitionStateIdx = -1;
        private int _selectedTransitionIdx = -1;

        // Canvas transform
        private Vector2 _scrollOffset = Vector2.Zero;
        private float _zoom = 1.0f;

        // Interaction modes
        private bool _isDraggingNode = false;
        private int _draggedNodeIndex = -1;
        private bool _isMakingTransition = false;
        private int _transitionSourceIndex = -1;
        private bool _isPanning = false;
        private Vector2 _panStartMouse;
        private Vector2 _panStartOffset;

        // Parameter editing
        private string _newParamName = "";
        private int _newParamType = 0; // 0=Bool, 1=Int, 2=Float

        // Visual constants
        private const float NODE_WIDTH = 180f;
        private const float NODE_HEIGHT = 50f;
        private const float NODE_HEADER_HEIGHT = 22f;
        private const float NODE_ROUNDING = 6f;
        private const float GRID_SIZE = 40f;
        private const float ARROW_SIZE = 10f;
        private const float TRANSITION_CLICK_THRESHOLD = 8f;
        private const float MIN_ZOOM = 0.3f;
        private const float MAX_ZOOM = 2.0f;

        // Colors
        private static readonly uint COL_GRID = ImGui.ColorConvertFloat4ToU32(new Vector4(0.2f, 0.2f, 0.2f, 0.4f));
        private static readonly uint COL_GRID_MAJOR = ImGui.ColorConvertFloat4ToU32(new Vector4(0.2f, 0.2f, 0.2f, 0.8f));
        private static readonly uint COL_NODE_BG = ImGui.ColorConvertFloat4ToU32(new Vector4(0.18f, 0.18f, 0.18f, 1f));
        private static readonly uint COL_NODE_BORDER = ImGui.ColorConvertFloat4ToU32(new Vector4(0.35f, 0.35f, 0.35f, 1f));
        private static readonly uint COL_NODE_SELECTED = ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.55f, 0.9f, 1f));
        private static readonly uint COL_HEADER_DEFAULT = ImGui.ColorConvertFloat4ToU32(new Vector4(0.85f, 0.5f, 0.15f, 1f));
        private static readonly uint COL_HEADER_NORMAL = ImGui.ColorConvertFloat4ToU32(new Vector4(0.35f, 0.35f, 0.35f, 1f));
        private static readonly uint COL_TRANSITION = ImGui.ColorConvertFloat4ToU32(new Vector4(0.85f, 0.85f, 0.85f, 0.8f));
        private static readonly uint COL_TRANSITION_SELECTED = ImGui.ColorConvertFloat4ToU32(new Vector4(0.3f, 0.7f, 1f, 1f));
        private static readonly uint COL_TRANSITION_MAKING = ImGui.ColorConvertFloat4ToU32(new Vector4(0.2f, 0.9f, 0.3f, 0.8f));
        private static readonly uint COL_TEXT = ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 1f, 1f, 1f));
        private static readonly uint COL_TEXT_DIM = ImGui.ColorConvertFloat4ToU32(new Vector4(0.7f, 0.7f, 0.7f, 1f));
        private static readonly uint COL_PANEL_BG = ImGui.ColorConvertFloat4ToU32(new Vector4(0.14f, 0.14f, 0.14f, 1f));

        public void SetController(AnimationController? controller)
        {
            if (_controller != controller)
            {
                _controller = controller;
                _selectedState = null;
                _selectedTransitionStateIdx = -1;
                _selectedTransitionIdx = -1;
                _isMakingTransition = false;
                _isDraggingNode = false;

                // Auto-layout states that have zero position
                if (_controller != null)
                {
                    AutoLayoutNewStates();
                }
            }
        }

        public void Render()
        {
            if (_controller == null) return;

            Vector2 windowSize = ImGui.GetContentRegionAvail();

            // Layout: Parameters (left, full height) | Right area (Graph top + Inspector bottom)
            float paramPanelWidth = 200f;
            float inspectorHeight = 180f; // Fixed height for state/transition inspector
            float rightWidth = windowSize.X - paramPanelWidth - 8f; // account for spacing
            if (rightWidth < 200f) rightWidth = 200f;
            float graphHeight = windowSize.Y - inspectorHeight - 8f; // account for spacing
            if (graphHeight < 100f) graphHeight = 100f;

            // ── Parameters Panel (Left, full height) ──
            ImGui.BeginChild("##ParamsPanel", new Vector2(paramPanelWidth, windowSize.Y), true);
            RenderParametersPanel();
            ImGui.EndChild();

            ImGui.SameLine();

            // ── Right area: Graph + Inspector stacked vertically ──
            ImGui.BeginGroup();

            // ── Node Graph (Top) ──
            ImGui.BeginChild("##GraphCanvas", new Vector2(rightWidth, graphHeight), true,
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            RenderNodeGraph();
            ImGui.EndChild();

            // ── Inspector Panel (Bottom, always visible) ──
            ImGui.BeginChild("##InspectorPanel", new Vector2(rightWidth, inspectorHeight), true);
            RenderBottomPanel();
            ImGui.EndChild();

            ImGui.EndGroup();
        }

        // ════════════════════════════════════════════════════════
        //  PARAMETERS PANEL
        // ════════════════════════════════════════════════════════

        private void RenderParametersPanel()
        {
            ImGui.TextColored(new Vector4(0.8f, 0.8f, 0.8f, 1f), "Parameters");
            ImGui.Separator();

            // Add new parameter
            ImGui.SetNextItemWidth(100);
            ImGui.InputText("##NewParam", ref _newParamName, 64);
            ImGui.SameLine();
            string[] typeNames = { "Bool", "Int", "Float" };
            ImGui.SetNextItemWidth(60);
            ImGui.Combo("##ParamType", ref _newParamType, typeNames, typeNames.Length);

            if (ImGui.Button("Add", new Vector2(-1, 0)) && !string.IsNullOrWhiteSpace(_newParamName))
            {
                switch (_newParamType)
                {
                    case 0:
                        if (!_controller!.Bool.ContainsKey(_newParamName))
                            _controller.Bool[_newParamName] = false;
                        break;
                    case 1:
                        if (!_controller!.Int.ContainsKey(_newParamName))
                            _controller.Int[_newParamName] = 0;
                        break;
                    case 2:
                        if (!_controller!.Float.ContainsKey(_newParamName))
                            _controller.Float[_newParamName] = 0f;
                        break;
                }
                _newParamName = "";
            }

            ImGui.Separator();
            ImGui.Spacing();

            // Bool parameters
            if (_controller!.Bool.Count > 0)
            {
                ImGui.TextColored(new Vector4(0.6f, 0.8f, 1f, 1f), "Bool");
                List<string> boolKeysToRemove = new();
                foreach (var key in _controller.Bool.Keys.ToList())
                {
                    bool val = _controller.Bool[key];
                    ImGui.PushID("b_" + key);
                    if (ImGui.Checkbox(key, ref val))
                    {
                        _controller.Bool[key] = val;
                    }
                    ImGui.SameLine();
                    if (ImGui.SmallButton("x"))
                    {
                        boolKeysToRemove.Add(key);
                    }
                    ImGui.PopID();
                }
                foreach (var k in boolKeysToRemove) _controller.Bool.Remove(k);
            }

            // Int parameters
            if (_controller.Int.Count > 0)
            {
                ImGui.Spacing();
                ImGui.TextColored(new Vector4(0.6f, 1f, 0.6f, 1f), "Int");
                List<string> intKeysToRemove = new();
                foreach (var key in _controller.Int.Keys.ToList())
                {
                    int val = _controller.Int[key];
                    ImGui.PushID("i_" + key);
                    ImGui.SetNextItemWidth(80);
                    if (ImGui.DragInt(key, ref val))
                    {
                        _controller.Int[key] = val;
                    }
                    ImGui.SameLine();
                    if (ImGui.SmallButton("x"))
                    {
                        intKeysToRemove.Add(key);
                    }
                    ImGui.PopID();
                }
                foreach (var k in intKeysToRemove) _controller.Int.Remove(k);
            }

            // Float parameters
            if (_controller.Float.Count > 0)
            {
                ImGui.Spacing();
                ImGui.TextColored(new Vector4(1f, 0.8f, 0.6f, 1f), "Float");
                List<string> floatKeysToRemove = new();
                foreach (var key in _controller.Float.Keys.ToList())
                {
                    float val = _controller.Float[key];
                    ImGui.PushID("f_" + key);
                    ImGui.SetNextItemWidth(80);
                    if (ImGui.DragFloat(key, ref val, 0.1f))
                    {
                        _controller.Float[key] = val;
                    }
                    ImGui.SameLine();
                    if (ImGui.SmallButton("x"))
                    {
                        floatKeysToRemove.Add(key);
                    }
                    ImGui.PopID();
                }
                foreach (var k in floatKeysToRemove) _controller.Float.Remove(k);
            }
        }

        // ════════════════════════════════════════════════════════
        //  NODE GRAPH
        // ════════════════════════════════════════════════════════

        private void RenderNodeGraph()
        {
            ImDrawListPtr drawList = ImGui.GetWindowDrawList();
            Vector2 canvasOrigin = ImGui.GetCursorScreenPos();
            Vector2 canvasSize = ImGui.GetContentRegionAvail();

            // Clip to canvas area
            ImGui.InvisibleButton("##GraphBg", canvasSize,
                ImGuiButtonFlags.MouseButtonLeft | ImGuiButtonFlags.MouseButtonRight | ImGuiButtonFlags.MouseButtonMiddle);
            bool isCanvasHovered = ImGui.IsItemHovered();

            drawList.PushClipRect(canvasOrigin, canvasOrigin + canvasSize, true);

            // ── Grid ──
            DrawGrid(drawList, canvasOrigin, canvasSize);

            // ── Handle zoom ──
            if (isCanvasHovered)
            {
                float wheel = ImGui.GetIO().MouseWheel;
                if (wheel != 0)
                {
                    float prevZoom = _zoom;
                    _zoom += wheel * 0.1f;
                    _zoom = Math.Clamp(_zoom, MIN_ZOOM, MAX_ZOOM);

                    // Zoom towards mouse position
                    Vector2 mousePos = ImGui.GetIO().MousePos;
                    Vector2 mouseCanvasPos = (mousePos - canvasOrigin - _scrollOffset) / prevZoom;
                    _scrollOffset = mousePos - canvasOrigin - mouseCanvasPos * _zoom;
                }
            }

            // ── Handle panning (middle mouse) ──
            if (isCanvasHovered && ImGui.IsMouseClicked(ImGuiMouseButton.Middle))
            {
                _isPanning = true;
                _panStartMouse = ImGui.GetIO().MousePos;
                _panStartOffset = _scrollOffset;
            }
            if (_isPanning)
            {
                if (ImGui.IsMouseDown(ImGuiMouseButton.Middle))
                {
                    Vector2 delta = ImGui.GetIO().MousePos - _panStartMouse;
                    _scrollOffset = _panStartOffset + delta;
                }
                else
                {
                    _isPanning = false;
                }
            }

            // ── Draw Transitions ──
            DrawTransitions(drawList, canvasOrigin);

            // ── Draw transition-in-progress line ──
            if (_isMakingTransition && _transitionSourceIndex >= 0 && _transitionSourceIndex < _controller!.States.Count)
            {
                Vector2 sourceCenter = GetNodeCenter(_controller.States[_transitionSourceIndex], canvasOrigin);
                Vector2 mousePos = ImGui.GetIO().MousePos;
                drawList.AddBezierCubic(sourceCenter, sourceCenter + new Vector2(50 * _zoom, 0),
                    mousePos - new Vector2(50 * _zoom, 0), mousePos, COL_TRANSITION_MAKING, 2f * _zoom);
            }

            // ── Draw Nodes ──
            DrawNodes(drawList, canvasOrigin, canvasSize);

            // ── Handle interactions ──
            HandleInteraction(canvasOrigin, canvasSize, isCanvasHovered);

            drawList.PopClipRect();

            // ── Status bar ──
            if (_isMakingTransition)
            {
                Vector2 textPos = canvasOrigin + new Vector2(10, canvasSize.Y - 25);
                drawList.AddText(textPos, COL_TRANSITION_MAKING, "Click a target state to create transition (ESC to cancel)");
            }
        }

        private void DrawGrid(ImDrawListPtr drawList, Vector2 origin, Vector2 size)
        {
            float gridStep = GRID_SIZE * _zoom;
            if (gridStep < 5f) gridStep *= 5f; // prevent too-fine grid

            float offsetX = _scrollOffset.X % gridStep;
            float offsetY = _scrollOffset.Y % gridStep;

            for (float x = offsetX; x < size.X; x += gridStep)
            {
                drawList.AddLine(origin + new Vector2(x, 0), origin + new Vector2(x, size.Y), COL_GRID);
            }
            for (float y = offsetY; y < size.Y; y += gridStep)
            {
                drawList.AddLine(origin + new Vector2(0, y), origin + new Vector2(size.X, y), COL_GRID);
            }

            // Major grid lines every 5 steps
            float majorStep = gridStep * 5f;
            float majorOffsetX = _scrollOffset.X % majorStep;
            float majorOffsetY = _scrollOffset.Y % majorStep;
            for (float x = majorOffsetX; x < size.X; x += majorStep)
            {
                drawList.AddLine(origin + new Vector2(x, 0), origin + new Vector2(x, size.Y), COL_GRID_MAJOR);
            }
            for (float y = majorOffsetY; y < size.Y; y += majorStep)
            {
                drawList.AddLine(origin + new Vector2(0, y), origin + new Vector2(size.X, y), COL_GRID_MAJOR);
            }
        }

        private void DrawTransitions(ImDrawListPtr drawList, Vector2 canvasOrigin)
        {
            for (int i = 0; i < _controller!.States.Count; i++)
            {
                var state = _controller.States[i];
                Vector2 sourceCenter = GetNodeCenter(state, canvasOrigin);

                for (int t = 0; t < state.Transitions.Count; t++)
                {
                    var trans = state.Transitions[t];
                    if (trans.NextStateIndex < 0 || trans.NextStateIndex >= _controller.States.Count) continue;

                    var targetState = _controller.States[trans.NextStateIndex];
                    Vector2 targetCenter = GetNodeCenter(targetState, canvasOrigin);

                    bool isSelected = (_selectedTransitionStateIdx == i && _selectedTransitionIdx == t);
                    uint color = isSelected ? COL_TRANSITION_SELECTED : COL_TRANSITION;
                    float thickness = isSelected ? 3f * _zoom : 2f * _zoom;

                    // Calculate edge points for a nicer look
                    Vector2 dir = Vector2.Normalize(targetCenter - sourceCenter);
                    Vector2 sourceEdge = sourceCenter + dir * (NODE_WIDTH * 0.5f * _zoom);
                    Vector2 targetEdge = targetCenter - dir * (NODE_WIDTH * 0.5f * _zoom);

                    // Offset for bidirectional transitions
                    Vector2 perpendicular = new Vector2(-dir.Y, dir.X) * 8f * _zoom;
                    sourceEdge += perpendicular;
                    targetEdge += perpendicular;

                    // Control points for bezier
                    float dist = Vector2.Distance(sourceEdge, targetEdge);
                    float tangentLen = Math.Min(dist * 0.4f, 80f * _zoom);
                    Vector2 cp1 = sourceEdge + dir * tangentLen;
                    Vector2 cp2 = targetEdge - dir * tangentLen;

                    drawList.AddBezierCubic(sourceEdge, cp1, cp2, targetEdge, color, thickness);

                    // Arrow head at target end
                    DrawArrowHead(drawList, targetEdge, dir, color, thickness);

                    // Draw small condition indicator
                    int condCount = trans.BoolParameters.Count + trans.IntParameters.Count + trans.FloatParameters.Count;
                    if (condCount > 0)
                    {
                        Vector2 midPoint = EvalBezier(sourceEdge, cp1, cp2, targetEdge, 0.5f);
                        string condText = condCount.ToString();
                        drawList.AddCircleFilled(midPoint, 10f * _zoom, isSelected ? COL_TRANSITION_SELECTED : COL_NODE_BG);
                        drawList.AddCircle(midPoint, 10f * _zoom, color, 12, thickness);
                        Vector2 textSize = ImGui.CalcTextSize(condText);
                        drawList.AddText(midPoint - textSize * 0.5f, COL_TEXT, condText);
                    }
                }
            }
        }

        private void DrawArrowHead(ImDrawListPtr drawList, Vector2 tip, Vector2 dir, uint color, float thickness)
        {
            float size = ARROW_SIZE * _zoom;
            Vector2 perp = new Vector2(-dir.Y, dir.X);
            Vector2 left = tip - dir * size + perp * size * 0.5f;
            Vector2 right = tip - dir * size - perp * size * 0.5f;
            drawList.AddTriangleFilled(tip, left, right, color);
        }

        private void DrawNodes(ImDrawListPtr drawList, Vector2 canvasOrigin, Vector2 canvasSize)
        {
            for (int i = 0; i < _controller!.States.Count; i++)
            {
                var state = _controller.States[i];
                Vector2 nodePos = canvasOrigin + _scrollOffset + state.EditorPosition * _zoom;
                Vector2 nodeSize = new Vector2(NODE_WIDTH, NODE_HEIGHT) * _zoom;
                Vector2 nodeMax = nodePos + nodeSize;

                bool isDefault = (i == 0);
                bool isSelected = (_selectedState == state);

                // Node background
                drawList.AddRectFilled(nodePos, nodeMax, COL_NODE_BG, NODE_ROUNDING * _zoom);

                // Header bar
                uint headerColor = isDefault ? COL_HEADER_DEFAULT : COL_HEADER_NORMAL;
                Vector2 headerMax = nodePos + new Vector2(nodeSize.X, NODE_HEADER_HEIGHT * _zoom);
                drawList.AddRectFilled(nodePos, headerMax, headerColor, NODE_ROUNDING * _zoom,
                    ImDrawFlags.RoundCornersTop);

                // Border
                uint borderColor = isSelected ? COL_NODE_SELECTED : COL_NODE_BORDER;
                float borderThickness = isSelected ? 2.5f : 1.5f;
                drawList.AddRect(nodePos, nodeMax, borderColor, NODE_ROUNDING * _zoom, ImDrawFlags.None, borderThickness);

                // State name in header
                string displayName = isDefault ? "\u25B6 " + state.Name : state.Name;
                Vector2 textPos = nodePos + new Vector2(8 * _zoom, 3 * _zoom);
                drawList.AddText(textPos, COL_TEXT, displayName);

                // Animation name below header
                string animName = state.animation?.Name ?? "(none)";
                Vector2 animTextPos = nodePos + new Vector2(8 * _zoom, (NODE_HEADER_HEIGHT + 5) * _zoom);
                drawList.AddText(animTextPos, COL_TEXT_DIM, animName);

                // Transition count indicator at bottom-right
                if (state.Transitions.Count > 0)
                {
                    string transText = state.Transitions.Count + " trans";
                    Vector2 transTextSize = ImGui.CalcTextSize(transText);
                    Vector2 transPos = nodeMax - new Vector2(transTextSize.X + 5 * _zoom, transTextSize.Y + 2 * _zoom);
                    drawList.AddText(transPos, COL_TEXT_DIM, transText);
                }
            }
        }

        private void HandleInteraction(Vector2 canvasOrigin, Vector2 canvasSize, bool isCanvasHovered)
        {
            Vector2 mousePos = ImGui.GetIO().MousePos;

            // Cancel making transition on Escape
            if (_isMakingTransition && ImGui.IsKeyPressed(ImGuiKey.Escape))
            {
                _isMakingTransition = false;
                _transitionSourceIndex = -1;
            }

            // ── Left click ──
            if (isCanvasHovered && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                int clickedNode = GetNodeAtPosition(mousePos, canvasOrigin);

                if (_isMakingTransition)
                {
                    // Complete transition creation
                    if (clickedNode >= 0 && clickedNode != _transitionSourceIndex)
                    {
                        var trans = new StateTransition { NextStateIndex = clickedNode };
                        _controller!.States[_transitionSourceIndex].Transitions.Add(trans);
                    }
                    _isMakingTransition = false;
                    _transitionSourceIndex = -1;
                }
                else if (clickedNode >= 0)
                {
                    // Select node
                    _selectedState = _controller!.States[clickedNode];
                    _selectedTransitionStateIdx = -1;
                    _selectedTransitionIdx = -1;

                    // Start dragging
                    _isDraggingNode = true;
                    _draggedNodeIndex = clickedNode;
                }
                else
                {
                    // Check if clicked on a transition
                    bool hitTransition = TrySelectTransition(mousePos, canvasOrigin);
                    if (!hitTransition)
                    {
                        _selectedState = null;
                        _selectedTransitionStateIdx = -1;
                        _selectedTransitionIdx = -1;
                    }
                }
            }

            // ── Dragging node ──
            if (_isDraggingNode && _draggedNodeIndex >= 0 && _draggedNodeIndex < _controller!.States.Count)
            {
                if (ImGui.IsMouseDown(ImGuiMouseButton.Left))
                {
                    Vector2 delta = ImGui.GetIO().MouseDelta;
                    _controller.States[_draggedNodeIndex].EditorPosition += delta / _zoom;
                }
                else
                {
                    _isDraggingNode = false;
                    _draggedNodeIndex = -1;
                }
            }

            // ── Right click context menu ──
            if (isCanvasHovered && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                int clickedNode = GetNodeAtPosition(mousePos, canvasOrigin);
                if (clickedNode >= 0)
                {
                    _selectedState = _controller!.States[clickedNode];
                    ImGui.OpenPopup("NodeContextMenu");
                }
                else
                {
                    ImGui.OpenPopup("CanvasContextMenu");
                }
            }

            // ── Node context menu ──
            if (ImGui.BeginPopup("NodeContextMenu"))
            {
                if (_selectedState != null)
                {
                    ImGui.TextColored(new Vector4(0.8f, 0.8f, 0.3f, 1f), _selectedState.Name);
                    ImGui.Separator();

                    int stateIdx = _controller!.States.IndexOf(_selectedState);

                    if (ImGui.MenuItem("Make Transition"))
                    {
                        _isMakingTransition = true;
                        _transitionSourceIndex = stateIdx;
                    }

                    if (stateIdx > 0 && ImGui.MenuItem("Set as Default"))
                    {
                        // Move to index 0
                        var state = _controller.States[stateIdx];
                        _controller.States.RemoveAt(stateIdx);
                        _controller.States.Insert(0, state);
                        // Fix all transition indices
                        FixTransitionIndices(stateIdx, 0);
                    }

                    if (ImGui.MenuItem("Rename"))
                    {
                        // Will be handled via the inspector
                    }

                    ImGui.Separator();
                    if (ImGui.MenuItem("Delete State"))
                    {
                        RemoveState(stateIdx);
                        _selectedState = null;
                    }
                }
                ImGui.EndPopup();
            }

            // ── Canvas context menu ──
            if (ImGui.BeginPopup("CanvasContextMenu"))
            {
                if (ImGui.MenuItem("Add State"))
                {
                    Vector2 newPos = (mousePos - canvasOrigin - _scrollOffset) / _zoom;
                    var newState = new AnimationState
                    {
                        Name = "New State " + _controller!.States.Count,
                        EditorPosition = newPos
                    };
                    _controller.States.Add(newState);
                    _selectedState = newState;
                }
                ImGui.EndPopup();
            }
        }

        // ════════════════════════════════════════════════════════
        //  RIGHT INSPECTOR PANEL (State or Transition)
        // ════════════════════════════════════════════════════════

        private void RenderBottomPanel()
        {
            if (_selectedState != null)
            {
                RenderStateInspector();
            }
            else if (_selectedTransitionIdx >= 0)
            {
                RenderTransitionInspector();
            }
            else
            {
                ImGui.TextDisabled("Select a state or transition to edit");
            }
        }

        private void RenderStateInspector()
        {
            int stateIdx = _controller!.States.IndexOf(_selectedState!);
            bool isDefault = stateIdx == 0;

            // ── Header ──
            ImGui.TextColored(new Vector4(0.85f, 0.5f, 0.15f, 1f), isDefault ? "\u25B6 Default State" : "State");
            ImGui.Separator();
            ImGui.Spacing();

            // ── Name editing ──
            ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1f), "Name");
            string name = _selectedState!.Name;
            ImGui.SetNextItemWidth(-1);
            if (ImGui.InputText("##StateName", ref name, 64))
            {
                _selectedState.Name = name;
            }

            ImGui.Spacing();

            // ── Animation assignment ──
            ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1f), "Animation");
            List<Animation> allAnimations = AssetPool.allAnimations;
            string[] animNames = new string[allAnimations.Count + 1];
            animNames[0] = "(none)";
            for (int i = 0; i < allAnimations.Count; i++)
            {
                animNames[i + 1] = allAnimations[i].Name;
            }

            int selectedIdx = 0;
            if (_selectedState.animation != null)
            {
                for (int i = 0; i < allAnimations.Count; i++)
                {
                    if (_selectedState.animation.Name == allAnimations[i].Name)
                    {
                        selectedIdx = i + 1;
                        break;
                    }
                }
            }

            ImGui.SetNextItemWidth(-1);
            if (ImGui.Combo("##StateAnim", ref selectedIdx, animNames, animNames.Length))
            {
                _selectedState.animation = selectedIdx > 0 ? allAnimations[selectedIdx - 1] : null;
            }

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            // ── Transitions from this state ──
            ImGui.TextColored(new Vector4(0.7f, 0.7f, 0.7f, 1f), "Transitions (" + _selectedState.Transitions.Count + ")");

            for (int t = 0; t < _selectedState.Transitions.Count; t++)
            {
                var trans = _selectedState.Transitions[t];
                string targetName = trans.NextStateIndex >= 0 && trans.NextStateIndex < _controller.States.Count
                    ? _controller.States[trans.NextStateIndex].Name : "???";
                int condCount = trans.BoolParameters.Count + trans.IntParameters.Count + trans.FloatParameters.Count;

                ImGui.PushID("st_" + t);

                // Clickable transition row
                bool clicked = ImGui.Selectable("\u2192 " + targetName + " (" + condCount + " cond)",
                    _selectedTransitionStateIdx == stateIdx && _selectedTransitionIdx == t);
                if (clicked)
                {
                    _selectedTransitionStateIdx = stateIdx;
                    _selectedTransitionIdx = t;
                    _selectedState = null; // Switch to transition view
                }

                ImGui.PopID();
            }

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            // ── Actions ──
            if (!isDefault && stateIdx >= 0)
            {
                if (ImGui.Button("Set as Default", new Vector2(-1, 0)))
                {
                    var state = _controller.States[stateIdx];
                    _controller.States.RemoveAt(stateIdx);
                    _controller.States.Insert(0, state);
                    FixTransitionIndices(stateIdx, 0);
                }
                ImGui.Spacing();
            }

            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.7f, 0.15f, 0.15f, 1f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.9f, 0.2f, 0.2f, 1f));
            if (ImGui.Button("Delete State", new Vector2(-1, 0)))
            {
                RemoveState(stateIdx);
                _selectedState = null;
            }
            ImGui.PopStyleColor(2);
        }

        private void RenderTransitionInspector()
        {
            ImGui.TextColored(new Vector4(0.3f, 0.7f, 1f, 1f), "Transition");
            ImGui.Separator();

            if (_selectedTransitionStateIdx < 0 || _selectedTransitionIdx < 0 ||
                _selectedTransitionStateIdx >= _controller!.States.Count)
            {
                ImGui.TextDisabled("No transition selected");
                return;
            }

            var state = _controller.States[_selectedTransitionStateIdx];
            if (_selectedTransitionIdx >= state.Transitions.Count)
            {
                ImGui.TextDisabled("Invalid transition");
                return;
            }

            var trans = state.Transitions[_selectedTransitionIdx];

            // Back button to return to state view
            if (ImGui.SmallButton("\u25C0 Back to " + state.Name))
            {
                _selectedState = state;
                _selectedTransitionStateIdx = -1;
                _selectedTransitionIdx = -1;
                return;
            }

            ImGui.Spacing();

            // Source → Target label
            string sourceName = state.Name;
            string targetName = trans.NextStateIndex >= 0 && trans.NextStateIndex < _controller.States.Count
                ? _controller.States[trans.NextStateIndex].Name : "???";
            ImGui.TextColored(new Vector4(0.6f, 0.8f, 1f, 1f), sourceName + "  \u2192  " + targetName);
            ImGui.Spacing();

            // ── Conditions ──
            ImGui.TextColored(new Vector4(0.9f, 0.9f, 0.5f, 1f), "Conditions");
            ImGui.Separator();

            // Bool conditions
            List<string> boolToRemove = new();
            foreach (var key in trans.BoolParameters.Keys.ToList())
            {
                bool val = trans.BoolParameters[key];
                ImGui.PushID("tc_b_" + key);
                ImGui.Text("Bool:");
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(0.6f, 0.8f, 1f, 1f), key);
                ImGui.SameLine();
                string boolLabel = val ? "= true" : "= false";
                if (ImGui.SmallButton(boolLabel))
                {
                    trans.BoolParameters[key] = !val;
                }
                ImGui.SameLine();
                if (ImGui.SmallButton("x"))
                {
                    boolToRemove.Add(key);
                }
                ImGui.PopID();
            }
            foreach (var k in boolToRemove) trans.BoolParameters.Remove(k);

            // Int conditions
            List<string> intToRemove = new();
            foreach (var key in trans.IntParameters.Keys.ToList())
            {
                int val = trans.IntParameters[key];
                ImGui.PushID("tc_i_" + key);
                ImGui.Text("Int:");
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(0.6f, 1f, 0.6f, 1f), key);
                ImGui.SameLine();
                ImGui.SetNextItemWidth(60);
                if (ImGui.DragInt("##val", ref val))
                {
                    trans.IntParameters[key] = val;
                }
                ImGui.SameLine();
                if (ImGui.SmallButton("x"))
                {
                    intToRemove.Add(key);
                }
                ImGui.PopID();
            }
            foreach (var k in intToRemove) trans.IntParameters.Remove(k);

            // Float conditions
            List<string> floatToRemove = new();
            foreach (var key in trans.FloatParameters.Keys.ToList())
            {
                float val = trans.FloatParameters[key];
                ImGui.PushID("tc_f_" + key);
                ImGui.Text("Float:");
                ImGui.SameLine();
                ImGui.TextColored(new Vector4(1f, 0.8f, 0.6f, 1f), key);
                ImGui.SameLine();
                ImGui.SetNextItemWidth(60);
                if (ImGui.DragFloat("##val", ref val, 0.1f))
                {
                    trans.FloatParameters[key] = val;
                }
                ImGui.SameLine();
                if (ImGui.SmallButton("x"))
                {
                    floatToRemove.Add(key);
                }
                ImGui.PopID();
            }
            foreach (var k in floatToRemove) trans.FloatParameters.Remove(k);

            ImGui.Spacing();
            ImGui.Separator();

            // ── Add condition from existing parameters ──
            ImGui.TextColored(new Vector4(0.5f, 0.8f, 0.5f, 1f), "Add Condition");

            foreach (var key in _controller.Bool.Keys)
            {
                if (!trans.BoolParameters.ContainsKey(key))
                {
                    ImGui.PushID("add_b_" + key);
                    if (ImGui.SmallButton("+ Bool: " + key))
                    {
                        trans.BoolParameters[key] = true;
                    }
                    ImGui.PopID();
                }
            }
            foreach (var key in _controller.Int.Keys)
            {
                if (!trans.IntParameters.ContainsKey(key))
                {
                    ImGui.PushID("add_i_" + key);
                    if (ImGui.SmallButton("+ Int: " + key))
                    {
                        trans.IntParameters[key] = 0;
                    }
                    ImGui.PopID();
                }
            }
            foreach (var key in _controller.Float.Keys)
            {
                if (!trans.FloatParameters.ContainsKey(key))
                {
                    ImGui.PushID("add_f_" + key);
                    if (ImGui.SmallButton("+ Float: " + key))
                    {
                        trans.FloatParameters[key] = 0f;
                    }
                    ImGui.PopID();
                }
            }

            ImGui.Spacing();
            ImGui.Separator();

            // Delete transition button
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.7f, 0.15f, 0.15f, 1f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.9f, 0.2f, 0.2f, 1f));
            if (ImGui.Button("Delete Transition", new Vector2(-1, 0)))
            {
                state.Transitions.RemoveAt(_selectedTransitionIdx);
                _selectedTransitionStateIdx = -1;
                _selectedTransitionIdx = -1;
            }
            ImGui.PopStyleColor(2);
        }



        // ════════════════════════════════════════════════════════
        //  HELPERS
        // ════════════════════════════════════════════════════════

        private Vector2 GetNodeCenter(AnimationState state, Vector2 canvasOrigin)
        {
            Vector2 nodePos = canvasOrigin + _scrollOffset + state.EditorPosition * _zoom;
            return nodePos + new Vector2(NODE_WIDTH, NODE_HEIGHT) * 0.5f * _zoom;
        }

        private int GetNodeAtPosition(Vector2 mousePos, Vector2 canvasOrigin)
        {
            // Iterate in reverse so topmost drawn node is picked first
            for (int i = _controller!.States.Count - 1; i >= 0; i--)
            {
                var state = _controller.States[i];
                Vector2 nodePos = canvasOrigin + _scrollOffset + state.EditorPosition * _zoom;
                Vector2 nodeSize = new Vector2(NODE_WIDTH, NODE_HEIGHT) * _zoom;
                Vector2 nodeMax = nodePos + nodeSize;

                if (mousePos.X >= nodePos.X && mousePos.X <= nodeMax.X &&
                    mousePos.Y >= nodePos.Y && mousePos.Y <= nodeMax.Y)
                {
                    return i;
                }
            }
            return -1;
        }

        private bool TrySelectTransition(Vector2 mousePos, Vector2 canvasOrigin)
        {
            float bestDist = TRANSITION_CLICK_THRESHOLD * _zoom;
            int bestStateIdx = -1;
            int bestTransIdx = -1;

            for (int i = 0; i < _controller!.States.Count; i++)
            {
                var state = _controller.States[i];
                Vector2 sourceCenter = GetNodeCenter(state, canvasOrigin);

                for (int t = 0; t < state.Transitions.Count; t++)
                {
                    var trans = state.Transitions[t];
                    if (trans.NextStateIndex < 0 || trans.NextStateIndex >= _controller.States.Count) continue;

                    var targetState = _controller.States[trans.NextStateIndex];
                    Vector2 targetCenter = GetNodeCenter(targetState, canvasOrigin);

                    // Sample bezier at multiple points and find closest
                    Vector2 dir = Vector2.Normalize(targetCenter - sourceCenter);
                    Vector2 sourceEdge = sourceCenter + dir * (NODE_WIDTH * 0.5f * _zoom);
                    Vector2 targetEdge = targetCenter - dir * (NODE_WIDTH * 0.5f * _zoom);
                    Vector2 perpendicular = new Vector2(-dir.Y, dir.X) * 8f * _zoom;
                    sourceEdge += perpendicular;
                    targetEdge += perpendicular;

                    float dist = Vector2.Distance(sourceEdge, targetEdge);
                    float tangentLen = Math.Min(dist * 0.4f, 80f * _zoom);
                    Vector2 cp1 = sourceEdge + dir * tangentLen;
                    Vector2 cp2 = targetEdge - dir * tangentLen;

                    for (float s = 0; s <= 1f; s += 0.05f)
                    {
                        Vector2 point = EvalBezier(sourceEdge, cp1, cp2, targetEdge, s);
                        float d = Vector2.Distance(mousePos, point);
                        if (d < bestDist)
                        {
                            bestDist = d;
                            bestStateIdx = i;
                            bestTransIdx = t;
                        }
                    }
                }
            }

            if (bestStateIdx >= 0)
            {
                _selectedTransitionStateIdx = bestStateIdx;
                _selectedTransitionIdx = bestTransIdx;
                _selectedState = null; // Deselect node when selecting transition
                return true;
            }
            return false;
        }

        private Vector2 EvalBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
        {
            float u = 1f - t;
            return u * u * u * p0 + 3 * u * u * t * p1 + 3 * u * t * t * p2 + t * t * t * p3;
        }

        private void AutoLayoutNewStates()
        {
            float x = 50f;
            float y = 50f;
            int perRow = 4;
            int col = 0;

            for (int i = 0; i < _controller!.States.Count; i++)
            {
                var state = _controller.States[i];
                if (state.EditorPosition == Vector2.Zero && i > 0)
                {
                    state.EditorPosition = new Vector2(x + col * (NODE_WIDTH + 40), y + (i / perRow) * (NODE_HEIGHT + 60));
                    col = (col + 1) % perRow;
                }
                else if (i == 0 && state.EditorPosition == Vector2.Zero)
                {
                    state.EditorPosition = new Vector2(x, y);
                }
            }
        }

        private void RemoveState(int index)
        {
            _controller!.States.RemoveAt(index);

            // Fix up all transition indices
            foreach (var state in _controller.States)
            {
                for (int t = state.Transitions.Count - 1; t >= 0; t--)
                {
                    if (state.Transitions[t].NextStateIndex == index)
                    {
                        state.Transitions.RemoveAt(t);
                    }
                    else if (state.Transitions[t].NextStateIndex > index)
                    {
                        state.Transitions[t].NextStateIndex--;
                    }
                }
            }
        }

        private void FixTransitionIndices(int oldIndex, int newIndex)
        {
            // After moving a state from oldIndex to newIndex, fix all references
            foreach (var state in _controller!.States)
            {
                foreach (var trans in state.Transitions)
                {
                    if (trans.NextStateIndex == oldIndex)
                    {
                        trans.NextStateIndex = newIndex;
                    }
                    else if (oldIndex > newIndex)
                    {
                        // State moved backward: indices between [newIndex, oldIndex) shift up by 1
                        if (trans.NextStateIndex >= newIndex && trans.NextStateIndex < oldIndex)
                        {
                            trans.NextStateIndex++;
                        }
                    }
                    else
                    {
                        // State moved forward: indices between (oldIndex, newIndex] shift down by 1
                        if (trans.NextStateIndex > oldIndex && trans.NextStateIndex <= newIndex)
                        {
                            trans.NextStateIndex--;
                        }
                    }
                }
            }
        }
    }
}
