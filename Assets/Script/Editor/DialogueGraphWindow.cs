using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DialogueGraphWindow : EditorWindow
{
    private enum PortType
    {
        Next,
        ReplaceMain,
        Answer
    }

    private struct DraggingPort
    {
        public DialogueNode SourceNode;
        public PortType Type;
        public int AnswerIndex;
        public Vector2 StartCanvasPos;
        public Color Color;
    }

    private class DialogueNode
    {
        public DialogueData Data;
        public DialogueData TreeRoot;
        public Rect Rect;
        public int Depth;
        public Rect NextPortRect;
        public Rect ReplacePortRect;
        public readonly List<Rect> AnswerPortRects = new List<Rect>();
    }

    private readonly List<DialogueData> rootDialogues = new List<DialogueData>();
    private PNJInfoData sourcePNJ;
    private readonly List<DialogueData> pnjDialogues = new List<DialogueData>();
    private int selectedPnjDialogueIndex = 0; // 0 = Tous

    private readonly Dictionary<DialogueData, DialogueNode> nodes = new Dictionary<DialogueData, DialogueNode>();
    private DialogueData selectedDialogue;
    private SerializedObject selectedSerializedObject;
    private string renameBuffer = "";

    private Vector2 panOffset = Vector2.zero;
    private bool isPanning;
    private DialogueNode draggingNode;
    private bool isDraggingPort;
    private DraggingPort activePort;

    private float inspectorWidth = 420f;
    private bool isResizingInspector;
    private Vector2 inspectorScrollPos;

    private const float ToolbarHeight = 28f;
    private const float NodeWidth = 250f;
    private const float NodeHorizontalSpacing = 100f;
    private const float NodeVerticalSpacing = 35f;
    private const float TreeVerticalSpacing = 80f;
    private const float PortSize = 12f;

    [MenuItem("ShopCrafter/Tools/Dialogue Graph Editor")]
    public static void ShowWindow()
    {
        DialogueGraphWindow window = GetWindow<DialogueGraphWindow>("Dialogue Graph");
        window.minSize = new Vector2(860f, 540f);
    }

    public static void OpenWithPNJ(PNJInfoData pnjInfo)
    {
        DialogueGraphWindow window = GetWindow<DialogueGraphWindow>("Dialogue Graph");
        window.minSize = new Vector2(860f, 540f);
        window.LoadPNJ(pnjInfo);
        window.Focus();
    }

    private void OnGUI()
    {
        inspectorWidth = Mathf.Clamp(inspectorWidth, 280f, Mathf.Max(320f, position.width * 0.65f));

        DrawToolbar();

        float canvasWidth = position.width - inspectorWidth;
        Rect canvasRect = new Rect(0f, ToolbarHeight, canvasWidth, position.height - ToolbarHeight);
        Rect splitterRect = new Rect(canvasWidth - 3f, ToolbarHeight, 6f, position.height - ToolbarHeight);
        Rect inspectorRect = new Rect(canvasWidth, ToolbarHeight, inspectorWidth, position.height - ToolbarHeight);

        DrawCanvas(canvasRect);
        DrawInspectorPanel(inspectorRect);
        HandleSplitter(splitterRect);

        if (isDraggingPort || isPanning || draggingNode != null || isResizingInspector)
        {
            Repaint();
        }
    }

    private void HandleSplitter(Rect splitterRect)
    {
        EditorGUIUtility.AddCursorRect(splitterRect, MouseCursor.ResizeHorizontal);
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0 && splitterRect.Contains(e.mousePosition))
        {
            isResizingInspector = true;
            e.Use();
        }
        else if (e.type == EventType.MouseDrag && isResizingInspector)
        {
            inspectorWidth = Mathf.Clamp(position.width - e.mousePosition.x, 280f, position.width * 0.65f);
            e.Use();
        }
        else if (e.type == EventType.MouseUp && isResizingInspector)
        {
            isResizingInspector = false;
            e.Use();
        }
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(ToolbarHeight));

        EditorGUI.BeginChangeCheck();
        PNJInfoData newPnj = (PNJInfoData)EditorGUILayout.ObjectField(sourcePNJ, typeof(PNJInfoData), false, GUILayout.Width(170));
        if (EditorGUI.EndChangeCheck())
        {
            LoadPNJ(newPnj);
        }

        if (sourcePNJ != null)
        {
            string[] options = new string[pnjDialogues.Count + 1];
            options[0] = $"Tous ({pnjDialogues.Count})";
            for (int i = 0; i < pnjDialogues.Count; i++)
            {
                options[i + 1] = pnjDialogues[i] != null ? $"{i + 1}. {pnjDialogues[i].name}" : $"{i + 1}. (Vide)";
            }

            selectedPnjDialogueIndex = Mathf.Clamp(selectedPnjDialogueIndex, 0, options.Length - 1);

            EditorGUI.BeginChangeCheck();
            selectedPnjDialogueIndex = EditorGUILayout.Popup(selectedPnjDialogueIndex, options, EditorStyles.toolbarPopup, GUILayout.Width(180));
            if (EditorGUI.EndChangeCheck())
            {
                ApplyPNJDialogueFilter();
            }

            if (sourcePNJ.IdentityInfo is PNJIdentityArchetype && GUILayout.Button("+ Ajouter au PNJ", EditorStyles.toolbarButton, GUILayout.Width(105)))
            {
                AddNewDialogueToPNJ();
            }
        }
        else
        {
            DialogueData currentSingle = rootDialogues.Count > 0 ? rootDialogues[0] : null;
            EditorGUI.BeginChangeCheck();
            DialogueData newRoot = (DialogueData)EditorGUILayout.ObjectField(currentSingle, typeof(DialogueData), false, GUILayout.Width(190));
            if (EditorGUI.EndChangeCheck())
            {
                LoadSingleRootDialogue(newRoot);
            }

            if (GUILayout.Button("+ Nouveau Root", EditorStyles.toolbarButton, GUILayout.Width(105)))
            {
                CreateNewRootDialogue();
            }
        }

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Auto-Layout", EditorStyles.toolbarButton, GUILayout.Width(80)))
        {
            RebuildGraph(true);
        }

        if (GUILayout.Button("Recentrer", EditorStyles.toolbarButton, GUILayout.Width(70)))
        {
            panOffset = Vector2.zero;
        }

        if (GUILayout.Button("↻", EditorStyles.toolbarButton, GUILayout.Width(26)))
        {
            if (sourcePNJ != null)
            {
                RefreshPNJDialogues();
                ApplyPNJDialogueFilter();
            }
            else
            {
                RebuildGraph(false);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void LoadPNJ(PNJInfoData pnjInfo)
    {
        sourcePNJ = pnjInfo;
        selectedPnjDialogueIndex = 0;
        RefreshPNJDialogues();

        if (sourcePNJ == null)
        {
            rootDialogues.Clear();
            nodes.Clear();
            SelectDialogue(null);
            return;
        }

        if (pnjDialogues.Count == 0)
        {
            ShowNotification(new GUIContent("Aucun DialogueData trouvé sur ce PNJ."), 1.5f);
        }

        ApplyPNJDialogueFilter();
    }

    private void RefreshPNJDialogues()
    {
        pnjDialogues.Clear();
        if (sourcePNJ == null) return;

        if (sourcePNJ.IdentityInfo is PNJIdentityArchetype archetype && archetype.possibleDialogues != null)
        {
            foreach (DialogueData d in archetype.possibleDialogues)
            {
                if (d != null && !pnjDialogues.Contains(d))
                    pnjDialogues.Add(d);
            }
            return;
        }

        if (sourcePNJ.IdentityInfo != null)
        {
            SerializedObject idSo = new SerializedObject(sourcePNJ.IdentityInfo);
            SerializedProperty prop = idSo.GetIterator();
            while (prop.NextVisible(true))
            {
                if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue is DialogueData d)
                {
                    if (!pnjDialogues.Contains(d))
                        pnjDialogues.Add(d);
                }
            }
        }
    }

    private void ApplyPNJDialogueFilter(bool resetPositions = true)
    {
        rootDialogues.Clear();

        if (selectedPnjDialogueIndex == 0)
        {
            rootDialogues.AddRange(pnjDialogues);
        }
        else
        {
            int idx = selectedPnjDialogueIndex - 1;
            if (idx >= 0 && idx < pnjDialogues.Count && pnjDialogues[idx] != null)
            {
                rootDialogues.Add(pnjDialogues[idx]);
            }
            else
            {
                selectedPnjDialogueIndex = 0;
                rootDialogues.AddRange(pnjDialogues);
            }
        }

        if (selectedDialogue == null || !nodes.ContainsKey(selectedDialogue))
        {
            SelectDialogue(rootDialogues.Count > 0 ? rootDialogues[0] : null);
        }

        RebuildGraph(resetPositions);
        if (resetPositions) panOffset = Vector2.zero;
    }

    private void AddNewDialogueToPNJ()
    {
        if (sourcePNJ == null || !(sourcePNJ.IdentityInfo is PNJIdentityArchetype archetype)) return;

        string folder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(archetype));
        if (string.IsNullOrEmpty(folder)) folder = "Assets";

        string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folder, $"{sourcePNJ.name}_Dialogue{pnjDialogues.Count + 1}.asset"));
        DialogueData created = CreateInstance<DialogueData>();
        AssetDatabase.CreateAsset(created, assetPath);

        Undo.RecordObject(archetype, "Add Dialogue to PNJ Archetype");
        if (archetype.possibleDialogues == null) archetype.possibleDialogues = new List<DialogueData>();
        archetype.possibleDialogues.Add(created);
        EditorUtility.SetDirty(archetype);
        AssetDatabase.SaveAssets();

        RefreshPNJDialogues();
        selectedPnjDialogueIndex = pnjDialogues.IndexOf(created) + 1;
        SelectDialogue(created);
        ApplyPNJDialogueFilter();
    }

    private void RemoveDialogueFromArchetype(DialogueData target, bool deleteAsset)
    {
        if (sourcePNJ == null || !(sourcePNJ.IdentityInfo is PNJIdentityArchetype archetype)) return;

        Undo.RecordObject(archetype, "Remove Dialogue from PNJ Archetype");
        if (archetype.possibleDialogues != null)
        {
            archetype.possibleDialogues.RemoveAll(d => d == target);
        }
        EditorUtility.SetDirty(archetype);

        if (deleteAsset)
        {
            string assetPath = AssetDatabase.GetAssetPath(target);
            UnlinkDialogueReferences(target);
            if (!string.IsNullOrEmpty(assetPath))
            {
                AssetDatabase.DeleteAsset(assetPath);
            }
        }

        AssetDatabase.SaveAssets();
        RefreshPNJDialogues();
        selectedPnjDialogueIndex = 0;
        SelectDialogue(pnjDialogues.Count > 0 ? pnjDialogues[0] : null);
        ApplyPNJDialogueFilter(true);
    }

    private void InsertNewBaseBeforeNode(DialogueNode targetNode)
    {
        DialogueData oldData = targetNode.Data;
        DialogueData newBase = CreateLinkedDialogueAsset(oldData, $"{oldData.name}_Base");

        Undo.RecordObject(newBase, "Link New Base Dialogue");
        newBase.HasNextDialogue = true;
        newBase.NextDialogue = oldData;
        EditorUtility.SetDirty(newBase);

        ReplaceRootReference(oldData, newBase);
        AssetDatabase.SaveAssets();

        if (sourcePNJ != null)
        {
            RefreshPNJDialogues();
            if (selectedPnjDialogueIndex > 0)
            {
                int idx = pnjDialogues.IndexOf(newBase);
                selectedPnjDialogueIndex = idx >= 0 ? idx + 1 : 0;
            }
            SelectDialogue(newBase);
            ApplyPNJDialogueFilter(true);
        }
        else
        {
            SelectDialogue(newBase);
            RebuildGraph(true);
        }
    }

    private void PromoteNodeToRoot(DialogueNode targetNode)
    {
        DialogueData newRoot = targetNode.Data;
        DialogueData oldRoot = targetNode.TreeRoot;

        if (oldRoot == null || oldRoot == newRoot) return;

        // Si l'ancien root pointait en Next vers le nouveau root, on inverse le lien proprement
        if (oldRoot.HasNextDialogue && oldRoot.NextDialogue == newRoot)
        {
            Undo.RecordObject(oldRoot, "Swap Root Next Link");
            Undo.RecordObject(newRoot, "Swap Root Next Link");

            oldRoot.NextDialogue = null;
            oldRoot.HasNextDialogue = false;

            if (!newRoot.HasNextDialogue || newRoot.NextDialogue == null)
            {
                newRoot.HasNextDialogue = true;
                newRoot.NextDialogue = oldRoot;
            }

            EditorUtility.SetDirty(oldRoot);
            EditorUtility.SetDirty(newRoot);
        }

        ReplaceRootReference(oldRoot, newRoot);
        AssetDatabase.SaveAssets();

        if (sourcePNJ != null)
        {
            RefreshPNJDialogues();
            if (selectedPnjDialogueIndex > 0)
            {
                int idx = pnjDialogues.IndexOf(newRoot);
                selectedPnjDialogueIndex = idx >= 0 ? idx + 1 : 0;
            }
            SelectDialogue(newRoot);
            ApplyPNJDialogueFilter(true);
        }
        else
        {
            SelectDialogue(newRoot);
            RebuildGraph(true);
        }
    }

    private void ReplaceRootReference(DialogueData oldRoot, DialogueData newRoot)
    {
        if (sourcePNJ != null && sourcePNJ.IdentityInfo is PNJIdentityArchetype archetype && archetype.possibleDialogues != null)
        {
            Undo.RecordObject(archetype, "Replace Root Dialogue in Archetype");
            int idx = archetype.possibleDialogues.IndexOf(oldRoot);
            if (idx >= 0)
            {
                archetype.possibleDialogues[idx] = newRoot;
            }
            else if (!archetype.possibleDialogues.Contains(newRoot))
            {
                archetype.possibleDialogues.Add(newRoot);
            }
            EditorUtility.SetDirty(archetype);
        }
        else
        {
            int idx = rootDialogues.IndexOf(oldRoot);
            if (idx >= 0)
            {
                rootDialogues[idx] = newRoot;
            }
            else if (!rootDialogues.Contains(newRoot))
            {
                rootDialogues.Add(newRoot);
            }
        }
    }

    private void CreateNewRootDialogue()
    {
        string path = EditorUtility.SaveFilePanelInProject("Nouveau DialogueData", "NewDialogue", "asset", "Choisis l'emplacement du DialogueData");
        if (string.IsNullOrEmpty(path)) return;

        DialogueData created = CreateInstance<DialogueData>();
        AssetDatabase.CreateAsset(created, path);
        AssetDatabase.SaveAssets();
        sourcePNJ = null;
        LoadSingleRootDialogue(created);
    }

    private void LoadSingleRootDialogue(DialogueData dialogue)
    {
        rootDialogues.Clear();
        if (dialogue != null) rootDialogues.Add(dialogue);
        SelectDialogue(dialogue);
        RebuildGraph(true);
        panOffset = Vector2.zero;
    }

    private void SelectDialogue(DialogueData dialogue)
    {
        selectedDialogue = dialogue;
        selectedSerializedObject = dialogue != null ? new SerializedObject(dialogue) : null;
        renameBuffer = dialogue != null ? dialogue.name : "";
    }

    private void RebuildGraph(bool resetPositions, Dictionary<DialogueData, Vector2> overridePositions = null)
    {
        Dictionary<DialogueData, Vector2> savedPositions = new Dictionary<DialogueData, Vector2>();
        if (!resetPositions)
        {
            foreach (var kvp in nodes)
            {
                if (kvp.Key != null)
                    savedPositions[kvp.Key] = kvp.Value.Rect.position;
            }
        }

        if (overridePositions != null)
        {
            foreach (var kvp in overridePositions)
                savedPositions[kvp.Key] = kvp.Value;
        }

        nodes.Clear();
        if (rootDialogues.Count == 0) return;

        float currentTreeStartY = 40f;

        foreach (DialogueData root in rootDialogues)
        {
            if (root == null || nodes.ContainsKey(root)) continue;

            Queue<(DialogueData data, int depth)> queue = new Queue<(DialogueData, int)>();
            Dictionary<int, List<DialogueNode>> columns = new Dictionary<int, List<DialogueNode>>();

            queue.Enqueue((root, 0));

            while (queue.Count > 0)
            {
                var (current, depth) = queue.Dequeue();
                if (current == null || nodes.ContainsKey(current)) continue;

                DialogueNode node = new DialogueNode
                {
                    Data = current,
                    TreeRoot = root,
                    Depth = depth,
                    Rect = new Rect(0f, 0f, NodeWidth, EstimateNodeHeight(current))
                };

                nodes.Add(current, node);

                if (!columns.ContainsKey(depth))
                    columns[depth] = new List<DialogueNode>();
                columns[depth].Add(node);

                if (current.HasNextDialogue && current.NextDialogue != null)
                    queue.Enqueue((current.NextDialogue, depth + 1));

                if (current.HasQuestion && current.Answers != null)
                {
                    foreach (Answer answer in current.Answers)
                    {
                        if (answer.AnswerDialogueData != null)
                            queue.Enqueue((answer.AnswerDialogueData, depth + 1));
                        if (answer.ReplaceMainDialogue != null)
                            queue.Enqueue((answer.ReplaceMainDialogue, depth + 1));
                    }
                }

                if (current.ReplaceMainDialogue != null)
                    queue.Enqueue((current.ReplaceMainDialogue, depth + 1));
            }

            float maxTreeY = currentTreeStartY;

            foreach (var kvp in columns)
            {
                int col = kvp.Key;
                float y = currentTreeStartY;

                foreach (DialogueNode node in kvp.Value)
                {
                    if ((!resetPositions || overridePositions != null) && savedPositions.TryGetValue(node.Data, out Vector2 pos))
                    {
                        node.Rect.position = pos;
                    }
                    else
                    {
                        node.Rect.position = new Vector2(40f + col * (NodeWidth + NodeHorizontalSpacing), y);
                        y += node.Rect.height + NodeVerticalSpacing;
                    }

                    maxTreeY = Mathf.Max(maxTreeY, node.Rect.yMax);
                }
            }

            currentTreeStartY = maxTreeY + TreeVerticalSpacing;
        }
    }

    private float EstimateNodeHeight(DialogueData data)
    {
        float height = 56f;
        int lineCount = data.Lines != null ? Mathf.Min(data.Lines.Count, 3) : 0;
        height += Mathf.Max(1, lineCount) * 18f;
        if (data.Lines != null && data.Lines.Count > 3) height += 14f;

        height += 22f; // Row Next
        height += 22f; // Row Replace Main

        if (data.HasQuestion && data.Answers != null && data.Answers.Count > 0)
        {
            height += 6f + data.Answers.Count * 22f;
        }

        return Mathf.Max(115f, height);
    }

    private void DrawCanvas(Rect canvasRect)
    {
        GUI.BeginGroup(canvasRect);

        EditorGUI.DrawRect(new Rect(0, 0, canvasRect.width, canvasRect.height), new Color(0.16f, 0.16f, 0.16f));
        DrawGrid(canvasRect, 20f, 0.15f, Color.gray);
        DrawGrid(canvasRect, 100f, 0.3f, Color.gray);

        if (rootDialogues.Count == 0)
        {
            GUIStyle centerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                normal = { textColor = new Color(0.7f, 0.7f, 0.7f) }
            };
            GUI.Label(new Rect(0, 0, canvasRect.width, canvasRect.height - 40f), "Glisse un PNJInfoData ou un DialogueData dans la barre du haut.", centerStyle);

            if (GUI.Button(new Rect(canvasRect.width * 0.5f - 90f, canvasRect.height * 0.5f + 10f, 180f, 30f), "+ Créer un DialogueData"))
            {
                CreateNewRootDialogue();
            }

            GUI.EndGroup();
            return;
        }

        foreach (DialogueNode node in nodes.Values)
        {
            ComputeNodePortRects(node);
        }

        HandleCanvasEvents();
        DrawConnections();

        if (isDraggingPort)
        {
            Handles.BeginGUI();
            DrawBezierConnection(activePort.StartCanvasPos, Event.current.mousePosition, activePort.Color, "");
            Handles.EndGUI();
        }

        foreach (DialogueNode node in new List<DialogueNode>(nodes.Values))
        {
            DrawNode(node);
        }

        GUI.EndGroup();
    }

    private void ComputeNodePortRects(DialogueNode node)
    {
        DialogueData data = node.Data;
        node.Rect.height = EstimateNodeHeight(data);
        Rect screenRect = new Rect(node.Rect.position + panOffset, node.Rect.size);

        float y = screenRect.y + 28f;
        int lineCount = data.Lines != null ? data.Lines.Count : 0;
        y += Mathf.Max(1, Mathf.Min(lineCount, 3)) * 18f;
        if (lineCount > 3) y += 14f;

        node.NextPortRect = new Rect(screenRect.xMax - PortSize * 0.5f, y + 3f, PortSize, PortSize);
        y += 22f;

        node.ReplacePortRect = new Rect(screenRect.xMax - PortSize * 0.5f, y + 3f, PortSize, PortSize);
        y += 22f;

        node.AnswerPortRects.Clear();
        if (data.HasQuestion && data.Answers != null && data.Answers.Count > 0)
        {
            y += 6f;
            for (int i = 0; i < data.Answers.Count; i++)
            {
                node.AnswerPortRects.Add(new Rect(screenRect.xMax - PortSize * 0.5f, y + 4f, PortSize, PortSize));
                y += 22f;
            }
        }
    }

    private void DrawGrid(Rect rect, float spacing, float opacity, Color color)
    {
        int widthDivs = Mathf.CeilToInt(rect.width / spacing);
        int heightDivs = Mathf.CeilToInt(rect.height / spacing);

        Handles.BeginGUI();
        Handles.color = new Color(color.r, color.g, color.b, opacity);

        Vector3 offset = new Vector3(panOffset.x % spacing, panOffset.y % spacing, 0f);

        for (int i = 0; i <= widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(spacing * i + offset.x, 0f, 0f), new Vector3(spacing * i + offset.x, rect.height, 0f));
        }
        for (int j = 0; j <= heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(0f, spacing * j + offset.y, 0f), new Vector3(rect.width, spacing * j + offset.y, 0f));
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void HandleCanvasEvents()
    {
        Event e = Event.current;

        if (e.type == EventType.MouseDown)
        {
            if (e.button == 2 || (e.button == 0 && e.alt))
            {
                isPanning = true;
                e.Use();
            }
            else if (e.button == 0)
            {
                if (TryBeginPortDrag(e.mousePosition))
                {
                    e.Use();
                    return;
                }

                DialogueNode clickedNode = GetNodeAtPosition(e.mousePosition);
                if (clickedNode != null)
                {
                    SelectDialogue(clickedNode.Data);
                    draggingNode = clickedNode;
                    GUI.FocusControl(null);
                    e.Use();
                }
            }
            else if (e.button == 1)
            {
                DialogueNode clickedNode = GetNodeAtPosition(e.mousePosition);
                if (clickedNode != null)
                {
                    SelectDialogue(clickedNode.Data);
                    ShowNodeContextMenu(clickedNode);
                }
                else
                {
                    ShowCanvasContextMenu();
                }
                e.Use();
            }
        }
        else if (e.type == EventType.MouseDrag)
        {
            if (isPanning)
            {
                panOffset += e.delta;
                e.Use();
            }
            else if (draggingNode != null)
            {
                draggingNode.Rect.position += e.delta;
                e.Use();
            }
            else if (isDraggingPort)
            {
                e.Use();
            }
        }
        else if (e.type == EventType.MouseUp)
        {
            if (isDraggingPort)
            {
                CompletePortDrag(e.mousePosition);
                isDraggingPort = false;
                e.Use();
            }
            isPanning = false;
            draggingNode = null;
        }
    }

    private bool TryBeginPortDrag(Vector2 mousePos)
    {
        foreach (DialogueNode node in nodes.Values)
        {
            if (ExpandRect(node.NextPortRect, 4f).Contains(mousePos))
            {
                isDraggingPort = true;
                activePort = new DraggingPort
                {
                    SourceNode = node,
                    Type = PortType.Next,
                    StartCanvasPos = node.NextPortRect.center,
                    Color = new Color(0.35f, 0.75f, 1f)
                };
                return true;
            }

            if (ExpandRect(node.ReplacePortRect, 4f).Contains(mousePos))
            {
                isDraggingPort = true;
                activePort = new DraggingPort
                {
                    SourceNode = node,
                    Type = PortType.ReplaceMain,
                    StartCanvasPos = node.ReplacePortRect.center,
                    Color = new Color(1f, 0.7f, 0.25f)
                };
                return true;
            }

            for (int i = 0; i < node.AnswerPortRects.Count; i++)
            {
                if (ExpandRect(node.AnswerPortRects[i], 4f).Contains(mousePos))
                {
                    isDraggingPort = true;
                    activePort = new DraggingPort
                    {
                        SourceNode = node,
                        Type = PortType.Answer,
                        AnswerIndex = i,
                        StartCanvasPos = node.AnswerPortRects[i].center,
                        Color = new Color(0.45f, 0.9f, 0.5f)
                    };
                    return true;
                }
            }
        }
        return false;
    }

    private void CompletePortDrag(Vector2 mousePos)
    {
        DialogueNode targetNode = GetNodeAtPosition(mousePos);
        DialogueData sourceData = activePort.SourceNode.Data;
        DialogueData targetData;
        Dictionary<DialogueData, Vector2> overridePos = null;

        if (targetNode != null && targetNode != activePort.SourceNode)
        {
            targetData = targetNode.Data;
        }
        else if (targetNode == null)
        {
            string suffix = activePort.Type == PortType.Next
                ? "_Next"
                : (activePort.Type == PortType.ReplaceMain ? "_Replace" : $"_Ans{activePort.AnswerIndex + 1}");

            targetData = CreateLinkedDialogueAsset(sourceData, sourceData.name + suffix);
            overridePos = new Dictionary<DialogueData, Vector2>
            {
                { targetData, mousePos - panOffset - new Vector2(0f, 20f) }
            };
        }
        else
        {
            return;
        }

        Undo.RecordObject(sourceData, "Connect Dialogue Port");

        switch (activePort.Type)
        {
            case PortType.Next:
                sourceData.HasNextDialogue = true;
                sourceData.NextDialogue = targetData;
                break;
            case PortType.ReplaceMain:
                sourceData.ReplaceMainDialogue = targetData;
                break;
            case PortType.Answer:
                if (sourceData.Answers != null && activePort.AnswerIndex < sourceData.Answers.Count)
                {
                    sourceData.Answers[activePort.AnswerIndex].AnswerDialogueData = targetData;
                }
                break;
        }

        EditorUtility.SetDirty(sourceData);
        SelectDialogue(targetData);
        RebuildGraph(false, overridePos);
    }

    private Rect ExpandRect(Rect r, float amount)
    {
        return new Rect(r.x - amount, r.y - amount, r.width + amount * 2f, r.height + amount * 2f);
    }

    private void ShowNodeContextMenu(DialogueNode node)
    {
        GenericMenu menu = new GenericMenu();
        DialogueData data = node.Data;
        bool isRoot = rootDialogues.Contains(data);
        bool hasArchetype = sourcePNJ != null && sourcePNJ.IdentityInfo is PNJIdentityArchetype;

        menu.AddItem(new GUIContent("Ajouter une Ligne"), false, () =>
        {
            Undo.RecordObject(data, "Add Dialogue Line");
            data.Lines.Add(new LineData { Line = "Nouvelle ligne..." });
            EditorUtility.SetDirty(data);
            SelectDialogue(data);
            RebuildGraph(false);
        });

        menu.AddItem(new GUIContent("Ajouter une Réponse + Créer Nœud lié"), false, () =>
        {
            Undo.RecordObject(data, "Add Answer & Linked Node");
            data.HasQuestion = true;
            DialogueData child = CreateLinkedDialogueAsset(data, $"{data.name}_Ans{data.Answers.Count + 1}");
            data.Answers.Add(new Answer
            {
                Line = $"Réponse {data.Answers.Count + 1}",
                AnswerDialogueData = child
            });
            EditorUtility.SetDirty(data);
            SelectDialogue(child);
            RebuildGraph(false);
        });

        menu.AddSeparator("");

        if (isRoot)
        {
            menu.AddItem(new GUIContent("Insérer un nouveau Dialogue avant (Nouveau Root)"), false, () =>
            {
                InsertNewBaseBeforeNode(node);
            });
        }
        else
        {
            menu.AddItem(new GUIContent("Définir comme Root (Base)"), false, () =>
            {
                PromoteNodeToRoot(node);
            });
        }

        menu.AddSeparator("");

        if (isRoot && hasArchetype)
        {
            menu.AddItem(new GUIContent("Retirer de l'archétype PNJ"), false, () =>
            {
                RemoveDialogueFromArchetype(data, false);
            });

            menu.AddItem(new GUIContent("Retirer de l'archétype & Supprimer l'asset"), false, () =>
            {
                if (EditorUtility.DisplayDialog("Supprimer le DialogueData ?", $"Retirer '{data.name}' de l'archétype et supprimer définitivement le fichier ?", "Supprimer", "Annuler"))
                {
                    RemoveDialogueFromArchetype(data, true);
                }
            });
        }
        else if (!isRoot)
        {
            menu.AddItem(new GUIContent("Supprimer le fichier Asset"), false, () =>
            {
                if (EditorUtility.DisplayDialog("Supprimer le DialogueData ?", $"Supprimer définitivement '{data.name}' du projet ?", "Supprimer", "Annuler"))
                {
                    string assetPath = AssetDatabase.GetAssetPath(data);
                    UnlinkDialogueReferences(data);
                    AssetDatabase.DeleteAsset(assetPath);
                    AssetDatabase.SaveAssets();
                    SelectDialogue(rootDialogues.Count > 0 ? rootDialogues[0] : null);
                    RebuildGraph(false);
                }
            });
        }

        menu.ShowAsContext();
    }

    private void ShowCanvasContextMenu()
    {
        GenericMenu menu = new GenericMenu();
        if (sourcePNJ != null && sourcePNJ.IdentityInfo is PNJIdentityArchetype)
        {
            menu.AddItem(new GUIContent("+ Ajouter un Dialogue à ce PNJ"), false, AddNewDialogueToPNJ);
        }
        else
        {
            menu.AddItem(new GUIContent("Créer un nouveau Dialogue (Root)"), false, CreateNewRootDialogue);
        }
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Auto-Layout"), false, () => RebuildGraph(true));
        menu.AddItem(new GUIContent("Recentrer la vue"), false, () => panOffset = Vector2.zero);
        menu.ShowAsContext();
    }

    private void UnlinkDialogueReferences(DialogueData target)
    {
        foreach (DialogueNode node in nodes.Values)
        {
            DialogueData d = node.Data;
            if (d == null || d == target) continue;

            bool dirty = false;
            if (d.NextDialogue == target) { d.NextDialogue = null; d.HasNextDialogue = false; dirty = true; }
            if (d.ReplaceMainDialogue == target) { d.ReplaceMainDialogue = null; dirty = true; }
            if (d.Answers != null)
            {
                foreach (Answer ans in d.Answers)
                {
                    if (ans.AnswerDialogueData == target) { ans.AnswerDialogueData = null; dirty = true; }
                    if (ans.ReplaceMainDialogue == target) { ans.ReplaceMainDialogue = null; dirty = true; }
                }
            }
            if (dirty) EditorUtility.SetDirty(d);
        }
    }

    private DialogueNode GetNodeAtPosition(Vector2 mousePos)
    {
        foreach (DialogueNode node in nodes.Values)
        {
            Rect screenRect = new Rect(node.Rect.position + panOffset, node.Rect.size);
            if (screenRect.Contains(mousePos))
            {
                return node;
            }
        }
        return null;
    }

    private void DrawConnections()
    {
        Handles.BeginGUI();

        foreach (DialogueNode node in nodes.Values)
        {
            DialogueData data = node.Data;

            if (data.HasNextDialogue && data.NextDialogue != null && nodes.TryGetValue(data.NextDialogue, out DialogueNode nextNode))
            {
                Rect toRect = new Rect(nextNode.Rect.position + panOffset, nextNode.Rect.size);
                DrawBezierConnection(node.NextPortRect.center, new Vector2(toRect.xMin, toRect.y + 12f), new Color(0.35f, 0.75f, 1f), "Next");
            }

            if (data.ReplaceMainDialogue != null && nodes.TryGetValue(data.ReplaceMainDialogue, out DialogueNode replaceNode))
            {
                Rect toRect = new Rect(replaceNode.Rect.position + panOffset, replaceNode.Rect.size);
                DrawBezierConnection(node.ReplacePortRect.center, new Vector2(toRect.xMin, toRect.y + 12f), new Color(1f, 0.7f, 0.25f), "Replace");
            }

            if (data.HasQuestion && data.Answers != null)
            {
                for (int i = 0; i < data.Answers.Count && i < node.AnswerPortRects.Count; i++)
                {
                    Answer ans = data.Answers[i];
                    if (ans.AnswerDialogueData != null && nodes.TryGetValue(ans.AnswerDialogueData, out DialogueNode ansNode))
                    {
                        Rect toRect = new Rect(ansNode.Rect.position + panOffset, ansNode.Rect.size);
                        DrawBezierConnection(node.AnswerPortRects[i].center, new Vector2(toRect.xMin, toRect.y + 12f), new Color(0.45f, 0.9f, 0.5f), $"Rép. {i + 1}");
                    }

                    if (ans.ReplaceMainDialogue != null && nodes.TryGetValue(ans.ReplaceMainDialogue, out DialogueNode ansReplaceNode))
                    {
                        Rect toRect = new Rect(ansReplaceNode.Rect.position + panOffset, ansReplaceNode.Rect.size);
                        DrawBezierConnection(node.AnswerPortRects[i].center + Vector2.up * 4f, new Vector2(toRect.xMin, toRect.y + 12f), new Color(1f, 0.7f, 0.25f), "Replace");
                    }
                }
            }
        }

        Handles.EndGUI();
    }

    private void DrawBezierConnection(Vector2 start, Vector2 end, Color color, string label)
    {
        float tangent = Mathf.Clamp(Mathf.Abs(end.x - start.x) * 0.5f, 40f, 140f);
        Handles.DrawBezier(start, end, start + Vector2.right * tangent, end + Vector2.left * tangent, color, null, 3f);

        if (!string.IsNullOrEmpty(label))
        {
            Vector2 mid = (start + end) * 0.5f;
            GUIStyle tagStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = color }
            };
            GUI.Label(new Rect(mid.x - 45f, mid.y - 10f, 90f, 18f), label, tagStyle);
        }
    }

    private void DrawNode(DialogueNode node)
    {
        DialogueData data = node.Data;
        Rect screenRect = new Rect(node.Rect.position + panOffset, node.Rect.size);

        bool isSelected = selectedDialogue == data;
        bool isRoot = rootDialogues.Contains(data);

        Color borderColor = isSelected ? new Color(0.3f, 0.85f, 1f) : (isRoot ? new Color(0.45f, 0.9f, 0.5f) : new Color(0.08f, 0.08f, 0.08f));
        EditorGUI.DrawRect(new Rect(screenRect.x - 2, screenRect.y - 2, screenRect.width + 4, screenRect.height + 4), borderColor);
        EditorGUI.DrawRect(screenRect, new Color(0.24f, 0.24f, 0.24f));

        Color headerColor = isRoot ? new Color(0.18f, 0.42f, 0.28f) : new Color(0.18f, 0.28f, 0.4f);
        Rect headerRect = new Rect(screenRect.x, screenRect.y, screenRect.width, 24f);
        EditorGUI.DrawRect(headerRect, headerColor);

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            normal = { textColor = Color.white },
            padding = new RectOffset(8, 6, 3, 0)
        };
        GUI.Label(headerRect, data.name, titleStyle);

        EditorGUI.DrawRect(new Rect(screenRect.x - 5f, screenRect.y + 7f, 10f, 10f), new Color(0.8f, 0.8f, 0.8f));

        float y = screenRect.y + 28f;
        int lineCount = data.Lines != null ? data.Lines.Count : 0;

        if (lineCount == 0)
        {
            GUI.Label(new Rect(screenRect.x + 8, y, screenRect.width - 16, 18), "(Aucune ligne)", EditorStyles.miniLabel);
            y += 18f;
        }
        else
        {
            int previewCount = Mathf.Min(lineCount, 3);
            for (int i = 0; i < previewCount; i++)
            {
                string txt = data.Lines[i].Line ?? "";
                if (txt.Length > 34) txt = txt.Substring(0, 34) + "...";
                GUI.Label(new Rect(screenRect.x + 8, y, screenRect.width - 16, 18), $"• {txt}", EditorStyles.miniLabel);
                y += 18f;
            }
            if (lineCount > 3)
            {
                GUI.Label(new Rect(screenRect.x + 8, y - 2, screenRect.width - 16, 14), $"+ {lineCount - 3} autres...", EditorStyles.centeredGreyMiniLabel);
                y += 14f;
            }
        }

        string nextLabel = data.HasNextDialogue && data.NextDialogue != null ? $"→ Next: {data.NextDialogue.name}" : "→ Next (tirer pour lier)";
        GUIStyle nextStyle = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(0.45f, 0.8f, 1f) } };
        GUI.Label(new Rect(screenRect.x + 8, y, screenRect.width - 24, 18), nextLabel, nextStyle);
        DrawPort(node.NextPortRect, new Color(0.35f, 0.75f, 1f));
        y += 22f;

        string repLabel = data.ReplaceMainDialogue != null ? $"↻ Replace: {data.ReplaceMainDialogue.name}" : "↻ Replace Main";
        GUIStyle repStyle = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(1f, 0.75f, 0.35f) } };
        GUI.Label(new Rect(screenRect.x + 8, y, screenRect.width - 24, 18), repLabel, repStyle);
        DrawPort(node.ReplacePortRect, new Color(1f, 0.7f, 0.25f));
        y += 22f;

        if (data.HasQuestion && data.Answers != null && data.Answers.Count > 0)
        {
            y += 6f;
            for (int i = 0; i < data.Answers.Count; i++)
            {
                string ansText = string.IsNullOrEmpty(data.Answers[i].Line) ? $"Réponse {i + 1}" : data.Answers[i].Line;
                if (ansText.Length > 26) ansText = ansText.Substring(0, 26) + "...";

                Rect ansRect = new Rect(screenRect.x + 8, y, screenRect.width - 22, 20f);
                EditorGUI.DrawRect(ansRect, new Color(0.18f, 0.18f, 0.18f));
                GUI.Label(new Rect(ansRect.x + 4, ansRect.y + 1, ansRect.width - 8, 18), $"↳ {ansText}", EditorStyles.miniLabel);

                if (i < node.AnswerPortRects.Count)
                {
                    DrawPort(node.AnswerPortRects[i], new Color(0.45f, 0.9f, 0.5f));
                }
                y += 22f;
            }
        }
    }

    private void DrawPort(Rect rect, Color color)
    {
        EditorGUI.DrawRect(new Rect(rect.x - 1, rect.y - 1, rect.width + 2, rect.height + 2), Color.black);
        EditorGUI.DrawRect(rect, color);
    }

    private void DrawInspectorPanel(Rect inspectorRect)
    {
        EditorGUI.DrawRect(inspectorRect, new Color(0.21f, 0.21f, 0.21f));
        EditorGUI.DrawRect(new Rect(inspectorRect.x, inspectorRect.y, 2f, inspectorRect.height), new Color(0.1f, 0.1f, 0.1f));

        GUILayout.BeginArea(new Rect(inspectorRect.x + 8, inspectorRect.y + 6, inspectorRect.width - 16, inspectorRect.height - 12));

        if (selectedDialogue == null || selectedSerializedObject == null)
        {
            EditorGUILayout.HelpBox("Clique sur un nœud pour l'éditer.\nTire un port coloré à droite d'un nœud vers un autre nœud (ou dans le vide pour en créer un).", MessageType.Info);
            GUILayout.EndArea();
            return;
        }

        bool prevWide = EditorGUIUtility.wideMode;
        float prevLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.wideMode = true;
        EditorGUIUtility.labelWidth = Mathf.Clamp(inspectorWidth * 0.32f, 90f, 135f);

        selectedSerializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        renameBuffer = EditorGUILayout.TextField(renameBuffer, GUILayout.ExpandWidth(true));
        if (renameBuffer != selectedDialogue.name && !string.IsNullOrWhiteSpace(renameBuffer) && GUILayout.Button("Renommer", GUILayout.Width(72)))
        {
            string path = AssetDatabase.GetAssetPath(selectedDialogue);
            AssetDatabase.RenameAsset(path, renameBuffer.Trim());
            AssetDatabase.SaveAssets();
        }
        if (GUILayout.Button("Ping", GUILayout.Width(42)))
        {
            EditorGUIUtility.PingObject(selectedDialogue);
            Selection.activeObject = selectedDialogue;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(4);

        inspectorScrollPos = EditorGUILayout.BeginScrollView(inspectorScrollPos, GUIStyle.none, GUI.skin.verticalScrollbar);

        EditorGUI.BeginChangeCheck();

        DrawLinesSection();
        EditorGUILayout.Space(8);
        DrawFlowSection();
        EditorGUILayout.Space(8);

        SerializedProperty effectProp = selectedSerializedObject.FindProperty("EffectPreset");
        if (effectProp != null)
        {
            EditorGUILayout.PropertyField(effectProp);
        }

        if (EditorGUI.EndChangeCheck())
        {
            selectedSerializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(selectedDialogue);
            RebuildGraph(false);
        }

        EditorGUILayout.EndScrollView();

        EditorGUIUtility.wideMode = prevWide;
        EditorGUIUtility.labelWidth = prevLabelWidth;
        GUILayout.EndArea();
    }

    private void DrawLinesSection()
    {
        SerializedProperty linesProp = selectedSerializedObject.FindProperty("Lines");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Lignes ({linesProp.arraySize})", EditorStyles.boldLabel);
        if (GUILayout.Button("+ Ligne", GUILayout.Width(65)))
        {
            linesProp.InsertArrayElementAtIndex(linesProp.arraySize);
            linesProp.GetArrayElementAtIndex(linesProp.arraySize - 1).FindPropertyRelative("Line").stringValue = "";
            selectedSerializedObject.ApplyModifiedProperties();
            RebuildGraph(false);
        }
        EditorGUILayout.EndHorizontal();

        GUIStyle wrappedArea = new GUIStyle(EditorStyles.textArea) { wordWrap = true };

        for (int i = 0; i < linesProp.arraySize; i++)
        {
            SerializedProperty lineElem = linesProp.GetArrayElementAtIndex(i);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Ligne {i}", EditorStyles.miniBoldLabel);
            if (GUILayout.Button("X", GUILayout.Width(22)))
            {
                linesProp.DeleteArrayElementAtIndex(i);
                selectedSerializedObject.ApplyModifiedProperties();
                RebuildGraph(false);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }
            EditorGUILayout.EndHorizontal();

            SerializedProperty textProp = lineElem.FindPropertyRelative("Line");
            textProp.stringValue = EditorGUILayout.TextArea(textProp.stringValue, wrappedArea, GUILayout.MinHeight(38), GUILayout.ExpandWidth(true));

            EditorGUILayout.PropertyField(lineElem.FindPropertyRelative("Emotion"));
            EditorGUILayout.PropertyField(lineElem.FindPropertyRelative("Reward"));

            EditorGUILayout.EndVertical();
        }
    }

    private void DrawFlowSection()
    {
        EditorGUILayout.LabelField("Flux & Branches", EditorStyles.boldLabel);

        SerializedProperty hasNextProp = selectedSerializedObject.FindProperty("HasNextDialogue");
        SerializedProperty nextProp = selectedSerializedObject.FindProperty("NextDialogue");
        SerializedProperty replaceMainProp = selectedSerializedObject.FindProperty("ReplaceMainDialogue");

        EditorGUILayout.PropertyField(hasNextProp);
        if (hasNextProp.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(nextProp, new GUIContent("Next"));
            if (nextProp.objectReferenceValue == null && GUILayout.Button("+", GUILayout.Width(24)))
            {
                nextProp.objectReferenceValue = CreateLinkedDialogueAsset(selectedDialogue, selectedDialogue.name + "_Next");
                selectedSerializedObject.ApplyModifiedProperties();
                RebuildGraph(false);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(replaceMainProp, new GUIContent("Replace Main"));
        if (replaceMainProp.objectReferenceValue == null && GUILayout.Button("+", GUILayout.Width(24)))
        {
            replaceMainProp.objectReferenceValue = CreateLinkedDialogueAsset(selectedDialogue, selectedDialogue.name + "_Replace");
            selectedSerializedObject.ApplyModifiedProperties();
            RebuildGraph(false);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(6);

        SerializedProperty hasQuestionProp = selectedSerializedObject.FindProperty("HasQuestion");
        SerializedProperty answersProp = selectedSerializedObject.FindProperty("Answers");

        EditorGUILayout.PropertyField(hasQuestionProp);
        if (hasQuestionProp.boolValue)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Réponses ({answersProp.arraySize})", EditorStyles.miniBoldLabel);
            if (GUILayout.Button("+ Réponse", GUILayout.Width(72)))
            {
                answersProp.InsertArrayElementAtIndex(answersProp.arraySize);
                selectedSerializedObject.ApplyModifiedProperties();
                RebuildGraph(false);
            }
            if (GUILayout.Button("+ Rép. & Nœud", GUILayout.Width(92)))
            {
                int idx = answersProp.arraySize;
                answersProp.InsertArrayElementAtIndex(idx);
                SerializedProperty newAns = answersProp.GetArrayElementAtIndex(idx);
                newAns.FindPropertyRelative("Line").stringValue = $"Réponse {idx + 1}";
                newAns.FindPropertyRelative("AnswerDialogueData").objectReferenceValue =
                    CreateLinkedDialogueAsset(selectedDialogue, $"{selectedDialogue.name}_Ans{idx + 1}");
                selectedSerializedObject.ApplyModifiedProperties();
                RebuildGraph(false);
            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < answersProp.arraySize; i++)
            {
                SerializedProperty ansElem = answersProp.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField($"Réponse {i + 1}", EditorStyles.miniBoldLabel);
                if (GUILayout.Button("X", GUILayout.Width(22)))
                {
                    answersProp.DeleteArrayElementAtIndex(i);
                    selectedSerializedObject.ApplyModifiedProperties();
                    RebuildGraph(false);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    break;
                }
                EditorGUILayout.EndHorizontal();

                SerializedProperty ansText = ansElem.FindPropertyRelative("Line");
                ansText.stringValue = EditorGUILayout.TextField("Texte", ansText.stringValue);

                SerializedProperty ansDialogue = ansElem.FindPropertyRelative("AnswerDialogueData");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(ansDialogue, new GUIContent("Suite"));
                if (ansDialogue.objectReferenceValue == null && GUILayout.Button("+", GUILayout.Width(24)))
                {
                    ansDialogue.objectReferenceValue = CreateLinkedDialogueAsset(selectedDialogue, $"{selectedDialogue.name}_Ans{i + 1}");
                    selectedSerializedObject.ApplyModifiedProperties();
                    RebuildGraph(false);
                }
                EditorGUILayout.EndHorizontal();

                SerializedProperty ansReplace = ansElem.FindPropertyRelative("ReplaceMainDialogue");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(ansReplace, new GUIContent("Replace Main"));
                if (ansReplace.objectReferenceValue == null && GUILayout.Button("+", GUILayout.Width(24)))
                {
                    ansReplace.objectReferenceValue = CreateLinkedDialogueAsset(selectedDialogue, $"{selectedDialogue.name}_Ans{i + 1}_Replace");
                    selectedSerializedObject.ApplyModifiedProperties();
                    RebuildGraph(false);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(ansElem.FindPropertyRelative("Reward"));
                EditorGUILayout.PropertyField(ansElem.FindPropertyRelative("Cost"));

                EditorGUILayout.EndVertical();
            }
        }
    }

    private DialogueData CreateLinkedDialogueAsset(DialogueData parent, string baseName)
    {
        string parentPath = AssetDatabase.GetAssetPath(parent);
        string folder = string.IsNullOrEmpty(parentPath) ? "Assets" : Path.GetDirectoryName(parentPath);
        string assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folder, baseName + ".asset"));

        DialogueData newDialogue = CreateInstance<DialogueData>();
        AssetDatabase.CreateAsset(newDialogue, assetPath);
        AssetDatabase.SaveAssets();
        return newDialogue;
    }
}