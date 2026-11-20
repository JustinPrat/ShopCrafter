using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class GameFlowDebugWindow : EditorWindow
{
    private enum Tab
    {
        Milestones,
        Quests,
        PNJsRuntime,
        PNJCreator
    }

    private enum MilestoneSubTab
    {
        RuntimeRoadmap,
        EditorCreator
    }

    private Tab currentTab = Tab.Milestones;
    private MilestoneSubTab milestoneSubTab = MilestoneSubTab.EditorCreator;

    private Vector2 scrollPos;
    private Vector2 leftListScrollPos;
    private Vector2 rightInspectorScrollPos;

    private MilestoneManager cachedMilestoneManager;
    private QuestManager cachedQuestManager;
    private PNJManager cachedPNJManager;
    private ManagerRefs cachedManagerRefs;

    private int repToAdd = 50;
    private string questSearchFilter = "";
    private int questStateFilterIndex = 0;

    private List<MilestoneData> allMilestones = new List<MilestoneData>();
    private MilestoneData selectedMilestone;
    private Editor cachedMilestoneEditor;
    private string newMilestoneName = "Milestone_New";
    private string milestoneFolder = "Assets/Resources/Data/Milestones";
    private const string MilestoneFolderPref = "ShopCrafter_MilestoneFolder";

    private List<PNJInfoData> allPNJs = new List<PNJInfoData>();
    private List<PNJPool> allPNJPools = new List<PNJPool>();
    private PNJInfoData selectedPNJ;
    private Editor cachedPNJEditor;
    private string pnjSearchFilter = "";
    private string newPnjName = "PNJ_New";
    private string pnjFolder = "Assets/Resources/Data/PNJ";
    private bool createLinkedIdentity = true;
    private bool createLinkedDialogue = true;
    private int selectedIdentityTypeIndex = 0;
    private List<Type> identityTypes = new List<Type>();
    private string[] identityTypeNames = Array.Empty<string>();
    private int selectedPoolIndex = 0;
    private bool addToSpecialPool = false;
    private const string PNJFolderPref = "ShopCrafter_PNJFolder";

    [MenuItem("ShopCrafter/Tools/Game Flow & Debug")]
    public static void ShowWindow()
    {
        GameFlowDebugWindow window = GetWindow<GameFlowDebugWindow>("Game Flow & Debug");
        window.minSize = new Vector2(760f, 520f);
    }

    private void OnEnable()
    {
        milestoneFolder = EditorPrefs.GetString(MilestoneFolderPref, milestoneFolder);
        pnjFolder = EditorPrefs.GetString(PNJFolderPref, pnjFolder);
        RefreshAssetDatabases();
    }

    private void OnFocus()
    {
        RefreshAssetDatabases();
    }

    private void OnDisable()
    {
        ClearEditor(ref cachedMilestoneEditor);
        ClearEditor(ref cachedPNJEditor);
    }

    private void ClearEditor(ref Editor editor)
    {
        if (editor != null)
        {
            DestroyImmediate(editor);
            editor = null;
        }
    }

    private void RefreshAssetDatabases()
    {
        allMilestones = FindAllAssets<MilestoneData>().OrderBy(m => m.name).ToList();
        allPNJs = FindAllAssets<PNJInfoData>().OrderBy(p => p.name).ToList();
        allPNJPools = FindAllAssets<PNJPool>().OrderBy(p => p.name).ToList();

        identityTypes = TypeCache.GetTypesDerivedFrom<IdentityData>()
            .Where(t => !t.IsAbstract && !t.IsGenericType)
            .OrderBy(t => t.Name)
            .ToList();

        identityTypeNames = identityTypes.Select(t => t.Name).ToArray();
        selectedIdentityTypeIndex = Mathf.Clamp(selectedIdentityTypeIndex, 0, Mathf.Max(0, identityTypeNames.Length - 1));
    }

    private List<T> FindAllAssets<T>() where T : UnityEngine.Object
    {
        List<T> result = new List<T>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) result.Add(asset);
        }
        return result;
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(6);
        currentTab = (Tab)GUILayout.Toolbar(
            (int)currentTab,
            new[] { "Milestones", "Quêtes (Play)", "PNJs (Play)", "Créateur PNJ" },
            GUILayout.Height(26)
        );
        EditorGUILayout.Space(6);

        RefreshManagers();

        switch (currentTab)
        {
            case Tab.Milestones:
                DrawMilestonesRootTab();
                break;
            case Tab.Quests:
                DrawQuestsTab();
                break;
            case Tab.PNJsRuntime:
                DrawPNJsRuntimeTab();
                break;
            case Tab.PNJCreator:
                DrawPNJCreatorTab();
                break;
        }

        if (Application.isPlaying)
        {
            Repaint();
        }
    }

    private void RefreshManagers()
    {
        if (cachedMilestoneManager == null) cachedMilestoneManager = FindFirstObjectByType<MilestoneManager>();
        if (cachedQuestManager == null) cachedQuestManager = FindFirstObjectByType<QuestManager>();
        if (cachedPNJManager == null) cachedPNJManager = FindFirstObjectByType<PNJManager>();

        if (cachedManagerRefs == null && cachedMilestoneManager != null)
            cachedManagerRefs = GetPrivateField<ManagerRefs>(cachedMilestoneManager, "managerRefs");
        if (cachedManagerRefs == null && cachedQuestManager != null)
            cachedManagerRefs = GetPrivateField<ManagerRefs>(cachedQuestManager, "managerRefs");
        if (cachedManagerRefs == null && cachedPNJManager != null)
            cachedManagerRefs = GetPrivateField<ManagerRefs>(cachedPNJManager, "managerRefs");
    }

    private void DrawMilestonesRootTab()
    {
        milestoneSubTab = (MilestoneSubTab)GUILayout.Toolbar(
            (int)milestoneSubTab,
            new[] { "Roadmap & Debug (Play)", "Éditeur & Créateur (Edit/Play)" },
            GUILayout.Height(22)
        );
        EditorGUILayout.Space(6);

        if (milestoneSubTab == MilestoneSubTab.RuntimeRoadmap)
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Lance le Play Mode pour contrôler la progression des Milestones en direct.", MessageType.Info);
                return;
            }
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            DrawMilestonesRuntimeTab();
            EditorGUILayout.EndScrollView();
        }
        else
        {
            DrawMilestoneEditorCreatorTab();
        }
    }

    private void DrawMilestoneEditorCreatorTab()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical(GUILayout.Width(280f));

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Créer un Milestone", EditorStyles.boldLabel);
        newMilestoneName = EditorGUILayout.TextField("Nom", newMilestoneName);

        EditorGUILayout.BeginHorizontal();
        milestoneFolder = EditorGUILayout.TextField("Dossier", milestoneFolder);
        if (GUILayout.Button("...", GUILayout.Width(26)))
        {
            string picked = EditorUtility.OpenFolderPanel("Dossier des Milestones", "Assets", "");
            if (!string.IsNullOrEmpty(picked) && picked.StartsWith(Application.dataPath))
            {
                milestoneFolder = "Assets" + picked.Substring(Application.dataPath.Length);
                EditorPrefs.SetString(MilestoneFolderPref, milestoneFolder);
            }
        }
        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = new Color(0.45f, 0.85f, 0.55f);
        if (GUILayout.Button("+ Créer MilestoneData", GUILayout.Height(26)))
        {
            CreateNewMilestone();
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(6);

        List<MilestoneData> managerList = cachedMilestoneManager != null
            ? GetPrivateField<List<MilestoneData>>(cachedMilestoneManager, "milestoneList")
            : null;

        leftListScrollPos = EditorGUILayout.BeginScrollView(leftListScrollPos);

        if (managerList != null)
        {
            EditorGUILayout.LabelField($"Ordre dans MilestoneManager ({managerList.Count})", EditorStyles.boldLabel);
            for (int i = 0; i < managerList.Count; i++)
            {
                MilestoneData m = managerList[i];
                if (m == null) continue;

                bool isSel = selectedMilestone == m;
                GUI.backgroundColor = isSel ? new Color(0.6f, 0.85f, 1f) : Color.white;
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUI.backgroundColor = Color.white;

                if (GUILayout.Button($"{i}. {m.name}", EditorStyles.label))
                {
                    SelectMilestone(m);
                }

                EditorGUI.BeginDisabledGroup(i == 0);
                if (GUILayout.Button("▲", GUILayout.Width(20)))
                {
                    Undo.RecordObject(cachedMilestoneManager, "Reorder Milestone");
                    (managerList[i - 1], managerList[i]) = (managerList[i], managerList[i - 1]);
                    EditorUtility.SetDirty(cachedMilestoneManager);
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(i >= managerList.Count - 1);
                if (GUILayout.Button("▼", GUILayout.Width(20)))
                {
                    Undo.RecordObject(cachedMilestoneManager, "Reorder Milestone");
                    (managerList[i + 1], managerList[i]) = (managerList[i], managerList[i + 1]);
                    EditorUtility.SetDirty(cachedMilestoneManager);
                }
                EditorGUI.EndDisabledGroup();

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    Undo.RecordObject(cachedMilestoneManager, "Remove Milestone From Manager");
                    managerList.RemoveAt(i);
                    EditorUtility.SetDirty(cachedMilestoneManager);
                    EditorGUILayout.EndHorizontal();
                    break;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(8);
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Tous les assets ({allMilestones.Count})", EditorStyles.boldLabel);
        if (GUILayout.Button("↻", GUILayout.Width(24))) RefreshAssetDatabases();
        EditorGUILayout.EndHorizontal();

        foreach (MilestoneData m in allMilestones)
        {
            if (m == null) continue;
            bool isSel = selectedMilestone == m;
            bool inManager = managerList != null && managerList.Contains(m);

            GUI.backgroundColor = isSel ? new Color(0.6f, 0.85f, 1f) : Color.white;
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUI.backgroundColor = Color.white;

            if (GUILayout.Button(m.name, EditorStyles.label))
            {
                SelectMilestone(m);
            }

            if (managerList != null && !inManager && GUILayout.Button("+ Mgr", GUILayout.Width(46)))
            {
                Undo.RecordObject(cachedMilestoneManager, "Add Milestone To Manager");
                managerList.Add(m);
                EditorUtility.SetDirty(cachedMilestoneManager);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        if (selectedMilestone == null)
        {
            EditorGUILayout.HelpBox("Sélectionne un Milestone à gauche ou crées-en un nouveau pour l'éditer.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(selectedMilestone.name, EditorStyles.boldLabel);
            if (GUILayout.Button("Ping", GUILayout.Width(45)))
            {
                EditorGUIUtility.PingObject(selectedMilestone);
                Selection.activeObject = selectedMilestone;
            }
            if (GUILayout.Button("Dupliquer", GUILayout.Width(72)))
            {
                string src = AssetDatabase.GetAssetPath(selectedMilestone);
                string dst = AssetDatabase.GenerateUniqueAssetPath(src);
                if (AssetDatabase.CopyAsset(src, dst))
                {
                    AssetDatabase.SaveAssets();
                    RefreshAssetDatabases();
                    SelectMilestone(AssetDatabase.LoadAssetAtPath<MilestoneData>(dst));
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ Créer ObjectiveData", GUILayout.Height(22)))
            {
                ShowCreateSubAssetMenu<ObjectiveData>(created =>
                {
                    Undo.RecordObject(selectedMilestone, "Add Objective");
                    if (selectedMilestone.Objectives == null) selectedMilestone.Objectives = new List<ObjectiveData>();
                    selectedMilestone.Objectives.Add(created);
                    EditorUtility.SetDirty(selectedMilestone);
                });
            }
            if (GUILayout.Button("+ Créer DailyTicketData", GUILayout.Height(22)))
            {
                ShowCreateSubAssetMenu<DailyTicketData>(created =>
                {
                    Undo.RecordObject(selectedMilestone, "Add DailyTicket");
                    if (selectedMilestone.DailyTicketDatasPool == null)
                        selectedMilestone.DailyTicketDatasPool = new List<MilestoneData.DailyTicketWeighted>();
                    selectedMilestone.DailyTicketDatasPool.Add(new MilestoneData.DailyTicketWeighted
                    {
                        DailyTicketData = created,
                        WeightPercentChance = 50
                    });
                    EditorUtility.SetDirty(selectedMilestone);
                });
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);

            rightInspectorScrollPos = EditorGUILayout.BeginScrollView(rightInspectorScrollPos);
            if (cachedMilestoneEditor == null || cachedMilestoneEditor.target != selectedMilestone)
            {
                ClearEditor(ref cachedMilestoneEditor);
                cachedMilestoneEditor = Editor.CreateEditor(selectedMilestone);
            }
            cachedMilestoneEditor?.OnInspectorGUI();
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void SelectMilestone(MilestoneData m)
    {
        selectedMilestone = m;
        ClearEditor(ref cachedMilestoneEditor);
        if (m != null) cachedMilestoneEditor = Editor.CreateEditor(m);
    }

    private void CreateNewMilestone()
    {
        EnsureFolderExists(milestoneFolder);
        EditorPrefs.SetString(MilestoneFolderPref, milestoneFolder);

        string safeName = string.IsNullOrWhiteSpace(newMilestoneName) ? "Milestone_New" : newMilestoneName.Trim();
        string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(milestoneFolder, safeName + ".asset"));

        MilestoneData created = CreateInstance<MilestoneData>();
        created.ReputationRequired = 100;
        created.NumberTickets = 2;
        if (cachedManagerRefs != null) created.Refs = cachedManagerRefs;

        AssetDatabase.CreateAsset(created, path);
        AssetDatabase.SaveAssets();

        if (cachedMilestoneManager != null)
        {
            var list = GetPrivateField<List<MilestoneData>>(cachedMilestoneManager, "milestoneList");
            if (list != null)
            {
                Undo.RecordObject(cachedMilestoneManager, "Add New Milestone");
                list.Add(created);
                EditorUtility.SetDirty(cachedMilestoneManager);
            }
        }

        RefreshAssetDatabases();
        SelectMilestone(created);
        ShowNotification(new GUIContent($"Milestone '{created.name}' créé !"), 1.2f);
    }

    private void ShowCreateSubAssetMenu<TBase>(Action<TBase> onCreated) where TBase : ScriptableObject
    {
        GenericMenu menu = new GenericMenu();
        var types = TypeCache.GetTypesDerivedFrom<TBase>()
            .Where(t => !t.IsAbstract && !t.IsGenericType)
            .OrderBy(t => t.Name)
            .ToList();

        if (!typeof(TBase).IsAbstract) types.Insert(0, typeof(TBase));

        foreach (Type t in types)
        {
            menu.AddItem(new GUIContent(t.Name), false, () =>
            {
                string path = EditorUtility.SaveFilePanelInProject($"Créer {t.Name}", t.Name, "asset", "Emplacement de l'asset");
                if (string.IsNullOrEmpty(path)) return;

                TBase asset = (TBase)CreateInstance(t);
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                onCreated?.Invoke(asset);
            });
        }

        menu.ShowAsContext();
    }

    private void DrawMilestonesRuntimeTab()
    {
        if (cachedMilestoneManager == null)
        {
            EditorGUILayout.HelpBox("Aucun MilestoneManager trouvé dans la scène.", MessageType.Warning);
            return;
        }

        List<MilestoneData> milestoneList = GetPrivateField<List<MilestoneData>>(cachedMilestoneManager, "milestoneList");
        int currentIndex = cachedMilestoneManager.CurrentMilestoneIndex;
        int currentRep = cachedMilestoneManager.CurrentMilestoneReputation;
        MilestoneData currentData = cachedMilestoneManager.CurrentMilestone;
        bool isReady = cachedMilestoneManager.MilestoneReady;

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Contrôles Milestone", EditorStyles.boldLabel);

        if (currentData != null && currentData.IsSpecialCharacter)
        {
            EditorGUILayout.HelpBox("Ce milestone dépend d'un événement spécial (IsSpecialCharacter).", MessageType.Info);
            if (GUILayout.Button("Déclencher Milestone Special (Ready)", GUILayout.Height(26)))
            {
                cachedManagerRefs?.GameEventsManager?.milestoneEvents.MilestoneUpgrade();
            }
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            repToAdd = EditorGUILayout.IntField("Réputation", repToAdd);
            if (GUILayout.Button($"+{repToAdd} Rep", GUILayout.Width(95)))
            {
                cachedManagerRefs?.GameEventsManager?.milestoneEvents.GainReputation(repToAdd);
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Remplir Réputation (Ready)", GUILayout.Height(24)) && currentData != null)
            {
                int missing = Mathf.Max(0, currentData.ReputationRequired - currentRep);
                cachedManagerRefs?.GameEventsManager?.milestoneEvents.GainReputation(missing);
            }
        }

        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = new Color(0.45f, 0.85f, 0.55f);
        if (GUILayout.Button("Valider & Récupérer Récompenses", GUILayout.Height(28)))
        {
            if (currentData != null && !isReady && cachedManagerRefs?.GameEventsManager != null)
            {
                if (currentData.IsSpecialCharacter)
                    cachedManagerRefs.GameEventsManager.milestoneEvents.MilestoneUpgrade();
                else
                    cachedManagerRefs.GameEventsManager.milestoneEvents.GainReputation(Mathf.Max(0, currentData.ReputationRequired - currentRep));
            }
            cachedManagerRefs?.GameEventsManager?.milestoneEvents.ValidateMilestone();
        }
        GUI.backgroundColor = Color.white;

        if (GUILayout.Button("Claim Rewards seuls", GUILayout.Width(145), GUILayout.Height(28)))
        {
            InvokePrivateMethod(cachedMilestoneManager, "GetMilestoneRewards");
            ShowNotification(new GUIContent("Récompenses attribuées !"), 1f);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(8);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Objectifs & Tickets du Milestone actuel", EditorStyles.boldLabel);

        var objectives = cachedMilestoneManager.CurrentObjectiveBehaviors;
        if (objectives != null && objectives.Count > 0)
        {
            foreach (var obj in objectives)
            {
                if (obj != null) EditorGUILayout.LabelField($"• {FormatObjectSummary(obj)}", EditorStyles.wordWrappedLabel);
            }
        }
        else
        {
            EditorGUILayout.LabelField("Aucun objectif actif.", EditorStyles.miniLabel);
        }

        var tickets = cachedMilestoneManager.MilestoneDailyTickets;
        if (tickets != null && tickets.Count > 0)
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField($"Tickets journaliers ({tickets.Count}) :", EditorStyles.miniBoldLabel);
            foreach (var ticket in tickets)
            {
                if (ticket != null) EditorGUILayout.LabelField($"  - {FormatObjectSummary(ticket)}", EditorStyles.miniLabel);
            }
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Roadmap des Milestones", EditorStyles.boldLabel);
        if (milestoneList == null || milestoneList.Count == 0) return;

        for (int i = 0; i < milestoneList.Count; i++)
        {
            MilestoneData mData = milestoneList[i];
            if (mData == null) continue;

            bool isCompleted = i < currentIndex;
            bool isCurrent = i == currentIndex;

            GUI.backgroundColor = isCompleted
                ? new Color(0.7f, 1f, 0.75f)
                : (isCurrent ? new Color(0.75f, 0.9f, 1f) : Color.white);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.backgroundColor = Color.white;

            EditorGUILayout.BeginHorizontal();
            string statusIcon = isCompleted ? "[✓]" : (isCurrent ? "[▶]" : "[  ]");
            string specialBadge = mData.IsSpecialCharacter ? " [Spécial]" : "";
            EditorGUILayout.LabelField($"{statusIcon} Milestone {i} : {mData.name}{specialBadge}", EditorStyles.boldLabel);

            if (!isCurrent && GUILayout.Button("Aller ici", GUILayout.Width(68), GUILayout.Height(18)))
            {
                JumpToMilestone(i, milestoneList);
            }
            EditorGUILayout.EndHorizontal();

            if (mData.IsSpecialCharacter)
            {
                string specialStatus = isCompleted ? "Complété (Spécial)" : (isCurrent ? (isReady ? "Prêt à valider !" : "En attente événement spécial") : "Verrouillé");
                EditorGUILayout.LabelField(specialStatus, EditorStyles.miniBoldLabel);
            }
            else
            {
                int req = Mathf.Max(1, mData.ReputationRequired);
                int val = isCompleted ? req : (isCurrent ? currentRep : 0);
                Rect progressRect = GUILayoutUtility.GetRect(18f, 16f);
                string progressLabel = isCompleted ? $"Complété ({req}/{req} Rep)" : (isCurrent ? $"{val} / {req} Rep {(isReady ? "(PRÊT !)" : "")}" : $"0 / {req} Rep");
                EditorGUI.ProgressBar(progressRect, Mathf.Clamp01((float)val / req), progressLabel);
            }

            if (mData.Objectives != null && mData.Objectives.Count > 0)
            {
                EditorGUILayout.LabelField("Objectifs : " + string.Join(", ", mData.Objectives.Where(o => o != null).Select(o => o.name)), EditorStyles.miniLabel);
            }

            if (mData.CompleteRewards != null && mData.CompleteRewards.Count > 0)
            {
                var rewardNames = mData.CompleteRewards
                    .Where(r => r != null && r.Value != null)
                    .Select(r =>
                    {
                        var disp = r.Value.GetRewardDisplayData();
                        return disp != null && !string.IsNullOrEmpty(disp.DisplayName) ? disp.DisplayName : r.Value.ToString();
                    });
                EditorGUILayout.LabelField("Récompenses : " + string.Join(", ", rewardNames), EditorStyles.miniLabel);
            }

            EditorGUILayout.EndVertical();
        }
    }

    private void JumpToMilestone(int targetIndex, List<MilestoneData> milestoneList)
    {
        if (targetIndex < 0 || targetIndex >= milestoneList.Count) return;

        MilestoneData oldMilestone = cachedMilestoneManager.CurrentMilestone;
        var behaviors = cachedMilestoneManager.CurrentObjectiveBehaviors;

        oldMilestone?.OnExitMilestone(behaviors);

        MilestoneData nextMilestone = milestoneList[targetIndex];
        SetPrivateField(cachedMilestoneManager, "currentMilestoneIndex", targetIndex);
        SetPrivateField(cachedMilestoneManager, "currentMilestone", nextMilestone);
        SetPrivateField(cachedMilestoneManager, "currentMilestoneReputation", 0);
        SetPrivateField(cachedMilestoneManager, "milestoneReady", false);

        nextMilestone.OnEnterMilestone(behaviors);
        InvokePrivateMethod(cachedMilestoneManager, "RerollDailyTicket", nextMilestone);
        cachedManagerRefs?.GameEventsManager?.milestoneEvents.ReachMilestone(targetIndex);
    }

    private void DrawQuestsTab()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Lance le Play Mode pour gérer l'état des Quêtes.", MessageType.Info);
            return;
        }

        if (cachedQuestManager == null)
        {
            EditorGUILayout.HelpBox("Aucun QuestManager trouvé dans la scène.", MessageType.Warning);
            return;
        }

        Dictionary<string, Quest> questMap = GetPrivateField<Dictionary<string, Quest>>(cachedQuestManager, "questMap");
        if (questMap == null || questMap.Count == 0)
        {
            EditorGUILayout.HelpBox("Aucune quête chargée dans QuestManager.", MessageType.Info);
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.BeginHorizontal();
        questSearchFilter = EditorGUILayout.TextField(questSearchFilter, EditorStyles.toolbarSearchField);
        if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(22)))
        {
            questSearchFilter = "";
            GUI.FocusControl(null);
        }
        EditorGUILayout.EndHorizontal();

        string[] stateFilters = new[] { "Toutes" }.Concat(Enum.GetNames(typeof(QuestState))).ToArray();
        questStateFilterIndex = EditorGUILayout.Popup("Filtrer par état", questStateFilterIndex, stateFilters);

        if (GUILayout.Button("Reset PlayerPrefs Quêtes", GUILayout.Height(22)))
        {
            foreach (var q in questMap.Values)
            {
                if (q?.Info != null) PlayerPrefs.DeleteKey(q.Info.ID);
            }
            PlayerPrefs.Save();
            ShowNotification(new GUIContent("Sauvegardes quêtes supprimées"), 1.2f);
        }

        EditorGUILayout.Space(6);

        foreach (Quest quest in questMap.Values)
        {
            if (quest == null || quest.Info == null) continue;

            if (questStateFilterIndex > 0 && quest.State != (QuestState)(questStateFilterIndex - 1))
                continue;

            if (!string.IsNullOrEmpty(questSearchFilter) &&
                quest.Info.ID.IndexOf(questSearchFilter, StringComparison.OrdinalIgnoreCase) < 0 &&
                quest.Info.name.IndexOf(questSearchFilter, StringComparison.OrdinalIgnoreCase) < 0)
                continue;

            DrawQuestCard(quest);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawQuestCard(Quest quest)
    {
        Color stateColor = quest.State switch
        {
            QuestState.IN_PROGRESS => new Color(0.75f, 0.9f, 1f),
            QuestState.CAN_FINISH => new Color(1f, 0.92f, 0.6f),
            QuestState.FINISHED => new Color(0.7f, 1f, 0.75f),
            _ => Color.white
        };

        GUI.backgroundColor = stateColor;
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUI.backgroundColor = Color.white;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"{quest.Info.name} ({quest.Info.ID})", EditorStyles.boldLabel);
        EditorGUILayout.LabelField(quest.State.ToString(), EditorStyles.miniBoldLabel, GUILayout.Width(105));
        EditorGUILayout.EndHorizontal();

        QuestData data = quest.GetQuestData();
        int totalSteps = data.questStepStates != null ? data.questStepStates.Length : 0;
        EditorGUILayout.LabelField($"Étape actuelle : {data.questStepIndex} / {totalSteps}", EditorStyles.miniLabel);

        EditorGUILayout.BeginHorizontal();
        if (quest.State == QuestState.NOT_RECEIVED && GUILayout.Button("Démarrer", GUILayout.Height(22)))
            cachedManagerRefs?.GameEventsManager?.questEvents.StartQuest(quest.Info.ID, null);

        if (quest.State == QuestState.IN_PROGRESS && GUILayout.Button("Étape suivante", GUILayout.Height(22)))
            cachedManagerRefs?.GameEventsManager?.questEvents.AdvanceQuest(quest.Info.ID);

        if (quest.State == QuestState.IN_PROGRESS || quest.State == QuestState.CAN_FINISH)
        {
            GUI.backgroundColor = new Color(0.45f, 0.85f, 0.55f);
            if (GUILayout.Button("Terminer & Récompenses", GUILayout.Height(22)))
                cachedManagerRefs?.GameEventsManager?.questEvents.FinishQuest(quest.Info.ID);
            GUI.backgroundColor = Color.white;
        }

        if (GUILayout.Button("◎", GUILayout.Width(26), GUILayout.Height(22)))
        {
            EditorGUIUtility.PingObject(quest.Info);
            Selection.activeObject = quest.Info;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void DrawPNJsRuntimeTab()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Lance le Play Mode pour contrôler le spawn des PNJs et le cycle de la journée.", MessageType.Info);
            return;
        }

        if (cachedPNJManager == null)
        {
            EditorGUILayout.HelpBox("Aucun PNJManager trouvé dans la scène.", MessageType.Warning);
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        List<PNJBrain> activePnjs = GetPrivateField<List<PNJBrain>>(cachedPNJManager, "pnjList");
        List<PNJBrain> respawnPnjs = GetPrivateField<List<PNJBrain>>(cachedPNJManager, "needRespawnPNJList");
        List<PNJInfoData> basicPool = GetPrivateField<List<PNJInfoData>>(cachedPNJManager, "PNJDataPoolList");
        List<PNJInfoData> specialPool = GetPrivateField<List<PNJInfoData>>(cachedPNJManager, "SpecialPNJDataPoolList");
        int dayIndex = GetPrivateField<int>(cachedPNJManager, "dayIndex");
        int targetCount = GetPrivateField<int>(cachedPNJManager, "targetNumberPnj");

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField($"Cycle Journée — Jour {dayIndex} | Période : {cachedPNJManager.CurrentDayPeriod}", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Start Morning", GUILayout.Height(24))) cachedPNJManager.StartDay();
        if (GUILayout.Button("Start Afternoon (Spawn)", GUILayout.Height(24))) cachedPNJManager.StartAfternoon();
        if (GUILayout.Button("Near End Day", GUILayout.Height(24))) InvokePrivateMethod(cachedPNJManager, "NearDayEnd");
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(6);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField($"Gestion du Spawn ({activePnjs?.Count ?? 0} / {targetCount} actifs | {respawnPnjs?.Count ?? 0} en file respawn)", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        GUI.backgroundColor = new Color(0.45f, 0.85f, 0.55f);
        if (GUILayout.Button("Forcer Spawn PNJ (Pool)", GUILayout.Height(28)))
            InvokePrivateMethod(cachedPNJManager, "SpawnPNJ");
        GUI.backgroundColor = Color.white;

        if (GUILayout.Button("Reset Identity Factories", GUILayout.Width(155), GUILayout.Height(28)))
            cachedPNJManager.ResetIdentityFactories();
        EditorGUILayout.EndHorizontal();

        if (basicPool != null && basicPool.Count > 0)
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField($"Pool Basique disponible ({basicPool.Count}) :", EditorStyles.miniBoldLabel);
            foreach (PNJInfoData info in basicPool.ToList())
                if (info != null) DrawPoolPnjRow(info);
        }

        if (specialPool != null && specialPool.Count > 0)
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField($"Pool Spécial disponible ({specialPool.Count}) :", EditorStyles.miniBoldLabel);
            foreach (PNJInfoData info in specialPool.ToList())
                if (info != null) DrawPoolPnjRow(info);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField($"PNJs actifs dans la scène ({activePnjs?.Count ?? 0})", EditorStyles.boldLabel);

        if (activePnjs != null)
        {
            foreach (PNJBrain pnj in activePnjs.ToList())
            {
                if (pnj == null) continue;
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                string pnjName = pnj.Data != null ? pnj.Data.Identity.Name : pnj.name;
                string specialTag = (pnj.Data != null && pnj.Data.IsSpecial) ? " [SPÉCIAL]" : "";
                EditorGUILayout.LabelField($"{pnjName}{specialTag}", EditorStyles.boldLabel);

                if (GUILayout.Button("Focus", GUILayout.Width(52)))
                {
                    Selection.activeGameObject = pnj.gameObject;
                    SceneView.lastActiveSceneView?.FrameSelected();
                }

                GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
                if (GUILayout.Button("Retirer", GUILayout.Width(58)))
                    cachedPNJManager.RemovePnj(pnj);
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawPoolPnjRow(PNJInfoData pnjData)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"• {pnjData.name}", EditorStyles.miniLabel);
        if (GUILayout.Button("Spawn", GUILayout.Width(55), GUILayout.Height(18)))
            SpawnSpecificPNJ(pnjData);
        EditorGUILayout.EndHorizontal();
    }

    private void SpawnSpecificPNJ(PNJInfoData pnjData)
    {
        if (pnjData == null || pnjData.PnjPrefab == null) return;
        PNJBrain pnj = Instantiate(pnjData.PnjPrefab).GetComponent<PNJBrain>();
        if (pnj == null) return;

        pnj.Setup(pnjData);
        if (pnj.NavMeshAgent != null) pnj.NavMeshAgent.Warp(cachedPNJManager.PnjSpawnOutside);

        List<PNJBrain> activePnjs = GetPrivateField<List<PNJBrain>>(cachedPNJManager, "pnjList");
        activePnjs?.Add(pnj);
    }

    private void DrawPNJCreatorTab()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical(GUILayout.Width(300f));

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Créer un nouveau PNJ", EditorStyles.boldLabel);

        newPnjName = EditorGUILayout.TextField("Nom du PNJ", newPnjName);

        EditorGUILayout.BeginHorizontal();
        pnjFolder = EditorGUILayout.TextField("Dossier", pnjFolder);
        if (GUILayout.Button("...", GUILayout.Width(26)))
        {
            string picked = EditorUtility.OpenFolderPanel("Dossier des PNJs", "Assets", "");
            if (!string.IsNullOrEmpty(picked) && picked.StartsWith(Application.dataPath))
            {
                pnjFolder = "Assets" + picked.Substring(Application.dataPath.Length);
                EditorPrefs.SetString(PNJFolderPref, pnjFolder);
            }
        }
        EditorGUILayout.EndHorizontal();

        createLinkedIdentity = EditorGUILayout.Toggle("Créer IdentityData lié", createLinkedIdentity);
        if (createLinkedIdentity && identityTypeNames.Length > 0)
        {
            EditorGUI.indentLevel++;
            selectedIdentityTypeIndex = EditorGUILayout.Popup("Type d'Identity", selectedIdentityTypeIndex, identityTypeNames);
            EditorGUI.indentLevel--;
        }

        createLinkedDialogue = EditorGUILayout.Toggle("Créer DialogueData lié", createLinkedDialogue);

        if (allPNJPools.Count > 0)
        {
            string[] poolNames = new[] { "Aucun Pool" }.Concat(allPNJPools.Select(p => p.name)).ToArray();
            selectedPoolIndex = Mathf.Clamp(selectedPoolIndex, 0, poolNames.Length - 1);
            selectedPoolIndex = EditorGUILayout.Popup("Ajouter au Pool", selectedPoolIndex, poolNames);
            if (selectedPoolIndex > 0)
            {
                addToSpecialPool = EditorGUILayout.Toggle("Mettre dans SpecialPool", addToSpecialPool);
            }
        }

        EditorGUILayout.Space(4);
        GUI.backgroundColor = new Color(0.45f, 0.85f, 0.55f);
        if (GUILayout.Button("+ Créer PNJ Complet", GUILayout.Height(28)))
        {
            CreateCompletePNJ();
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(6);

        EditorGUILayout.BeginHorizontal();
        pnjSearchFilter = EditorGUILayout.TextField(pnjSearchFilter, EditorStyles.toolbarSearchField);
        if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(22)))
        {
            pnjSearchFilter = "";
            GUI.FocusControl(null);
        }
        if (GUILayout.Button("↻", EditorStyles.toolbarButton, GUILayout.Width(24)))
        {
            RefreshAssetDatabases();
        }
        EditorGUILayout.EndHorizontal();

        leftListScrollPos = EditorGUILayout.BeginScrollView(leftListScrollPos);
        foreach (PNJInfoData pnj in allPNJs)
        {
            if (pnj == null) continue;
            if (!string.IsNullOrEmpty(pnjSearchFilter) &&
                pnj.name.IndexOf(pnjSearchFilter, StringComparison.OrdinalIgnoreCase) < 0)
                continue;

            bool isSel = selectedPNJ == pnj;
            GUI.backgroundColor = isSel ? new Color(0.6f, 0.85f, 1f) : Color.white;
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUI.backgroundColor = Color.white;

            if (GUILayout.Button(pnj.name, EditorStyles.label))
            {
                SelectPNJ(pnj);
            }

            if (Application.isPlaying && cachedPNJManager != null && GUILayout.Button("Spawn", GUILayout.Width(50)))
            {
                SpawnSpecificPNJ(pnj);
            }

            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        if (selectedPNJ == null)
        {
            EditorGUILayout.HelpBox("Sélectionne un PNJInfoData à gauche ou crée un nouveau PNJ complet.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(selectedPNJ.name, EditorStyles.boldLabel);
            if (GUILayout.Button("Ping", GUILayout.Width(45)))
            {
                EditorGUIUtility.PingObject(selectedPNJ);
                Selection.activeObject = selectedPNJ;
            }
            if (GUILayout.Button("Dupliquer", GUILayout.Width(72)))
            {
                string src = AssetDatabase.GetAssetPath(selectedPNJ);
                string dst = AssetDatabase.GenerateUniqueAssetPath(src);
                if (AssetDatabase.CopyAsset(src, dst))
                {
                    AssetDatabase.SaveAssets();
                    RefreshAssetDatabases();
                    SelectPNJ(AssetDatabase.LoadAssetAtPath<PNJInfoData>(dst));
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = new Color(0.45f, 0.75f, 1f);
            if (GUILayout.Button("Éditer Dialogue (Graph)", GUILayout.Height(22)))
            {
                DialogueGraphWindow.OpenWithPNJ(selectedPNJ);
            }
            GUI.backgroundColor = Color.white;

            if (allPNJPools.Count > 0 && GUILayout.Button("Ajouter à un PNJPool...", GUILayout.Height(22)))
            {
                ShowAddPNJToPoolMenu(selectedPNJ);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);

            rightInspectorScrollPos = EditorGUILayout.BeginScrollView(rightInspectorScrollPos);
            if (cachedPNJEditor == null || cachedPNJEditor.target != selectedPNJ)
            {
                ClearEditor(ref cachedPNJEditor);
                cachedPNJEditor = Editor.CreateEditor(selectedPNJ);
            }
            cachedPNJEditor?.OnInspectorGUI();
            EditorGUILayout.EndScrollView();
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
    }

    private void SelectPNJ(PNJInfoData pnj)
    {
        selectedPNJ = pnj;
        ClearEditor(ref cachedPNJEditor);
        if (pnj != null) cachedPNJEditor = Editor.CreateEditor(pnj);
    }

    private void CreateCompletePNJ()
    {
        EnsureFolderExists(pnjFolder);
        EditorPrefs.SetString(PNJFolderPref, pnjFolder);

        string safeName = string.IsNullOrWhiteSpace(newPnjName) ? "PNJ_New" : newPnjName.Trim();
        string subFolder = Path.Combine(pnjFolder, safeName);
        EnsureFolderExists(subFolder);

        DialogueData createdDialogue = null;
        if (createLinkedDialogue)
        {
            string dlgPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(subFolder, $"{safeName}_Dialogue.asset"));
            createdDialogue = CreateInstance<DialogueData>();
            AssetDatabase.CreateAsset(createdDialogue, dlgPath);
        }

        IdentityData createdIdentity = null;
        if (createLinkedIdentity && identityTypes.Count > 0)
        {
            Type idType = identityTypes[Mathf.Clamp(selectedIdentityTypeIndex, 0, identityTypes.Count - 1)];
            string idPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(subFolder, $"{safeName}_Identity.asset"));
            createdIdentity = (IdentityData)CreateInstance(idType);
            AssetDatabase.CreateAsset(createdIdentity, idPath);

            if (createdDialogue != null)
            {
                TryAssignReferenceByType(createdIdentity, createdDialogue);
            }
        }

        string pnjPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(subFolder, $"{safeName}_Info.asset"));
        PNJInfoData createdPNJ = CreateInstance<PNJInfoData>();
        AssetDatabase.CreateAsset(createdPNJ, pnjPath);

        if (createdIdentity != null) TryAssignReferenceByType(createdPNJ, createdIdentity);
        if (createdDialogue != null) TryAssignReferenceByType(createdPNJ, createdDialogue);

        if (selectedPoolIndex > 0 && selectedPoolIndex - 1 < allPNJPools.Count)
        {
            PNJPool pool = allPNJPools[selectedPoolIndex - 1];
            Undo.RecordObject(pool, "Add PNJ to Pool");
            if (addToSpecialPool) pool.SpecialPNJPool.Add(createdPNJ);
            else pool.PNJPoolList.Add(createdPNJ);
            EditorUtility.SetDirty(pool);
        }

        AssetDatabase.SaveAssets();
        RefreshAssetDatabases();
        SelectPNJ(createdPNJ);
        ShowNotification(new GUIContent($"PNJ '{safeName}' créé !"), 1.5f);
    }

    private void ShowAddPNJToPoolMenu(PNJInfoData pnj)
    {
        GenericMenu menu = new GenericMenu();
        foreach (PNJPool pool in allPNJPools)
        {
            menu.AddItem(new GUIContent($"{pool.name}/Pool Basique"), pool.PNJPoolList.Contains(pnj), () =>
            {
                Undo.RecordObject(pool, "Toggle PNJ in Basic Pool");
                if (pool.PNJPoolList.Contains(pnj)) pool.PNJPoolList.Remove(pnj);
                else pool.PNJPoolList.Add(pnj);
                EditorUtility.SetDirty(pool);
            });
            menu.AddItem(new GUIContent($"{pool.name}/Pool Spécial"), pool.SpecialPNJPool.Contains(pnj), () =>
            {
                Undo.RecordObject(pool, "Toggle PNJ in Special Pool");
                if (pool.SpecialPNJPool.Contains(pnj)) pool.SpecialPNJPool.Remove(pnj);
                else pool.SpecialPNJPool.Add(pnj);
                EditorUtility.SetDirty(pool);
            });
        }
        menu.ShowAsContext();
    }

    private void TryAssignReferenceByType(UnityEngine.Object targetAsset, UnityEngine.Object valueToAssign)
    {
        SerializedObject so = new SerializedObject(targetAsset);
        SerializedProperty prop = so.GetIterator();
        Type valueType = valueToAssign.GetType();

        while (prop.NextVisible(true))
        {
            if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue == null)
            {
                if (prop.type.Contains(valueType.Name) ||
                    (valueToAssign is IdentityData && prop.type.Contains("IdentityData")) ||
                    (valueToAssign is DialogueData && prop.type.Contains("DialogueData")))
                {
                    prop.objectReferenceValue = valueToAssign;
                    so.ApplyModifiedProperties();
                    EditorUtility.SetDirty(targetAsset);
                    return;
                }
            }
        }
    }

    private void EnsureFolderExists(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath)) return;

        string[] parts = folderPath.Replace("\\", "/").Split('/');
        string current = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            string next = $"{current}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }
            current = next;
        }
    }

    private string FormatObjectSummary(object obj)
    {
        Type t = obj.GetType();
        var parts = new List<string> { t.Name };

        foreach (PropertyInfo prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(float) || prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(string))
            {
                try { parts.Add($"{prop.Name}: {prop.GetValue(obj)}"); } catch { }
            }
        }

        foreach (FieldInfo field in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (field.FieldType == typeof(int) || field.FieldType == typeof(float) || field.FieldType == typeof(bool))
            {
                parts.Add($"{field.Name}: {field.GetValue(obj)}");
            }
        }

        return string.Join(" | ", parts);
    }

    private T GetPrivateField<T>(object target, string fieldName)
    {
        if (target == null) return default;
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        return field != null ? (T)field.GetValue(target) : default;
    }

    private void SetPrivateField(object target, string fieldName, object value)
    {
        if (target == null) return;
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(target, value);
    }

    private void InvokePrivateMethod(object target, string methodName, params object[] parameters)
    {
        if (target == null) return;
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        method?.Invoke(target, parameters);
    }
}