using System;
using System.Collections.Generic;
using System.Reflection;
using TMPEffects.Databases.AnimationDatabase;
using TMPEffects.TMPAnimations;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore;

public class TMPTextToolWindow : EditorWindow
{
    private enum WindowTab
    {
        TagGenerator,
        MultiGlyphEditor
    }

    private enum TMPType
    {
        Animation,
        Event,
        Color,
        Icon
    }

    private WindowTab currentTab = WindowTab.TagGenerator;

    // --- ONGLET 1 : GÉNÉRATEUR DE BALISES ---
    private TMPAnimationDatabase tmpAnimDatabase;
    private TMPType tmpType = TMPType.Animation;

    private int selectedAnimIndex = 0;
    private TMPEvents tmpEventName;
    private Color tmpColor = Color.white;
    private string tmpIconName = "";
    private Sprite iconPreview;

    private string iconSearchFilter = "";
    private Vector2 iconScrollPos;
    private bool showIconPicker = true;

    private string sampleText = "";
    private readonly List<string> history = new List<string>(5);
    private const int MaxHistory = 5;
    private const string HistoryPrefsKey = "ShopCrafter_TMPTextTool_History";
    private const string AnimDbPrefsKey = "ShopCrafter_TMPTextTool_AnimDB";

    // --- ONGLET 2 : ÉDITEUR MULTI-GLYPHES ---
    private TMP_SpriteAsset targetSpriteAsset;
    private bool editFaceInfo = true;
    private float facePointSize = 32f;
    private float faceScale = 1f;
    private float faceAscent = 26f;
    private float faceBaseline = 0f;
    private float faceDescent = -6f;

    private bool useIndexRange = false;
    private int rangeStart = 0;
    private int rangeEnd = 0;

    private bool applyBX = true;
    private float batchBX = 0f;
    private bool applyBY = true;
    private float batchBY = 26f;
    private bool applyAD = true;
    private bool adMatchWidth = true;
    private float batchAD = 32f;
    private bool applyGlyphScale = true;
    private float batchGlyphScale = 1f;

    [MenuItem("ShopCrafter/Tools/TMP Text Tool")]
    public static void ShowWindow()
    {
        TMPTextToolWindow window = GetWindow<TMPTextToolWindow>("TMP Text Tool");
        window.minSize = new Vector2(380f, 480f);
    }

    private void OnEnable()
    {
        LoadHistory();
        string dbGuid = EditorPrefs.GetString(AnimDbPrefsKey, "");
        if (!string.IsNullOrEmpty(dbGuid))
        {
            string path = AssetDatabase.GUIDToAssetPath(dbGuid);
            tmpAnimDatabase = AssetDatabase.LoadAssetAtPath<TMPAnimationDatabase>(path);
        }

        targetSpriteAsset = Resources.Load<TMP_SpriteAsset>("Data/TMPIconData/TextIcons");
        SyncFieldsFromSpriteAsset();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(6);
        currentTab = (WindowTab)GUILayout.Toolbar(
            (int)currentTab,
            new[] { "Générateur de balises", "Éditeur Multi-Glyphes" },
            GUILayout.Height(26)
        );
        EditorGUILayout.Space(8);

        if (currentTab == WindowTab.TagGenerator)
        {
            DrawTagGeneratorTab();
        }
        else
        {
            DrawMultiGlyphTab();
        }
    }

    // =========================================================================
    // ONGLET 1 : GÉNÉRATEUR DE BALISES + HISTORIQUE
    // =========================================================================
    private void DrawTagGeneratorTab()
    {
        tmpType = (TMPType)GUILayout.Toolbar((int)tmpType, Enum.GetNames(typeof(TMPType)), GUILayout.Height(24));
        EditorGUILayout.Space(8);

        DrawTypeSettings();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        sampleText = EditorGUILayout.TextField(
            new GUIContent("Texte à entourer (opt.)", "Laisse vide pour générer uniquement les balises"),
            sampleText
        );

        string generatedTag = BuildGeneratedTag();
        EditorGUILayout.LabelField("Résultat généré :", EditorStyles.miniBoldLabel);
        EditorGUILayout.SelectableLabel(generatedTag, EditorStyles.textField, GUILayout.Height(22));

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Copier & Ajouter à l'historique", GUILayout.Height(28)))
        {
            CopyAndRecord(generatedTag);
        }
        if (GUILayout.Button("Copier seul", GUILayout.Width(100), GUILayout.Height(28)))
        {
            EditorGUIUtility.systemCopyBuffer = generatedTag;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(12);
        DrawHistorySection();
    }

    private void DrawTypeSettings()
    {
        switch (tmpType)
        {
            case TMPType.Animation:
                EditorGUI.BeginChangeCheck();
                tmpAnimDatabase = (TMPAnimationDatabase)EditorGUILayout.ObjectField("TMP Anim Database", tmpAnimDatabase, typeof(TMPAnimationDatabase), false);
                if (EditorGUI.EndChangeCheck() && tmpAnimDatabase != null)
                {
                    string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(tmpAnimDatabase));
                    EditorPrefs.SetString(AnimDbPrefsKey, guid);
                }

                string[] animNames = GetTMPAnimNames();
                if (animNames.Length > 0)
                {
                    selectedAnimIndex = Mathf.Clamp(selectedAnimIndex, 0, animNames.Length - 1);
                    selectedAnimIndex = EditorGUILayout.Popup("Animation", selectedAnimIndex, animNames);
                }
                else
                {
                    EditorGUILayout.HelpBox("Assigne une TMPAnimationDatabase valide.", MessageType.Info);
                }
                break;

            case TMPType.Event:
                tmpEventName = (TMPEvents)EditorGUILayout.EnumPopup("TMP Event Name", tmpEventName);
                break;

            case TMPType.Color:
                tmpColor = EditorGUILayout.ColorField("TMP Color", tmpColor);
                break;

            case TMPType.Icon:
                DrawIconSelector();
                break;
        }
    }

    private void DrawIconSelector()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Icône sélectionnée", string.IsNullOrEmpty(tmpIconName) ? "Aucune" : tmpIconName);
        if (iconPreview != null)
        {
            Texture2D previewTex = AssetPreview.GetAssetPreview(iconPreview) ?? iconPreview.texture;
            GUILayout.Label(previewTex, GUILayout.Width(32), GUILayout.Height(32));
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button(showIconPicker ? "Masquer la galerie ▲" : "Afficher la galerie ▼"))
        {
            showIconPicker = !showIconPicker;
        }

        if (!showIconPicker) return;

        TMP_SpriteAsset spriteAsset = targetSpriteAsset != null
            ? targetSpriteAsset
            : Resources.Load<TMP_SpriteAsset>("Data/TMPIconData/TextIcons");

        if (spriteAsset == null)
        {
            EditorGUILayout.HelpBox("TextIcons introuvable dans Resources/Data/TMPIconData/", MessageType.Warning);
            return;
        }

        iconSearchFilter = EditorGUILayout.TextField("Rechercher", iconSearchFilter);
        iconScrollPos = EditorGUILayout.BeginScrollView(iconScrollPos, EditorStyles.helpBox, GUILayout.Height(180));

        float iconSize = 44f;
        float spacing = 4f;
        int columns = Mathf.Max(1, Mathf.FloorToInt((position.width - 35f) / (iconSize + spacing)));
        int count = 0;

        EditorGUILayout.BeginHorizontal();
        foreach (TMP_SpriteCharacter character in spriteAsset.spriteCharacterTable)
        {
            if (!string.IsNullOrEmpty(iconSearchFilter) &&
                character.name.IndexOf(iconSearchFilter, StringComparison.OrdinalIgnoreCase) < 0)
            {
                continue;
            }

            Sprite sp = spriteAsset.spriteGlyphTable[(int)character.glyphIndex].sprite;
            Texture2D tex = AssetPreview.GetAssetPreview(sp) ?? (sp != null ? sp.texture : null);

            GUI.backgroundColor = (character.name == tmpIconName) ? Color.cyan : Color.white;
            if (GUILayout.Button(new GUIContent(tex, character.name), GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
            {
                tmpIconName = character.name;
                iconPreview = sp;
            }

            count++;
            if (count % columns == 0)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();
    }

    private string BuildGeneratedTag()
    {
        string front = "";
        string back = "";

        switch (tmpType)
        {
            case TMPType.Event:
                back = $"<?{tmpEventName}>";
                break;

            case TMPType.Animation:
                string[] anims = GetTMPAnimNames();
                string anim = anims.Length > 0 ? anims[Mathf.Clamp(selectedAnimIndex, 0, anims.Length - 1)] : "anim";
                front = $"<{anim}>";
                back = $"</{anim}>";
                break;

            case TMPType.Color:
                front = $"<color=#{tmpColor.ToHexString()}>";
                back = "</color>";
                break;

            case TMPType.Icon:
                front = $"<sprite name=\"{tmpIconName}\">";
                break;
        }

        return front + sampleText + back;
    }

    private void CopyAndRecord(string value)
    {
        if (string.IsNullOrEmpty(value)) return;

        EditorGUIUtility.systemCopyBuffer = value;
        history.Remove(value);
        history.Insert(0, value);

        if (history.Count > MaxHistory)
        {
            history.RemoveRange(MaxHistory, history.Count - MaxHistory);
        }

        SaveHistory();
    }

    private void DrawHistorySection()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Historique ({history.Count}/{MaxHistory})", EditorStyles.boldLabel);
        if (history.Count > 0 && GUILayout.Button("Vider", EditorStyles.miniButton, GUILayout.Width(50)))
        {
            history.Clear();
            SaveHistory();
        }
        EditorGUILayout.EndHorizontal();

        if (history.Count == 0)
        {
            EditorGUILayout.HelpBox("Aucun élément copié pour le moment.", MessageType.None);
            return;
        }

        for (int i = 0; i < history.Count; i++)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.SelectableLabel(history[i], GUILayout.Height(18));
            if (GUILayout.Button("Copier", GUILayout.Width(60), GUILayout.Height(20)))
            {
                EditorGUIUtility.systemCopyBuffer = history[i];
                ShowNotification(new GUIContent("Copié !"), 0.8f);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    // =========================================================================
    // ONGLET 2 : ÉDITEUR MULTI-GLYPHES (BATCH METRICS & FACE INFO)
    // =========================================================================
    private void DrawMultiGlyphTab()
    {
        EditorGUI.BeginChangeCheck();
        targetSpriteAsset = (TMP_SpriteAsset)EditorGUILayout.ObjectField("TMP Sprite Asset", targetSpriteAsset, typeof(TMP_SpriteAsset), false);
        if (EditorGUI.EndChangeCheck())
        {
            SyncFieldsFromSpriteAsset();
        }

        if (targetSpriteAsset == null)
        {
            EditorGUILayout.HelpBox("Sélectionne un TMP_SpriteAsset pour modifier ses glyphes en lot.", MessageType.Warning);
            return;
        }

        int totalGlyphs = targetSpriteAsset.spriteGlyphTable.Count;
        EditorGUILayout.LabelField($"Nombre total de glyphes : {totalGlyphs}", EditorStyles.miniLabel);
        EditorGUILayout.Space(6);

        // --- 1. FACE INFO ---
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        editFaceInfo = EditorGUILayout.ToggleLeft("Modifier Face Info (Global)", editFaceInfo, EditorStyles.boldLabel);
        if (editFaceInfo)
        {
            EditorGUI.indentLevel++;
            facePointSize = EditorGUILayout.FloatField("Point Size", facePointSize);
            faceScale = EditorGUILayout.FloatField("Scale", faceScale);
            faceAscent = EditorGUILayout.FloatField("Ascent Line", faceAscent);
            faceBaseline = EditorGUILayout.FloatField("Baseline", faceBaseline);
            faceDescent = EditorGUILayout.FloatField("Descent Line", faceDescent);
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(6);

        // --- 2. CIBLE DES GLYPHES ---
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Cible des glyphes", EditorStyles.boldLabel);
        useIndexRange = EditorGUILayout.Toggle("Limiter à une plage d'index", useIndexRange);
        if (useIndexRange)
        {
            EditorGUI.indentLevel++;
            rangeStart = Mathf.Clamp(EditorGUILayout.IntField("Index début", rangeStart), 0, Mathf.Max(0, totalGlyphs - 1));
            rangeEnd = Mathf.Clamp(EditorGUILayout.IntField("Index fin", rangeEnd), rangeStart, Mathf.Max(0, totalGlyphs - 1));
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(6);

        // --- 3. MÉTRIQUES DES GLYPHES ---
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Métriques à appliquer", EditorStyles.boldLabel);

        // BX
        EditorGUILayout.BeginHorizontal();
        applyBX = EditorGUILayout.Toggle(applyBX, GUILayout.Width(18));
        EditorGUI.BeginDisabledGroup(!applyBX);
        batchBX = EditorGUILayout.FloatField("Bearing X (BX)", batchBX);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        // BY
        EditorGUILayout.BeginHorizontal();
        applyBY = EditorGUILayout.Toggle(applyBY, GUILayout.Width(18));
        EditorGUI.BeginDisabledGroup(!applyBY);
        batchBY = EditorGUILayout.FloatField("Bearing Y (BY)", batchBY);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        // AD
        EditorGUILayout.BeginHorizontal();
        applyAD = EditorGUILayout.Toggle(applyAD, GUILayout.Width(18));
        EditorGUI.BeginDisabledGroup(!applyAD);
        adMatchWidth = EditorGUILayout.ToggleLeft("AD = Largeur (W)", adMatchWidth, GUILayout.Width(130));
        if (!adMatchWidth)
        {
            batchAD = EditorGUILayout.FloatField(batchAD);
        }
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        // Scale
        EditorGUILayout.BeginHorizontal();
        applyGlyphScale = EditorGUILayout.Toggle(applyGlyphScale, GUILayout.Width(18));
        EditorGUI.BeginDisabledGroup(!applyGlyphScale);
        batchGlyphScale = EditorGUILayout.FloatField("Glyph Scale", batchGlyphScale);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(8);

        // --- BOUTONS D'ACTION & PRESET ---
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Auto-remplir (Centré sur W/H)", GUILayout.Height(24)))
        {
            AutoDetectPresetFromFirstGlyph();
        }
        if (GUILayout.Button("Relire valeurs actuelles", GUILayout.Height(24)))
        {
            SyncFieldsFromSpriteAsset();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(4);

        GUI.backgroundColor = new Color(0.4f, 0.85f, 0.5f);
        if (GUILayout.Button("Appliquer aux glyphes sélectionnés", GUILayout.Height(32)))
        {
            ApplyBatchGlyphChanges();
        }
        GUI.backgroundColor = Color.white;
    }

    private void SyncFieldsFromSpriteAsset()
    {
        if (targetSpriteAsset == null) return;

        FaceInfo info = targetSpriteAsset.faceInfo;
        facePointSize = info.pointSize > 0 ? info.pointSize : 32f;
        faceScale = info.scale > 0 ? info.scale : 1f;
        faceAscent = info.ascentLine;
        faceBaseline = info.baseline;
        faceDescent = info.descentLine;

        rangeStart = 0;
        rangeEnd = Mathf.Max(0, targetSpriteAsset.spriteGlyphTable.Count - 1);

        if (targetSpriteAsset.spriteGlyphTable.Count > 0)
        {
            GlyphMetrics m = targetSpriteAsset.spriteGlyphTable[0].metrics;
            batchBX = m.horizontalBearingX;
            batchBY = m.horizontalBearingY;
            batchAD = m.horizontalAdvance;
            batchGlyphScale = targetSpriteAsset.spriteGlyphTable[0].scale;
        }
    }

    private void AutoDetectPresetFromFirstGlyph()
    {
        if (targetSpriteAsset == null || targetSpriteAsset.spriteGlyphTable.Count == 0) return;

        GlyphMetrics first = targetSpriteAsset.spriteGlyphTable[0].metrics;
        float size = first.height > 0 ? first.height : 32f;

        editFaceInfo = true;
        facePointSize = size;
        faceScale = 1f;
        faceAscent = Mathf.Round(size * 0.8f);
        faceBaseline = 0f;
        faceDescent = -Mathf.Round(size * 0.2f);

        applyBX = true;
        batchBX = 0f;
        applyBY = true;
        batchBY = Mathf.Round(size * 0.8f);
        applyAD = true;
        adMatchWidth = true;
        applyGlyphScale = true;
        batchGlyphScale = 1f;
    }

    private void ApplyBatchGlyphChanges()
    {
        if (targetSpriteAsset == null) return;

        Undo.RecordObject(targetSpriteAsset, "Batch Edit TMP Sprite Glyphs");

        if (editFaceInfo)
        {
            FaceInfo info = targetSpriteAsset.faceInfo;
            info.pointSize = (int)facePointSize;
            info.scale = faceScale;
            info.ascentLine = faceAscent;
            info.baseline = faceBaseline;
            info.descentLine = faceDescent;
            info.lineHeight = faceAscent - faceDescent;
            targetSpriteAsset.faceInfo = info;
        }

        int start = useIndexRange ? rangeStart : 0;
        int end = useIndexRange ? rangeEnd : targetSpriteAsset.spriteGlyphTable.Count - 1;

        for (int i = start; i <= end && i < targetSpriteAsset.spriteGlyphTable.Count; i++)
        {
            TMP_SpriteGlyph glyph = targetSpriteAsset.spriteGlyphTable[i];
            GlyphMetrics m = glyph.metrics;

            if (applyBX) m.horizontalBearingX = batchBX;
            if (applyBY) m.horizontalBearingY = batchBY;
            if (applyAD) m.horizontalAdvance = adMatchWidth ? m.width : batchAD;

            glyph.metrics = m;

            if (applyGlyphScale)
            {
                glyph.scale = batchGlyphScale;
            }
        }

        targetSpriteAsset.UpdateLookupTables();
        EditorUtility.SetDirty(targetSpriteAsset);
        AssetDatabase.SaveAssets();

        // Force le rafraîchissement des textes TMP déjà présents dans la scène
        TMPro_EventManager.ON_SPRITE_ASSET_PROPERTY_CHANGED(true, targetSpriteAsset);
        ShowNotification(new GUIContent($"{end - start + 1} glyphes mis à jour !"), 1.5f);
    }

    // =========================================================================
    // UTILITAIRES
    // =========================================================================
    private string[] GetTMPAnimNames()
    {
        if (tmpAnimDatabase == null || tmpAnimDatabase.BasicAnimationDatabase == null)
            return Array.Empty<string>();

        var type = tmpAnimDatabase.BasicAnimationDatabase.GetType();
        FieldInfo field = type.GetField("animations", BindingFlags.NonPublic | BindingFlags.Instance);

        if (field?.GetValue(tmpAnimDatabase.BasicAnimationDatabase) is Dictionary<string, TMPAnimation> dict)
        {
            return new List<string>(dict.Keys).ToArray();
        }

        return Array.Empty<string>();
    }

    private void SaveHistory()
    {
        EditorPrefs.SetString(HistoryPrefsKey, string.Join("|||", history));
    }

    private void LoadHistory()
    {
        history.Clear();
        string raw = EditorPrefs.GetString(HistoryPrefsKey, "");
        if (!string.IsNullOrEmpty(raw))
        {
            history.AddRange(raw.Split(new[] { "|||" }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}