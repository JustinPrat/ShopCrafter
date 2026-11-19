using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DialogueGraphWindow : EditorWindow
{
    private class DialogueNode
    {
        public DialogueData Data;
        public Rect Rect;
        public int Depth;
    }

    private DialogueData rootDialogue;
    private PNJInfoData sourcePNJ;

    private readonly Dictionary<DialogueData, DialogueNode> nodes = new Dictionary<DialogueData, DialogueNode>();
    private DialogueData selectedDialogue;
    private SerializedObject selectedSerializedObject;
    private string renameBuffer = "";

    private Vector2 panOffset = Vector2.zero;
    private bool isPanning;
    private DialogueNode draggingNode;
    private Vector2 inspectorScrollPos;

    private const float InspectorWidth = 340f;
    private const float ToolbarHeight = 28f;
    private const float NodeWidth = 240f;
    private const float NodeHorizontalSpacing = 90f;
    private const float NodeVerticalSpacing = 35f;

    [MenuItem("ShopCrafter/Tools/Dialogue Graph Editor")]
    public static void ShowWindow()
    {
        DialogueGraphWindow window = GetWindow<DialogueGraphWindow>("Dialogue Graph");
        window.minSize = new Vector2(820f, 540f);
    }

    public static void OpenWithPNJ(PNJInfoData pnjInfo)
    {
        DialogueGraphWindow window = GetWindow<DialogueGraphWindow>("Dialogue Graph");
        window.minSize = new Vector2(820f, 540f);
        window.sourcePNJ = pnjInfo;

        DialogueData extracted = window.ExtractDialogueFromPNJ(pnjInfo);
        if (extracted != null)
        {
            window.LoadRootDialogue(extracted);
        }
        else
        {
            window.ShowNotification(new GUIContent("Aucun DialogueData trouvé sur ce PNJ."), 1.5f);
        }

        window.Focus();
    }

    private void OnGUI()
    {
        DrawToolbar();

        Rect canvasRect = new Rect(0f, ToolbarHeight, position.width - InspectorWidth, position.height - ToolbarHeight);
        Rect inspectorRect = new Rect(position.width - InspectorWidth, ToolbarHeight, InspectorWidth, position.height - ToolbarHeight);

        DrawCanvas(canvasRect);
        DrawInspectorPanel(inspectorRect);
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(ToolbarHeight));

        EditorGUI.BeginChangeCheck();
        sourcePNJ = (PNJInfoData)EditorGUILayout.ObjectField("PNJ Info", sourcePNJ, typeof(PNJInfoData), false, GUILayout.Width(220));
        if (EditorGUI.EndChangeCheck() && sourcePNJ != null)
        {
            DialogueData extracted = ExtractDialogueFromPNJ(sourcePNJ);
            if (extracted != null)
            {
                LoadRootDialogue(extracted);
            }
            else
            {
                ShowNotification(new GUIContent("Aucun DialogueData trouvé sur ce PNJInfoData."), 1.5f);
            }
        }

        EditorGUI.BeginChangeCheck();
        DialogueData newRoot = (DialogueData)EditorGUILayout.ObjectField("Dialogue", rootDialogue, typeof(DialogueData), false, GUILayout.Width(230));
        if (EditorGUI.EndChangeCheck())
        {
            sourcePNJ = null;
            LoadRootDialogue(newRoot);
        }

        if (GUILayout.Button("+ Nouveau Root", EditorStyles.toolbarButton, GUILayout.Width(105)))
        {
            CreateNewRootDialogue();
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
            RebuildGraph(false);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void CreateNewRootDialogue()
    {
        string path = EditorUtility.SaveFilePanelInProject("Nouveau DialogueData", "NewDialogue", "asset", "Choisis l'emplacement du DialogueData");
        if (string.IsNullOrEmpty(path)) return;

        DialogueData created = CreateInstance<DialogueData>();
        AssetDatabase.CreateAsset(created, path);
        AssetDatabase.SaveAssets();
        sourcePNJ = null;
        LoadRootDialogue(created);
    }

    private DialogueData ExtractDialogueFromPNJ(PNJInfoData pnjInfo)
    {
        if (pnjInfo == null) return null;

        SerializedObject so = new SerializedObject(pnjInfo);
        SerializedProperty prop = so.GetIterator();
        while (prop.NextVisible(true))
        {
            if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue != null)
            {
                if (prop.objectReferenceValue is DialogueData directDialogue)
                    return directDialogue;

                if (prop.objectReferenceValue is IdentityData identityData)
                {
                    Identity id = identityData.GetIdentity();
                    if (id != null && id.Dialogue != null)
                        return id.Dialogue;
                }
            }
        }

        return null;
    }

    private void LoadRootDialogue(DialogueData dialogue)
    {
        rootDialogue = dialogue;
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

    private void RebuildGraph(bool resetPositions)
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

        nodes.Clear();
        if (rootDialogue == null) return;

        Queue<(DialogueData data, int depth)> queue = new Queue<(DialogueData, int)>();
        Dictionary<int, List<DialogueNode>> columns = new Dictionary<int, List<DialogueNode>>();

        queue.Enqueue((rootDialogue, 0));

        while (queue.Count > 0)
        {
            var (current, depth) = queue.Dequeue();
            if (current == null || nodes.ContainsKey(current)) continue;

            float estimatedHeight = EstimateNodeHeight(current);
            DialogueNode node = new DialogueNode
            {
                Data = current,
                Depth = depth,
                Rect = new Rect(0f, 0f, NodeWidth, estimatedHeight)
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

        foreach (var kvp in columns)
        {
            int col = kvp.Key;
            float y = 40f;
            foreach (DialogueNode node in kvp.Value)
            {
                if (!resetPositions && savedPositions.TryGetValue(node.Data, out Vector2 pos))
                {
                    node.Rect.position = pos;
                }
                else
                {
                    node.Rect.position = new Vector2(40f + col * (NodeWidth + NodeHorizontalSpacing), y);
                    y += node.Rect.height + NodeVerticalSpacing;
                }
            }
        }
    }

    private float EstimateNodeHeight(DialogueData data)
    {
        float height = 62f;
        int lineCount = data.Lines != null ? Mathf.Min(data.Lines.Count, 3) : 0;
        height += lineCount * 20f;

        if (data.HasQuestion && data.Answers != null && data.Answers.Count > 0)
        {
            height += 22f + data.Answers.Count * 22f;
        }
        else if (data.HasNextDialogue)
        {
            height += 24f;
        }

        if (data.ReplaceMainDialogue != null)
        {
            height += 20f;
        }

        return Mathf.Max(100f, height);
    }

    private void DrawCanvas(Rect canvasRect)
    {
        GUI.BeginGroup(canvasRect);

        EditorGUI.DrawRect(new Rect(0, 0, canvasRect.width, canvasRect.height), new Color(0.16f, 0.16f, 0.16f));
        DrawGrid(canvasRect, 20f, 0.15f, Color.gray);
        DrawGrid(canvasRect, 100f, 0.3f, Color.gray);

        if (rootDialogue == null)
        {
            GUIStyle centerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 13,
                normal = { textColor = new Color(0.7f, 0.7f, 0.7f) }
            };
            GUI.Label(new Rect(0, 0, canvasRect.width, canvasRect.height - 40f), "Glisse un DialogueData / PNJInfoData ou clique sur '+ Nouveau Root'.", centerStyle);

            if (GUI.Button(new Rect(canvasRect.width * 0.5f - 90f, canvasRect.height * 0.5f + 10f, 180f, 30f), "+ Créer un DialogueData"))
            {
                CreateNewRootDialogue();
            }

            GUI.EndGroup();
            return;
        }

        HandleCanvasEvents(canvasRect);
        DrawConnections();

        foreach (DialogueNode node in new List<DialogueNode>(nodes.Values))
        {
            DrawNode(node);
        }

        GUI.EndGroup();
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

    private void HandleCanvasEvents(Rect canvasRect)
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
        }
        else if (e.type == EventType.MouseUp)
        {
            isPanning = false;
            draggingNode = null;
        }
    }

    private void ShowNodeContextMenu(DialogueNode node)
    {
        GenericMenu menu = new GenericMenu();
        DialogueData data = node.Data;

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

        menu.AddItem(new GUIContent("Créer & Lier NextDialogue"), false, () =>
        {
            Undo.RecordObject(data, "Create Next Dialogue");
            data.HasNextDialogue = true;
            DialogueData child = CreateLinkedDialogueAsset(data, $"{data.name}_Next");
            data.NextDialogue = child;
            EditorUtility.SetDirty(data);
            SelectDialogue(child);
            RebuildGraph(false);
        });

        menu.AddItem(new GUIContent("Créer & Lier ReplaceMainDialogue"), false, () =>
        {
            Undo.RecordObject(data, "Create Replace Main Dialogue");
            DialogueData child = CreateLinkedDialogueAsset(data, $"{data.name}_Replace");
            data.ReplaceMainDialogue = child;
            EditorUtility.SetDirty(data);
            SelectDialogue(child);
            RebuildGraph(false);
        });

        menu.AddSeparator("");

        menu.AddItem(new GUIContent("Dupliquer cet asset"), false, () =>
        {
            string srcPath = AssetDatabase.GetAssetPath(data);
            string dstPath = AssetDatabase.GenerateUniqueAssetPath(srcPath);
            if (AssetDatabase.CopyAsset(srcPath, dstPath))
            {
                AssetDatabase.SaveAssets();
                DialogueData copy = AssetDatabase.LoadAssetAtPath<DialogueData>(dstPath);
                EditorGUIUtility.PingObject(copy);
            }
        });

        if (data != rootDialogue)
        {
            menu.AddItem(new GUIContent("Supprimer le fichier Asset"), false, () =>
            {
                if (EditorUtility.DisplayDialog("Supprimer le DialogueData ?", $"Supprimer définitivement '{data.name}' du projet ?", "Supprimer", "Annuler"))
                {
                    string assetPath = AssetDatabase.GetAssetPath(data);
                    UnlinkDialogueReferences(data);
                    AssetDatabase.DeleteAsset(assetPath);
                    AssetDatabase.SaveAssets();
                    SelectDialogue(rootDialogue);
                    RebuildGraph(false);
                }
            });
        }

        menu.ShowAsContext();
    }

    private void ShowCanvasContextMenu()
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Créer un nouveau Dialogue (Root)"), false, CreateNewRootDialogue);
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
            Rect fromRect = new Rect(node.Rect.position + panOffset, node.Rect.size);
            DialogueData data = node.Data;

            if (data.HasNextDialogue && data.NextDialogue != null && nodes.TryGetValue(data.NextDialogue, out DialogueNode nextNode))
            {
                Rect toRect = new Rect(nextNode.Rect.position + panOffset, nextNode.Rect.size);
                DrawBezierConnection(
                    new Vector2(fromRect.xMax, fromRect.y + fromRect.height * 0.5f),
                    new Vector2(toRect.xMin, toRect.y + 20f),
                    new Color(0.35f, 0.75f, 1f),
                    "Next"
                );
            }

            if (data.ReplaceMainDialogue != null && nodes.TryGetValue(data.ReplaceMainDialogue, out DialogueNode replaceNode))
            {
                Rect toRect = new Rect(replaceNode.Rect.position + panOffset, replaceNode.Rect.size);
                DrawBezierConnection(
                    new Vector2(fromRect.xMax, fromRect.yMax - 12f),
                    new Vector2(toRect.xMin, toRect.y + 20f),
                    new Color(1f, 0.7f, 0.25f),
                    "Replace Main"
                );
            }

            if (data.HasQuestion && data.Answers != null)
            {
                for (int i = 0; i < data.Answers.Count; i++)
                {
                    Answer ans = data.Answers[i];
                    float yPos = fromRect.yMax - (data.Answers.Count - i) * 22f + 10f;

                    if (ans.AnswerDialogueData != null && nodes.TryGetValue(ans.AnswerDialogueData, out DialogueNode ansNode))
                    {
                        Rect toRect = new Rect(ansNode.Rect.position + panOffset, ansNode.Rect.size);
                        DrawBezierConnection(
                            new Vector2(fromRect.xMax, yPos),
                            new Vector2(toRect.xMin, toRect.y + 20f),
                            new Color(0.45f, 0.9f, 0.5f),
                            $"Réponse {i + 1}"
                        );
                    }

                    if (ans.ReplaceMainDialogue != null && nodes.TryGetValue(ans.ReplaceMainDialogue, out DialogueNode ansReplaceNode))
                    {
                        Rect toRect = new Rect(ansReplaceNode.Rect.position + panOffset, ansReplaceNode.Rect.size);
                        DrawBezierConnection(
                            new Vector2(fromRect.xMax, yPos + 4f),
                            new Vector2(toRect.xMin, toRect.y + 20f),
                            new Color(1f, 0.7f, 0.25f),
                            "Replace"
                        );
                    }
                }
            }
        }

        Handles.EndGUI();
    }

    private void DrawBezierConnection(Vector2 start, Vector2 end, Color color, string label)
    {
        float tangent = Mathf.Clamp(Mathf.Abs(end.x - start.x) * 0.5f, 40f, 140f);
        Handles.DrawBezier(
            start,
            end,
            start + Vector2.right * tangent,
            end + Vector2.left * tangent,
            color,
            null,
            3f
        );

        Vector2 mid = (start + end) * 0.5f;
        GUIStyle tagStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = color }
        };
        GUI.Label(new Rect(mid.x - 45f, mid.y - 10f, 90f, 18f), label, tagStyle);
    }

    private void DrawNode(DialogueNode node)
    {
        DialogueData data = node.Data;
        node.Rect.height = EstimateNodeHeight(data);
        Rect screenRect = new Rect(node.Rect.position + panOffset, node.Rect.size);

        bool isSelected = selectedDialogue == data;
        bool isRoot = rootDialogue == data;

        Color borderColor = isSelected ? new Color(0.3f, 0.85f, 1f) : (isRoot ? new Color(0.45f, 0.9f, 0.5f) : new Color(0.08f, 0.08f, 0.08f));
        EditorGUI.DrawRect(new Rect(screenRect.x - 2, screenRect.y - 2, screenRect.width + 4, screenRect.height + 4), borderColor);
        EditorGUI.DrawRect(screenRect, new Color(0.24f, 0.24f, 0.24f));

        Color headerColor = isRoot ? new Color(0.18f, 0.42f, 0.28f) : new Color(0.18f, 0.28f, 0.4f);
        Rect headerRect = new Rect(screenRect.x, screenRect.y, screenRect.width, 24f);
        EditorGUI.DrawRect(headerRect, headerColor);

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            normal = { textColor = Color.white },
            padding = new RectOffset(6, 6, 3, 0)
        };
        GUI.Label(headerRect, data.name, titleStyle);

        float y = screenRect.y + 28f;
        int lineCount = data.Lines != null ? data.Lines.Count : 0;

        if (lineCount == 0)
        {
            GUI.Label(new Rect(screenRect.x + 8, y, screenRect.width - 16, 18), "(Aucune ligne)", EditorStyles.miniLabel);
            y += 20f;
        }
        else
        {
            int previewCount = Mathf.Min(lineCount, 3);
            for (int i = 0; i < previewCount; i++)
            {
                string txt = data.Lines[i].Line ?? "";
                if (txt.Length > 32) txt = txt.Substring(0, 32) + "...";
                GUI.Label(new Rect(screenRect.x + 8, y, screenRect.width - 16, 18), $"• {txt}", EditorStyles.miniLabel);
                y += 20f;
            }
            if (lineCount > 3)
            {
                GUI.Label(new Rect(screenRect.x + 8, y - 4, screenRect.width - 16, 16), $"+ {lineCount - 3} autres lignes...", EditorStyles.centeredGreyMiniLabel);
                y += 14f;
            }
        }

        if (data.ReplaceMainDialogue != null)
        {
            GUIStyle repStyle = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(1f, 0.75f, 0.35f) } };
            GUI.Label(new Rect(screenRect.x + 8, y, screenRect.width - 16, 18), $"↻ Replace: {data.ReplaceMainDialogue.name}", repStyle);
            y += 20f;
        }

        if (data.HasQuestion && data.Answers != null && data.Answers.Count > 0)
        {
            y += 4f;
            for (int i = 0; i < data.Answers.Count; i++)
            {
                string ansText = string.IsNullOrEmpty(data.Answers[i].Line) ? $"Réponse {i + 1}" : data.Answers[i].Line;
                if (ansText.Length > 26) ansText = ansText.Substring(0, 26) + "...";

                Rect ansRect = new Rect(screenRect.x + 8, y, screenRect.width - 16, 20f);
                EditorGUI.DrawRect(ansRect, new Color(0.18f, 0.18f, 0.18f));
                GUI.Label(new Rect(ansRect.x + 4, ansRect.y + 1, ansRect.width - 8, 18), $"↳ {ansText}", EditorStyles.miniLabel);
                y += 22f;
            }
        }
        else if (data.HasNextDialogue)
        {
            string nextName = data.NextDialogue != null ? data.NextDialogue.name : "(Vide)";
            GUIStyle nextStyle = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = new Color(0.45f, 0.8f, 1f) } };
            GUI.Label(new Rect(screenRect.x + 8, y, screenRect.width - 16, 18), $"→ Next: {nextName}", nextStyle);
        }
    }

    private void DrawInspectorPanel(Rect inspectorRect)
    {
        EditorGUI.DrawRect(inspectorRect, new Color(0.21f, 0.21f, 0.21f));
        EditorGUI.DrawRect(new Rect(inspectorRect.x, inspectorRect.y, 2f, inspectorRect.height), new Color(0.1f, 0.1f, 0.1f));

        GUILayout.BeginArea(new Rect(inspectorRect.x + 8, inspectorRect.y + 6, inspectorRect.width - 16, inspectorRect.height - 12));

        if (selectedDialogue == null || selectedSerializedObject == null)
        {
            EditorGUILayout.HelpBox("Clique sur un nœud du graphe pour éditer ses lignes, réponses et liens.\nClic droit sur un nœud pour créer des branches rapidement.", MessageType.Info);
            GUILayout.EndArea();
            return;
        }

        selectedSerializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        renameBuffer = EditorGUILayout.TextField(renameBuffer);
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

        inspectorScrollPos = EditorGUILayout.BeginScrollView(inspectorScrollPos);

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
            textProp.stringValue = EditorGUILayout.TextArea(textProp.stringValue, GUILayout.MinHeight(38));

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
            EditorGUILayout.PropertyField(nextProp, GUIContent.none);
            if (nextProp.objectReferenceValue == null && GUILayout.Button("+ Créer", GUILayout.Width(60)))
            {
                DialogueData created = CreateLinkedDialogueAsset(selectedDialogue, selectedDialogue.name + "_Next");
                nextProp.objectReferenceValue = created;
                selectedSerializedObject.ApplyModifiedProperties();
                RebuildGraph(false);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(replaceMainProp, new GUIContent("Replace Main"));
        if (replaceMainProp.objectReferenceValue == null && GUILayout.Button("+ Créer", GUILayout.Width(60)))
        {
            DialogueData created = CreateLinkedDialogueAsset(selectedDialogue, selectedDialogue.name + "_Replace");
            replaceMainProp.objectReferenceValue = created;
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
                if (ansDialogue.objectReferenceValue == null && GUILayout.Button("+ Créer", GUILayout.Width(60)))
                {
                    DialogueData created = CreateLinkedDialogueAsset(selectedDialogue, $"{selectedDialogue.name}_Ans{i + 1}");
                    ansDialogue.objectReferenceValue = created;
                    selectedSerializedObject.ApplyModifiedProperties();
                    RebuildGraph(false);
                }
                EditorGUILayout.EndHorizontal();

                SerializedProperty ansReplace = ansElem.FindPropertyRelative("ReplaceMainDialogue");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(ansReplace, new GUIContent("Replace Main"));
                if (ansReplace.objectReferenceValue == null && GUILayout.Button("+ Créer", GUILayout.Width(60)))
                {
                    DialogueData created = CreateLinkedDialogueAsset(selectedDialogue, $"{selectedDialogue.name}_Ans{i + 1}_Replace");
                    ansReplace.objectReferenceValue = created;
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