using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sequencer.Actions;
using UnityEditor;
using UnityEngine;
using SeqComponent = Sequencer.Sequencer;

public class SequencerEditorWindow : EditorWindow
{
    private class TimelineBlock
    {
        public int Index;
        public SeqComponent.Action Action;
        public float StartTime;
        public float Duration;
        public int Lane;
        public bool IsInstant;
    }

    private SeqComponent targetSequencer;
    private SerializedObject serializedSequencer;
    private bool lockSelection = false;

    private int selectedIndex = -1;
    private Editor cachedScriptableEditor;
    private UnityEngine.Object cachedScriptableTarget;

    private float pixelsPerSecond = 140f;
    private Vector2 timelineScrollPos;
    private Vector2 inspectorScrollPos;
    private bool isPanning = false;

    private const float ToolbarHeight = 28f;
    private const float InspectorWidth = 350f;
    private const float RulerHeight = 24f;
    private const float LaneHeight = 46f;
    private const float LaneSpacing = 6f;

    [MenuItem("ShopCrafter/Tools/Sequencer Editor")]
    public static void ShowWindow()
    {
        SequencerEditorWindow window = GetWindow<SequencerEditorWindow>("Sequencer Editor");
        window.minSize = new Vector2(820f, 480f);
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        OnSelectionChange();
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        ClearCachedScriptableEditor();
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        serializedSequencer = null;
        ClearCachedScriptableEditor();

        if (targetSequencer != null)
        {
            serializedSequencer = new SerializedObject(targetSequencer);
        }
        else
        {
            OnSelectionChange();
        }

        Repaint();
    }

    private void OnSelectionChange()
    {
        if (lockSelection) return;

        if (Selection.activeGameObject != null &&
            Selection.activeGameObject.TryGetComponent(out SeqComponent seq))
        {
            SetTarget(seq);
            Repaint();
        }
    }

    private void SetTarget(SeqComponent seq)
    {
        if (targetSequencer == seq && serializedSequencer != null && serializedSequencer.targetObject != null)
            return;

        bool targetChanged = targetSequencer != seq;
        targetSequencer = seq;
        serializedSequencer = seq != null ? new SerializedObject(seq) : null;

        if (targetChanged)
        {
            selectedIndex = -1;
            ClearCachedScriptableEditor();
        }
    }

    private void ClearCachedScriptableEditor()
    {
        if (cachedScriptableEditor != null)
        {
            DestroyImmediate(cachedScriptableEditor);
            cachedScriptableEditor = null;
        }
        cachedScriptableTarget = null;
    }

    private void OnGUI()
    {
        DrawToolbar();

        if (targetSequencer != null && (serializedSequencer == null || serializedSequencer.targetObject == null))
        {
            serializedSequencer = new SerializedObject(targetSequencer);
        }

        if (targetSequencer == null || serializedSequencer == null)
        {
            EditorGUILayout.HelpBox("Sélectionne un GameObject contenant un composant Sequencer dans la hiérarchie.", MessageType.Info);
            return;
        }

        serializedSequencer.Update();

        SerializedProperty actionsProp = serializedSequencer.FindProperty("actions");
        if (actionsProp == null)
        {
            EditorGUILayout.HelpBox("Propriété 'actions' introuvable.", MessageType.Error);
            return;
        }

        if (selectedIndex >= actionsProp.arraySize)
        {
            selectedIndex = actionsProp.arraySize - 1;
        }

        Rect timelineRect = new Rect(0f, ToolbarHeight, position.width - InspectorWidth, position.height - ToolbarHeight);
        Rect inspectorRect = new Rect(position.width - InspectorWidth, ToolbarHeight, InspectorWidth, position.height - ToolbarHeight);

        DrawTimelinePanel(timelineRect, actionsProp);
        DrawInspectorPanel(inspectorRect, actionsProp);

        serializedSequencer.ApplyModifiedProperties();

        if (Application.isPlaying)
        {
            Repaint();
        }
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(ToolbarHeight));

        EditorGUI.BeginChangeCheck();
        SeqComponent newTarget = (SeqComponent)EditorGUILayout.ObjectField(targetSequencer, typeof(SeqComponent), true, GUILayout.Width(180));
        if (EditorGUI.EndChangeCheck())
        {
            SetTarget(newTarget);
        }

        lockSelection = GUILayout.Toggle(lockSelection, lockSelection ? "🔒 Lock" : "🔓 Lock", EditorStyles.toolbarButton, GUILayout.Width(62));

        if (targetSequencer != null && serializedSequencer != null && serializedSequencer.targetObject != null)
        {
            SerializedProperty playModeProp = serializedSequencer.FindProperty("playMode");
            if (playModeProp != null)
            {
                EditorGUILayout.PropertyField(playModeProp, GUIContent.none, GUILayout.Width(80));
            }

            GUILayout.Space(8);

            if (GUILayout.Button("+ Ajouter Action", EditorStyles.toolbarDropDown, GUILayout.Width(115)))
            {
                int insertIdx = targetSequencer.Actions != null ? targetSequencer.Actions.Count : 0;
                ShowQuickAddMenu(insertIdx, SeqComponent.Action.ActionType.After);
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField("Zoom", GUILayout.Width(38));
            pixelsPerSecond = GUILayout.HorizontalSlider(pixelsPerSecond, 50f, 350f, GUILayout.Width(80));

            GUILayout.Space(8);

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);
            GUI.backgroundColor = new Color(0.45f, 0.9f, 0.55f);
            if (GUILayout.Button("▶ Play", EditorStyles.toolbarButton, GUILayout.Width(55)))
            {
                targetSequencer.StartSequence();
            }
            GUI.backgroundColor = new Color(1f, 0.55f, 0.55f);
            if (GUILayout.Button("■ Stop", EditorStyles.toolbarButton, GUILayout.Width(55)))
            {
                targetSequencer.StopSequence();
            }
            GUI.backgroundColor = Color.white;
            EditorGUI.EndDisabledGroup();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawTimelinePanel(Rect rect, SerializedProperty actionsProp)
    {
        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

        HandleTimelinePanAndZoom(rect);

        List<TimelineBlock> blocks = BuildTimelineBlocks();
        float totalDuration = blocks.Count > 0 ? blocks.Max(b => b.StartTime + b.Duration) : 0f;
        int maxLane = blocks.Count > 0 ? blocks.Max(b => b.Lane) : 0;

        float contentWidth = Mathf.Max(rect.width - 16f, (totalDuration + 2.5f) * pixelsPerSecond + 140f);
        float contentHeight = Mathf.Max(rect.height - 16f, RulerHeight + (maxLane + 2) * (LaneHeight + LaneSpacing) + 60f);

        timelineScrollPos = GUI.BeginScrollView(rect, timelineScrollPos, new Rect(0f, 0f, contentWidth, contentHeight));

        DrawTimeRuler(contentWidth, contentHeight, totalDuration, blocks.Count > 0);

        foreach (TimelineBlock block in blocks)
        {
            DrawTimelineBlock(block, actionsProp);
        }

        DrawInlineAddButtons(blocks, totalDuration);

        GUI.EndScrollView();
    }

    private void HandleTimelinePanAndZoom(Rect timelineRect)
    {
        Event e = Event.current;

        if (e.type == EventType.MouseDown && timelineRect.Contains(e.mousePosition))
        {
            if (e.button == 2 || (e.button == 0 && e.alt))
            {
                isPanning = true;
                e.Use();
            }
        }
        else if (e.type == EventType.MouseDrag && isPanning)
        {
            timelineScrollPos -= e.delta;
            timelineScrollPos.x = Mathf.Max(0f, timelineScrollPos.x);
            timelineScrollPos.y = Mathf.Max(0f, timelineScrollPos.y);
            e.Use();
            Repaint();
        }
        else if (e.type == EventType.MouseUp && isPanning)
        {
            isPanning = false;
            e.Use();
        }
        else if (e.type == EventType.ScrollWheel && e.control && timelineRect.Contains(e.mousePosition))
        {
            pixelsPerSecond = Mathf.Clamp(pixelsPerSecond - e.delta.y * 12f, 50f, 350f);
            e.Use();
            Repaint();
        }
    }

    private void DrawInlineAddButtons(List<TimelineBlock> blocks, float totalDuration)
    {
        float afterX = 16f + totalDuration * pixelsPerSecond + (blocks.Count > 0 ? 10f : 0f);
        float afterY = RulerHeight + 10f;
        Rect addAfterRect = new Rect(afterX, afterY, 88f, LaneHeight);

        GUI.backgroundColor = new Color(0.25f, 0.55f, 0.85f);
        if (GUI.Button(addAfterRect, "+ After"))
        {
            int insertIdx = targetSequencer.Actions != null ? targetSequencer.Actions.Count : 0;
            ShowQuickAddMenu(insertIdx, SeqComponent.Action.ActionType.After);
        }
        GUI.backgroundColor = Color.white;

        if (selectedIndex >= 0 && selectedIndex < blocks.Count)
        {
            TimelineBlock selBlock = blocks[selectedIndex];
            int groupEndIndex = GetGroupEndIndex(selectedIndex);
            int maxGroupLane = selBlock.Lane;

            for (int i = selectedIndex + 1; i <= groupEndIndex && i < blocks.Count; i++)
            {
                maxGroupLane = Mathf.Max(maxGroupLane, blocks[i].Lane);
            }

            float joinX = 16f + selBlock.StartTime * pixelsPerSecond;
            float joinY = RulerHeight + 10f + (maxGroupLane + 1) * (LaneHeight + LaneSpacing);
            Rect addJoinRect = new Rect(joinX, joinY, 88f, 30f);

            GUI.backgroundColor = new Color(0.85f, 0.58f, 0.2f);
            if (GUI.Button(addJoinRect, "+ Join ici"))
            {
                ShowQuickAddMenu(groupEndIndex + 1, SeqComponent.Action.ActionType.Join);
            }
            GUI.backgroundColor = Color.white;
        }
    }

    private void DrawTimeRuler(float width, float height, float totalDuration, bool hasBlocks)
    {
        EditorGUI.DrawRect(new Rect(0f, 0f, width, RulerHeight), new Color(0.2f, 0.2f, 0.2f));

        float step = pixelsPerSecond >= 160f ? 0.25f : (pixelsPerSecond >= 90f ? 0.5f : 1f);
        int maxTicks = Mathf.CeilToInt(width / (step * pixelsPerSecond));

        Handles.BeginGUI();
        for (int i = 0; i <= maxTicks; i++)
        {
            float t = i * step;
            float x = 16f + t * pixelsPerSecond;

            Handles.color = new Color(1f, 1f, 1f, 0.08f);
            Handles.DrawLine(new Vector3(x, RulerHeight, 0f), new Vector3(x, height, 0f));

            Handles.color = new Color(1f, 1f, 1f, 0.35f);
            Handles.DrawLine(new Vector3(x, RulerHeight - 7f, 0f), new Vector3(x, RulerHeight, 0f));

            GUI.Label(new Rect(x + 3f, 3f, 45f, 16f), $"{t:0.##}s", EditorStyles.miniLabel);
        }

        if (hasBlocks)
        {
            float endX = 16f + totalDuration * pixelsPerSecond;
            Handles.color = new Color(1f, 1f, 1f, 0.22f);
            Handles.DrawLine(new Vector3(endX, RulerHeight, 0f), new Vector3(endX, height, 0f));
            GUI.Label(new Rect(endX + 4f, 3f, 75f, 16f), $"Fin ({totalDuration:0.##}s)", EditorStyles.miniBoldLabel);
        }
        Handles.EndGUI();
    }

    private void DrawTimelineBlock(TimelineBlock block, SerializedProperty actionsProp)
    {
        float x = 16f + block.StartTime * pixelsPerSecond;
        float y = RulerHeight + 10f + block.Lane * (LaneHeight + LaneSpacing);
        float w = Mathf.Max(52f, block.Duration * pixelsPerSecond);
        Rect blockRect = new Rect(x, y, w, LaneHeight);

        bool isSelected = block.Index == selectedIndex;
        bool isExecuting = Application.isPlaying && block.Action != null && block.Action.IsExecuting;

        Color baseColor = GetActionColor(block.Action);
        if (isExecuting)
            baseColor = new Color(0.25f, 0.85f, 0.45f);

        Color borderColor = isSelected ? Color.white : (isExecuting ? Color.green : new Color(0.08f, 0.08f, 0.08f));
        EditorGUI.DrawRect(new Rect(blockRect.x - 2, blockRect.y - 2, blockRect.width + 4, blockRect.height + 4), borderColor);
        EditorGUI.DrawRect(blockRect, baseColor);

        Color badgeColor = block.Action.type == SeqComponent.Action.ActionType.Join
            ? new Color(0.95f, 0.65f, 0.2f)
            : new Color(0.2f, 0.6f, 0.95f);
        EditorGUI.DrawRect(new Rect(blockRect.x, blockRect.y, 5f, blockRect.height), badgeColor);

        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            normal = { textColor = Color.white },
            clipping = TextClipping.Clip
        };
        GUIStyle subStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            normal = { textColor = new Color(0.92f, 0.92f, 0.92f) },
            clipping = TextClipping.Clip
        };

        string actionName = GetActionDisplayName(block.Action);
        string typeTag = block.Index == 0 ? "START" : (block.Action.type == SeqComponent.Action.ActionType.Join ? "JOIN" : "AFTER");
        string durText = block.IsInstant ? "Instant" : $"{block.Duration:0.##}s";

        GUI.Label(new Rect(blockRect.x + 8, blockRect.y + 4, blockRect.width - 12, 18), $"#{block.Index} {actionName}", titleStyle);
        GUI.Label(new Rect(blockRect.x + 8, blockRect.y + 24, blockRect.width - 12, 16), $"{typeTag} • {durText}", subStyle);

        Event e = Event.current;
        if (e.type == EventType.MouseDown && blockRect.Contains(e.mousePosition) && e.button != 2 && !e.alt)
        {
            selectedIndex = block.Index;
            GUI.FocusControl(null);

            if (e.button == 1)
            {
                ShowBlockContextMenu(block.Index);
            }

            e.Use();
            Repaint();
        }
    }

    private void ShowBlockContextMenu(int index)
    {
        GenericMenu menu = new GenericMenu();
        SeqComponent.Action action = targetSequencer.Actions[index];
        int groupEnd = GetGroupEndIndex(index);

        menu.AddItem(new GUIContent("Insérer avant (After)"), false, () =>
            ShowQuickAddMenu(index, SeqComponent.Action.ActionType.After));

        menu.AddItem(new GUIContent("Insérer après (After)"), false, () =>
            ShowQuickAddMenu(groupEnd + 1, SeqComponent.Action.ActionType.After));

        menu.AddItem(new GUIContent("Ajouter en parallèle (Join)"), false, () =>
            ShowQuickAddMenu(groupEnd + 1, SeqComponent.Action.ActionType.Join));

        menu.AddSeparator("");

        if (action is SeqComponent.ActionScriptable sc && sc.ActionData != null)
        {
            menu.AddItem(new GUIContent("Dupliquer le Scriptable et remplacer"), false, () =>
                DuplicateAndReplaceScriptable(index));
            menu.AddSeparator("");
        }

        menu.AddItem(new GUIContent("Basculer After / Join"), false, () =>
        {
            Undo.RecordObject(targetSequencer, "Toggle Action Type");
            action.type = action.type == SeqComponent.Action.ActionType.After
                ? SeqComponent.Action.ActionType.Join
                : SeqComponent.Action.ActionType.After;
            EditorUtility.SetDirty(targetSequencer);
        });

        if (index > 0)
        {
            menu.AddItem(new GUIContent("Déplacer avant (↑)"), false, () => MoveAction(index, index - 1));
        }
        if (index < targetSequencer.Actions.Count - 1)
        {
            menu.AddItem(new GUIContent("Déplacer après (↓)"), false, () => MoveAction(index, index + 1));
        }

        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Supprimer"), false, () => DeleteAction(index));
        menu.ShowAsContext();
    }

    private void DuplicateAndReplaceScriptable(int index)
    {
        if (targetSequencer == null || targetSequencer.Actions == null) return;
        if (index < 0 || index >= targetSequencer.Actions.Count) return;

        if (targetSequencer.Actions[index] is not SeqComponent.ActionScriptable sc || sc.ActionData == null)
            return;

        string sourcePath = AssetDatabase.GetAssetPath(sc.ActionData);
        if (string.IsNullOrEmpty(sourcePath))
            return;

        string newPath = AssetDatabase.GenerateUniqueAssetPath(sourcePath);
        if (!AssetDatabase.CopyAsset(sourcePath, newPath))
            return;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        SequenceActionData duplicatedAsset = AssetDatabase.LoadAssetAtPath<SequenceActionData>(newPath);
        if (duplicatedAsset == null)
            return;

        Undo.RecordObject(targetSequencer, "Duplicate and Replace Scriptable Action");
        sc.ActionData = duplicatedAsset;
        EditorUtility.SetDirty(targetSequencer);
        serializedSequencer.Update();

        ClearCachedScriptableEditor();
        EditorGUIUtility.PingObject(duplicatedAsset);
        Repaint();
    }

    private void DrawInspectorPanel(Rect rect, SerializedProperty actionsProp)
    {
        EditorGUI.DrawRect(rect, new Color(0.21f, 0.21f, 0.21f));
        EditorGUI.DrawRect(new Rect(rect.x, rect.y, 2f, rect.height), new Color(0.08f, 0.08f, 0.08f));

        GUILayout.BeginArea(new Rect(rect.x + 8f, rect.y + 6f, rect.width - 16f, rect.height - 12f));

        if (selectedIndex < 0 || selectedIndex >= actionsProp.arraySize)
        {
            EditorGUILayout.HelpBox("Clique sur un bloc dans la timeline pour l'éditer.\n\n• Clic Molette + Drag : déplacer la timeline\n• Ctrl + Molette : zoomer\n• + After / + Join : ajouter en 1 clic", MessageType.Info);
            GUILayout.EndArea();
            return;
        }

        SerializedProperty actionProp = actionsProp.GetArrayElementAtIndex(selectedIndex);
        SeqComponent.Action actionObj = targetSequencer.Actions[selectedIndex];

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"#{selectedIndex} {GetActionDisplayName(actionObj)}", EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(selectedIndex <= 0);
        if (GUILayout.Button("▲", GUILayout.Width(24))) MoveAction(selectedIndex, selectedIndex - 1);
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(selectedIndex >= actionsProp.arraySize - 1);
        if (GUILayout.Button("▼", GUILayout.Width(24))) MoveAction(selectedIndex, selectedIndex + 1);
        EditorGUI.EndDisabledGroup();

        GUI.backgroundColor = new Color(1f, 0.55f, 0.55f);
        if (GUILayout.Button("X", GUILayout.Width(24)))
        {
            DeleteAction(selectedIndex);
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
            return;
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(4);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("+ Avant", GUILayout.Height(22)))
        {
            ShowQuickAddMenu(selectedIndex, SeqComponent.Action.ActionType.After);
        }
        if (GUILayout.Button("+ Après", GUILayout.Height(22)))
        {
            ShowQuickAddMenu(GetGroupEndIndex(selectedIndex) + 1, SeqComponent.Action.ActionType.After);
        }
        if (GUILayout.Button("+ Join", GUILayout.Height(22)))
        {
            ShowQuickAddMenu(GetGroupEndIndex(selectedIndex) + 1, SeqComponent.Action.ActionType.Join);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(6);

        inspectorScrollPos = EditorGUILayout.BeginScrollView(inspectorScrollPos);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        SerializedProperty typeProp = actionProp.FindPropertyRelative("type");
        typeProp.enumValueIndex = GUILayout.Toolbar(typeProp.enumValueIndex, new[] { "After (Séquentiel)", "Join (Parallèle)" }, GUILayout.Height(22));

        SerializedProperty changeTargetProp = actionProp.FindPropertyRelative("ChangeTarget");
        EditorGUILayout.PropertyField(changeTargetProp, new GUIContent("Override Target"));
        if (changeTargetProp.boolValue)
        {
            EditorGUILayout.PropertyField(actionProp.FindPropertyRelative("Target"));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(6);

        SerializedProperty actionDataProp = actionProp.FindPropertyRelative("ActionData");

        if (actionObj is SeqComponent.ActionReference)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Paramètres (Ref Action)", EditorStyles.boldLabel);
            DrawChildProperties(actionDataProp);
            EditorGUILayout.EndVertical();
        }
        else if (actionObj is SeqComponent.ActionScriptable)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Asset (Scriptable Action)", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(actionDataProp, new GUIContent("Action Data"));

            UnityEngine.Object soObj = actionDataProp.objectReferenceValue;
            if (soObj != null)
            {
                EditorGUILayout.Space(6);
                if (cachedScriptableTarget != soObj)
                {
                    ClearCachedScriptableEditor();
                    cachedScriptableTarget = soObj;
                    cachedScriptableEditor = Editor.CreateEditor(soObj);
                }

                if (cachedScriptableEditor != null)
                {
                    cachedScriptableEditor.OnInspectorGUI();
                }
            }
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void DrawChildProperties(SerializedProperty parentProp)
    {
        if (parentProp == null) return;

        SerializedProperty copy = parentProp.Copy();
        SerializedProperty end = copy.GetEndProperty();

        bool enterChildren = true;
        while (copy.NextVisible(enterChildren) && !SerializedProperty.EqualContents(copy, end))
        {
            enterChildren = false;
            EditorGUILayout.PropertyField(copy, true);
        }
    }

    private void ShowQuickAddMenu(int insertIndex, SeqComponent.Action.ActionType actionType)
    {
        GenericMenu menu = new GenericMenu();

        var refTypes = TypeCache.GetTypesDerivedFrom<SequenceRefActionData>()
            .Where(t => !t.IsAbstract && !t.IsGenericType)
            .OrderBy(t => t.Name)
            .ToList();

        foreach (Type t in refTypes)
        {
            string label = t.Name.Replace("ActionData", "").Replace("Action", "");
            menu.AddItem(new GUIContent(label), false, () => InsertRefAction(t, insertIndex, actionType));
        }

        menu.AddSeparator("");

        string[] guids = AssetDatabase.FindAssets("t:SequenceActionData");
        if (guids.Length > 0)
        {
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SequenceActionData asset = AssetDatabase.LoadAssetAtPath<SequenceActionData>(path);
                if (asset != null)
                {
                    menu.AddItem(new GUIContent($"Assets existants/{asset.name}"), false, () => InsertScriptableAction(asset, insertIndex, actionType));
                }
            }
        }

        menu.AddItem(new GUIContent("Scriptable/Slot vide"), false, () => InsertScriptableAction(null, insertIndex, actionType));

        var soTypes = TypeCache.GetTypesDerivedFrom<SequenceActionData>()
            .Where(t => !t.IsAbstract && !t.IsGenericType)
            .OrderBy(t => t.Name)
            .ToList();

        foreach (Type t in soTypes)
        {
            menu.AddItem(new GUIContent($"Scriptable/Créer nouvel asset/{t.Name}"), false, () => CreateAndInsertScriptableAction(t, insertIndex, actionType));
        }

        menu.ShowAsContext();
    }

    private void InsertRefAction(Type refActionType, int insertIndex, SeqComponent.Action.ActionType actionType)
    {
        EnsureActionsList();
        Undo.RecordObject(targetSequencer, "Add Ref Action");

        var instance = (SequenceRefActionData)Activator.CreateInstance(refActionType);
        var wrapper = new SeqComponent.ActionReference
        {
            ActionData = instance,
            type = actionType
        };

        int idx = Mathf.Clamp(insertIndex, 0, targetSequencer.Actions.Count);
        targetSequencer.Actions.Insert(idx, wrapper);
        selectedIndex = idx;
        EditorUtility.SetDirty(targetSequencer);
        serializedSequencer.Update();
    }

    private void InsertScriptableAction(SequenceActionData data, int insertIndex, SeqComponent.Action.ActionType actionType)
    {
        EnsureActionsList();
        Undo.RecordObject(targetSequencer, "Add Scriptable Action");

        var wrapper = new SeqComponent.ActionScriptable
        {
            ActionData = data,
            type = actionType
        };

        int idx = Mathf.Clamp(insertIndex, 0, targetSequencer.Actions.Count);
        targetSequencer.Actions.Insert(idx, wrapper);
        selectedIndex = idx;
        EditorUtility.SetDirty(targetSequencer);
        serializedSequencer.Update();
    }

    private void CreateAndInsertScriptableAction(Type soType, int insertIndex, SeqComponent.Action.ActionType actionType)
    {
        string path = EditorUtility.SaveFilePanelInProject(
            $"Créer {soType.Name}",
            soType.Name,
            "asset",
            "Choisis où sauvegarder l'asset d'action"
        );
        if (string.IsNullOrEmpty(path)) return;

        SequenceActionData asset = (SequenceActionData)CreateInstance(soType);
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        InsertScriptableAction(asset, insertIndex, actionType);
    }

    private int GetGroupEndIndex(int index)
    {
        if (targetSequencer.Actions == null || index < 0 || index >= targetSequencer.Actions.Count)
            return index;

        int end = index;
        for (int i = index + 1; i < targetSequencer.Actions.Count; i++)
        {
            if (targetSequencer.Actions[i].type == SeqComponent.Action.ActionType.After)
                break;
            end = i;
        }
        return end;
    }

    private void EnsureActionsList()
    {
        if (targetSequencer.Actions == null)
        {
            FieldInfo field = typeof(SeqComponent).GetField("actions", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(targetSequencer, new List<SeqComponent.Action>());
        }
    }

    private void MoveAction(int fromIndex, int toIndex)
    {
        if (targetSequencer.Actions == null) return;
        if (toIndex < 0 || toIndex >= targetSequencer.Actions.Count) return;

        Undo.RecordObject(targetSequencer, "Reorder Sequence Action");
        SeqComponent.Action item = targetSequencer.Actions[fromIndex];
        targetSequencer.Actions.RemoveAt(fromIndex);
        targetSequencer.Actions.Insert(toIndex, item);
        selectedIndex = toIndex;
        EditorUtility.SetDirty(targetSequencer);
        serializedSequencer.Update();
    }

    private void DeleteAction(int index)
    {
        if (targetSequencer.Actions == null || index < 0 || index >= targetSequencer.Actions.Count) return;

        Undo.RecordObject(targetSequencer, "Delete Sequence Action");
        targetSequencer.Actions.RemoveAt(index);
        selectedIndex = Mathf.Clamp(selectedIndex, -1, targetSequencer.Actions.Count - 1);
        EditorUtility.SetDirty(targetSequencer);
        serializedSequencer.Update();
    }

    private List<TimelineBlock> BuildTimelineBlocks()
    {
        var blocks = new List<TimelineBlock>();
        if (targetSequencer.Actions == null) return blocks;

        float cursorTime = 0f;
        float currentBlockStart = 0f;
        int currentLane = 0;

        for (int i = 0; i < targetSequencer.Actions.Count; i++)
        {
            SeqComponent.Action action = targetSequencer.Actions[i];
            if (action == null) continue;

            float dur = EstimateActionDuration(action, out bool isInstant);

            if (i == 0 || action.type == SeqComponent.Action.ActionType.After)
            {
                currentBlockStart = cursorTime;
                currentLane = 0;
                cursorTime += dur;
            }
            else
            {
                currentLane++;
            }

            blocks.Add(new TimelineBlock
            {
                Index = i,
                Action = action,
                StartTime = currentBlockStart,
                Duration = dur,
                Lane = currentLane,
                IsInstant = isInstant
            });
        }

        return blocks;
    }

    private float EstimateActionDuration(SeqComponent.Action action, out bool isInstant)
    {
        object dataObj = null;
        if (action is SeqComponent.ActionScriptable sc) dataObj = sc.ActionData;
        else if (action is SeqComponent.ActionReference rf) dataObj = rf.ActionData;

        if (dataObj != null)
        {
            Type t = dataObj.GetType();
            string[] candidateNames = { "Duration", "duration", "Value", "value", "Delay", "delay", "Time", "time" };
            foreach (string name in candidateNames)
            {
                FieldInfo f = t.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (f != null && f.FieldType == typeof(float))
                {
                    float val = (float)f.GetValue(dataObj);
                    if (val > 0.001f)
                    {
                        isInstant = false;
                        return val;
                    }
                }
            }
        }

        isInstant = true;
        return 0.2f;
    }

    private string GetActionDisplayName(SeqComponent.Action action)
    {
        if (action is SeqComponent.ActionScriptable sc)
        {
            return sc.ActionData != null ? sc.ActionData.name : "Scriptable (Vide)";
        }
        if (action is SeqComponent.ActionReference rf)
        {
            return rf.ActionData != null ? rf.ActionData.GetType().Name.Replace("ActionData", "") : "Ref (Vide)";
        }
        return "Action";
    }

    private Color GetActionColor(SeqComponent.Action action)
    {
        string name = GetActionDisplayName(action).ToLowerInvariant();
        if (name.Contains("delay") || name.Contains("wait")) return new Color(0.35f, 0.35f, 0.42f);
        if (name.Contains("scale") || name.Contains("move") || name.Contains("rotat")) return new Color(0.22f, 0.42f, 0.58f);
        if (name.Contains("sound") || name.Contains("audio")) return new Color(0.52f, 0.32f, 0.55f);
        if (name.Contains("fade") || name.Contains("color")) return new Color(0.26f, 0.48f, 0.45f);
        return action is SeqComponent.ActionScriptable
            ? new Color(0.38f, 0.34f, 0.52f)
            : new Color(0.28f, 0.38f, 0.48f);
    }
}